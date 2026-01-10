export type PokemonSpriteDetails = {
    frontDefault: string;
    frontShiny: string;
    backDefault: string;
    backShiny: string;
    frontFemale?: string;
    frontShinyFemale?: string;
    backFemale?: string;
    backShinyFemale?: string;
};

export type PokemonStatDetails = {
    name: string;
    baseStat: number;
};

export type PokemonTypeDetails = {
    name: string;
};

export type PokemonInnerDetails = {
    baseExperienceFromDefeating: number;
    height: number;
    weight: number;
    sprites: PokemonSpriteDetails;
    stats: PokemonStatDetails[];
    types: PokemonTypeDetails[];
    isLegendary: boolean;
};
