using BT.Common.FastArray.Proto;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Schemas.Common;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Schemas.Extensions;

internal static class PokemonDetailsExtensions
{
    public static PokemonInnerDetails CreatePokemonInnerDetails(this WildPokemon wildPokemon)
    {
        if (wildPokemon.Pokemon is null ||
            wildPokemon.PokemonSpecies is null)
        {
            throw new PokeGameApiServerException(
                "Failed to create pokemon inner details as owned pokemon missing data");
        }

        return new PokemonInnerDetails
        {   
            BaseExperienceFromDefeating = wildPokemon.Pokemon.BaseExperienceFromDefeating,
            Height = wildPokemon.Pokemon.Height,
            Weight = wildPokemon.Pokemon.Weight,
            IsLegendary = wildPokemon.PokemonSpecies.IsLegendary,
            Sprites = wildPokemon.Pokemon.Sprites.CreatePokemonSpriteDetails(),
            Types = wildPokemon.Pokemon.Types.FastArraySelect(x => x.CreatePokemonTypeEnum()).ToArray(),
            Stats = wildPokemon.Pokemon.Stats.FastArraySelect(x => x.CreatePokemonStatDetails()).ToArray()
        };
    }
    public static PokemonInnerDetails CreatePokemonInnerDetails(this OwnedPokemon ownedPokemon)
    {
        if (ownedPokemon.Pokemon is null ||
            ownedPokemon.PokemonSpecies is null)
        {
            throw new PokeGameApiServerException(
                "Failed to create pokemon inner details as owned pokemon missing data");
        }
        
        return new PokemonInnerDetails
        {
            BaseExperienceFromDefeating = ownedPokemon.Pokemon.BaseExperienceFromDefeating,
            Height = ownedPokemon.Pokemon.Height,
            Weight = ownedPokemon.Pokemon.Weight,
            IsLegendary = ownedPokemon.PokemonSpecies.IsLegendary,
            Sprites = ownedPokemon.Pokemon.Sprites.CreatePokemonSpriteDetails(),
            Stats = ownedPokemon.Pokemon.Stats.FastArraySelect(x => x.CreatePokemonStatDetails()).ToArray(),
            Types = ownedPokemon.Pokemon.Types.FastArraySelect(x => x.CreatePokemonTypeEnum()).ToArray()
        };
    }

    private static PokemonTypeEnum CreatePokemonTypeEnum(this PokemonType pokemonType)
    {
        return Enum.Parse<PokemonTypeEnum>(pokemonType.Type.Name, true);
    }
    private static PokemonStatDetails CreatePokemonStatDetails(this PokemonStat pokeStat)
    {
        return new PokemonStatDetails
        {
            Name = Enum.Parse<PokemonStatEnum>(pokeStat.Stat.Name, true),
            BaseStat = pokeStat.BaseStat,
        };
    }

    private static PokemonSpriteDetails CreatePokemonSpriteDetails(this PokemonSprites pokemon)
    {
        return new PokemonSpriteDetails
        {
            BackDefault = pokemon.BackDefault,
            BackShiny = pokemon.BackShiny,
            FrontDefault = pokemon.FrontDefault,
            FrontShiny = pokemon.FrontShiny,
            BackFemale = pokemon.BackFemale,
            BackShinyFemale = pokemon.BackShinyFemale,
            FrontFemale = pokemon.FrontFemale,
            FrontShinyFemale = pokemon.FrontShinyFemale,
        };
    }
}