namespace AnujTiles.NotificationEngine.Logging;

public interface INotificationLogService
{
    Task<long> CreateAsync(
        NotificationLog log,
        CancellationToken cancellationToken = default);

    Task CompleteAsync(
        long id,
        string status,
        string? providerMessageId = null,
        string? errorMessage = null,
        CancellationToken cancellationToken = default);
}