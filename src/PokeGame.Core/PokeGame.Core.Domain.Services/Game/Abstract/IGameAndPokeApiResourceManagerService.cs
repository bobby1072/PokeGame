using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Abstract;

internal interface IGameAndPokeApiResourceManagerService
{
    Task<IReadOnlyCollection<OwnedPokemon>> GetFullOwnedPokemon(IReadOnlyCollection<Guid> ownedPokemonId);
}