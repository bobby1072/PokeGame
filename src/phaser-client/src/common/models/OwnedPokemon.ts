// Minimal type for Move data - essential fields without NamedApiResource nesting
export type MinimalMove = {
    id: number;
    name: string;
    accuracy: number | null;
    effectChance: number | null;
    pp: number | null;
    power: number | null;
    priority: number;
    damageClass: {
        name: string;
    };
    type: {
        name: string;
    };
};

// Minimal type for PokemonSpecies - essential fields without NamedApiResource nesting
export type MinimalPokemonSpecies = {
    id: number;
    name: string;
    order: number;
    genderRate: number;
    captureRate: number | null;
    baseHappiness: number | null;
    isBaby: boolean;
    isLegendary: boolean;
    isMythical: boolean;
    hatchCounter: number | null;
    hasGenderDifferences: boolean;
    formsSwitchable: boolean;
};

// Minimal type for Pokemon - essential fields without NamedApiResource nesting
export type MinimalPokemon = {
    id: number;
    name: string;
    baseExperience: number | null;
    height: number;
    isDefault: boolean;
    order: number;
    weight: number;
    sprites: {
        frontDefault: string | null;
        frontShiny: string | null;
        backDefault: string | null;
        backShiny: string | null;
    };
    stats: Array<{
        baseStat: number;
        effort: number;
        stat: {
            name: string;
        };
    }>;
    types: Array<{
        slot: number;
        type: {
            name: string;
        };
    }>;
};

// Unified type for owned Pokemon - starts as shallow, can be enriched with deep data
export type OwnedPokemon = {
    Id: string;
    GameSaveId: string;
    PokemonResourceName: string;
    CaughtAt: Date;
    PokemonLevel: number;
    CurrentExperience: number;
    CurrentHp: number;
    MoveOneResourceName: string;
    MoveOne?: MinimalMove; // Only populated when deep data is fetched
    MoveTwoResourceName?: string;
    MoveTwo?: MinimalMove; // Only populated when deep data is fetched
    MoveThreeResourceName?: string;
    MoveThree?: MinimalMove; // Only populated when deep data is fetched
    MoveFourResourceName?: string;
    MoveFour?: MinimalMove; // Only populated when deep data is fetched
    PokemonSpecies?: MinimalPokemonSpecies; // Only populated when deep data is fetched
    Pokemon?: MinimalPokemon; // Only populated when deep data is fetched
};
