export enum PermissionAbilityPermissionType {
    Read = "Read",
    Create = "Create",
    Manage = "Manage",
}

export type PermissionAbility = {
    resourceName: string;
    permissionType: PermissionAbilityPermissionType;
};
