using System.Text.Json;

namespace PokeGame.Core.Common.Services.Abstract;

internal interface IPokedexJsonFileControllerService
{
    Task<JsonDocument> GetPokedexAsync();
}