import Phaser from "phaser";
import { EventBus } from "../../game/EventBus";

export class BasiliaTownScene extends Phaser.Scene {
    private player!: Phaser.Types.Physics.Arcade.SpriteWithDynamicBody;
    private cursors!: Phaser.Types.Input.Keyboard.CursorKeys;
    private mapImage!: Phaser.GameObjects.Image;
    // We'll sample pixel colors from the source texture to approximate path-only movement

    constructor() {
        super(BasiliaTownScene.name);
    }

    create() {
        // Add the town background centered
    this.mapImage = this.add.image(512, 384, "basiliaTown");
    // Ensure background fits canvas
    const scaleX = this.scale.width / this.mapImage.width;
    const scaleY = this.scale.height / this.mapImage.height;
    const scale = Math.max(scaleX, scaleY);
    this.mapImage.setScale(scale).setScrollFactor(0);

        // Create player near center
    this.player = this.physics.add.sprite(512, 384, "myPlayer");
    // Scale player to a reasonable size on the map
    const baseHeight = 48; // target display height in pixels
    const ph = this.player.height > 0 ? this.player.height : 96;
    this.player.setScale(baseHeight / ph);
        this.player.setDepth(10);
        this.player.setCollideWorldBounds(true);
        this.player.body.setSize(this.player.width * 0.6, this.player.height * 0.8);

        // Input
    this.cursors = this.input.keyboard!.createCursorKeys();
    this.input.keyboard?.addCapture(["UP", "DOWN", "LEFT", "RIGHT"]);

    // World and camera bounds
    const worldW = this.mapImage.displayWidth;
    const worldH = this.mapImage.displayHeight;
    this.physics.world.setBounds(0, 0, worldW, worldH);
    this.cameras.main.setBounds(0, 0, worldW, worldH);
    this.cameras.main.startFollow(this.player, true, 0.1, 0.1);

        EventBus.emit("current-scene-ready", this);
    }

    private canMoveTo(worldX: number, worldY: number): boolean {
        // Translate world position into texture pixel coordinates for 'basiliaTown'
        const texture = this.textures.get("basiliaTown");
        const src = texture.getSourceImage() as HTMLImageElement;
        if (!src) return false;

    const left = this.mapImage.getTopLeft().x;
    const top = this.mapImage.getTopLeft().y;

        const u = (worldX - left) / this.mapImage.displayWidth;
        const v = (worldY - top) / this.mapImage.displayHeight;

        if (u < 0 || u > 1 || v < 0 || v > 1) return false;

    const tx = Math.floor(u * src.width);
    const ty = Math.floor(v * src.height);

    // Guard bounds to avoid null pixels
    const clampedX = Phaser.Math.Clamp(tx, 0, (src.width ?? 1) - 1);
    const clampedY = Phaser.Math.Clamp(ty, 0, (src.height ?? 1) - 1);
    const pixel = this.textures.getPixel(clampedX, clampedY, "basiliaTown");
    if (!pixel) return true; // if unknown, allow movement

    // Heuristic: treat lighter pixels as walkable (paths/roads); tune as needed
    const r = pixel.red ?? (pixel as any).r ?? 0;
    const g = pixel.green ?? (pixel as any).g ?? 0;
    const b = pixel.blue ?? (pixel as any).b ?? 0;
    const brightness = (r + g + b) / (3 * 255);
    return brightness > 0.2; // adjust threshold if movement is too restricted/permissive
    }

    update() {
        const speed = 150;
        let vx = 0;
        let vy = 0;

        if (this.cursors.left?.isDown) vx = -speed;
        else if (this.cursors.right?.isDown) vx = speed;

        if (this.cursors.up?.isDown) vy = -speed;
        else if (this.cursors.down?.isDown) vy = speed;

        // Normalize diagonal speed
        if (vx !== 0 && vy !== 0) {
            const inv = 1 / Math.SQRT2;
            vx *= inv;
            vy *= inv;
        }

        // Attempt movement with simple collision checks by sampling in front of the sprite
    const dt = Math.min(0.05, this.game.loop.delta / 1000); // clamp dt to reduce jitter
        const nextX = this.player.x + vx * dt;
        const nextY = this.player.y + vy * dt;

        // Sample a few points around the feet for tighter blocking
        const footY = nextY + (this.player.displayHeight / 2) - 2;
        const samples = [
            { x: nextX, y: footY },
            { x: nextX + 6, y: footY },
            { x: nextX - 6, y: footY },
        ];

        const allowed = samples.every((p) => this.canMoveTo(p.x, p.y));

        if (allowed) {
            this.player.setVelocity(vx, vy);
        } else {
            // Try axis-aligned movement fallback to allow sliding along edges
            let ax = 0, ay = 0;
            if (vx !== 0) {
                const nX = this.player.x + Math.sign(vx) * speed * dt;
                const ok = this.canMoveTo(nX, footY) && this.canMoveTo(nX + 10, footY) && this.canMoveTo(nX - 10, footY);
                if (ok) ax = Math.sign(vx) * speed;
            }
            if (vy !== 0) {
                const nY = this.player.y + Math.sign(vy) * speed * dt;
                const ok = this.canMoveTo(this.player.x, nY + this.player.height * 0.3);
                if (ok) ay = Math.sign(vy) * speed;
            }
            this.player.setVelocity(ax, ay);
        }

        if (vx === 0 && vy === 0) {
            this.player.setVelocity(0, 0);
        }
    }
}
