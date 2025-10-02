using PokeGame.Core.Schemas;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Pokedex;

namespace PokeGame.Core.Persistence.Entities.Extensions;

internal static class PersistableDomainModelExtensions
{
    public static PokedexPokemonEntity ToEntity(this PokedexPokemon pokemon)
    {
        return new PokedexPokemonEntity
        {
            Id = pokemon.Id,
            Attack = pokemon.Stats.Attack,
            Defence = pokemon.Stats.Defence,
            Speed = pokemon.Stats.Speed,
            SpecialAttack = pokemon.Stats.SpecialAttack,
            SpecialDefence = pokemon.Stats.SpecialDefence,
            Hp = pokemon.Stats.Hp,
            Type1 = pokemon.Type.Type1.ToString(),
            Type2 = pokemon.Type.Type2?.ToString(),
            ChineseName = pokemon.ChineseName,
            EnglishName = pokemon.EnglishName,
            FrenchName = pokemon.FrenchName,
            JapaneseName = pokemon.JapaneseName,
        };
    }
    public static GameSaveEntity ToEntity(this GameSave gameSave)
    {
        return new GameSaveEntity
        {
            Id = gameSave.Id,
            CharacterName = gameSave.CharacterName,
            UserId = gameSave.UserId,
            DateCreated = gameSave.DateCreated,
            LastPlayed = gameSave.LastPlayed
        };
    }
    public static UserEntity ToEntity(this User runtimeObj)
    {
        return new UserEntity
        {
            Id = runtimeObj.Id,
            Email = runtimeObj.Email,
            Name = runtimeObj.Name,
            DateCreated = runtimeObj.DateCreated,
            DateModified = runtimeObj.DateModified,
        };
    }

    public static OwnedPokemonEntity ToEntity(this OwnedPokemon ownedPokemon)
    {
        return new OwnedPokemonEntity
        {
            Id = ownedPokemon.Id,
            GameSaveId = ownedPokemon.GameSaveId,
            ResourceName = ownedPokemon.ResourceName,
            CaughtAt = ownedPokemon.CaughtAt,
            InDeck = ownedPokemon.InDeck,
            PokemonLevel = ownedPokemon.PokemonLevel,
            CurrentExperience = ownedPokemon.CurrentExperience,
            CurrentHp = ownedPokemon.CurrentHp,
            MoveOneResourceName = ownedPokemon.MoveOneResourceName,
            MoveTwoResourceName = ownedPokemon.MoveTwoResourceName,
            MoveThreeResourceName = ownedPokemon.MoveThreeResourceName,
            MoveFourResourceName = ownedPokemon.MoveFourResourceName
        };
    }

    public static ItemStackEntity ToEntity(this ItemStack itemStack)
    {
        return new ItemStackEntity
        {
            Id = itemStack.Id,
            GameSaveId = itemStack.GameSaveId,
            ResourceName = itemStack.ResourceName,
            Quantity = itemStack.Quantity
        };
    }
}
