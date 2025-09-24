using System.Text.Json;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Domain.Services.Pokemon.Abstract;

namespace PokeGame.Core.Domain.Services.Pokemon.Concrete;

internal sealed class PokedexJsonFactory : IPokedexJsonFactory
{
    private readonly string _jsonFilePath;
    private readonly ILogger<PokedexJsonFactory> _logger;

    public PokedexJsonFactory(string jsonFilePath, ILogger<PokedexJsonFactory> logger)
    {
        _jsonFilePath = jsonFilePath;
        _logger = logger;
    }

    public async Task<JsonDocument> GetPokedexJsonDocAsync()
    {
        var fullJsonPath = Path.GetFullPath(_jsonFilePath);

        _logger.LogInformation("Getting Pokedex from file path: {FilePath}", fullJsonPath);

        var readJson = await File.ReadAllTextAsync(fullJsonPath);

        _logger.LogDebug("Found this pokedex json: {PokedexStringJson}", readJson);

        return JsonDocument.Parse(readJson);
    }
}
