import { EventBus } from "../EventBus";
import { BasePlayableFreeroamScene } from "./BasePlayableFreeroamScene";

export class BasiliaTownScene extends BasePlayableFreeroamScene {
    public constructor() {
        super(BasiliaTownScene.name);
    }

    public preload() {
        this.load.setPath("assets");
        this.load.tilemapTiledJSON("basiliaTownMap", "BasiliaTownMap.json");
    }

    protected getTilemapKey(): string {
        return "basiliaTownMap";
    }

    protected getTilesetKeys(): string[] {
        return ["tileset_d5xdb0y"];
    }

    protected getStartPosition(): Phaser.Math.Vector2 {
        return new Phaser.Math.Vector2(240, 240);
    }

    public override create() {
        super.create();
        EventBus.emit("current-scene-ready", this);
    }
}
