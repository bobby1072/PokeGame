import React, { useEffect } from "react";
import { useGameSaveContext } from "../contexts/GameSaveContext";
import { GameSaveSelectionPage } from "../pages/GameSaveSelectionPage";
import { GameSave } from "../models/GameSave";
import { useGetPokeGameUserContext } from "../contexts/PokeGameUserContext";
import { useGetPokeGameHttpClientContext } from "../contexts/PokeGameCoreHttpClientContext";

interface GameSelectionWrapperProps {
    children: React.ReactNode;
}

export const GameSelectionWrapper: React.FC<GameSelectionWrapperProps> = ({ children }) => {
    const { currentGameSave, setCurrentGameSave } = useGameSaveContext();
    const currentUser = useGetPokeGameUserContext();
    const httpClient = useGetPokeGameHttpClientContext();

    // Ensure HTTP client has user ID before rendering
    useEffect(() => {
        if (currentUser?.id) {
            httpClient.setUserId(currentUser.id);
        }
    }, [httpClient, currentUser?.id]);

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