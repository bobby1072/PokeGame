using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
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

    public OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon, int xpToAdd)
    {
        var maxXpforLevel = _pokeGameRules.BaseXpCeiling;
        for (int i = 1; i <= ownedPokemon.PokemonLevel; i++)
        {
            maxXpforLevel = (int)(maxXpforLevel * _pokeGameRules.XpMultiplier);
        }

        ownedPokemon.CurrentExperience = ownedPokemon.CurrentExperience + xpToAdd;

        if (ownedPokemon.CurrentExperience >= maxXpforLevel)
        {
            ownedPokemon.PokemonLevel++;
            ownedPokemon.CurrentExperience = ownedPokemon.CurrentExperience - maxXpforLevel;
            ownedPokemon.CurrentHp = GetPokemonMaxHp(ownedPokemon);
        }

        return ownedPokemon;
    }

    public OwnedPokemon RefillOwnedPokemonHp(OwnedPokemon ownedPokemon)
    {
        ownedPokemon.CurrentHp = GetPokemonMaxHp(ownedPokemon);
        return ownedPokemon;
    }

    private  int GetPokemonMaxHp(OwnedPokemon ownedPokemon)
    {
        int evTerm = _pokeGameRules.HpCalculationStats.DefaultEV / 4; // floor division automatically
        double core = ((2 *(ownedPokemon.PokedexPokemon?.Stats.Hp ?? throw new PokeGameApiServerException("Pokedex pokemon not attached owned pokemon"))
            + _pokeGameRules.HpCalculationStats.DefaultIV + evTerm) * ownedPokemon.PokemonLevel) / 100.0;
        int hp = (int)Math.Floor(core) + ownedPokemon.PokemonLevel + 10;
        return hp;
    }
}