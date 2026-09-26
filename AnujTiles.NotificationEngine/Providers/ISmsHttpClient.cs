namespace AnujTiles.NotificationEngine.Providers;

public interface ISmsHttpClient
{
    Task<string> PostAsync(
        string url,
        Dictionary<string, string> headers,
        object payload,
        CancellationToken cancellationToken = default);
}