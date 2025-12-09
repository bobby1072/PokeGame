using System.Text.Json.Serialization;
using BT.Common.FastArray.Proto;
using PokeGame.Core.Common.Permissions;

namespace PokeGame.Core.Schemas.Game;

public sealed class GameSaveDataActual: DomainModel<GameSaveDataActual>
{
    public required string LastPlayedScene  { get; set; }
    public required int LastPlayedLocationX { get; set; }
    public required int LastPlayedLocationY { get; set; }
    public List<GameSaveDataActualDeckPokemon> DeckPokemon { get; set; } = [];
    public List<GameDataActualUnlockedGameResource> UnlockedGameResources { get; set; } = [];


    private List<PermissionAbility>? _abilities;
    
    [JsonIgnore]
    public List<PermissionAbility> Abilities
    {
        get
        {
            return _abilities ??= BuildAbilities();
        }
    }
    
    public override bool Equals(GameSaveDataActual? other)
    {
        return LastPlayedScene == other?.LastPlayedScene &&
               LastPlayedLocationX == other.LastPlayedLocationX &&
               LastPlayedLocationY == other.LastPlayedLocationY &&
               DeckPokemon.SequenceEqual(other.DeckPokemon) &&
               UnlockedGameResources.SequenceEqual(other.UnlockedGameResources);;
    }


    private List<PermissionAbility> BuildAbilities()
    {
        return UnlockedGameResources
            .FastArraySelect(x => new PermissionAbility
            {
                PermissionType = PermissionAbilityPermissionType.Read,
                ResourceName = x.ResourceName,
            }).ToList();
    }
}