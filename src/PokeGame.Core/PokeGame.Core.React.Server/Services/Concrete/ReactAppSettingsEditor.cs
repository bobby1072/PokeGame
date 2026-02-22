using System.Text.Json;
using BT.Common.Polly.Extensions;
using BT.Common.Polly.Models.Concrete;
using BT.Common.Services.Concrete;
using PokeGame.Core.React.Server.Services.Abstract;

namespace PokeGame.Core.React.Server.Services.Concrete;

internal sealed class ReactAppSettingsEditor: IReactAppSettingsEditor
{
    private readonly string _reactAppSettingsFilePath;
    private readonly ILogger<ReactAppSettingsEditor> _logger;
    private static readonly PollyRetrySettings DefaulyEditAppSettingsRetryPolicy = new ()
    {
        TotalAttempts = 2
    };
    public ReactAppSettingsEditor(string reactAppSettingsFilePath, ILogger<ReactAppSettingsEditor> logger)
    {
        _reactAppSettingsFilePath = reactAppSettingsFilePath;
        _logger = logger;
    }


    public async Task UpdateAppSettingsAsync(Dictionary<string, string> keyValueUpdates, CancellationToken cancellationToken = default)
    {
        using var activity = TelemetryHelperService.ActivitySource.StartActivity();
        foreach (var keyValuePair in keyValueUpdates)
        {
            activity?.SetTag(keyValuePair.Key, keyValuePair.Value);
        }
        _logger.LogInformation("About to attempt to update ReactAppSettings on path: {Path}..", _reactAppSettingsFilePath);
        var retryPipeline = DefaulyEditAppSettingsRetryPolicy.ToPipeline();
        
        await retryPipeline.ExecuteAsync(async ct => await HandleUpdateAppSettingsAsync(keyValueUpdates, ct), cancellationToken);
    }

    private async Task HandleUpdateAppSettingsAsync(Dictionary<string, string> keyValueUpdates,
        CancellationToken cancellationToken = default)
    {
        var readFileAppSettings = await File.ReadAllTextAsync(_reactAppSettingsFilePath, cancellationToken);
        var parsedAppSettings = JsonSerializer.Deserialize<Dictionary<string, string>>(readFileAppSettings)
            ?? throw new JsonException("Failed to parse ReactAppSettings file");
        var newAppSettings = parsedAppSettings.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        foreach (var keyValuePair in keyValueUpdates)
        {
            if (newAppSettings.ContainsKey(keyValuePair.Key))
            {
                newAppSettings[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        if (newAppSettings.Any(x => !parsedAppSettings.TryGetValue(x.Key, out var setting) || !setting.Equals(x.Value)))
        {
            await File.WriteAllTextAsync(_reactAppSettingsFilePath, JsonSerializer.Serialize(newAppSettings), cancellationToken);
        }
    }
}