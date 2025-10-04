import React, { createContext, useContext, useState, ReactNode } from "react";
import { GameSave } from "../models/GameSave";

interface GameSaveContextType {
    currentGameSave: GameSave | null;
    setCurrentGameSave: (gameSave: GameSave | null) => void;
    clearCurrentGameSave: () => void;
}

const GameSaveContext = createContext<GameSaveContextType | undefined>(undefined);

interface GameSaveProviderProps {
    children: ReactNode;
}

export const GameSaveProvider: React.FC<GameSaveProviderProps> = ({ children }) => {
    const [currentGameSave, setCurrentGameSave] = useState<GameSave | null>(null);

    const clearCurrentGameSave = () => {
        setCurrentGameSave(null);
    };

    const value: GameSaveContextType = {
        currentGameSave,
        setCurrentGameSave,
        clearCurrentGameSave,
    };

    return (
        <GameSaveContext.Provider value={value}>
            {children}
        </GameSaveContext.Provider>
    );
};

export const useGameSaveContext = (): GameSaveContextType => {
    const context = useContext(GameSaveContext);
    if (context === undefined) {
        throw new Error("useGameSaveContext must be used within a GameSaveProvider");
    }
    return context;
};