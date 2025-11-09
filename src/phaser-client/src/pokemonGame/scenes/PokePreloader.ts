import { Scene } from "phaser";
import BasiliaTownStarterHomeScene from "./BasiliaTownStarterHomeScene.ts";

export class PokePreloader extends Scene {
    public static readonly SCENE_KEY = "PokePreloader";

    public constructor() {
        super(PokePreloader.SCENE_KEY);
    }

    public create() {
        this.scene.start(BasiliaTownStarterHomeScene.SCENE_KEY);
    }
}
