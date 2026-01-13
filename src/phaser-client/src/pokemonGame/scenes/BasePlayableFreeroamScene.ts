import Phaser, { Types } from "phaser";
import { Scene } from "phaser";
import { HubConnection } from "@microsoft/signalr";
import { GameSave } from "../../common/models/GameSave";
import { OwnedPokemon } from "../../common/models/OwnedPokemon";

export abstract class BasePlayableFreeroamScene extends Scene {
    protected player!: Types.Physics.Arcade.SpriteWithDynamicBody;
    protected cursors!: Types.Input.Keyboard.CursorKeys;
    protected moveSpeed = 140;
    protected spawnData?: { x: number; y: number };
    private autoSaveTimer?: Phaser.Time.TimerEvent;
    private hubConnection?: HubConnection;
    private currentGameSave?: GameSave | null;

    public constructor(sceneKey: string) {
        super(sceneKey);
    }

    public init(data: any) {
        // Store spawn data if provided
        if (data && data.x !== undefined && data.y !== undefined) {
            this.spawnData = { x: data.x, y: data.y };
        }
    }

    public create() {
        // Get SignalR connection and game save from game registry
        this.hubConnection = this.game.registry.get("hubConnection");
        this.currentGameSave = this.game.registry.get("currentGameSave");

        // Set up auto-save timer
        if (this.hubConnection && this.currentGameSave) {
            const autoSaveIntervalSeconds =
                this.game.registry.get("autoSaveIntervalSeconds") || 12;
            this.autoSaveTimer = this.time.addEvent({
                delay: autoSaveIntervalSeconds * 1000,
                callback: this.saveGame,
                callbackScope: this,
                loop: true,
            });
        }

        // Create tilemap
        const map = this.make.tilemap({ key: this.getTilemapKey() });

        // Add tilesets
        const tilesetKeys = this.getTilesetKeys();
        const tilesets: Phaser.Tilemaps.Tileset[] = [];

        // Match tilesets by name from the map to the provided keys
        map.tilesets.forEach((tilesetData) => {
            const tilesetName = tilesetData.name;
            // Find the matching key - for now just use the first key that matches
            const matchingKey = tilesetKeys.find((key) =>
                key.includes(
                    tilesetName.replace("tileset_by_chaoticcherrycake_", "")
                )
            );

            if (matchingKey) {
                const tileset = map.addTilesetImage(tilesetName, matchingKey);
                if (tileset) {
                    tilesets.push(tileset);
                }
            }
        });

        // Create all layers from the tilemap
        const collidableLayers: Phaser.Tilemaps.TilemapLayer[] = [];
        const sceneTransitionData: Array<{
            sceneName: string;
            startPosition?: { x: number; y: number };
            tiles: Array<{ x: number; y: number }>;
        }> = [];

        map.layers.forEach((_layerData, index) => {
            // Check for scene transition property BEFORE creating the layer
            const layerProperties = map.layers[index].properties;
            let isSceneTransition = false;

            if (layerProperties) {
                const sceneProp = layerProperties.find(
                    (p: any) => p.name === "ge_scene"
                );
                if (sceneProp && (sceneProp as any).value) {
                    isSceneTransition = true;

                    // Parse scene transition data
                    try {
                        const sceneData = JSON.parse((sceneProp as any).value);
                        if (sceneData.SceneName) {
                            // Get tile positions from this layer
                            const tiles: Array<{ x: number; y: number }> = [];
                            const layerData = map.layers[index].data;

                            for (let y = 0; y < layerData.length; y++) {
                                for (let x = 0; x < layerData[y].length; x++) {
                                    if (layerData[y][x].index > 0) {
                                        tiles.push({ x, y });
                                    }
                                }
                            }

                            sceneTransitionData.push({
                                sceneName: sceneData.SceneName,
                                startPosition: sceneData.StartPosition,
                                tiles,
                            });
                        }
                    } catch (e) {
                        console.error("Failed to parse ge_scene JSON:", e);
                    }
                }
            }

            // Skip creating the layer if it's a scene transition
            if (!isSceneTransition) {
                const layer = map.createLayer(index, tilesets, 0, 0);
                if (layer) {
                    layer.setSkipCull(true);

                    if (layerProperties) {
                        const collidesProp = layerProperties.find(
                            (p: any) => p.name === "ge_collide"
                        );

                        if (collidesProp && (collidesProp as any).value) {
                            layer.setCollisionByExclusion([-1, 0]);
                            collidableLayers.push(layer);
                        }
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

        // Create player at custom position if provided, otherwise use default
        const startPos = this.spawnData
            ? new Phaser.Math.Vector2(this.spawnData.x, this.spawnData.y)
            : this.getStartPosition();

        this.player = this.physics.add.sprite(
            startPos.x,
            startPos.y,
            "myPlayer"
        );

        // Scale player to exactly 16 pixels (one tile) to prevent overlapping transition zones
        const baseHeight = 16;
        const ph = this.player.height > 0 ? this.player.height : 96;
        this.player.setScale(baseHeight / ph);
        this.player.setDepth(10);
        this.player.setCollideWorldBounds(true);

        // Set player collision body to be smaller than sprite for tighter collision
        this.player.body.setSize(
            this.player.width * 0.5,
            this.player.height * 0.5
        );
        this.player.body.setOffset(
            this.player.width * 0.25,
            this.player.height * 0.25
        );

        // Add collision between player and collidable layers
        collidableLayers.forEach((layer) => {
            this.physics.add.collider(this.player, layer);
        });

        // Create invisible zones for scene transitions
        sceneTransitionData.forEach((transitionData) => {
            transitionData.tiles.forEach((tile) => {
                // Create a zone at each tile position
                const zone = this.add.zone(
                    tile.x * 16 + 8,
                    tile.y * 16 + 8,
                    16,
                    16
                );
                this.physics.add.existing(zone);

                this.physics.add.overlap(
                    this.player,
                    zone,
                    () => {
                        let spawnData: any = {};

                        if (transitionData.startPosition) {
                            // Convert tile coordinates to pixel coordinates
                            spawnData = {
                                x: transitionData.startPosition.x * 16 + 8,
                                y: transitionData.startPosition.y * 16 + 8,
                            };
                        }

                        // Stop current scene and start new one
                        this.scene.stop();
                        this.scene.start(transitionData.sceneName, spawnData);
                    },
                    undefined,
                    this
                );
            });
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

        // Register shutdown event to clean up timer
        this.events.once("shutdown", this.onShutdown, this);
    }

    // Abstract methods that must be implemented by subclasses
    protected abstract getTilemapKey(): string;
    protected abstract getTilesetKeys(): string[];
    protected abstract getStartPosition(): Phaser.Math.Vector2;

    private async saveGame() {
        if (!this.hubConnection || !this.currentGameSave) {
            console.warn(
                "Cannot save game: missing hub connection or game save data"
            );
            return;
        }

        try {
            // Convert player pixel position to tile coordinates
            const tileX = Math.floor(this.player.x / 16);
            const tileY = Math.floor(this.player.y / 16);

            // Ensure coordinates are within valid range (0-29)
            const clampedX = Math.max(0, Math.min(29, tileX));
            const clampedY = Math.max(0, Math.min(29, tileY));

            // Get current Pokemon deck from registry (may have been updated)
            const currentDeck = this.game.registry.get("pokemonDeck") as
                | OwnedPokemon[]
                | undefined;

            // Build deck Pokemon array from context
            const deckPokemon =
                currentDeck && currentDeck.length > 0
                    ? currentDeck.map((pokemon) => ({
                          ownedPokemonId: pokemon.id,
                      }))
                    : this.currentGameSave.gameSaveData.gameData.deckPokemon;

            const gameSaveData = {
                id: this.currentGameSave.gameSaveData.id,
                gameSaveId: this.currentGameSave.gameSaveData.gameSaveId,
                gameData: {
                    lastPlayedScene: this.scene.key,
                    lastPlayedLocationX: clampedX,
                    lastPlayedLocationY: clampedY,
                    deckPokemon,
                },
            };

            await this.hubConnection.invoke("SaveGame", gameSaveData);
            console.log("Game auto-saved successfully");
        } catch (error) {
            console.error("Failed to auto-save game:", error);
        }
    }

    private onShutdown() {
        // Clean up the auto-save timer when scene shuts down
        if (this.autoSaveTimer) {
            this.autoSaveTimer.destroy();
            this.autoSaveTimer = undefined;
        }
    }

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
