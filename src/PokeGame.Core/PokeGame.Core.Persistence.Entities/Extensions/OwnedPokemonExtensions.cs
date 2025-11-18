using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Persistence.Entities.Extensions;

public static class OwnedPokemonExtensions
{
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
            MoveFourResourceName = ownedPokemon.MoveFourResourceName,
        };
    }
}
