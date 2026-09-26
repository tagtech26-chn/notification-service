namespace AnujTiles.NotificationEngine.Providers;

public interface IWhatsAppProviderRegistry
{
    IWhatsAppProvider Get(string providerName);
}

public sealed class WhatsAppProviderRegistry : IWhatsAppProviderRegistry
{
    private readonly Dictionary<string, IWhatsAppProvider> _providers;

    public WhatsAppProviderRegistry(
        IEnumerable<IWhatsAppProvider> providers)
    {
        _providers = providers
            .Where(x => !string.IsNullOrWhiteSpace(x.ProviderName))
            .ToDictionary(
                x => x.ProviderName,
                StringComparer.OrdinalIgnoreCase);
    }

    public IWhatsAppProvider Get(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
        {
            throw new InvalidOperationException(
                "WhatsApp provider name is not configured.");
        }

        if (!_providers.TryGetValue(
                providerName,
                out var provider))
        {
            throw new InvalidOperationException(
                $"WhatsApp provider '{providerName}' is not registered.");
        }

        return provider;
    }
}