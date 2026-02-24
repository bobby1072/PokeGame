import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { PokeGameThemeProvider } from "./common/contexts/ThemeContext";
import { AppSettingsContextProvider } from "./common/contexts/AppSettingsContext";
import { PokeGameCoreHttpClientContextProvider } from "./common/contexts/PokeGameCoreHttpClientContext";
import { PokeGameUserContextProvider } from "./common/contexts/PokeGameUserContext";
import { SignalRGameSessionProvider } from "./common/contexts/SignalRGameSessionContext";
import { PokemonDeckProvider } from "./common/contexts/PokemonDeckContext";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import PokemonPage from "./pokemonGame/PokemonPage";

export const App: React.FC = () => {
    return (
        <PokeGameThemeProvider>
            <QueryClientProvider client={new QueryClient()}>
                <AppSettingsContextProvider>
                    <PokeGameCoreHttpClientContextProvider>
                        <PokeGameUserContextProvider>
                            <SignalRGameSessionProvider>
                                <PokemonDeckProvider>
                                    <BrowserRouter
                                        basename={import.meta.env.BASE_URL}
                                    >
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
    );
};
