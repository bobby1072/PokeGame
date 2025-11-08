import Phaser, { Types } from "phaser";
import { Scene } from "phaser";

export abstract class BasePlayableFreeroamScene extends Scene {
    protected player!: Types.Physics.Arcade.SpriteWithDynamicBody;
    protected cursors!: Types.Input.Keyboard.CursorKeys;
    protected moveSpeed = 140;

    public constructor(sceneKey: string) {
        super(sceneKey);
    }

    public create() {
        // Create tilemap
        const map = this.make.tilemap({ key: this.getTilemapKey() });

        // Add tilesets
        const tilesetKeys = this.getTilesetKeys();
        const tilesets: Phaser.Tilemaps.Tileset[] = [];

        tilesetKeys.forEach((key: string, index: number) => {
            const tilesetName = map.tilesets[index].name;
            const tileset = map.addTilesetImage(tilesetName, key);
            if (tileset) {
                tilesets.push(tileset);
            }
        });

        // Create all layers from the tilemap
        const collidableLayers: Phaser.Tilemaps.TilemapLayer[] = [];
        map.layers.forEach((_layerData, index) => {
            const layer = map.createLayer(index, tilesets, 0, 0);
            if (layer) {
                // Fix tile bleeding/black lines by disabling pixel snapping
                layer.setSkipCull(true);

                // Check if this layer has the ge_collide property set to true
                const layerProperties = map.layers[index].properties;

                if (layerProperties) {
                    const collidesProp = layerProperties.find(
                        (p: any) => p.name === "ge_collide"
                    );

                    if (collidesProp && (collidesProp as any).value) {
                        // Set collision ONLY on tiles with actual tile data (index > 0)
                        // This prevents setting collision on empty tiles
                        layer.setCollisionByExclusion([-1, 0]);
                        collidableLayers.push(layer);
                    }
                }
            }
        });

        // Set world bounds based on tilemap dimensions
        const worldW = map.widthInPixels;
        const worldH = map.heightInPixels;
        this.physics.world.setBounds(0, 0, worldW, worldH);

        // Don't bound the camera - let it show the full canvas
        // Just center it on the map
        this.cameras.main.centerOn(worldW / 2, worldH / 2);

        // Create player
        const startPos = this.getStartPosition();
        this.player = this.physics.add.sprite(
            startPos.x,
            startPos.y,
            "myPlayer"
        );

        // Scale player
        const baseHeight = 24; // Reduced from 48 to make player smaller
        const ph = this.player.height > 0 ? this.player.height : 96;
        this.player.setScale(baseHeight / ph);
        this.player.setDepth(10);
        this.player.setCollideWorldBounds(true);

        // Set player collision body
        this.player.body.setSize(
            this.player.width * 0.6,
            this.player.height * 0.8
        );
        this.player.body.setOffset(
            this.player.width * 0.2,
            this.player.height * 0.1
        );

        // Add collision between player and collidable layers
        collidableLayers.forEach((layer) => {
            this.physics.add.collider(this.player, layer);
        });

        // Setup camera to follow player
        this.cameras.main.startFollow(this.player, true, 0.1, 0.1);

        // Calculate zoom to fit the map better in the canvas
        // Canvas is 1024x768, map is 480x480
        const zoomX = this.scale.width / worldW; // 1024 / 480 = ~2.13
        const zoomY = this.scale.height / worldH; // 768 / 480 = 1.6
        const zoom = Math.min(zoomX, zoomY) * 1.75; // Use the smaller to ensure map fits, with 40% increase
        this.cameras.main.setZoom(zoom);
        this.cameras.main.roundPixels = true; // Prevent sub-pixel rendering artifacts

        // Setup input
        this.cursors = this.input.keyboard!.createCursorKeys();
        this.input.keyboard?.addCapture(["UP", "DOWN", "LEFT", "RIGHT"]);
    }

    // Abstract methods that must be implemented by subclasses
    protected abstract getTilemapKey(): string;
    protected abstract getTilesetKeys(): string[];
    protected abstract getStartPosition(): Phaser.Math.Vector2;

    public update(_time: number, _delta: number) {
        if (!this.player) return;

        // Simple 8-directional movement
        let vx = 0;
        let vy = 0;

        if (this.cursors.left?.isDown) vx = -1;
        else if (this.cursors.right?.isDown) vx = 1;

        if (this.cursors.up?.isDown) vy = -1;
        else if (this.cursors.down?.isDown) vy = 1;

        if (vx !== 0 || vy !== 0) {
            // Normalize diagonal movement
            const length = Math.sqrt(vx * vx + vy * vy);
            vx = (vx / length) * this.moveSpeed;
            vy = (vy / length) * this.moveSpeed;

            this.player.setVelocity(vx, vy);
        } else {
            this.player.setVelocity(0, 0);
        }
    }
}
