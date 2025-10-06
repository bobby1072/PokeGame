using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class PokeGameRuleHelperService
{
    private readonly PokeGameRules _pokeGameRules;
    private readonly ILogger<PokeGameRuleHelperService> _logger;
    public PokeGameRuleHelperService(PokeGameRules pokeGameRules, ILogger<PokeGameRuleHelperService> logger)
    {
        _pokeGameRules = pokeGameRules;
        _logger = logger;
    }

    public OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon)
    {
        throw new NotImplementedException();
    }

    public OwnedPokemon RefillOwnedPokemonHp(OwnedPokemon ownedPokemon)
    {
        throw new NotImplementedException();
    }
}