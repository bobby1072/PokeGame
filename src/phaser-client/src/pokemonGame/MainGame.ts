import { AUTO, Game } from "phaser";
import { PokeBoot } from "./scenes/PokeBoot.ts";
import { PokePreloader } from "./scenes/PokePreloader.ts";
import { BasiliaTownScene } from "./scenes/BasiliaTownScene.ts";
import BasiliaTownStarterHomeScene from "./scenes/BasiliaTownStarterHomeScene.ts";
import BasiliaTownStarterLabScene from "./scenes/BasiliaTownStarterLabScene.ts";
import { BasiliaForestScene } from "./scenes/BasiliaForestScene.ts";

const config: Phaser.Types.Core.GameConfig = {
    type: AUTO,
    width: 1024,
    height: 768,
    parent: "pokemon-game-container",
    backgroundColor: "#000000",
    scale: {
        mode: Phaser.Scale.FIT,
        autoCenter: Phaser.Scale.CENTER_BOTH,
    },
    render: {
        pixelArt: true, // Prevents antialiasing and fixes tile bleeding
        antialias: false,
        roundPixels: true,
    },
    physics: {
        default: "arcade",
        arcade: {
            gravity: { x: 0, y: 0 },
            debug: false,
        },
    },
    scene: [
        PokeBoot,
        PokePreloader,
        BasiliaTownScene,
        BasiliaTownStarterHomeScene,
        BasiliaTownStarterLabScene,
        BasiliaForestScene,
    ] as any,
};

const StartPokemonGame = (parent: string) => {
    return new Game({ ...config, parent });
};

export default StartPokemonGame;
