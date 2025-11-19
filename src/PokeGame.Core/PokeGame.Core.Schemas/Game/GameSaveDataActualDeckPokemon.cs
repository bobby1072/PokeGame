namespace PokeGame.Core.Schemas.Game;

public sealed class GameSaveDataActualDeckPokemon: DomainModel<GameSaveDataActualDeckPokemon>
{
    public required Guid OwnedPokemonId { get; set; }

    public override bool Equals(GameSaveDataActualDeckPokemon? other)
    {
        return other?.OwnedPokemonId == OwnedPokemonId;
    }
}