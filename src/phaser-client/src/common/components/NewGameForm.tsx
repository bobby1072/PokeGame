import React, { useState } from "react";
import {
    Paper,
    TextField,
    Button,
    Typography,
    Box,
    CircularProgress,
} from "@mui/material";
import { useSaveNewGameMutation } from "../hooks/useSaveNewGameMutation";
import { GameSave } from "../models/GameSave";
import { ErrorComponent } from "./ErrorComponent";

interface NewGameFormProps {
    onGameCreated: (gameSave: GameSave) => void;
    onCancel: () => void;
}

export const NewGameForm: React.FC<NewGameFormProps> = ({ onGameCreated, onCancel }) => {
    const [characterName, setCharacterName] = useState("");
    const [isCreating, setIsCreating] = useState(false);
    const saveNewGameMutation = useSaveNewGameMutation();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!characterName.trim()) return;

        setIsCreating(true);
        try {
            const newGameSave = await saveNewGameMutation.mutateAsync({
                input: { newCharacterName: characterName.trim() }
            });
            onGameCreated(newGameSave);
        } catch (error) {
            console.error("Failed to create new game:", error);
        } finally {
            setIsCreating(false);
        }
    };

    return (
        <Paper
            elevation={3}
            sx={{
                p: 3,
                mb: 3,
                border: 2,
                borderColor: "primary.main",
                backgroundColor: "primary.50",
            }}
        >
            <Typography variant="h5" component="h3" gutterBottom>
                Create New Game
            </Typography>
            
            <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2 }}>
                <TextField
                    fullWidth
                    id="characterName"
                    label="Character Name"
                    value={characterName}
                    onChange={(e) => setCharacterName(e.target.value)}
                    placeholder="Enter your character name"
                    disabled={isCreating}
                    required
                    inputProps={{ maxLength: 50 }}
                    sx={{ mb: 3 }}
                    helperText={`${characterName.length}/50 characters`}
                />
                
                <Box sx={{ display: "flex", gap: 2, justifyContent: "flex-end" }}>
                    <Button
                        variant="outlined"
                        onClick={onCancel}
                        disabled={isCreating}
                        size="large"
                    >
                        Cancel
                    </Button>
                    <Button
                        type="submit"
                        variant="contained"
                        disabled={!characterName.trim() || isCreating}
                        size="large"
                        startIcon={isCreating ? <CircularProgress size={20} /> : null}
                    >
                        {isCreating ? "Creating..." : "Create Game"}
                    </Button>
                </Box>
            </Box>
            
            <ErrorComponent error={saveNewGameMutation.error} />
        </Paper>
    );
};