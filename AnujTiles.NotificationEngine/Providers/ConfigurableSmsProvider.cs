using AnujTiles.NotificationEngine.Configuration;
using AnujTiles.NotificationEngine.Models;
using Microsoft.Extensions.Options;

namespace AnujTiles.NotificationEngine.Providers;

public sealed class ConfigurableSmsProvider : ISmsProvider
{
    private readonly SmsOptions _options;

    public ConfigurableSmsProvider(
        IOptions<NotificationOptions> options)
    {
        _options = options.Value.Sms;
    }

    public Task<NotificationResult> SendAsync(
        SmsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return Task.FromResult(
                NotificationResult.Failed(
                    "SMS provider is disabled."));
        }

        if (string.IsNullOrWhiteSpace(_options.Provider))
        {
            return Task.FromResult(
                NotificationResult.Failed(
                    "SMS provider is not configured."));
        }

        return Task.FromResult(
            NotificationResult.Failed(
                $"SMS provider '{_options.Provider}' is configured but its API adapter is not implemented yet."));
    }
}