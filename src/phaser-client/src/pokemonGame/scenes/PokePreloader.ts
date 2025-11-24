import { Scene } from "phaser";
import BasiliaTownStarterHomeScene from "./BasiliaTownStarterHomeScene.ts";
import { GameSave } from "../../common/models/GameSave";

export class PokePreloader extends Scene {
    public static readonly SCENE_KEY = "PokePreloader";

    public constructor() {
        super(PokePreloader.SCENE_KEY);
    }

    public create() {
        // Get saved game data from registry
        const currentGameSave: GameSave | null = this.game.registry.get("currentGameSave");
        
        if (currentGameSave?.gameSaveData?.gameData) {
            const { lastPlayedScene, lastPlayedLocationX, lastPlayedLocationY } = currentGameSave.gameSaveData.gameData;
            
            // Convert tile coordinates (0-29) to pixel coordinates (centered in tile)
            const pixelX = lastPlayedLocationX * 16 + 8;
            const pixelY = lastPlayedLocationY * 16 + 8;
            
            // Start at the saved scene and position
            this.scene.start(lastPlayedScene, { x: pixelX, y: pixelY });
        } else {
            // Fallback to default starting scene if no save data
            this.scene.start(BasiliaTownStarterHomeScene.SCENE_KEY);
        }
    }
}
