import Phaser, { Types } from "phaser";
import { EventBus } from "../../game/EventBus";
import { BasePlayablePokemonScene } from "./BasePlayablePokemonScene";

export class BasiliaTownScene extends BasePlayablePokemonScene {
    private player!: Types.Physics.Arcade.SpriteWithDynamicBody;
    private cursors!: Types.Input.Keyboard.CursorKeys;
    private isMoving = false;
    private targetPos?: Phaser.Math.Vector2;
    private moveSpeed = 180; // px/sec toward next cell center
    private debugText?: Phaser.GameObjects.Text;

    public constructor() {
        super(BasiliaTownScene.name);
    }

    public create() {
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
        this.player.body.setSize(
            this.player.width * 0.6,
            this.player.height * 0.8
        );

        // Input
        this.cursors = this.input.keyboard!.createCursorKeys();
        this.input.keyboard?.addCapture(["UP", "DOWN", "LEFT", "RIGHT"]);
        this.gridToggleKey = this.input.keyboard?.addKey(
            Phaser.Input.Keyboard.KeyCodes.G
        );

        // World and camera bounds
        const worldW = this.mapImage.displayWidth;
        const worldH = this.mapImage.displayHeight;
        this.physics.world.setBounds(0, 0, worldW, worldH);
        this.cameras.main.setBounds(0, 0, worldW, worldH);
        this.cameras.main.startFollow(this.player, true, 0.1, 0.1);

        // Build grid from image sampling (you can override cells manually after this)
        this.buildGridFromImage();

        // Make rectangle (20,5) to (27,11) non-walkable (inclusive)
        this.setBlockedRect(20, 5, 27, 11, true);

        // Snap player to nearest cell center
        const startCell = this.worldToGrid(this.player.x, this.player.y);
        const startCenter = this.gridToWorldCenter(startCell.ix, startCell.iy);
        this.player.setPosition(startCenter.x, startCenter.y);

        // Draw overlay (visibility controlled by base boolean)
        this.drawGridOverlay();

        // Debug HUD and authoring helpers
        this.debugText = this.add
            .text(10, 10, "", {
                fontSize: "12px",
                color: "#ffffff",
                backgroundColor: "#00000099",
                padding: { left: 6, right: 6, top: 3, bottom: 3 } as any,
            })
            .setScrollFactor(0)
            .setDepth(1000);

        this.input.on("pointermove", (p: Phaser.Input.Pointer) => {
            const { ix, iy } = this.worldToGrid(p.worldX, p.worldY);
            const ok = this.inGrid(ix, iy) ? this.walkable[iy][ix] : false;
            this.debugText?.setText(
                `G: toggle grid  |  Click: toggle cell\nCell: (${ix}, ${iy}) Walkable: ${ok}`
            );
        });

        this.input.on("pointerdown", (p: Phaser.Input.Pointer) => {
            const { ix, iy } = this.worldToGrid(p.worldX, p.worldY);
            if (this.inGrid(ix, iy)) {
                this.walkable[iy][ix] = !this.walkable[iy][ix];
                this.drawGridOverlay();
            }
        });

        EventBus.emit("current-scene-ready", this);
    }

    // --- Grid helpers ---
    // remove grid helpers/overlay; inherited from BasePokemonScene

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

    // Used by grid builder; separate from runtime movement checks
    protected override sampleWalkableAt(
        worldX: number,
        worldY: number
    ): boolean {
        return this.canMoveTo(worldX, worldY);
    }

    public update(time: number, delta: number) {
        super.update(time, delta); // handle base overlay toggle
        // Base handles G toggle for overlay via showGridOverlay

        // If currently moving, continue toward target
        if (this.isMoving && this.targetPos) {
            const dx = this.targetPos.x - this.player.x;
            const dy = this.targetPos.y - this.player.y;
            const dist = Math.hypot(dx, dy);

            if (dist <= 2) {
                this.player.setVelocity(0, 0);
                this.player.setPosition(this.targetPos.x, this.targetPos.y);
                this.isMoving = false;
                this.targetPos = undefined;
                return;
            }

            const nx = dx / dist;
            const ny = dy / dist;
            this.player.setVelocity(nx * this.moveSpeed, ny * this.moveSpeed);
            return;
        }

        // Not moving: read input (4-direction only)
        let gx = 0,
            gy = 0;
        if (this.cursors.left?.isDown) gx = -1;
        else if (this.cursors.right?.isDown) gx = 1;

        if (this.cursors.up?.isDown) gy = -1;
        else if (this.cursors.down?.isDown) gy = 1;

        // Prevent diagonal steps; prioritize horizontal
        if (gx !== 0 && gy !== 0) gy = 0;

        if (gx === 0 && gy === 0) {
            this.player.setVelocity(0, 0);
            return;
        }

        const { ix, iy } = this.worldToGrid(this.player.x, this.player.y);
        const nx = Phaser.Math.Clamp(ix + gx, 0, this.cols - 1);
        const ny = Phaser.Math.Clamp(iy + gy, 0, this.rows - 1);

        if (this.inGrid(nx, ny) && this.walkable[ny][nx]) {
            this.targetPos = this.gridToWorldCenter(nx, ny);
            this.isMoving = true;
        } else {
            this.player.setVelocity(0, 0);
        }
    }
}
