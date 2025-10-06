import { HubConnection } from "@microsoft/signalr";
import { createContext } from "react";
import { GameSession } from "../models/GameSession";
import { useConnectToSignalRQuery } from "../hooks/useConnectToSignalR";
import { LoadingComponent } from "../components/LoadingComponent";
import { ErrorComponent } from "../components/ErrorComponent";

const SignalRGameSessionContext = createContext<
    { hubConnection: HubConnection; gameSession: GameSession } | undefined
>(undefined);

export const SignalRGameSessionProvider: React.FC<{
    children: React.ReactNode;
}> = ({ children }) => {
    const { data, error, isLoading } = useConnectToSignalRQuery();

    if (isLoading) {
        return <LoadingComponent variant="fullscreen" />;
    }

    if (error) {
        return <ErrorComponent error={error} />;
    }

    if (!data || !data.hubConnection || !data.gameSession) {
        return (
            <ErrorComponent error="Failed to establish SignalR connection" />
        );
    }

    const connectionData = {
        hubConnection: data.hubConnection,
        gameSession: data.gameSession,
    };

    return (
        <SignalRGameSessionContext.Provider value={connectionData}>
            {children}
        </SignalRGameSessionContext.Provider>
    );
};
