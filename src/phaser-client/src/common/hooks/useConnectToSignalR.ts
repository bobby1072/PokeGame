import { useQuery } from "@tanstack/react-query";
import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel,
} from "@microsoft/signalr";
import { QueryKeys } from "./QueryKeys";
import { useGetAppSettingsContext } from "../contexts/AppSettingsContext";
import { useGameSaveContext } from "../contexts/GameSaveContext";
import { useEffect, useState } from "react";
import { SignalREventKeys } from "./SignalREventKeys";
import { GameSession } from "../models/GameSession";
import { BaseWebOutcome, WebOutcome } from "../models/WebOutcome";

interface SignalRState {
    error?: Error;
    hubConnection?: HubConnection;
    gameSession?: GameSession;
}

const hubConnectBuilder = new HubConnectionBuilder()
    .withAutomaticReconnect()
    .configureLogging(
        process.env.NODE_ENV === "development" ? LogLevel.Debug : LogLevel.None
    );

export const useConnectToSignalRQuery = () => {
    const applicationSettings = useGetAppSettingsContext();
    const gameSave = useGameSaveContext();
    const [state, setState] = useState<SignalRState>({});
    const query = useQuery<HubConnection, Error>({
        queryKey: [QueryKeys.ConnectToSignalR],
        queryFn: async () => {
            const localHub = hubConnectBuilder
                .withUrl(
                    `${applicationSettings.pokeGameCoreSignalRUrl}/Api/SignalR/PokeGameSession?GameSaveId=${gameSave.currentGameSave?.id}&UserId=${gameSave.currentGameSave?.userId}`
                )
                .build();

            await localHub.start();
            return localHub;
        },
        select: (data) => {
            setState((prev) => ({ ...prev, hubConnection: data }));
            return data;
        },
    });
    useEffect(() => {
        if (query.error) {
            setState((prev) => ({ ...prev, error: query.error }));
        }
    }, [query.error]);
    useEffect(() => {
        if (state.hubConnection) {
            state.hubConnection.on(
                SignalREventKeys.GameSessionStarted,
                (data: WebOutcome<GameSession>) => {
                    setState((prev) => ({
                        ...prev,
                        gameSession:
                            data.isSuccess && data.data ? data.data : undefined,
                    }));
                }
            );
            state.hubConnection.on(
                SignalREventKeys.GameSessionConnectionFailed,
                (data: BaseWebOutcome) => {
                    setState((prev) => ({
                        ...prev,
                        error: new Error(
                            data.exceptionMessage || "Unknown error"
                        ),
                    }));
                }
            );
        }
    }, [state.hubConnection]);

    return {
        ...query,
        data: {
            hubConnection: state.hubConnection,
            gameSession: state.gameSession,
        },
        error: state.error,
    };
};
