using PokeGame.Core.Common.Permissions;

namespace PokeGame.Core.Common.Extensions;

public static class PermissionsExtensions
{
    public static bool Can(this PermissionAbility permissionAbility, string resourceName, PermissionAbilityPermissionType permission)
    {
        return permissionAbility.ResourceName == resourceName && permissionAbility.PermissionType == permission;
    }

    public static bool Can(this IEnumerable<PermissionAbility> abilities, string resourceName, PermissionAbilityPermissionType permission)
    {
        return abilities.Any(x => Can(x, resourceName, permission));
    }
}