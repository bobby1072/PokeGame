namespace PokeGame.Core.Common.GameInformationData;

public sealed record WildPokemonRange
{
    public required IntRange PokedexRange { get; init; }
    public required IntRange PokemonLevelRange { get; init; }
}