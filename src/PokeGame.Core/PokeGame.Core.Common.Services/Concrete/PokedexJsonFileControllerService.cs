using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Services.Abstract;

namespace PokeGame.Core.Common.Services.Concrete;

internal sealed class PokedexJsonFileControllerService: IPokedexJsonFileControllerService
{
    private readonly string _jsonFilePath;
    private readonly ILogger<PokedexJsonFileControllerService> _logger;

    public PokedexJsonFileControllerService(
        [FromKeyedServices(Constants.ServiceKeys.PokedexJsonFilePath)]string jsonFilePath,
        ILogger<PokedexJsonFileControllerService> logger
    )
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