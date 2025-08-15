import { Scene } from "phaser";
import { BasiliaTownScene } from "./BasiliaTownScene.ts";

export class PokePreloader extends Scene {
    constructor() {
        super(PokePreloader.name);
    }

    create() {
        this.scene.start(BasiliaTownScene.name);
    }
}
