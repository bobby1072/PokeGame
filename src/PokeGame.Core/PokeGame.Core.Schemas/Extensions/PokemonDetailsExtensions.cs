using BT.Common.FastArray.Proto;
using PokeGame.Core.Common.Exceptions;
using PokeGame.Core.Schemas.Common;
using PokeGame.Core.Schemas.Game;
using PokeGame.Core.Schemas.Game.PokemonRelated;
using PokeGame.Core.Schemas.PokeApi;

namespace PokeGame.Core.Schemas.Extensions;

internal static class PokemonDetailsExtensions
{
    public static PokemonInnerDetails? CreatePokemonInnerDetails(this WildPokemon wildPokemon)
    {
        if (wildPokemon.Pokemon is null ||
            wildPokemon.PokemonSpecies is null)
        {
            return null;
        }
        return new PokemonInnerDetails
        {   
            BaseExperienceFromDefeating = wildPokemon.Pokemon.BaseExperienceFromDefeating,
            Height = wildPokemon.Pokemon.Height,
            Weight = wildPokemon.Pokemon.Weight,
            IsLegendary = wildPokemon.PokemonSpecies.IsLegendary,
            Sprites = wildPokemon.Pokemon.Sprites.CreatePokemonSpriteDetails(),
            Types = wildPokemon.Pokemon.Types.FastArraySelect(x => x.CreatePokemonTypeEnum()).ToArray(),
            Stats = wildPokemon.Pokemon.Stats.FastArraySelect(x => x.CreatePokemonStatDetails()).ToArray(),
            MoveOne = wildPokemon.MoveOne?.CreatePokemonMoveDetails(),
            MoveTwo = wildPokemon.MoveTwo?.CreatePokemonMoveDetails(),
            MoveThree = wildPokemon.MoveThree?.CreatePokemonMoveDetails(),
            MoveFour = wildPokemon.MoveFour?.CreatePokemonMoveDetails(),
        };
    }
    public static PokemonInnerDetails? CreatePokemonInnerDetails(this OwnedPokemon ownedPokemon)
    {
        if (ownedPokemon.Pokemon is null ||
            ownedPokemon.PokemonSpecies is null)
        {

            return null;
        }
        
        return new PokemonInnerDetails
        {
            BaseExperienceFromDefeating = ownedPokemon.Pokemon.BaseExperienceFromDefeating,
            Height = ownedPokemon.Pokemon.Height,
            Weight = ownedPokemon.Pokemon.Weight,
            IsLegendary = ownedPokemon.PokemonSpecies.IsLegendary,
            Sprites = ownedPokemon.Pokemon.Sprites.CreatePokemonSpriteDetails(),
            Stats = ownedPokemon.Pokemon.Stats.FastArraySelect(x => x.CreatePokemonStatDetails()).ToArray(),
            Types = ownedPokemon.Pokemon.Types.FastArraySelect(x => x.CreatePokemonTypeEnum()).ToArray(),
            MoveOne = ownedPokemon.MoveOne?.CreatePokemonMoveDetails(),
            MoveTwo = ownedPokemon.MoveTwo?.CreatePokemonMoveDetails(),
            MoveThree = ownedPokemon.MoveThree?.CreatePokemonMoveDetails(),
            MoveFour = ownedPokemon.MoveFour?.CreatePokemonMoveDetails(),
            
        };
    }

    private static PokemonMoveDetails CreatePokemonMoveDetails(this Move move)
    {
        return new PokemonMoveDetails
        {
            Accuracy = move.Accuracy ?? 100,
            Healing = move.Meta.Healing,
            Power = move.Power,
            Priority = move.Priority,
            Type = Enum.Parse<PokemonTypeEnum>(move.Type.Name, true),
            AilmentChance = move.Meta.AilmentChance,
            AilmentName = move.Meta.Ailment.Name,
            CritRate = move.Meta.CritRate,
            DamageClass = Enum.Parse<DamageClassTypeEnum>(move.DamageClass.Name, true),
            EffectChance = move.EffectChance,
            FlinchChance = move.Meta.FlinchChance,
            MoveName = move.Name,
            PowerPoints = move.PowerPoints,
            StatChance = move.Meta.StatChance,
            MaxTurns = move.Meta.MaxTurns,
            MinTurns = move.Meta.MinTurns,
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