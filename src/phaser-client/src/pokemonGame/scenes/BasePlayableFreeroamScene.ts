import Phaser, { Types } from "phaser";
import { BasePlayablePokemonScene } from "./BasePlayablePokemonScene";

export abstract class BasePlayableFreeroamScene extends BasePlayablePokemonScene {
    protected player!: Types.Physics.Arcade.SpriteWithDynamicBody;
    protected cursors!: Types.Input.Keyboard.CursorKeys;
    protected isMoving = false;
    protected targetPos?: Phaser.Math.Vector2;
    protected moveSpeed = 180;
    protected debugText?: Phaser.GameObjects.Text;

    public constructor(sceneKey: string) {
        super(sceneKey);
    }

    public create() {
        // Add background (inheritor must add image in preload)
        this.mapImage = this.add.image(512, 384, this.getMapTextureKey());
        const scaleX = this.scale.width / this.mapImage.width;
        const scaleY = this.scale.height / this.mapImage.height;
        const scale = Math.max(scaleX, scaleY);
        this.mapImage.setScale(scale).setScrollFactor(0);

        // Create player at custom start position
        const startPos = this.getStartPosition();
        this.player = this.physics.add.sprite(
            startPos.x,
            startPos.y,
            "myPlayer"
        );
        const baseHeight = 48;
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

        // World/camera bounds
        const worldW = this.mapImage.displayWidth;
        const worldH = this.mapImage.displayHeight;
        this.physics.world.setBounds(0, 0, worldW, worldH);
        this.cameras.main.setBounds(0, 0, worldW, worldH);
        this.cameras.main.startFollow(this.player, true, 0.1, 0.1);

        // Build grid and configure blocked areas
        this.buildGridFromImage();
        this.configureBlockedAreas();

        // Snap player to nearest cell center
        const startCell = this.worldToGrid(this.player.x, this.player.y);
        const startCenter = this.gridToWorldCenter(startCell.ix, startCell.iy);
        this.player.setPosition(startCenter.x, startCenter.y);

        // Draw overlay
        this.drawGridOverlay();

        // Debug HUD
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
                `G: toggle grid\nCell: (${ix}, ${iy}) Walkable: ${ok}`
            );
        });
    }

    // Abstract: inheritors must provide map texture key
    protected abstract getMapTextureKey(): string;

    // Abstract: inheritors must provide starting position
    protected abstract getStartPosition(): Phaser.Math.Vector2;

    // Abstract: inheritors must configure blocked areas
    protected abstract configureBlockedAreas(): void;

    // Used by grid builder; separate from runtime movement checks
    protected override sampleWalkableAt(
        worldX: number,
        worldY: number
    ): boolean {
        return this.canMoveTo(worldX, worldY);
    }

    protected canMoveTo(worldX: number, worldY: number): boolean {
        // Default: allow all movement; inheritors can override for pixel-based blocking
        return true;
    }

    public update(time: number, delta: number) {
        super.update(time, delta);
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
        if (gx !== 0 && gy !== 0) gy = 0;
        if (gx === 0 && gy === 0) {
            this.player.setVelocity(0, 0);
            this.isMoving = false;
            this.targetPos = undefined;
            // Snap to nearest valid cell
            const { ix, iy } = this.worldToGrid(this.player.x, this.player.y);
            const snapPos = this.gridToWorldCenter(
                Phaser.Math.Clamp(ix, 0, this.cols - 1),
                Phaser.Math.Clamp(iy, 0, this.rows - 1)
            );
            this.player.setPosition(snapPos.x, snapPos.y);
            return;
        }
        const { ix, iy } = this.worldToGrid(this.player.x, this.player.y);
        const nx = ix + gx;
        const ny = iy + gy;
        // If out of bounds or not walkable, reset movement state and snap to nearest valid cell
        if (!this.inGrid(nx, ny) || !this.walkable[ny]?.[nx]) {
            this.player.setVelocity(0, 0);
            this.isMoving = false;
            this.targetPos = undefined;
            const snapPos = this.gridToWorldCenter(
                Phaser.Math.Clamp(ix, 0, this.cols - 1),
                Phaser.Math.Clamp(iy, 0, this.rows - 1)
            );
            this.player.setPosition(snapPos.x, snapPos.y);
            return;
        }
        // Otherwise, move to next cell
        this.targetPos = this.gridToWorldCenter(nx, ny);
        this.isMoving = true;
    }
}
