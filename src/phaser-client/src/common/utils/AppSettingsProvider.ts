import axios from "axios";

enum AppSettingsKeys {
    serviceName = "serviceName",
    releaseVersion = "appReleaseVersion",
    pokeGameCoreApiUrl = "pokeGameCoreApiUrl",
    pokeGameCoreSignalRUrl = "pokeGameCoreSignalRUrl",
    autoSaveIntervalSeconds = "autoSaveIntervalSeconds",
}

export type AppSettings = {
    [K in keyof typeof AppSettingsKeys]: string;
};

export default abstract class AppSettingsProvider {
    private static appSettingsCache: Record<string, any> | null = null;

    public static async GetAllAppSettings(): Promise<AppSettings> {
        const settings = await AppSettingsProvider.LoadAppSettings();
        return Object.entries(AppSettingsKeys).reduce(
            (acc, [key, val]) => ({
                ...acc,
                [key]: AppSettingsProvider.TryGetValue(val as any, settings),
            }),
            {},
        ) as AppSettings;
    }

    private static async LoadAppSettings(): Promise<Record<string, any>> {
        if (AppSettingsProvider.appSettingsCache) {
            return AppSettingsProvider.appSettingsCache;
        }

        try {
            const base = import.meta.env.BASE_URL ?? "/";
            const response = await axios.get(`${base}reactappsettings.json`);
            if (!response.data) {
                throw new Error(
                    `Failed to load app settings: ${response.status}`,
                );
            }
            AppSettingsProvider.appSettingsCache = response.data;
            return response.data;
        } catch (error) {
            console.error("Error loading app settings:", error);
            throw error;
        }
    }

    private static TryGetValue(
        key: AppSettingsKeys,
        settings: Record<string, any>,
    ): string | undefined | null {
        try {
            const prodResult = AppSettingsProvider.FindVal(
                key.toString(),
                settings,
            );

            return prodResult?.toString();
        } catch {
            return undefined;
        }
    }
    private static FindVal(
        key: string,
        jsonDoc: Record<string, any>,
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
