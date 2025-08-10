import React from "react";
import { Box, Container, Typography, Divider } from "@mui/material";
import { useGetAppSettingsContext } from "../contexts/AppSettingsContext";

interface PageBaseProps {
    children: React.ReactNode;
    maxWidth?: "xs" | "sm" | "md" | "lg" | "xl" | false;
    showFooter?: boolean;
}

export const PageBase: React.FC<PageBaseProps> = ({
    children,
    maxWidth = "lg",
    showFooter = true,
}) => {
    const appSettings = useGetAppSettingsContext();

    return (
        <Box
            sx={{
                display: "flex",
                flexDirection: "column",
                minHeight: "100vh",
            }}
        >
            {/* Main content area */}
            <Box sx={{ flex: 1 }}>
                <Container maxWidth={maxWidth} sx={{ py: 2 }}>
                    {children}
                </Container>
            </Box>

            {/* Footer */}
            {showFooter && (
                <Box
                    component="footer"
                    sx={{
                        mt: "auto",
                        py: 2,
                        backgroundColor: "background.paper",
                        borderTop: 1,
                        borderColor: "divider",
                    }}
                >
                    <Container maxWidth={maxWidth}>
                        <Divider sx={{ mb: 2 }} />
                        <Box
                            sx={{
                                display: "flex",
                                justifyContent: "space-between",
                                alignItems: "center",
                                flexWrap: "wrap",
                                gap: 1,
                            }}
                        >
                            <Typography
                                variant="body2"
                                color="text.secondary"
                                sx={{ fontSize: "0.875rem" }}
                            >
                                {appSettings.serviceName || "PokeGame"}
                            </Typography>
                            <Typography
                                variant="body2"
                                color="text.secondary"
                                sx={{ fontSize: "0.875rem" }}
                            >
                                Version {appSettings.releaseVersion || "1.0"}
                            </Typography>
                        </Box>
                    </Container>
                </Box>
            )}
        </Box>
    );
};
