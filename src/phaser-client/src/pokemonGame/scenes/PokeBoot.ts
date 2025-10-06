import { Scene } from "phaser";
import { PokePreloader } from "./PokePreloader.ts";

export class PokeBoot extends Scene {
    public constructor() {
        super(PokeBoot.name);
    }

    public create() {
        this.scene.start(PokePreloader.name);
    }
}
