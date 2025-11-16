using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Schemas.Game;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class PokeGameRuleHelperService : IPokeGameRuleHelperService
{
    private readonly ConfigurablePokeGameRules _configurablePokeGameRules;
    private readonly ILogger<PokeGameRuleHelperService> _logger;
    private int[]? _fullStandardPokedexIndexArrayInstance;
    private int[] _fullStandardPokedexIndexArray
    {
        get =>
            _fullStandardPokedexIndexArrayInstance ??= GetFullPokedexIndexArray(
                _configurablePokeGameRules.StandardPokemonPokedexRange
            );
    }
    private int[]? _fullLegendaryPokedexIndexArrayInstance;
    private int[] _fullLegendaryPokedexIndexArray
    {
        get =>
            _fullLegendaryPokedexIndexArrayInstance ??= GetFullPokedexIndexArray(
                _configurablePokeGameRules.LegendaryPokemonPokedexRange
            );
    }

    public PokeGameRuleHelperService(
        ConfigurablePokeGameRules configurablePokeGameRules,
        ILogger<PokeGameRuleHelperService> logger
    )
    {
        _configurablePokeGameRules = configurablePokeGameRules;
        _logger = logger;
    }

    public int GetRandomPokemonNumberFromStandardPokedexRange()
    {
        var randomArrayIndex = Random.Shared.Next(0, _fullStandardPokedexIndexArray.Length);

        return _fullStandardPokedexIndexArray[randomArrayIndex];
    }

    public int GetRandomPokemonNumberFromLegendaryPokedexRange()
    {
        var randomArrayIndex = Random.Shared.Next(0, _fullLegendaryPokedexIndexArray.Length);

        return _fullStandardPokedexIndexArray[randomArrayIndex];
    }

    public OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon, int xpToAdd)
    {
        _logger.LogInformation(
            "Attemtpting to add xp to owned pokemon with id: {OwnedPokemonId}",
            ownedPokemon.Id
        );

        _logger.LogDebug("Owned pokemon before xp added: {@OwnedPokemon}", ownedPokemon);

        if (ownedPokemon.PokemonSpecies == null)
            throw new PokeGameApiServerException("Pokemon species not attached to owned pokemon");

        var originalLevel = ownedPokemon.PokemonLevel;
        var (newLevel, newExperience, newHp) = CalculateXpAndLevelUp(
            ownedPokemon.PokemonLevel,
            ownedPokemon.CurrentExperience,
            xpToAdd,
            ownedPokemon.PokemonSpecies.IsLegendary,
            level =>
            {
                var tempPokemon = ownedPokemon;
                tempPokemon.PokemonLevel = level;
                return GetPokemonMaxHp(tempPokemon);
            }
        );

        ownedPokemon.PokemonLevel = newLevel;
        ownedPokemon.CurrentExperience = newExperience;
        ownedPokemon.CurrentHp = newHp;

        if (newLevel > originalLevel)
        {
            _logger.LogDebug(
                "Owned pokemon with id: {OwnedPokemonId} has leveled up from: {OriginalLevel} --> {NewLevel}",
                ownedPokemon.Id,
                originalLevel,
                newLevel
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

    private (int newLevel, int newCurrentExperience, int newCurrentHp) CalculateXpAndLevelUp(
        int currentLevel,
        int currentExperience,
        int xpToAdd,
        bool isLegendary,
        Func<int, int> getMaxHpForLevel
    )
    {
        var maxXpForLevel = _configurablePokeGameRules.BaseXpCeiling;
        var multiplierToUse = isLegendary
            ? _configurablePokeGameRules.LegendaryXpMultiplier
            : _configurablePokeGameRules.XpMultiplier;

        for (int i = 1; i <= currentLevel; i++)
        {
            maxXpForLevel = (int)(maxXpForLevel * multiplierToUse);
        }

        var newExperience = currentExperience + xpToAdd;
        var newLevel = currentLevel;
        var newHp = getMaxHpForLevel(currentLevel);

        while (newExperience >= maxXpForLevel && newLevel < 100)
        {
            newExperience -= maxXpForLevel;
            newLevel++;
            newHp = getMaxHpForLevel(newLevel);
            maxXpForLevel = (int)(maxXpForLevel * multiplierToUse);
        }

        return (newLevel, newExperience, newHp);
    }

    private int GetPokemonMaxHp(OwnedPokemon ownedPokemon)
    {
        int evTerm = _configurablePokeGameRules.HpCalculationStats.DefaultEV / 4;
        double core =
            (
                2
                    * (
                        ownedPokemon.PokedexPokemon?.Stats.Hp
                        ?? throw new PokeGameApiServerException(
                            "Pokedex pokemon not attached to owned pokemon"
                        )
                    )
                + _configurablePokeGameRules.HpCalculationStats.DefaultIV
                + evTerm
            )
            * ownedPokemon.PokemonLevel
            / 100.0;
        int hp = (int)Math.Floor(core) + ownedPokemon.PokemonLevel + 10;
        return hp;
    }

    private static int[] GetFullPokedexIndexArray(PokedexRange pokedexRange)
    {
        var fullPokeDexIndexArray = new List<int>();
        for (int i = pokedexRange.Min; i <= pokedexRange.Max; i++)
        {
            fullPokeDexIndexArray.Add(i);
        }

        return fullPokeDexIndexArray.Union(pokedexRange.Extras).ToArray();
    }
}
