import Phaser, { GameObjects, Scene } from "phaser";

/**
 * Base scene with a square grid overlay and helpers for grid-aware movement/authoring.
 *
 * Usage:
 * - Subclass and set `this.mapImage` in create().
 * - Call `this.buildGridFromImage()` to populate `walkable` using `sampleWalkableAt`.
 * - Then call `this.drawGridOverlay()` to render the grid.
 * - Toggle visibility via `this.showGridOverlay = true/false` at runtime.
 * - Optionally set `this.gridToggleKey = this.input.keyboard?.addKey(KeyCodes.G)`
 *   so pressing G will toggle the overlay (handled in super.update()).
 * - Override `protected sampleWalkableAt(x, y)` for custom walkability rules.
 */
export abstract class BasePlayablePokemonScene extends Scene {
    // Background image the grid is anchored to. Must be assigned by subclass.
    protected mapImage!: GameObjects.Image;

    // Grid config/state
    protected tileSize = 32;
    protected cols = 0;
    protected rows = 0;
    protected walkable: boolean[][] = [];

    // Overlay
    protected gridGraphics?: Phaser.GameObjects.Graphics;
    public showGridOverlay = true; // public boolean to control overlay visibility
    protected gridToggleKey?: Phaser.Input.Keyboard.Key;

    public update(_time: number, _delta: number): void {
        // Handle developer toggle
        if (
            this.gridToggleKey &&
            Phaser.Input.Keyboard.JustDown(this.gridToggleKey)
        ) {
            this.setShowGridOverlay(!this.showGridOverlay);
        }
    }

    // --- Grid helpers (usable by subclasses) ---
    protected worldToGrid(x: number, y: number) {
        const topLeft = this.mapImage.getTopLeft();
        const lx = x - topLeft.x;
        const ly = y - topLeft.y;
        const ix = Phaser.Math.Clamp(
            Math.floor(lx / this.tileSize),
            0,
            Math.max(0, this.cols - 1)
        );
        const iy = Phaser.Math.Clamp(
            Math.floor(ly / this.tileSize),
            0,
            Math.max(0, this.rows - 1)
        );
        return { ix, iy };
    }

    protected gridToWorldCenter(ix: number, iy: number) {
        const topLeft = this.mapImage.getTopLeft();
        const x = topLeft.x + ix * this.tileSize + this.tileSize / 2;
        const y = topLeft.y + iy * this.tileSize + this.tileSize / 2;
        return new Phaser.Math.Vector2(x, y);
    }

    protected inGrid(ix: number, iy: number) {
        return ix >= 0 && iy >= 0 && ix < this.cols && iy < this.rows;
    }

    protected setBlockedRect(
        x1: number,
        y1: number,
        x2: number,
        y2: number,
        blocked = true
    ) {
        if (!this.walkable.length) return;
        const minX = Phaser.Math.Clamp(Math.min(x1, x2), 0, this.cols - 1);
        const maxX = Phaser.Math.Clamp(Math.max(x1, x2), 0, this.cols - 1);
        const minY = Phaser.Math.Clamp(Math.min(y1, y2), 0, this.rows - 1);
        const maxY = Phaser.Math.Clamp(Math.max(y1, y2), 0, this.rows - 1);
        for (let y = minY; y <= maxY; y++) {
            for (let x = minX; x <= maxX; x++) {
                this.walkable[y][x] = !blocked;
            }
        }
    }

    protected buildGridFromImage() {
        if (!this.mapImage) {
            throw new Error(
                "BasePokemonScene: mapImage must be assigned before building the grid."
            );
        }
        this.cols = Math.max(
            1,
            Math.floor(this.mapImage.displayWidth / this.tileSize)
        );
        this.rows = Math.max(
            1,
            Math.floor(this.mapImage.displayHeight / this.tileSize)
        );
        this.walkable = new Array(this.rows);
        for (let y = 0; y < this.rows; y++) {
            this.walkable[y] = new Array(this.cols);
            for (let x = 0; x < this.cols; x++) {
                const c = this.gridToWorldCenter(x, y);
                this.walkable[y][x] = this.sampleWalkableAt(c.x, c.y);
            }
        }
    }

    protected drawGridOverlay() {
        if (!this.gridGraphics) {
            this.gridGraphics = this.add.graphics();
            this.gridGraphics.setDepth(50);
            this.gridGraphics.setScrollFactor(
                this.mapImage.scrollFactorX,
                this.mapImage.scrollFactorY
            );
        }

        const g = this.gridGraphics;
        g.clear();

        const topLeft = this.mapImage.getTopLeft();
        g.lineStyle(1, 0xffffff, 0.15);

        for (let y = 0; y < this.rows; y++) {
            for (let x = 0; x < this.cols; x++) {
                const rx = topLeft.x + x * this.tileSize;
                const ry = topLeft.y + y * this.tileSize;

                if (!this.walkable[y][x]) {
                    g.fillStyle(0xff4d4f, 0.2);
                    g.fillRect(rx, ry, this.tileSize, this.tileSize);
                } else {
                    g.fillStyle(0x52c41a, 0.08);
                    g.fillRect(rx, ry, this.tileSize, this.tileSize);
                }

                g.strokeRect(rx, ry, this.tileSize, this.tileSize);
            }
        }

        g.setVisible(this.showGridOverlay);
    }

    // Default: everything is walkable; override for custom logic in subclasses.
    // Called by buildGridFromImage().
    protected sampleWalkableAt(_worldX: number, _worldY: number): boolean {
        return true;
    }

    // Convenience: setter to keep graphics visibility in sync
    public setShowGridOverlay(show: boolean) {
        this.showGridOverlay = show;
        this.gridGraphics?.setVisible(show);
    }
}
