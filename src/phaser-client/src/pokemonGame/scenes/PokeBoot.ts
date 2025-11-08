import { Scene } from "phaser";
import { PokePreloader } from "./PokePreloader.ts";

export class PokeBoot extends Scene {
    public constructor() {
        super(PokeBoot.name);
    }

    public preload() {
        this.load.setPath("assets");
        this.load.image(
            "tileset_d5xdb0y",
            "tileset_by_chaoticcherrycake_d5xdb0y.png"
        );
        this.load.image("myPlayer", "myPlayer.png");
        this.load.image("background", "bg.png");
    }

    public create() {
        this.scene.start(PokePreloader.name);
    }
}
