import { http, HttpResponse } from "msw";

export const mockHandlers = [
    http.get("/reactappsettings.json", () => {
        return HttpResponse.json({
            serviceName: "phaser-poke-game",
            releaseVersion: "1.0",
            pokeGameCoreApiUrl: "",
            pokeGameCoreSignalRUrl: "",
            autoSaveIntervalSeconds: "12",
        });
    }),
];
