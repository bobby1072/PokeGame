namespace PokeGame.Core.Schemas.Input;

public sealed record NewGameSaveInput
{
    public required string NewCharacterName { get; init; } 
}