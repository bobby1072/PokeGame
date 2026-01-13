using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class OwnedPokemonExtensions
{
    public static OwnedPokemonEntity ToEntity(this OwnedPokemon ownedPokemon)
    {
        return new OwnedPokemonEntity
        {
            Id = ownedPokemon.Id,
            PokedexId = ownedPokemon.PokedexId,
            GameSaveId = ownedPokemon.GameSaveId,
            ResourceName = ownedPokemon.PokemonResourceName,
            CaughtAt = ownedPokemon.CaughtAt,
            PokemonLevel = ownedPokemon.PokemonLevel,
            CurrentExperience = ownedPokemon.CurrentExperience,
            CurrentHp = ownedPokemon.CurrentHp,
            MoveOneResourceName = ownedPokemon.MoveOneResourceName,
            MoveTwoResourceName = ownedPokemon.MoveTwoResourceName,
            MoveThreeResourceName = ownedPokemon.MoveThreeResourceName,
            MoveFourResourceName = ownedPokemon.MoveFourResourceName,
        };
    }
}
