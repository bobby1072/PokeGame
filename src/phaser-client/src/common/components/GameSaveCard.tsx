import React from "react";
import {
    Card,
    CardActionArea,
    CardContent,
    Typography,
    Box,
    Chip,
} from "@mui/material";
import { GameSave } from "../models/GameSave";

interface GameSaveCardProps {
    gameSave: GameSave;
    onSelect: (gameSave: GameSave) => void;
}

export const GameSaveCard: React.FC<GameSaveCardProps> = ({
    gameSave,
    onSelect,
}) => {
    const formatDate = (dateString: string) => {
        const stringAsDate = new Date(dateString);
        return `${stringAsDate.toLocaleDateString()} ${stringAsDate.toLocaleTimeString()}`;
    };

    const formatRelativeTime = (dateString: string) => {
        const date = new Date(dateString);
        const now = new Date();
        const diffInMinutes = Math.floor(
            (now.getTime() - date.getTime()) / (1000 * 60)
        );

        if (diffInMinutes < 60) {
            return `${diffInMinutes} minutes ago`;
        }
        const diffInHours = Math.floor(diffInMinutes / 60);
        if (diffInHours < 24) {
            return `${diffInHours} hours ago`;
        }
        const diffInDays = Math.floor(diffInHours / 24);
        return `${diffInDays} days ago`;
    };

    return (
        <Card
            sx={{
                height: "100%",
                transition: "all 0.2s ease-in-out",
                "&:hover": {
                    transform: "translateY(-4px)",
                    boxShadow: 4,
                },
            }}
        >
            <CardActionArea
                onClick={() => onSelect(gameSave)}
                sx={{ height: "100%" }}
            >
                <CardContent>
                    <Box
                        sx={{
                            display: "flex",
                            flexDirection: "column",
                            gap: 2,
                        }}
                    >
                        <Typography variant="h6" component="div" noWrap>
                            {gameSave.characterName}
                        </Typography>

                        <Box
                            sx={{
                                display: "flex",
                                flexDirection: "column",
                                gap: 1,
                            }}
                        >
                            <Box
                                sx={{
                                    display: "flex",
                                    alignItems: "center",
                                    gap: 1,
                                }}
                            >
                                <Typography
                                    variant="body2"
                                    color="text.secondary"
                                >
                                    Created:
                                </Typography>
                                <Typography variant="body2">
                                    {formatDate(gameSave.dateCreated)}
                                </Typography>
                            </Box>

                            <Box
                                sx={{
                                    display: "flex",
                                    alignItems: "center",
                                    gap: 1,
                                }}
                            >
                                <Typography
                                    variant="body2"
                                    color="text.secondary"
                                >
                                    Last Played:
                                </Typography>
                                <Chip
                                    label={formatRelativeTime(
                                        gameSave.lastPlayed
                                    )}
                                    size="small"
                                    color="primary"
                                    variant="outlined"
                                />
                            </Box>
                        </Box>
                    </Box>
                </CardContent>
            </CardActionArea>
        </Card>
    );
};
