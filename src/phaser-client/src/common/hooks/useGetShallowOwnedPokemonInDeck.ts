import { useQuery } from "@tanstack/react-query";
import { useGetPokeGameUserContext } from "../contexts/PokeGameUserContext";
import { useGetPokeGameHttpClientContext } from "../contexts/PokeGameCoreHttpClientContext";
import { ShallowOwnedPokemon } from "../models/ShallowOwnedPokemon";
import { QueryKeys } from "./QueryKeys";

export const useGetShallowOwnedPokemonInDeck = (gameSessionId: string) => {
    const pokeGameHttpClient = useGetPokeGameHttpClientContext();
    const currentUser = useGetPokeGameUserContext();

    return useQuery<ShallowOwnedPokemon[], Error>({
        queryKey: [
            QueryKeys.GetShallowOwnedPokemonInDeck,
            gameSessionId,
            currentUser?.id,
        ],
        queryFn: () =>
            pokeGameHttpClient.GetShallowOwnedPokemonInDeck(gameSessionId),
        enabled: !!gameSessionId && !!currentUser?.id,
    });
};
