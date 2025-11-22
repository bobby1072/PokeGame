import { GameSaveData } from "./GaveSaveData";

export type GameSave = {
    id?: string;
    userId: string;
    characterName: string;
    dateCreated: string;
    lastPlayed: string;
    gameSaveData: GameSaveData;
};
