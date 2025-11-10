import {
    ForwardedRef,
    forwardRef,
    useEffect,
    useLayoutEffect,
    useRef,
} from "react";
import StartPokemonGame from "./MainGame";
import { EventBus } from "./EventBus";

export interface IRefPokemonPhaserGame {
    game: Phaser.Game | null;
    scene: Phaser.Scene | null;
}

interface IProps {
    currentActiveScene?: (scene_instance: Phaser.Scene) => void;
}

const ActualPokemonPhaserGame = (
    { currentActiveScene }: IProps,
    ref: ForwardedRef<IRefPokemonPhaserGame>
) => {
    const game = useRef<Phaser.Game | null>(null!);

    useLayoutEffect(() => {
        if (game.current === null) {
            game.current = StartPokemonGame("pokemon-game-container");

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
    }, [ref]);

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
