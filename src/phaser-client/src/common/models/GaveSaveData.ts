import { GameSaveDataActual } from "./GameSaveDataActual";

export type GameSaveData = {
    id: number;
    gameSaveId: string;
    gameData: GameSaveDataActual;
};
