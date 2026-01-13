import { PokemonInnerDetails } from "./PokemonDetails";

export type OwnedPokemon = {
    id: string;
    gameSaveId: string;
    pokemonResourceName: string;
    caughtAt: string;
    pokemonLevel: number;
    currentExperience: number;
    currentHp: number;
    moveOneResourceName?: string;
    moveTwoResourceName?: string;
    moveThreeResourceName?: string;
    moveFourResourceName?: string;
    innerDetails: PokemonInnerDetails;
};
