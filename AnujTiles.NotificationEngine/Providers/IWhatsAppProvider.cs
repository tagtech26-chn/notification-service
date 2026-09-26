using AnujTiles.NotificationEngine.Models;

namespace AnujTiles.NotificationEngine.Providers;

public interface IWhatsAppProvider
{
    string ProviderName { get; }

    Task<NotificationResult> SendAsync(
        WhatsAppRequest request,
        CancellationToken cancellationToken = default);
}