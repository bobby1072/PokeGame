import { createContext, useContext, useState, useEffect } from "react";
import { PokeGameUser } from "../models/PokeGameUser";
import { LoginPage } from "../pages/LoginPage";
import { GameSaveProvider } from "./GameSaveContext";
import { GameSelectionWrapper } from "../components/GameSelectionWrapper";
import { useGetPokeGameHttpClientContext } from "./PokeGameCoreHttpClientContext";

export const PokeGameUserContext = createContext<PokeGameUser | undefined>(
    undefined
);

export const useGetPokeGameUserContext = () => {
    const value = useContext(PokeGameUserContext);

    if (!value) {
        throw new Error("No user registered");
    }

    return value;
};

export const PokeGameUserContextProvider: React.FC<{
    children: React.ReactNode;
}> = ({ children }) => {
    const [currentUser, setCurrentUser] = useState<PokeGameUser | undefined>(
        undefined
    );
    const httpClient = useGetPokeGameHttpClientContext();

    // Update HTTP client with user ID whenever user changes
    useEffect(() => {
        httpClient.setUserId(currentUser?.id || null);
    }, [httpClient, currentUser?.id]);

    const handleSetUser = (user: PokeGameUser) => {
        // Set user ID in HTTP client immediately when user is set
        httpClient.setUserId(user.id || null);
        setCurrentUser(user);
    };

    return (
        <PokeGameUserContext.Provider value={currentUser}>
            {!currentUser ? (
                // Step 1: If no user is logged in, show login page
                <LoginPage setUser={handleSetUser} />
            ) : (
                // Step 2: If user is logged in, wrap with GameSaveProvider and show game save selection
                <GameSaveProvider>
                    <GameSelectionWrapper>
                        {children}
                    </GameSelectionWrapper>
                </GameSaveProvider>
            )}
        </PokeGameUserContext.Provider>
    );
};
