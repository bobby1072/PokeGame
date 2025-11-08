import { Scene } from "phaser";
import BasiliaTownStarterHomeScene from "./BasiliaTownStarterHomeScene.ts";

export class PokePreloader extends Scene {
    public constructor() {
        super(PokePreloader.name);
    }

    public create() {
        this.scene.start(BasiliaTownStarterHomeScene.name);
    }
}
