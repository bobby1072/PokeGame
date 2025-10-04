import { Box, Typography, Button, Chip } from "@mui/material";
import { PokemonPhaserGame } from "./PokemonPhaserGame";
import { useGameSaveContext } from "../common/contexts/GameSaveContext";

export default function PokemonPage() {
    const { currentGameSave, clearCurrentGameSave } = useGameSaveContext();

    const handleChangeGameSave = () => {
        clearCurrentGameSave();
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString("en-US", {
            month: "short",
            day: "numeric",
            hour: "2-digit",
            minute: "2-digit",
        });
    };

    return (
        <Box
            sx={{
                width: "100%",
                height: "100vh",
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "center",
                bgcolor: "background.default",
            }}
        >
            {/* Game Header */}
            <Box
                sx={{
                    mb: 2,
                    display: "flex",
                    alignItems: "center",
                    gap: 2,
                    flexWrap: "wrap",
                    justifyContent: "center",
                }}
            >
                <Typography variant="h6" component="div">
                    Playing as: <strong>{currentGameSave?.characterName}</strong>
                </Typography>
                <Chip
                    label={`Last played: ${formatDate(currentGameSave?.lastPlayed || "")}`}
                    size="small"
                    color="primary"
                    variant="outlined"
                />
                <Button
                    variant="outlined"
                    size="small"
                    onClick={handleChangeGameSave}
                >
                    Change Game Save
                </Button>
                <Button
                    variant="text"
                    size="small"
                    href="/"
                >
                    Back to Default Game
                </Button>
            </Box>

            {/* Game Container */}
            <Box
                sx={{
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    flexGrow: 1,
                    width: "100%",
                }}
            >
                <PokemonPhaserGame />
            </Box>
        </Box>
    );
}
