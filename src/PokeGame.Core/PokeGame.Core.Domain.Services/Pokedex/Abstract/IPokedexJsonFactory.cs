using System.Text.Json;

namespace PokeGame.Core.Domain.Services.Pokedex.Abstract;

internal interface IPokedexJsonFactory
{
    Task<JsonDocument> GetPokedexJsonDocAsync();
}