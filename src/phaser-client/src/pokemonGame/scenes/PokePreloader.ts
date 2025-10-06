import { Scene } from "phaser";
import { BasiliaTownScene } from "./BasiliaTownScene.ts";

export class PokePreloader extends Scene {
    public constructor() {
        super(PokePreloader.name);
    }

    public preload() {
        // Load common assets used across all scenes
        this.load.spritesheet("myPlayer", "/assets/player.png", {
            frameWidth: 48,
            frameHeight: 48,
        });
    }

    public create() {
        // Start the first scene
        this.scene.start(BasiliaTownScene.name);
    }
}
