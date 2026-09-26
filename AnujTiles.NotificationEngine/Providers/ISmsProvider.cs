using AnujTiles.NotificationEngine.Models;

namespace AnujTiles.NotificationEngine.Providers;

public interface ISmsProvider
{
    Task<NotificationResult> SendAsync(
        SmsRequest request,
        CancellationToken cancellationToken = default);
}