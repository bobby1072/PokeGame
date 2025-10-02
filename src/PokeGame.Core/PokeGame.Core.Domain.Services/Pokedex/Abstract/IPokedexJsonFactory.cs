using System.Text.Json;

namespace PokeGame.Core.Domain.Services.Pokemon.Abstract;

internal interface IPokedexJsonFactory
{
    Task<JsonDocument> GetPokedexJsonDocAsync();
}