using BT.Common.FastArray.Proto;
using BT.Common.Services.Concrete;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Configurations;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Common.GameInformationData;
using PokeGame.Core.Domain.Services.Game.Abstract;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Domain.Services.Game.Concrete;

internal sealed class PokeGameRuleHelperService : IPokeGameRuleHelperService
{
    private readonly ConfigurablePokeGameRules _configurablePokeGameRules;
    private readonly ILogger<PokeGameRuleHelperService> _logger;

    public PokeGameRuleHelperService(
        ConfigurablePokeGameRules configurablePokeGameRules,
        ILogger<PokeGameRuleHelperService> logger
    )
    {
        _configurablePokeGameRules = configurablePokeGameRules;
        _logger = logger;
    }

    public (
        string? MoveOneResourceName,
        string? MoveTwoResourceName,
        string? MoveThreeResourceName,
        string? MoveFourResourceName
    ) GetRandomMoveSetFromPokemon(Pokemon pokemonSpecies, int level)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(pokemonSpecies.Id), pokemonSpecies.Id);
        activity?.SetTag(nameof(level), level);

        _logger.LogInformation(
            "Getting random move set from pokemon species id: {PokemonSpeciesId} at level: {Level}",
            pokemonSpecies.Id,
            level
        );

        var possibleMoves = pokemonSpecies
            .Moves.FastArrayWhere(m => m.VersionGroupDetails.Any(vg => vg.LevelLearnedAt <= level))
            .FastArraySelect(m => m.Move)
            .ToArray();

        _logger.LogDebug(
            "Possible moves for pokemon species id: {PokemonSpeciesId} at level: {Level} are: {@PossibleMoves}",
            pokemonSpecies.Id,
            level,
            possibleMoves
        );

        var selectedMoves = new List<string>();
        var movesToSelect = Math.Min(4, possibleMoves.Length);

        while (selectedMoves.Count < movesToSelect)
        {
            var randomIndex = Random.Shared.Next(0, possibleMoves.Length);
            var moveToAdd = possibleMoves[randomIndex];
            if (!selectedMoves.Contains(moveToAdd.Name))
            {
                selectedMoves.Add(moveToAdd.Name);
            }
        }

        _logger.LogInformation(
            "Selected moves for pokemon species id: {PokemonSpeciesId} at level: {Level} are: {@SelectedMoves}",
            pokemonSpecies.Id,
            level,
            selectedMoves
        );

        return (
            selectedMoves.ElementAtOrDefault(0),
            selectedMoves.ElementAtOrDefault(1),
            selectedMoves.ElementAtOrDefault(2),
            selectedMoves.ElementAtOrDefault(3)
        );
    }

    public int? GetRandomPokedexNumberFromIntRangeWithRandomEncounterIncluded(IntRange intRange)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(IntRange.Min), intRange.Min);
        activity?.SetTag(nameof(IntRange.Max), intRange.Max);
        activity?.SetTag(nameof(IntRange.Extras), string.Join(",", intRange.Extras));

        var encounterActuallyHappen = Random.Shared.Next(1, 101);

        if (
            encounterActuallyHappen
            > _configurablePokeGameRules.RandomPokemonEncounterLikelyhoodPercentage
        )
        {
            _logger.LogInformation(
                "No encounter happening based on random encounter probability percentage: {Likelihood}%",
                _configurablePokeGameRules.RandomPokemonEncounterLikelyhoodPercentage
            );
            return null;
        }

        return GetRandomNumberFromIntRange(intRange);
    }

    public int GetRandomNumberFromIntRange(IntRange intRange)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(IntRange.Min), intRange.Min);
        activity?.SetTag(nameof(IntRange.Max), intRange.Max);
        activity?.SetTag(nameof(IntRange.Extras), string.Join(",", intRange.Extras));

        _logger.LogDebug(
            "GetRandomPokemonNumberFromPokedexRange querying for random pokemon id using Pokedex range: {@PokedexRange}",
            intRange
        );

        var fullIndexList = GetFullPokedexIndexArray(intRange);
        var randomArrayIndex = Random.Shared.Next(0, fullIndexList.Length);

        _logger.LogInformation(
            "GetRandomPokemonNumberFromPokedexRange got random Pokedex id: {PokedexId}",
            randomArrayIndex
        );

        return fullIndexList[randomArrayIndex];
    }

    public int CalculateStat(int level, int baseStat)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(level), level);
        activity?.SetTag(nameof(baseStat), baseStat);

        int evTerm = _configurablePokeGameRules.StatCalculationStats.DefaultEV / 4;
        double core =
            (2 * baseStat + _configurablePokeGameRules.StatCalculationStats.DefaultIV + evTerm)
            * level
            / 100.0;
        int stat = (int)Math.Floor(core) + 5;
        return stat;
    }

    public OwnedPokemon AddXpToOwnedPokemon(OwnedPokemon ownedPokemon, int xpToAdd)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(ownedPokemon.Id), ownedPokemon.Id);
        activity?.SetTag(nameof(xpToAdd), xpToAdd);

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
            _logger.LogInformation(
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
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        activity?.SetTag(nameof(ownedPokemon.Id), ownedPokemon.Id);

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
        int evTerm = _configurablePokeGameRules.StatCalculationStats.DefaultEV / 4;
        double core =
            (
                2
                    * (
                        ownedPokemon
                            .Pokemon?.Stats.FastArrayFirst(x => x.Stat.Name == "hp")
                            .BaseStat
                        ?? throw new PokeGameApiServerException(
                            "Pokedex pokemon not attached to owned pokemon"
                        )
                    )
                + _configurablePokeGameRules.StatCalculationStats.DefaultIV
                + evTerm
            )
            * ownedPokemon.PokemonLevel
            / 100.0;
        int hp = (int)Math.Floor(core) + ownedPokemon.PokemonLevel + 10;
        return hp;
    }

    private static int[] GetFullPokedexIndexArray(IntRange intRange)
    {
        var fullPokeDexIndexArray = new List<int>();
        for (int i = intRange.Min; i <= intRange.Max; i++)
        {
            fullPokeDexIndexArray.Add(i);
        }

        return fullPokeDexIndexArray.Union(intRange.Extras).ToArray();
    }
}
