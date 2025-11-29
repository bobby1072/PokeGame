import axios, { AxiosInstance } from "axios";
import { SaveUserInput } from "../models/SaveUserInput";
import { PokeGameUser } from "../models/PokeGameUser";
import { WebOutcome } from "../models/WebOutcome";
import { GameSave } from "../models/GameSave";
import { NewGameSaveInput } from "../models/NewGameSaveInput";
import { ShallowOwnedPokemon } from "../models/ShallowOwnedPokemon";

export default class PokeGameCoreHttpClient {
    private readonly _axiosClient: AxiosInstance;
    private _userId: string | null = null;

    public constructor(baseURL: string) {
        this._axiosClient = axios.create({
            baseURL,
        });
    }

    public setUserId(userId: string | null): void {
        this._userId = userId;
    }

    private getHeadersWithUserId(): Record<string, string> {
        const headers: Record<string, string> = {};
        if (this._userId) {
            headers["UserId"] = this._userId;
        }
        return headers;
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
            input,
            {
                headers: this.getHeadersWithUserId(),
            }
        );

        if (!data.isSuccess || !data.data) {
            throw new Error(data.exceptionMessage || "Error occurred");
        }

        return data.data;
    }

    public async GetAllGameSavesForUser(): Promise<GameSave[]> {
        const { data } = await this._axiosClient.get<WebOutcome<GameSave[]>>(
            "Api/GameSave/GetAllForSelf",
            {
                headers: this.getHeadersWithUserId(),
            }
        );

        if (!data.isSuccess || !data.data) {
            throw new Error(data.exceptionMessage || "Error occurred");
        }

        return data.data;
    }

    public async GetShallowOwnedPokemonInDeck(
        gameSessionId: string
    ): Promise<ShallowOwnedPokemon[]> {
        const { data } = await this._axiosClient.get<
            WebOutcome<ShallowOwnedPokemon[]>
        >(
            `Api/GameSession/GetShallowOwnedPokemonInDeck?gameSessionId=${gameSessionId}`,
            {
                headers: this.getHeadersWithUserId(),
            }
        );

        if (!data.isSuccess || !data.data) {
            throw new Error(data.exceptionMessage || "Error occurred");
        }

        return data.data;
    }
}
