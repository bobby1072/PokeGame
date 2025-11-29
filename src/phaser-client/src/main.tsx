import React from "react";
import ReactDOM from "react-dom/client";
import { AppSettingsContextProvider } from "./common/contexts/AppSettingsContext.tsx";
import { PokeGameCoreHttpClientContextProvider } from "./common/contexts/PokeGameCoreHttpClientContext.tsx";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { PokeGameThemeProvider } from "./common/contexts/ThemeContext.tsx";
import { PokeGameUserContextProvider } from "./common/contexts/PokeGameUserContext.tsx";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import PokemonPage from "./pokemonGame/PokemonPage";
import { SignalRGameSessionProvider } from "./common/contexts/SignalRGameSessionContext.tsx";
import { PokemonDeckProvider } from "./common/contexts/PokemonDeckContext.tsx";

ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
        <PokeGameThemeProvider>
            <QueryClientProvider client={new QueryClient()}>
                <AppSettingsContextProvider>
                    <PokeGameCoreHttpClientContextProvider>
                        <PokeGameUserContextProvider>
                            <SignalRGameSessionProvider>
                                <PokemonDeckProvider>
                                    <BrowserRouter>
                                        <Routes>
                                            <Route
                                                path="/"
                                                element={<PokemonPage />}
                                            />
                                            <Route
                                                path="/pokemon"
                                                element={<PokemonPage />}
                                            />
                                        </Routes>
                                    </BrowserRouter>
                                </PokemonDeckProvider>
                            </SignalRGameSessionProvider>
                        </PokeGameUserContextProvider>
                    </PokeGameCoreHttpClientContextProvider>
                </AppSettingsContextProvider>
            </QueryClientProvider>
        </PokeGameThemeProvider>
    </React.StrictMode>
);
