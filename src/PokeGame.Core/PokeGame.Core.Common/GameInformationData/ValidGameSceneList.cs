namespace PokeGame.Core.Common.GameInformationData;

public sealed class ValidGameSceneList : List<string>
{
    public ValidGameSceneList()
        : base()
    {
        AddRange(["BasiliaTownScene", "BasiliaTownStarterHomeScene", "BasiliaTownStarterLabScene"]);
    }
}
