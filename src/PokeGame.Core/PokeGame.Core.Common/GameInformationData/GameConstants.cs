namespace PokeGame.Core.Common.GameInformationData;

public static class GameConstants
{
    public static class NewGameInfo
    {
        public const string StartingScene = SceneNames.BasiliaTownStarterHome;
        public const int StartingLocationX = 15;
        public const int StartingLocationY = 17;

        public static readonly IReadOnlyCollection<string> BaseUnlockedSceneNames =
        [
            SceneNames.BasiliaTownStarterHome,
            SceneNames.BasiliaTown,
            SceneNames.BasiliaTownStarterLab,
        ];
    }

    public static class PokemonGameRules
    {
        public static readonly IntRange StandardPokedexRange = new()
        {
            Min = 1,
            Max = 143,
            Extras = [147,148,149]
        };

        public static readonly IntRange LegendaryPokedexRange = new()
        {
            Extras = [144,145,146,150,151]
        };

        public static readonly IReadOnlyDictionary<string, WildPokemonRange> SceneWildPokemonRange = new Dictionary<string, WildPokemonRange>
        {
            [SceneNames.BasiliaForest] = new ()
            {
                PokedexRange = new ()
                {
                    Extras = [10, 13, 16, 19]
                },
                PokemonLevelRange = new ()
                {
                    Min = 1,
                    Max = 4
                }
            },
        };
    }
    
    public static class SceneNames
    {
        public const string BasiliaTown = "BasiliaTownScene";
        public const string BasiliaTownStarterHome = "BasiliaTownStarterHomeScene";
        public const string BasiliaTownStarterLab = "BasiliaTownStarterLabScene";
        public const string BasiliaForest = "BasiliaForestScene";

        public static readonly IReadOnlyCollection<string> ValidSceneList = [
            BasiliaTown,
            BasiliaTownStarterHome,
            BasiliaTownStarterLab,
            BasiliaForest
        ];
    }
}