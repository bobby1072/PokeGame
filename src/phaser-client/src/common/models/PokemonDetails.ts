import { PokemonTypeEnum } from "../enums/PokemonTypeEnum";
import { PokemonStatEnum } from "../enums/PokemonStatEnum";
import { DamageClassTypeEnum } from "../enums/DamageClassTypeEnum";

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
    name: PokemonStatEnum;
    baseStat: number;
};

export type PokemonMoveDetails = {
    moveName: string;
    accuracy: number;
    effectChance: number | null;
    powerPoints: number | null;
    priority: number;
    power: number | null;
    damageClass: DamageClassTypeEnum;
    ailmentName: string;
    ailmentChance: number;
    flinchChance: number;
    critRate: number;
    statChance: number;
    minTurns: number | null;
    maxTurns: number | null;
    healing: number;
    type: PokemonTypeEnum;
};

export type PokemonInnerDetails = {
    baseExperienceFromDefeating: number;
    height: number;
    weight: number;
    sprites: PokemonSpriteDetails;
    stats: PokemonStatDetails[];
    types: PokemonTypeEnum[];
    isLegendary: boolean;
    moveOne?: PokemonMoveDetails;
    moveTwo?: PokemonMoveDetails;
    moveThree?: PokemonMoveDetails;
    moveFour?: PokemonMoveDetails;
};
