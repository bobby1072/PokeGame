namespace PokeGame.Core.Common.Permissions;

public sealed record PermissionAbility
{
    public required string ResourceName { get; init; }
    public required PermissionAbilityPermissionType PermissionType { get; init; }
}