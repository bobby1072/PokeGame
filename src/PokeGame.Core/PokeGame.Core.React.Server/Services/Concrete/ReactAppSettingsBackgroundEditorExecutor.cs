using BT.Common.Services.Concrete;
using PokeGame.Core.React.Server.Configuration;
using PokeGame.Core.React.Server.Services.Abstract;

namespace PokeGame.Core.React.Server.Services.Concrete;

internal sealed class ReactAppSettingsBackgroundEditorExecutor: BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly PokeGameBackendSettings _pokeGameBackendSettings;
    private readonly ILogger<ReactAppSettingsBackgroundEditorExecutor> _logger;
    private const string _pokeGameCoreApiUrlKey = "pokeGameCoreApiUrl";
    private const string _pokeGameCoreSignalRUrlKey = "pokeGameCoreSignalRUrl";
    public ReactAppSettingsBackgroundEditorExecutor(IServiceScopeFactory serviceScopeFactory,
        PokeGameBackendSettings pokeGameBackendSettings,
        ILogger<ReactAppSettingsBackgroundEditorExecutor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _pokeGameBackendSettings = pokeGameBackendSettings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        await using var asyncScope = _serviceScopeFactory.CreateAsyncScope();
        var reactAppEditor = asyncScope.ServiceProvider.GetRequiredService<IReactAppSettingsEditor>();

        var updateDictionary = new Dictionary<string, string>
        {
            { _pokeGameCoreApiUrlKey, _pokeGameBackendSettings.PokeGameApiProdUri },
            { _pokeGameCoreSignalRUrlKey, _pokeGameBackendSettings.PokeGameSignalRProdUri }
        };

        try
        {
            await reactAppEditor.UpdateAppSettingsAsync(updateDictionary, stoppingToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception occurred whilst trying to update the ReactAppSettings");
        }
    }
}