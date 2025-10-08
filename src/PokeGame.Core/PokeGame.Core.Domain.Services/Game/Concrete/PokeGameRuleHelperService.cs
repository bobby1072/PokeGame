using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class PokeGameRuleHelperService : IPokeGameRuleHelperService
{
    private readonly PokeGameRules _pokeGameRules;
    private readonly ILogger<PokeGameRuleHelperService> _logger;
    private int[]? _fullPokedexIndexArrayInstance;
    private int[] _fullPokedexIndexArray
    {
        get => _fullPokedexIndexArrayInstance ??= GetFullPokedexIndexArray();
    }

    public PokeGameRuleHelperService(
        PokeGameRules pokeGameRules,
        ILogger<PokeGameRuleHelperService> logger
    )
    {
        _pokeGameRules = pokeGameRules;
        _logger = logger;
    }

    public int GetRandomPokemonNumberFromStandardPokedexRange()
    {
        var randomArrayIndex = Random.Shared.Next(0, _fullPokedexIndexArray.Length);

        return _fullPokedexIndexArray[randomArrayIndex];
    }

    public OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon, int xpToAdd)
    {
        _logger.LogInformation(
            "Attemtpting to add xp to owned pokemon with id: {OwnedPokemonId}",
            ownedPokemon.Id
        );

        _logger.LogDebug("Owned pokemon before xp added: {@OwnedPokemon}", ownedPokemon);

        var maxXpForLevel = _pokeGameRules.BaseXpCeiling;

        var multiplyerToUse =
            (
                ownedPokemon.PokemonSpecies?.IsLegendary
                ?? throw new PokeGameApiServerException(
                    "Pokemon species not attached to owned pokemon"
                )
            )
                ? _pokeGameRules.LegendaryXpMultiplier
                : _pokeGameRules.XpMultiplier;
        for (int i = 1; i <= ownedPokemon.PokemonLevel; i++)
        {
            maxXpForLevel = (int)(maxXpForLevel * multiplyerToUse);
        }

        ownedPokemon.CurrentExperience += xpToAdd;

        var originaLLevel = ownedPokemon.PokemonLevel;

        while (ownedPokemon.CurrentExperience >= maxXpForLevel && ownedPokemon.PokemonLevel < 100)
        {
            ownedPokemon.CurrentExperience -= maxXpForLevel;
            ownedPokemon.PokemonLevel++;
            ownedPokemon.CurrentHp = GetPokemonMaxHp(ownedPokemon);
            maxXpForLevel = (int)(maxXpForLevel * multiplyerToUse);

            _logger.LogDebug(
                "Owned pokemon with id: {OwnedPokemonId} has leveled up from: {OriginalLevel} --> {NewLevel}",
                ownedPokemon.Id,
                originaLLevel,
                ownedPokemon.PokemonLevel
            );
        }

        _logger.LogDebug("Owned pokemon after xp added: {@OwnedPokemon}", ownedPokemon);

        return ownedPokemon;
    }

    public OwnedPokemon RefillOwnedPokemonHp(OwnedPokemon ownedPokemon)
    {
        _logger.LogInformation(
            "Attempting to refill HP on owned pokemon with id: {OwnedPokemonId}",
            ownedPokemon.Id
        );

        var originalHp = ownedPokemon.CurrentHp;

        ownedPokemon.CurrentHp = GetPokemonMaxHp(ownedPokemon);

        _logger.LogDebug(
            "Owned pokemon with id: {OwnedPokemonId} went from hp: {Originalhp} --> {NewHp}",
            ownedPokemon.Id,
            originalHp,
            ownedPokemon.CurrentHp
        );

        return ownedPokemon;
    }

    private int GetPokemonMaxHp(OwnedPokemon ownedPokemon)
    {
        int evTerm = _pokeGameRules.HpCalculationStats.DefaultEV / 4;
        double core =
            (
                2
                    * (
                        ownedPokemon.PokedexPokemon?.Stats.Hp
                        ?? throw new PokeGameApiServerException(
                            "Pokedex pokemon not attached to owned pokemon"
                        )
                    )
                + _pokeGameRules.HpCalculationStats.DefaultIV
                + evTerm
            )
            * ownedPokemon.PokemonLevel
            / 100.0;
        int hp = (int)Math.Floor(core) + ownedPokemon.PokemonLevel + 10;
        return hp;
    }

    private int[] GetFullPokedexIndexArray()
    {
        var fullPokeDexIndexArray = new List<int>();
        for (
            int i = _pokeGameRules.StandardPokemonPokedexRange.Min;
            i <= _pokeGameRules.StandardPokemonPokedexRange.Max;
            i++
        )
        {
            fullPokeDexIndexArray.Add(i);
        }

        return fullPokeDexIndexArray
            .Union(_pokeGameRules.StandardPokemonPokedexRange.Extras)
            .ToArray();
    }
}
