import React from "react";
import ReactDOM from "react-dom/client";
import { App } from "./App.tsx";
import { AppSettingsContextProvider } from "./common/contexts/AppSettingsContext.tsx";
import { PokeGameCoreHttpClientContextProvider } from "./common/contexts/PokeGameCoreHttpClientContext.tsx";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { PokeGameThemeProvider } from "./common/contexts/ThemeContext.tsx";
import { PokeGameUserContextProvider } from "./common/contexts/PokeGameUserContext.tsx";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import PokemonPage from "./pokemonGame/PokemonPage";

ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
        <PokeGameThemeProvider>
            <AppSettingsContextProvider>
                <PokeGameCoreHttpClientContextProvider>
                    <QueryClientProvider client={new QueryClient()}>
                        <PokeGameUserContextProvider>
                            <BrowserRouter>
                                <Routes>
                                    <Route path="/" element={<App />} />
                                    <Route path="/game" element={<App />} />
                                    <Route path="/pokemon" element={<PokemonPage />} />
                                </Routes>
                            </BrowserRouter>
                        </PokeGameUserContextProvider>
                    </QueryClientProvider>
                </PokeGameCoreHttpClientContextProvider>
            </AppSettingsContextProvider>
        </PokeGameThemeProvider>
    </React.StrictMode>
);
