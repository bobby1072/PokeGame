import { EventBus } from "../EventBus";
import { BasePlayableFreeroamScene } from "./BasePlayableFreeroamScene";

export default class BasiliaTownStarterHomeScene extends BasePlayableFreeroamScene {
    public static readonly SCENE_KEY = "BasiliaTownStarterHomeScene";

    public constructor() {
        super(BasiliaTownStarterHomeScene.SCENE_KEY);
    }

    public preload() {
        this.load.setPath("assets");
        this.load.tilemapTiledJSON(
            "basiliaTownStarterHomeMap",
            "BasiliaTownStarterHomeInsideMap.json"
        );
    }

    protected getTilemapKey(): string {
        return "basiliaTownStarterHomeMap";
    }

    protected getTilesetKeys(): string[] {
        return ["tileset_d5xdb0y"];
    }

    protected getStartPosition(): Phaser.Math.Vector2 {
        // Start position near the door entrance (bottom center of the room)
        return new Phaser.Math.Vector2(240, 320);
    }

    public override create() {
        super.create();
        EventBus.emit("current-scene-ready", this);
    }
}
