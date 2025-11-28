import { Box, Typography, Button } from "@mui/material";
import { PokemonPhaserGame } from "./PokemonPhaserGame";
import { useGameSaveContext } from "../common/contexts/GameSaveContext";
import { useSignalRGameSession } from "../common/contexts/SignalRGameSessionContext";
import { useGetShallowOwnedPokemonInDeck } from "../common/hooks/useGetShallowOwnedPokemonInDeck";
import { LoadingComponent } from "../common/components/LoadingComponent";
import { ErrorComponent } from "../common/components/ErrorComponent";

export default function PokemonPage() {
    const { currentGameSave, clearCurrentGameSave } = useGameSaveContext();
    const { hubConnection, gameSession } = useSignalRGameSession();

    const {
        data: shallowDeck,
        isLoading,
        error,
    } = useGetShallowOwnedPokemonInDeck(gameSession.id);

    const handleChangeGameSave = () => {
        clearCurrentGameSave();
    };

    if (isLoading) {
        return <LoadingComponent variant="fullscreen" />;
    }

    if (error) {
        return <ErrorComponent error={error} variant="fullscreen" />;
    }

    return (
        <Box
            sx={{
                width: "100%",
                height: "100vh",
                display: "flex",
                flexDirection: "column",
                bgcolor: "background.default",
            }}
        >
            {/* Fixed Game Header */}
            <Box
                sx={{
                    flexShrink: 0, // Prevent header from shrinking
                    p: 2,
                    display: "flex",
                    alignItems: "center",
                    gap: 2,
                    flexWrap: "wrap",
                    justifyContent: "center",
                    borderBottom: 1,
                    borderColor: "divider",
                    bgcolor: "background.paper",
                    boxShadow: 1,
                }}
            >
                <Typography variant="h6" component="div">
                    Playing as:{" "}
                    <strong>{currentGameSave?.characterName}</strong>
                </Typography>
                <Button
                    variant="outlined"
                    size="small"
                    onClick={handleChangeGameSave}
                >
                    Change Game Save
                </Button>
                <Button variant="text" size="small" href="/">
                    Back to Default Game
                </Button>
            </Box>

            {/* Game Container - Takes remaining space */}
            <Box
                sx={{
                    flex: 1, // Take remaining space after header
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center", // Center the scaled game
                    overflow: "hidden", // Prevent scrollbars
                    p: 1, // Padding around the game
                    "& #pokemon-game-container": {
                        maxWidth: "100%",
                        maxHeight: "100%",
                        aspectRatio: "1024/768", // Maintain game aspect ratio
                        "& canvas": {
                            width: "100% !important",
                            height: "100% !important",
                            objectFit: "contain", // Scale to fit while maintaining aspect ratio
                        },
                    },
                }}
            >
                <PokemonPhaserGame
                    hubConnection={hubConnection}
                    currentGameSave={currentGameSave}
                    shallowDeck={shallowDeck}
                />
            </Box>
        </Box>
    );
}
