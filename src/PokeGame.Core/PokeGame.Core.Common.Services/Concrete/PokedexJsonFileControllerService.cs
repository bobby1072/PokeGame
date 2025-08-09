using System.Text.Json;
using Microsoft.Extensions.Logging;
using PokeGame.Core.Common.Services.Abstract;

namespace PokeGame.Core.Common.Services.Concrete;

internal sealed class PokedexJsonFileControllerService: IPokedexJsonFileControllerService
{
    private readonly string _jsonFilePath;
    private readonly ILogger<PokedexJsonFileControllerService> _logger;

    public PokedexJsonFileControllerService(string jsonFilePath, ILogger<PokedexJsonFileControllerService> logger)
    {
        _jsonFilePath = jsonFilePath;
        _logger = logger;
    }


    public async Task<JsonDocument> GetPokedexAsync()
    {
        var readJson = await File.ReadAllTextAsync(Path.GetFullPath(_jsonFilePath));
        
        return JsonDocument.Parse(readJson);
    }
}