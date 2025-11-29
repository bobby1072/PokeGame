using System.Text.Json;
using BT.Common.FastArray.Proto;
using BT.Common.Persistence.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Persistence.Entities;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Contexts;

internal sealed class PokeGameContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<PokedexPokemonEntity> PokedexPokemon { get; set; }
    public DbSet<GameSaveEntity> GameSaves { get; set; }
    public DbSet<OwnedPokemonEntity> OwnedPokemon { get; set; }
    public DbSet<OwnedItemEntity> OwnedItems { get; set; }
    public DbSet<GameSessionEntity> GameSessions { get; set; }
    public DbSet<GameSaveDataEntity> GameSaveData { get; set; }
    private static readonly JsonSerializerOptions _jsonBSerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public PokeGameContext(DbContextOptions<PokeGameContext> options)
        : base(options) { }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateDatesOnNewlyAddedOrModified();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        UpdateDatesOnNewlyAddedOrModified();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateDatesOnNewlyAddedOrModified();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override int SaveChanges()
    {
        UpdateDatesOnNewlyAddedOrModified();
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameSaveDataEntity>(ent =>
        {
            ent.HasOne<GameSaveEntity>()
                .WithOne(x => x.GameSaveData)
                .HasForeignKey<GameSaveDataEntity>(x => x.GameSaveId);

            ent.Property(x => x.GameData)
                .HasColumnType("jsonb")
                .HasConversion(x => SerializeGameSaveData(x), x => DeserializeGameSaveData(x));
        });

        modelBuilder.Entity<OwnedPokemonEntity>(ent =>
        {
            ent.HasOne(x => x.GameSave).WithMany().HasForeignKey(x => x.GameSaveId);
        });
    }

    private void UpdateDatesOnNewlyAddedOrModified()
    {
        var currentTime = DateTime.UtcNow;
        var updatingEntries = ChangeTracker
            .Entries()
            .FastArrayWhere(e => e.State is EntityState.Added or EntityState.Modified)
            .ToArray();

        foreach (var updatedEnt in updatingEntries)
        {
            if (updatedEnt.Entity is UserEntity userEntity)
            {
                if (updatedEnt.State == EntityState.Added)
                {
                    UpdateEntityDates<UserEntity, Guid?, User>(
                        userEntity,
                        [nameof(UserEntity.DateCreated), nameof(UserEntity.DateModified)],
                        currentTime
                    );
                }
                else if (updatedEnt.State == EntityState.Modified)
                {
                    UpdateEntityDates<UserEntity, Guid?, User>(
                        userEntity,
                        [nameof(UserEntity.DateModified)],
                        currentTime
                    );
                }
            }
            else if (updatedEnt.Entity is GameSaveEntity gameSaveEntity)
            {
                if (updatedEnt.State == EntityState.Added)
                {
                    UpdateEntityDates<GameSaveEntity, Guid?, GameSave>(
                        gameSaveEntity,
                        [nameof(GameSaveEntity.DateCreated)],
                        currentTime
                    );
                }
            }
            else if (updatedEnt.Entity is OwnedPokemonEntity ownedPokemon)
            {
                if (updatedEnt.State == EntityState.Added)
                {
                    UpdateEntityDates<OwnedPokemonEntity, Guid?, OwnedPokemon>(
                        ownedPokemon,
                        [nameof(OwnedPokemonEntity.CaughtAt)],
                        currentTime
                    );
                }
            }
            else if (updatedEnt.Entity is GameSessionEntity gameSession)
            {
                if (updatedEnt.State == EntityState.Added)
                {
                    UpdateEntityDates<GameSessionEntity, Guid?, GameSession>(
                        gameSession,
                        [nameof(GameSessionEntity.StartedAt)],
                        currentTime
                    );
                }
            }
        }
    }

    private static void UpdateEntityDates<TEnt, TId, TRuntime>(
        TEnt ent,
        IReadOnlyCollection<string> propertyNames,
        DateTime dateTime
    )
        where TEnt : BaseEntity<TId, TRuntime>
        where TRuntime : class
    {
        var entType = typeof(TEnt);
        foreach (var propName in propertyNames)
        {
            try
            {
                var propertyToUpdate = entType.GetProperty(propName);
                if (propertyToUpdate == null || propertyToUpdate.PropertyType != typeof(DateTime))
                {
                    continue;
                }

                propertyToUpdate.SetValue(ent, dateTime);
            }
            catch
            {
                //This is ok because we are just trying to update values
            }
        }
    }

    private static string SerializeGameSaveData(GameSaveDataActual entity)
    {
        return JsonSerializer.Serialize(entity, _jsonBSerializerOptions);
    }

    private static GameSaveDataActual DeserializeGameSaveData(string json)
    {
        return JsonSerializer.Deserialize<GameSaveDataActual>(json, _jsonBSerializerOptions)
            ?? throw new PokeGameApiServerException(
                "Failed to deserialize GameSaveDataActual from db json"
            );
    }
}
