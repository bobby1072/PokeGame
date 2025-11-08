import { EventBus } from "../EventBus";
import { BasePlayableFreeroamScene } from "./BasePlayableFreeroamScene";

export class BasiliaTownScene extends BasePlayableFreeroamScene {
    public constructor() {
        super(BasiliaTownScene.name);
    }

    protected getTilemapKey(): string {
        return "basiliaTownMap";
    }

    protected getTilesetKeys(): string[] {
        return ["tileset_ddi8611", "tileset_d5xdb0y"];
    }

    protected getStartPosition(): Phaser.Math.Vector2 {
        // Start position in pixels (center of map approximately)
        return new Phaser.Math.Vector2(240, 240);
    }

    public override create() {
        super.create();
        EventBus.emit("current-scene-ready", this);
    }
}
