export enum GameDataActualUnlockedGameResourceType {
    Scene = "Scene",
}

export type GameDataActualUnlockedGameResource = {
    type: GameDataActualUnlockedGameResourceType;
    resourceName: string;
};

export type GameSaveDataActual = {
    lastPlayedScene: string;
    lastPlayedLocationX: number;
    lastPlayedLocationY: number;
    deckPokemon: { ownedPokemonId: string }[];
    unlockedGameResources: GameDataActualUnlockedGameResource[];
};
