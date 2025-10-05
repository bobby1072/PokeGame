namespace PokeGame.Core.Api.Extensions;

internal static class HttpContextExtensions
{
    public static void TryAddToItems<T>(this HttpContext context, string itemKey,T item, ILogger? logger = null)
    {
        if (context.Items.TryAdd(itemKey, item))
        {
            logger?.LogDebug("Added item: {@Item} with key: {Key} to request items", 
                item,
                itemKey
            );
        }
        else
        {
            logger?.LogWarning("Failed to add item: {@Item} with key: {Key} to request items",
                item,
                itemKey
            );
        }
    }
}