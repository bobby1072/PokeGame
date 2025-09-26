using PokeGame.Core.Schemas;

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
}
