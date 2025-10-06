import React, { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import {
    Paper,
    TextField,
    Button,
    Typography,
    Box,
    Alert,
    Tabs,
    Tab,
} from "@mui/material";
import { PokeGameUser } from "../models/PokeGameUser";
import { SaveUserInput } from "../models/SaveUserInput";
import { useGetUserMutation } from "../hooks/useGetUserMutation";
import { useSaveUserMutation } from "../hooks/useSaveUserMutation";
import { PageBase } from "../components/PageBase";
import { ErrorComponent } from "../components/ErrorComponent";
import { LoadingComponent } from "../components/LoadingComponent";

interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
}

const TabPanel: React.FC<TabPanelProps> = (props) => {
    const { children, value, index, ...other } = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`login-tabpanel-${index}`}
            aria-labelledby={`login-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ pt: 3 }}>{children}</Box>}
        </div>
    );
}

// Zod schemas
const loginSchema = z.object({
    email: z
        .string()
        .email("Please enter a valid email address")
        .min(1, "Email is required"),
});

const registerSchema = z.object({
    email: z
        .email("Please enter a valid email address")
        .min(1, "Email is required"),
    name: z
        .string()
        .min(1, "Name is required")
        .min(2, "Name must be at least 2 characters"),
});

export type LoginInput = z.infer<typeof loginSchema>;
export type RegisterInput = z.infer<typeof registerSchema>;

export const LoginPage: React.FC<{
    setUser: (user: PokeGameUser) => void;
}> = ({ setUser }) => {
    const [tabValue, setTabValue] = useState(0);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    const getUserMutation = useGetUserMutation();
    const saveUserMutation = useSaveUserMutation();

    // Login form
    const {
        register: loginRegister,
        handleSubmit: handleLoginSubmit,
        formState: { errors: loginErrors, isDirty: loginIsDirty },
        reset: loginReset,
    } = useForm<LoginInput>({
        resolver: zodResolver(loginSchema),
        defaultValues: { email: "" },
    });

    // Register form
    const {
        register: registerRegister,
        handleSubmit: handleRegisterSubmit,
        formState: { errors: registerErrors, isDirty: registerIsDirty },
        reset: registerReset,
    } = useForm<RegisterInput>({
        resolver: zodResolver(registerSchema),
        defaultValues: { email: "", name: "" },
    });

    const handleTabChange = (
        _event: React.SyntheticEvent,
        newValue: number
    ) => {
        setTabValue(newValue);
        setSuccessMessage(null);
        loginReset();
        registerReset();
        getUserMutation.reset();
        saveUserMutation.reset();
    };

    // Handle successful login
    useEffect(() => {
        if (getUserMutation.data) {
            setSuccessMessage("Login successful!");
            setUser(getUserMutation.data);
        }
    }, [getUserMutation.data, setUser]);

    // Handle successful registration
    useEffect(() => {
        if (saveUserMutation.data) {
            setSuccessMessage(
                "Registration successful! You are now logged in."
            );
            setUser(saveUserMutation.data);
        }
    }, [saveUserMutation.data, setUser]);

    const onLoginSubmit = (data: LoginInput) => {
        setSuccessMessage(null);
        getUserMutation.mutate({ email: data.email });
    };

    const onRegisterSubmit = (data: RegisterInput) => {
        setSuccessMessage(null);
        const userInput: SaveUserInput = {
            email: data.email,
            name: data.name,
        };
        saveUserMutation.mutate({ userInput });
    };

    const isLoginLoading = getUserMutation.isPending;
    const isRegisterLoading = saveUserMutation.isPending;

    return (
        <PageBase maxWidth="sm">
            <Box sx={{ mt: 8 }}>
                <Paper elevation={6} sx={{ p: 4 }}>
                    <Typography
                        variant="h4"
                        component="h1"
                        align="center"
                        gutterBottom
                    >
                        PokeGame
                    </Typography>

                    <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
                        <Tabs
                            value={tabValue}
                            onChange={handleTabChange}
                            aria-label="login register tabs"
                            variant="fullWidth"
                        >
                            <Tab
                                label="Login"
                                id="login-tab-0"
                                aria-controls="login-tabpanel-0"
                            />
                            <Tab
                                label="Register"
                                id="login-tab-1"
                                aria-controls="login-tabpanel-1"
                            />
                        </Tabs>
                    </Box>

                    <TabPanel value={tabValue} index={0}>
                        <Box
                            component="form"
                            onSubmit={handleLoginSubmit(onLoginSubmit)}
                            sx={{ mt: 2 }}
                        >
                            <TextField
                                fullWidth
                                label="Email"
                                type="email"
                                {...loginRegister("email")}
                                margin="normal"
                                disabled={isLoginLoading}
                                autoComplete="email"
                                error={!!loginErrors.email}
                                helperText={loginErrors.email?.message}
                            />

                            <ErrorComponent error={loginErrors} />
                            <ErrorComponent error={getUserMutation.error} />

                            {successMessage && (
                                <Alert severity="success" sx={{ mt: 2 }}>
                                    {successMessage}
                                </Alert>
                            )}

                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{ mt: 3, mb: 2 }}
                                disabled={!loginIsDirty || isLoginLoading}
                            >
                                {isLoginLoading ? (
                                    <LoadingComponent variant="inline" size={24} />
                                ) : (
                                    "Login"
                                )}
                            </Button>
                        </Box>
                    </TabPanel>

                    <TabPanel value={tabValue} index={1}>
                        <Box
                            component="form"
                            onSubmit={handleRegisterSubmit(onRegisterSubmit)}
                            sx={{ mt: 2 }}
                        >
                            <TextField
                                fullWidth
                                label="Email"
                                type="email"
                                {...registerRegister("email")}
                                margin="normal"
                                disabled={isRegisterLoading}
                                autoComplete="email"
                                error={!!registerErrors.email}
                                helperText={registerErrors.email?.message}
                            />

                            <TextField
                                fullWidth
                                label="Name"
                                type="text"
                                {...registerRegister("name")}
                                margin="normal"
                                disabled={isRegisterLoading}
                                autoComplete="name"
                                error={!!registerErrors.name}
                                helperText={registerErrors.name?.message}
                            />

                            <ErrorComponent error={registerErrors} />
                            <ErrorComponent error={saveUserMutation.error} />

                            {successMessage && (
                                <Alert severity="success" sx={{ mt: 2 }}>
                                    {successMessage}
                                </Alert>
                            )}

                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                sx={{ mt: 3, mb: 2 }}
                                disabled={!registerIsDirty || isRegisterLoading}
                            >
                                {isRegisterLoading ? (
                                    <LoadingComponent variant="inline" size={24} />
                                ) : (
                                    "Register"
                                )}
                            </Button>
                        </Box>
                    </TabPanel>
                </Paper>
            </Box>
        </PageBase>
    );
};
