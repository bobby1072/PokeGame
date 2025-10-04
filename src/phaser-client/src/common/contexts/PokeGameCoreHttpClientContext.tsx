import { createContext, useContext, useMemo, useEffect } from "react";
import PokeGameCoreHttpClient from "../utils/PokeGameCoreHttpClient";
import { useGetAppSettingsContext } from "./AppSettingsContext";

export const PokeGameCoreHttpClientContext = createContext<
    PokeGameCoreHttpClient | undefined
>(undefined);

export const useGetPokeGameHttpClientContext = () => {
    const value = useContext(PokeGameCoreHttpClientContext);

    if (!value) {
        throw new Error("No http client registered");
    }

    return value;
};

interface PokeGameCoreHttpClientContextProviderProps {
    children: React.ReactNode;
    userId?: string;
}

export const PokeGameCoreHttpClientContextProvider: React.FC<PokeGameCoreHttpClientContextProviderProps> = ({ 
    children, 
    userId 
}) => {
    const settings = useGetAppSettingsContext();

    const httpClient = useMemo(() => {
        return new PokeGameCoreHttpClient(settings.pokeGameCoreApiUrl);
    }, [settings.pokeGameCoreApiUrl]);

    // Update the user ID whenever it changes
    useEffect(() => {
        httpClient.setUserId(userId || null);
    }, [httpClient, userId]);

    return (
        <PokeGameCoreHttpClientContext.Provider value={httpClient}>
            {children}
        </PokeGameCoreHttpClientContext.Provider>
    );
};
