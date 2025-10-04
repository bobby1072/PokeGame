import { createContext, useContext, useState } from "react";
import { PokeGameUser } from "../models/PokeGameUser";
import { LoginPage } from "../pages/LoginPage";
import { GameSaveProvider } from "./GameSaveContext";
import { GameSelectionWrapper } from "../components/GameSelectionWrapper";

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

    // Step 1: If no user is logged in, show login page
    if (!currentUser) return <LoginPage setUser={(u) => setCurrentUser(u)} />;

    // Step 2: If user is logged in, wrap with GameSaveProvider and show game save selection
    return (
        <PokeGameUserContext.Provider value={currentUser}>
            <GameSaveProvider>
                <GameSelectionWrapper>
                    {children}
                </GameSelectionWrapper>
            </GameSaveProvider>
        </PokeGameUserContext.Provider>
    );
};
