import { useMutation } from "@tanstack/react-query";
import { useGetPokeGameHttpClientContext } from "../contexts/PokeGameCoreHttpClientContext";
import { PokeGameUser } from "../models/PokeGameUser";
import { SaveUserInput } from "../models/SaveUserInput";

export const useSaveUserMutation = () => {
    const pokeGameHttpClient = useGetPokeGameHttpClientContext();

    const mutationResults = useMutation<
        PokeGameUser,
        Error,
        { userInput: SaveUserInput }
    >({
        mutationFn: ({ userInput: ui }) => pokeGameHttpClient.SaveUser(ui),
    });

    return { ...mutationResults };
};
