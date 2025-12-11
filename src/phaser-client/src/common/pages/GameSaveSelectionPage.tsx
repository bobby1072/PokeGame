import React, { useState } from "react";
import { Typography, Button, Box, Alert, Container } from "@mui/material";
import { useGetAllGameSavesQuery } from "../hooks/useGetAllGameSavesQuery";
import { useGameSaveContext } from "../contexts/GameSaveContext";
import { GameSaveCard } from "../components/GameSaveCard";
import { NewGameForm } from "../components/NewGameForm";
import { PageBase } from "../components/PageBase";
import { LoadingComponent } from "../components/LoadingComponent";
import { GameSave } from "../models/GameSave";

interface GameSaveSelectionPageProps {
    onGameSaveSelected?: (gameSave: GameSave) => void;
}

export const GameSaveSelectionPage: React.FC<GameSaveSelectionPageProps> = ({
    onGameSaveSelected,
}) => {
    const [showNewGameForm, setShowNewGameForm] = useState(false);
    const {
        data: gameSaves,
        isLoading,
        error,
        refetch,
    } = useGetAllGameSavesQuery();
    const { setCurrentGameSave } = useGameSaveContext();

    const handleSelectGameSave = (gameSave: GameSave) => {
        setCurrentGameSave(gameSave);
        onGameSaveSelected?.(gameSave);
    };

    const handleGameCreated = (newGameSave: GameSave) => {
        setShowNewGameForm(false);
        refetch(); // Refresh the list
        handleSelectGameSave(newGameSave); // Auto-select the newly created game
    };

    const handleCancelNewGame = () => {
        setShowNewGameForm(false);
    };

    if (isLoading) {
        return (
            <PageBase>
                <LoadingComponent
                    variant="page-section"
                    message="Loading game saves..."
                    data-testid="game-saves-loading"
                />
            </PageBase>
        );
    }

    if (error) {
        return (
            <PageBase>
                <Box
                    sx={{
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "center",
                        minHeight: "400px",
                        gap: 2,
                        pt: 4,
                    }}
                    data-testid="game-saves-error"
                >
                    <Alert severity="error" sx={{ mb: 2 }}>
                        Error loading game saves: {error.message}
                    </Alert>
                    <Button
                        variant="contained"
                        onClick={() => refetch()}
                        size="large"
                        data-testid="retry-button"
                    >
                        Try Again
                    </Button>
                </Box>
            </PageBase>
        );
    }

    return (
        <PageBase>
            <Container maxWidth="lg">
                {/* Header */}
                <Box sx={{ textAlign: "center", mb: 4 }}>
                    <Typography variant="h3" component="h1" gutterBottom>
                        Select Game Save
                    </Typography>
                    <Typography variant="h6" color="text.secondary">
                        Choose an existing game save or create a new one to
                        continue playing
                    </Typography>
                </Box>

                {/* New Game Form */}
                {showNewGameForm && (
                    <NewGameForm
                        onGameCreated={handleGameCreated}
                        onCancel={handleCancelNewGame}
                    />
                )}

                {/* Create New Game Button */}
                {!showNewGameForm && (
                    <Box sx={{ textAlign: "center", mb: 4 }}>
                        <Button
                            variant="contained"
                            size="large"
                            onClick={() => setShowNewGameForm(true)}
                            data-testid="create-new-game-button"
                            sx={{
                                py: 2,
                                px: 4,
                                fontSize: "1.2rem",
                                fontWeight: "bold",
                            }}
                        >
                            + Create New Game
                        </Button>
                    </Box>
                )}

                {/* Existing Game Saves */}
                {gameSaves && gameSaves.length > 0 && (
                    <Box>
                        <Typography
                            variant="h4"
                            component="h2"
                            sx={{ mb: 3, textAlign: "center" }}
                        >
                            Existing Game Saves ({gameSaves.length})
                        </Typography>
                        <Box
                            sx={{
                                display: "grid",
                                gridTemplateColumns: {
                                    xs: "1fr",
                                    sm: "repeat(2, 1fr)",
                                    md: "repeat(3, 1fr)",
                                },
                                gap: 3,
                            }}
                            data-testid="game-saves-grid"
                        >
                            {gameSaves.map((gameSave) => (
                                <GameSaveCard
                                    key={gameSave.id}
                                    gameSave={gameSave}
                                    onSelect={handleSelectGameSave}
                                />
                            ))}
                        </Box>
                    </Box>
                )}

                {/* No Game Saves Message */}
                {gameSaves && gameSaves.length === 0 && !showNewGameForm && (
                    <Box
                        sx={{
                            textAlign: "center",
                            py: 6,
                            px: 3,
                        }}
                        data-testid="no-game-saves-message"
                    >
                        <Typography variant="h5" gutterBottom>
                            No game saves found
                        </Typography>
                        <Typography variant="body1" color="text.secondary">
                            Create your first game save to start your Pokémon
                            adventure!
                        </Typography>
                    </Box>
                )}
            </Container>
        </PageBase>
    );
};
