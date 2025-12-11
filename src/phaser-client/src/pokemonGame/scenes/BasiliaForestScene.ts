import { EventBus } from "../EventBus";
import { BasePlayableFreeroamScene } from "./BasePlayableFreeroamScene";

export class BasiliaForestScene extends BasePlayableFreeroamScene {
    public static readonly SCENE_KEY = "BasiliaForestScene";

    public constructor() {
        super(BasiliaForestScene.SCENE_KEY);
    }

    public preload() {
        this.load.setPath("assets");
        this.load.tilemapTiledJSON("basiliaForestMap", "BasiliaForestMap.json");
    }

    protected getTilemapKey(): string {
        return "basiliaForestMap";
    }

    protected getTilesetKeys(): string[] {
        return ["tileset_d5xdb0y"];
    }

    protected getStartPosition(): Phaser.Math.Vector2 {
        return new Phaser.Math.Vector2(200, 300);
    }

    public override create(): void {
        super.create();
        EventBus.emit("current-scene-ready", this);
    }
}
