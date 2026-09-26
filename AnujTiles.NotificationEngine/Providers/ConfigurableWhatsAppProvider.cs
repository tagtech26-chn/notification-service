using AnujTiles.NotificationEngine.Configuration;
using AnujTiles.NotificationEngine.Models;
using Microsoft.Extensions.Options;

namespace AnujTiles.NotificationEngine.Providers;

public sealed class ConfigurableWhatsAppProvider : IWhatsAppProvider
{
    private readonly WhatsAppOptions _options;

    public ConfigurableWhatsAppProvider(
        IOptions<NotificationOptions> options)
    {
        _options = options.Value.WhatsApp;
    }

    public string ProviderName => "CONFIGURABLE";

    public Task<NotificationResult> SendAsync(
        WhatsAppRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return Task.FromResult(
                NotificationResult.Failed(
                    "WhatsApp provider is disabled."));
        }

        return Task.FromResult(
            NotificationResult.Failed(
                $"Provider '{ProviderName}' has no API adapter configured yet."));
    }
}