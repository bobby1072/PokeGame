import { useMutation } from "@tanstack/react-query";
import { useGetPokeGameHttpClientContext } from "../contexts/PokeGameCoreHttpClientContext";
import { GameSave } from "../models/GameSave";
import { NewGameSaveInput } from "../models/NewGameSaveInput";

export const useSaveNewGameMutation = () => {
    const pokeGameHttpClient = useGetPokeGameHttpClientContext();

    const mutationResults = useMutation<
        GameSave,
        Error,
        { input: NewGameSaveInput }
    >({
        mutationFn: ({ input }) => pokeGameHttpClient.SaveNewGame(input),
    });

    return { ...mutationResults };
};