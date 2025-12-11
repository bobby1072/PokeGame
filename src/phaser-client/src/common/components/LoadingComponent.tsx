import React from "react";
import {
    Box,
    CircularProgress,
    Typography,
    SxProps,
    Theme,
} from "@mui/material";

export interface LoadingComponentProps {
    /**
     * The variant of the loading component
     * - 'fullscreen': Takes up the full viewport height (100vh)
     * - 'page-section': Takes up a section of the page with padding
     * - 'inline': Small spinner for buttons or inline use
     */
    variant?: "fullscreen" | "page-section" | "inline";

    /**
     * Optional message to display below the spinner
     */
    message?: string;

    /**
     * Size of the CircularProgress spinner
     * Defaults: fullscreen=40, page-section=60, inline=24
     */
    size?: number;

    /**
     * Additional sx props for custom styling
     */
    sx?: SxProps<Theme>;
}

export const LoadingComponent: React.FC<LoadingComponentProps> = ({
    variant = "page-section",
    message,
    size,
    sx = {},
}) => {
    // Default sizes based on variant
    const defaultSize = {
        fullscreen: 40,
        "page-section": 60,
        inline: 24,
    }[variant];

    const spinnerSize = size ?? defaultSize;

    // Base styles for different variants
    const getVariantStyles = (): SxProps<Theme> => {
        switch (variant) {
            case "fullscreen":
                return {
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "center",
                    minHeight: "100vh",
                    gap: 2,
                };
            case "page-section":
                return {
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "center",
                    minHeight: "400px",
                    gap: 2,
                };
            case "inline":
                return {
                    display: "inline-flex",
                    alignItems: "center",
                    justifyContent: "center",
                };
            default:
                return {};
        }
    };

    const variantStyles = getVariantStyles();
    const combinedSx = Array.isArray(sx)
        ? [variantStyles, ...sx]
        : [variantStyles, sx];

    return (
        <Box sx={combinedSx} data-testid="loading-container">
            <CircularProgress
                size={spinnerSize}
                data-testid="loading-spinner"
                aria-label="Loading"
            />
            {message && variant !== "inline" && (
                <Typography
                    variant={variant === "fullscreen" ? "h6" : "h6"}
                    color="text.secondary"
                    textAlign="center"
                    data-testid="loading-message"
                >
                    {message}
                </Typography>
            )}
        </Box>
    );
};
