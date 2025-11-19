namespace PokeGame.Core.Common.GameInformationData;

public static class GameConstants
{
    public static class SceneNames
    {
        public const string BasiliaTown = "BasiliaTownScene";
        public const string BasiliaTownStarterHome = "BasiliaTownStarterHomeScene";
        public const string BasiliaTownStarterLab = "BasiliaTownStarterLabScene";

        public static readonly IReadOnlyCollection<string> ValidSceneList = [
            BasiliaTown,
            BasiliaTownStarterHome,
            BasiliaTownStarterLab
        ];
    }
}