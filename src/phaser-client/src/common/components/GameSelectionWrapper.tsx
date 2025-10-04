import React from "react";
import { useGameSaveContext } from "../contexts/GameSaveContext";
import { GameSaveSelectionPage } from "../pages/GameSaveSelectionPage";
import { GameSave } from "../models/GameSave";

interface GameSelectionWrapperProps {
    children: React.ReactNode;
}

export const GameSelectionWrapper: React.FC<GameSelectionWrapperProps> = ({ children }) => {
    const { currentGameSave, setCurrentGameSave } = useGameSaveContext();

    const handleGameSaveSelected = (gameSave: GameSave) => {
        setCurrentGameSave(gameSave);
    };

    // If no game save is selected, show the selection page
    if (!currentGameSave) {
        return (
            <GameSaveSelectionPage onGameSaveSelected={handleGameSaveSelected} />
        );
    }

    // If game save is selected, render the children (the actual game)
    return <>{children}</>;
};