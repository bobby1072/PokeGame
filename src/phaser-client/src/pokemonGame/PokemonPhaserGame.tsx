import {
    ForwardedRef,
    forwardRef,
    useEffect,
    useLayoutEffect,
    useRef,
} from "react";
import StartPokemonGame from "./MainGame";
import { EventBus } from "./EventBus";
import { HubConnection } from "@microsoft/signalr";
import { GameSave } from "../common/models/GameSave";
import { ShallowOwnedPokemon } from "../common/models/ShallowOwnedPokemon";
import { useGetAppSettingsContext } from "../common/contexts/AppSettingsContext";

export interface IRefPokemonPhaserGame {
    game: Phaser.Game | null;
    scene: Phaser.Scene | null;
}

interface IProps {
    currentActiveScene?: (scene_instance: Phaser.Scene) => void;
    hubConnection: HubConnection;
    currentGameSave: GameSave | null;
    shallowDeck?: ShallowOwnedPokemon[];
}

const ActualPokemonPhaserGame = (
    { currentActiveScene, hubConnection, currentGameSave, shallowDeck }: IProps,
    ref: ForwardedRef<IRefPokemonPhaserGame>
) => {
    const game = useRef<Phaser.Game | null>(null!);
    const appSettings = useGetAppSettingsContext();

    useLayoutEffect(() => {
        if (game.current === null) {
            game.current = StartPokemonGame("pokemon-game-container");

            // Store SignalR connection and game save in the game registry
            if (game.current) {
                game.current.registry.set("hubConnection", hubConnection);
                game.current.registry.set("currentGameSave", currentGameSave);
                game.current.registry.set("shallowDeck", shallowDeck || []);
                game.current.registry.set(
                    "autoSaveIntervalSeconds",
                    parseInt(appSettings.autoSaveIntervalSeconds || "12")
                );
            }

            if (typeof ref === "function") {
                ref({ game: game.current, scene: null });
            } else if (ref) {
                ref.current = { game: game.current, scene: null };
            }
        }

        return () => {
            if (game.current) {
                game.current.destroy(true);
                if (game.current !== null) {
                    game.current = null;
                }
            }
        };
    }, [
        ref,
        hubConnection,
        currentGameSave,
        shallowDeck,
        appSettings.autoSaveIntervalSeconds,
    ]);

    useEffect(() => {
        EventBus.on("current-scene-ready", (scene_instance: Phaser.Scene) => {
            if (
                currentActiveScene &&
                typeof currentActiveScene === "function"
            ) {
                currentActiveScene(scene_instance);
            }

            if (typeof ref === "function") {
                ref({ game: game.current, scene: scene_instance });
            } else if (ref) {
                ref.current = { game: game.current, scene: scene_instance };
            }
        });
        return () => {
            EventBus.removeListener("current-scene-ready");
        };
    }, [currentActiveScene, ref]);

    return (
        <div
            id="pokemon-game-container"
            style={{ width: 1024, height: 768 }}
        ></div>
    );
};

export const PokemonPhaserGame = forwardRef<IRefPokemonPhaserGame, IProps>(
    ActualPokemonPhaserGame
);
