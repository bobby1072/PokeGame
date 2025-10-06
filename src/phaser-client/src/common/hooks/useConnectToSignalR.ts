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
    isWaitingForGameSession: boolean;
}

const hubConnectBuilder = new HubConnectionBuilder()
    .withAutomaticReconnect()
    .configureLogging(
        process.env.NODE_ENV === "development" ? LogLevel.Debug : LogLevel.None
    );

export const useConnectToSignalRQuery = () => {
    const applicationSettings = useGetAppSettingsContext();
    const gameSave = useGameSaveContext();
    const [state, setState] = useState<SignalRState>({
        isWaitingForGameSession: false,
    });
    
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
        throwOnError: false,
    });

    // Handle successful connection
    useEffect(() => {
        if (query.data && !state.hubConnection) {
            setState((prev) => ({
                ...prev,
                hubConnection: query.data,
                isWaitingForGameSession: true,
            }));
        }
    }, [query.data, state.hubConnection]);

    // Handle query error
    useEffect(() => {
        if (query.error) {
            setState((prev) => ({ 
                ...prev, 
                error: query.error,
                isWaitingForGameSession: false,
            }));
        }
    }, [query.error]);

    // Setup SignalR event listeners
    useEffect(() => {
        if (state.hubConnection) {
            const handleGameSessionStarted = (data: WebOutcome<GameSession>) => {
                setState((prev) => ({
                    ...prev,
                    gameSession:
                        data.isSuccess && data.data ? data.data : undefined,
                    isWaitingForGameSession: false,
                }));
            };

            const handleGameSessionConnectionFailed = (data: BaseWebOutcome) => {
                setState((prev) => ({
                    ...prev,
                    error: new Error(
                        data.exceptionMessage || "Unknown error"
                    ),
                    isWaitingForGameSession: false,
                }));
            };

            state.hubConnection.on(
                SignalREventKeys.GameSessionStarted,
                handleGameSessionStarted
            );
            state.hubConnection.on(
                SignalREventKeys.GameSessionConnectionFailed,
                handleGameSessionConnectionFailed
            );
        }
    }, [state.hubConnection]);

    return {
        ...query,
        data: {
            hubConnection: state.hubConnection,
            gameSession: state.gameSession,
        },
        error: state.error || query.error,
        isLoading: query.isLoading || state.isWaitingForGameSession,
    };
};
