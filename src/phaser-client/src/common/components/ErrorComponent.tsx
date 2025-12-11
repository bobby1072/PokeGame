import React from "react";
import { Alert, Box, SxProps, Theme } from "@mui/material";
import { FieldErrors } from "react-hook-form";
import { AxiosError } from "axios";

interface ErrorComponentProps {
    error?: FieldErrors | Error | string | null;
    /**
     * The variant of the error component
     * - 'fullscreen': Takes up the full viewport height (100vh) with centered alert
     * - 'page-section': Standard error display with top margin
     * - 'inline': Inline error display
     */
    variant?: "fullscreen" | "page-section" | "inline";
    /**
     * Additional sx props for custom styling
     */
    sx?: SxProps<Theme>;
}

export const ErrorComponent: React.FC<ErrorComponentProps> = ({
    error,
    variant = "page-section",
    sx = {},
}) => {
    if (!error) return null;

    // Get container styles based on variant
    const getVariantStyles = (): SxProps<Theme> => {
        switch (variant) {
            case "fullscreen":
                return {
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "center",
                    minHeight: "100vh",
                    padding: 2,
                };
            case "page-section":
                return {
                    mt: 2,
                };
            case "inline":
                return {};
            default:
                return { mt: 2 };
        }
    };

    // Get alert styles based on variant
    const getAlertStyles = (): SxProps<Theme> => {
        if (variant === "fullscreen") {
            return {
                maxWidth: "600px",
                width: "100%",
                fontSize: "1.1rem",
            };
        }
        return {};
    };

    const variantStyles = getVariantStyles();
    const alertStyles = getAlertStyles();
    const combinedSx = Array.isArray(sx)
        ? [variantStyles, ...sx]
        : [variantStyles, sx];

    // Handle Error objects
    if (error instanceof AxiosError) {
        return (
            <Box sx={combinedSx} data-testid="error-container">
                <Alert
                    severity="error"
                    sx={alertStyles}
                    data-testid="error-alert"
                >
                    {error.response?.data.exceptionMessage ||
                        "An unexpected error occurred"}
                </Alert>
            </Box>
        );
    }

    if (error instanceof Error) {
        return (
            <Box sx={combinedSx} data-testid="error-container">
                <Alert
                    severity="error"
                    sx={alertStyles}
                    data-testid="error-alert"
                >
                    {error.message}
                </Alert>
            </Box>
        );
    }

    // Handle string errors
    if (typeof error === "string") {
        return (
            <Box sx={combinedSx} data-testid="error-container">
                <Alert
                    severity="error"
                    sx={alertStyles}
                    data-testid="error-alert"
                >
                    {error}
                </Alert>
            </Box>
        );
    }

    // Handle FieldErrors (form validation errors)
    if (typeof error === "object" && error !== null) {
        const errorMessages = Object.values(error)
            .filter((err) => err && typeof err === "object" && "message" in err)
            .map((err) => (err as { message: string }).message)
            .filter(Boolean);

        if (errorMessages.length === 0) return null;

        return (
            <Box sx={combinedSx} data-testid="error-container">
                {errorMessages.map((message, index) => (
                    <Alert
                        key={index}
                        severity="error"
                        data-testid="error-alert"
                        sx={{
                            ...alertStyles,
                            mb: index < errorMessages.length - 1 ? 1 : 0,
                        }}
                    >
                        {message}
                    </Alert>
                ))}
            </Box>
        );
    }

    return null;
};
