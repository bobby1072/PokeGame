import { useQuery } from "@tanstack/react-query";
import { useGetPokeGameHttpClientContext } from "../contexts/PokeGameCoreHttpClientContext";
import { GameSave } from "../models/GameSave";
import { QueryKeys } from "./QueryKeys";

export const useGetAllGameSavesQuery = () => {
    const pokeGameHttpClient = useGetPokeGameHttpClientContext();

    return useQuery<GameSave[], Error>({
        queryKey: [QueryKeys.GetAllGameSavesForSelf],
        queryFn: () => pokeGameHttpClient.GetAllGameSavesForUser(),
    });
};