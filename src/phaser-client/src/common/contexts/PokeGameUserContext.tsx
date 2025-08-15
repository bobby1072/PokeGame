import { createContext, useContext, useState } from "react";
import { PokeGameUser } from "../models/PokeGameUser";
import { LoginPage } from "../pages/LoginPage";

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

    if (!currentUser) return <LoginPage setUser={(u) => setCurrentUser(u)} />;

    return (
        <PokeGameUserContext.Provider value={currentUser}>
            {children}
        </PokeGameUserContext.Provider>
    );
};
