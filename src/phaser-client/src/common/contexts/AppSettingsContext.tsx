import { createContext, useContext } from "react";
import AppSettingsProvider, { AppSettings } from "../utils/AppSettingsProvider";
import { useQuery } from "@tanstack/react-query";
import { QueryKeys } from "../hooks/QueryKeys";
import { LoadingComponent } from "../components/LoadingComponent";

const AppSettingsContext = createContext<AppSettings | undefined>(undefined);

export const useGetAppSettingsContext = () => {
    const value = useContext(AppSettingsContext);

    if (!value) {
        throw new Error("No settings registered");
    }

    return value;
};

export const AppSettingsContextProvider: React.FC<{
    children: React.ReactNode;
}> = ({ children }) => {
    const {data, isLoading} = useQuery<AppSettings, Error>({
        queryKey: [QueryKeys.GetAppSettings],
        queryFn: () => AppSettingsProvider.GetAllAppSettings()
    });
    if (isLoading || !data) return <LoadingComponent variant="fullscreen"/>
    return (
        <AppSettingsContext.Provider
            value={data}
        >
            {children}
        </AppSettingsContext.Provider>
    );
};
