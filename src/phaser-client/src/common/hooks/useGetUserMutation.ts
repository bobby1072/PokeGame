import { useMutation } from "@tanstack/react-query";
import { PokeGameUser } from "../models/PokeGameUser";
import { useGetPokeGameHttpClientContext } from "../contexts/PokeGameCoreHttpClientContext";

export const useGetUserMutation = () => {
    const pokeGameHttpClient = useGetPokeGameHttpClientContext();
    const mutationResults = useMutation<
        PokeGameUser | null | undefined,
        Error,
        { email: string }
    >({
        mutationFn: ({ email }) => pokeGameHttpClient.GetUser(email),
    });

    return { ...mutationResults };
};
