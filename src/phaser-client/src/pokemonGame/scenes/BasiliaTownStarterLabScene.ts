import { EventBus } from "../EventBus";
import { BasePlayableFreeroamScene } from "./BasePlayableFreeroamScene";

export default class BasiliaTownStarterLabScene extends BasePlayableFreeroamScene {
    public static readonly SCENE_KEY = "BasiliaTownStarterLabScene";

    public constructor() {
        super(BasiliaTownStarterLabScene.SCENE_KEY);
    }
    public preload() {
        this.load.setPath("assets");
        this.load.tilemapTiledJSON(
            "basiliaTownStarterLabInsideMap",
            "BasiliaTownStarterLabInsideMap.json"
        );
    }

    protected getTilemapKey(): string {
        return "basiliaTownStarterLabInsideMap";
    }
    protected getTilesetKeys(): string[] {
        return ["tileset_d5xdb0y"];
    }

    protected getStartPosition(): Phaser.Math.Vector2 {
        return new Phaser.Math.Vector2(240, 320);
    }

    public override create() {
        super.create();
        EventBus.emit("current-scene-ready", this);
    }
}
