namespace PokeGame.Core.Schemas.Input;

public sealed record GetPokedexPokemonInput
{
    public int? Id { get; init; }
    public string? EnglishName { get; init; }
    public string? FrenchName { get; init; }
    public string? JapaneseName { get; init; }
    public string? ChineseName { get; init; }
    public bool FetchMultiple { get; init; }
}