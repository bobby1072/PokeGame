namespace PokeGame.Core.Schemas.Input;

public sealed record GameSaveInput
{
    public required Guid UserId { get; init; }
    public required string CharacterName { get; init; }
}