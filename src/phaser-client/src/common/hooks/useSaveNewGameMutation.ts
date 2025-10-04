import { useMutation } from "@tanstack/react-query";
import { useGetPokeGameHttpClientContext } from "../contexts/PokeGameCoreHttpClientContext";
import { useGetPokeGameUserContext } from "../contexts/PokeGameUserContext";
import { GameSave } from "../models/GameSave";
import { NewGameSaveInput } from "../models/NewGameSaveInput";

export const useSaveNewGameMutation = () => {
    const pokeGameHttpClient = useGetPokeGameHttpClientContext();
    const currentUser = useGetPokeGameUserContext();

    const mutationResults = useMutation<
        GameSave,
        Error,
        { input: NewGameSaveInput }
    >({
        mutationFn: async ({ input }) => {
            // Ensure user ID is set before making the request
            if (currentUser?.id) {
                pokeGameHttpClient.setUserId(currentUser.id);
            }
            return pokeGameHttpClient.SaveNewGame(input);
        },
    });

    return { ...mutationResults };
};