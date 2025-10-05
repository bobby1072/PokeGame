import appSettingsJson from "./reactappsettings.json";

enum AppSettingsKeys {
    serviceName = "serviceName",
    releaseVersion = "releaseVersion",
    pokeGameCoreApiUrl = "pokeGameCoreApiUrl",
    pokeGameCoreSignalRUrl = "pokeGameCoreSignalRUrl",
}

export type AppSettings = {
    [K in keyof typeof AppSettingsKeys]: string;
};

export default abstract class AppSettingsProvider {
    public static GetAllAppSettings(): AppSettings {
        return Object.entries(AppSettingsKeys).reduce(
            (acc, [key, val]) => ({
                ...acc,
                [key]: AppSettingsProvider.TryGetValue(val as any),
            }),
            {}
        ) as AppSettings;
    }
    private static TryGetValue(
        key: AppSettingsKeys
    ): string | undefined | null {
        try {
            const prodResult = AppSettingsProvider.FindVal(
                key.toString(),
                appSettingsJson
            );

            return prodResult?.toString();
        } catch {
            return undefined;
        }
    }
    private static FindVal(
        key: string,
        jsonDoc: Record<string, any>
    ): string | null | undefined {
        const keys = key.split(".");
        let result: any = jsonDoc;
        for (const k of keys) {
            try {
                if (result[k] !== undefined) {
                    result = result[k];
                }
            } catch {}
        }
        const final = result.toString();
        if (final.toLowerCase() === "[object object]") {
            return undefined;
        }
        return final;
    }
}
