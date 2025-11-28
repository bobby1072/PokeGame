export type ShallowOwnedPokemon = {
    Id: string;
    GameSaveId: string;
    PokemonResourceName: string;
    CaughtAt: Date;
    PokemonLevel: number;
    CurrentExperience: number;
    CurrentHp: number;
    MoveOneResourceName: string;
    MoveTwoResourceName?: string;
    MoveThreeResourceName?: string;
    MoveFourResourceName?: string;
};
