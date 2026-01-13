import React, {
    createContext,
    useContext,
    useState,
    ReactNode,
    useCallback,
} from "react";
import { OwnedPokemon } from "../models/OwnedPokemon";

interface PokemonDeckContextType {
    pokemonDeck: OwnedPokemon[];
    setPokemonDeck: (deck: OwnedPokemon[]) => void;
    updatePokemonWithDeepData: (deepPokemon: OwnedPokemon) => void;
    isLoadingDeepData: boolean;
    setIsLoadingDeepData: (loading: boolean) => void;
    deepDataError: string | null;
    setDeepDataError: (error: string | null) => void;
}

const PokemonDeckContext = createContext<PokemonDeckContextType | undefined>(
    undefined
);

interface PokemonDeckProviderProps {
    children: ReactNode;
}

export const PokemonDeckProvider: React.FC<PokemonDeckProviderProps> = ({
    children,
}) => {
    const [pokemonDeck, setPokemonDeck] = useState<OwnedPokemon[]>([]);
    const [isLoadingDeepData, setIsLoadingDeepData] = useState(false);
    const [deepDataError, setDeepDataError] = useState<string | null>(null);

    const updatePokemonWithDeepData = useCallback(
        (deepPokemon: OwnedPokemon) => {
            setPokemonDeck((currentDeck) => {
                const index = currentDeck.findIndex(
                    (p) => p.id === deepPokemon.id
                );
                if (index !== -1) {
                    const updatedDeck = [...currentDeck];
                    // Merge deep data into existing shallow data
                    updatedDeck[index] = {
                        ...updatedDeck[index],
                        ...deepPokemon,
                    };
                    return updatedDeck;
                }
                // If not found, add it to the deck
                return [...currentDeck, deepPokemon];
            });
        },
        []
    );

    const value: PokemonDeckContextType = {
        pokemonDeck,
        setPokemonDeck,
        updatePokemonWithDeepData,
        isLoadingDeepData,
        setIsLoadingDeepData,
        deepDataError,
        setDeepDataError,
    };

    return (
        <PokemonDeckContext.Provider value={value}>
            {children}
        </PokemonDeckContext.Provider>
    );
};

export const usePokemonDeck = (): PokemonDeckContextType => {
    const context = useContext(PokemonDeckContext);
    if (context === undefined) {
        throw new Error(
            "usePokemonDeck must be used within a PokemonDeckProvider"
        );
    }
    return context;
};
