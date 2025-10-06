import { EventBus } from "../EventBus";
import { BasePlayableFreeroamScene } from "./BasePlayableFreeroamScene";

export class BasiliaTownScene extends BasePlayableFreeroamScene {
    public constructor() {
        super(BasiliaTownScene.name);
    }

    protected getMapTextureKey(): string {
        return "basiliaTown";
    }

    protected getStartPosition(): Phaser.Math.Vector2 {
        // Near center of map
        return new Phaser.Math.Vector2(512, 384);
    }

    protected configureBlockedAreas(): void {
        // Block all border tiles as out of bounds
        const borderThickness = 1;
        // Top border
        this.setBlockedRect(0, 0, this.cols - 1, borderThickness - 1, true);
        // Bottom border
        this.setBlockedRect(
            0,
            this.rows - borderThickness,
            this.cols - 1,
            this.rows - 1,
            true
        );
        // Left border
        this.setBlockedRect(0, 0, borderThickness - 1, this.rows - 1, true);
        // Right border
        this.setBlockedRect(
            this.cols - borderThickness,
            0,
            this.cols - 1,
            this.rows - 1,
            true
        );
    }

    // protected override canMoveTo(worldX: number, worldY: number): boolean {
    //     // Pixel-based blocking logic from original scene
    //     const texture = this.textures.get("basiliaTown");
    //     const src = texture.getSourceImage() as HTMLImageElement;
    //     if (!src) return false;

    //     const left = this.mapImage.getTopLeft().x;
    //     const top = this.mapImage.getTopLeft().y;

    //     const u = (worldX - left) / this.mapImage.displayWidth;
    //     const v = (worldY - top) / this.mapImage.displayHeight;

    //     if (u < 0 || u > 1 || v < 0 || v > 1) return false;

    //     const tx = Math.floor(u * src.width);
    //     const ty = Math.floor(v * src.height);

    //     const clampedX = Phaser.Math.Clamp(tx, 0, (src.width ?? 1) - 1);
    //     const clampedY = Phaser.Math.Clamp(ty, 0, (src.height ?? 1) - 1);
    //     const pixel = this.textures.getPixel(clampedX, clampedY, "basiliaTown");
    //     if (!pixel) return true;

    //     const r = pixel.red ?? (pixel as any).r ?? 0;
    //     const g = pixel.green ?? (pixel as any).g ?? 0;
    //     const b = pixel.blue ?? (pixel as any).b ?? 0;
    //     const brightness = (r + g + b) / (3 * 255);
    //     return brightness > 0.2;
    // }

    public override create() {
        super.create();
        EventBus.emit("current-scene-ready", this);
    }
}
