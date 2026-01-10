import { PokemonInnerDetails } from './PokemonDetails';

export type WildPokemon = {
    pokemonResourceName: string;
    pokemonLevel: number;
    currentHp: number;
    moveOneResourceName?: string;
    moveTwoResourceName?: string;
    moveThreeResourceName?: string;
    moveFourResourceName?: string;
    innerDetails: PokemonInnerDetails;
};
