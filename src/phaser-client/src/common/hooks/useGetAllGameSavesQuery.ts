import { useQuery } from "@tanstack/react-query";
import { useGetPokeGameHttpClientContext } from "../contexts/PokeGameCoreHttpClientContext";
import { useGetPokeGameUserContext } from "../contexts/PokeGameUserContext";
import { GameSave } from "../models/GameSave";
import { QueryKeys } from "./QueryKeys";

export const useGetAllGameSavesQuery = () => {
    const pokeGameHttpClient = useGetPokeGameHttpClientContext();
    const currentUser = useGetPokeGameUserContext();

    return useQuery<GameSave[], Error>({
        queryKey: [QueryKeys.GetAllGameSavesForSelf, currentUser?.id],
        queryFn: () => pokeGameHttpClient.GetAllGameSavesForUser(),
        enabled: !!currentUser?.id, // Only run query when we have a user ID
    });
};