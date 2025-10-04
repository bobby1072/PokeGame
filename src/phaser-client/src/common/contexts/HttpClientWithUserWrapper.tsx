import React from "react";
import { PokeGameCoreHttpClientContextProvider } from "./PokeGameCoreHttpClientContext";
import { PokeGameUserContext } from "./PokeGameUserContext";

interface HttpClientWithUserWrapperProps {
    children: React.ReactNode;
}

export const HttpClientWithUserWrapper: React.FC<HttpClientWithUserWrapperProps> = ({ children }) => {
    // Use React.useContext directly to avoid throwing error when context is undefined
    const user = React.useContext(PokeGameUserContext);

    return (
        <PokeGameCoreHttpClientContextProvider userId={user?.id}>
            {children}
        </PokeGameCoreHttpClientContextProvider>
    );
};