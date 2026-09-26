using AnujTiles.NotificationEngine.Configuration;
using AnujTiles.NotificationEngine.Models;
using AnujTiles.NotificationEngine.Services;
using Microsoft.Extensions.Options;

namespace AnujTiles.NotificationEngine.Providers;


public sealed class SmsProvider : ISmsService
{
    private readonly ISmsProvider _provider;

    public SmsProvider(ISmsProvider provider)
    {
        _provider = provider;
    }

    public Task<NotificationResult> SendAsync(
        SmsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.MobileNumber))
        {
            return Task.FromResult(
                NotificationResult.Failed(
                    "SMS mobile number is required."));
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return Task.FromResult(
                NotificationResult.Failed(
                    "SMS message is required."));
        }

        return _provider.SendAsync(request, cancellationToken);
    }
}

public sealed class WhatsAppProvider : IWhatsAppService
{
    private readonly IWhatsAppProviderRegistry _registry;
    private readonly IOptions<NotificationOptions> _options;

    public WhatsAppProvider(
        IWhatsAppProviderRegistry registry,
        IOptions<NotificationOptions> options)
    {
        _registry = registry;
        _options = options;
    }

    public async Task<NotificationResult> SendAsync(
        WhatsAppRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.MobileNumber))
        {
            return NotificationResult.Failed(
                "WhatsApp mobile number is required.");
        }

        if (string.IsNullOrWhiteSpace(request.TemplateName))
        {
            return NotificationResult.Failed(
                "WhatsApp template name is required.");
        }

        try
        {
            var providerName = _options.Value.WhatsApp.Provider;

            var provider = _registry.Get(providerName);

            return await provider.SendAsync(
                request,
                cancellationToken);
        }
        catch (Exception ex)
        {
            return NotificationResult.Failed(ex.Message);
        }
    }
}