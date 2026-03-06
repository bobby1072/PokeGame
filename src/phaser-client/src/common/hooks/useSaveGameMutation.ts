import { useMutation } from "@tanstack/react-query";
import { HubConnection } from "@microsoft/signalr";
import { GameSaveData } from "../models/GaveSaveData";

export const useSaveGameMutation = (hubConnection?: HubConnection) => {
    const mutationResults = useMutation<void, Error, GameSaveData>({
        mutationFn: async (gameSaveData) => {
            if (!hubConnection) {
                throw new Error("No SignalR hub connection available");
            }
            await hubConnection.invoke("SaveGame", gameSaveData);
        },
    });

    return { ...mutationResults };
};
