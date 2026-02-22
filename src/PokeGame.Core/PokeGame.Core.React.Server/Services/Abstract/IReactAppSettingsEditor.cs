namespace PokeGame.Core.React.Server.Services.Abstract;

internal interface IReactAppSettingsEditor
{
    Task UpdateAppSettingsAsync(Dictionary<string, string> keyValueUpdates,
        CancellationToken cancellationToken = default);
}