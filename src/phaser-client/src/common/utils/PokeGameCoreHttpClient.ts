import axios, { AxiosInstance } from "axios";
import { SaveUserInput } from "../models/SaveUserInput";
import { PokeGameUser } from "../models/PokeGameUser";
import { WebOutcome } from "../models/WebOutcome";
import { GameSave } from "../models/GameSave";
import { NewGameSaveInput } from "../models/NewGameSaveInput";

export default class PokeGameCoreHttpClient {
    private readonly _axiosClient: AxiosInstance;

    public constructor(baseURL: string) {
        this._axiosClient = axios.create({
            baseURL,
        });
    }
    public async SaveUser(input: SaveUserInput): Promise<PokeGameUser> {
        const { data } = await this._axiosClient.post<WebOutcome<PokeGameUser>>(
            "Api/User/Save",
            input
        );

        if (!data.isSuccess || !data.data) {
            throw new Error(data.exceptionMessage || "Error occurred");
        }

        return data.data;
    }

    public async GetUser(
        email: string
    ): Promise<PokeGameUser | null | undefined> {
        const { data } = await this._axiosClient.get<WebOutcome<PokeGameUser>>(
            `Api/User/Get?email=${email}`
        );

        if (!data.isSuccess) {
            throw new Error(data.exceptionMessage || "Error occurred");
        }

        return data.data;
    }

    public async SaveNewGame(input: NewGameSaveInput): Promise<GameSave> {
        const { data } = await this._axiosClient.post<WebOutcome<GameSave>>(
            "Api/GameSave/SaveNew",
            input
        );

        if (!data.isSuccess || !data.data) {
            throw new Error(data.exceptionMessage || "Error occurred");
        }

        return data.data;
    }

    public async GetAllGameSavesForUser(): Promise<GameSave[]> {
        const { data } = await this._axiosClient.get<WebOutcome<GameSave[]>>(
            "Api/GameSave/GetAllForSelf"
        );

        if (!data.isSuccess || !data.data) {
            throw new Error(data.exceptionMessage || "Error occurred");
        }

        return data.data;
    }
}
