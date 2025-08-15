import { Scene } from "phaser";
import { PokePreloader } from "./PokePreloader.ts";

export class PokeBoot extends Scene {
    constructor() {
        super(PokeBoot.name);
    }

    preload() {
        this.load.setPath("assets");
        this.load.image("basiliaTown", "BasiliaTown.png");
        this.load.image("myPlayer", "myPlayer.png");
        this.load.image("background", "bg.png");
    }

    create() {
        this.scene.start(PokePreloader.name);
    }
}
