import { Scene } from "phaser";
import { BasiliaTownScene } from "./BasiliaTownScene.ts";

export class PokePreloader extends Scene {
    public constructor() {
        super(PokePreloader.name);
    }

    public create() {
        this.scene.start(BasiliaTownScene.name);
    }
}
