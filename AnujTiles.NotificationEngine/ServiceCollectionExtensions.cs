using AnujTiles.NotificationEngine.Configuration;
using AnujTiles.NotificationEngine.Logging;
using AnujTiles.NotificationEngine.Providers;
using AnujTiles.NotificationEngine.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnujTiles.NotificationEngine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection
        AddAnujTilesNotifications(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.Configure<NotificationOptions>(
            configuration.GetSection(
                NotificationOptions.SectionName));

        services.Configure<NotificationLoggingOptions>(
            configuration.GetSection(
                NotificationLoggingOptions.SectionName));

        var loggingOptions =
            configuration
                .GetSection(
                    NotificationLoggingOptions.SectionName)
                .Get<NotificationLoggingOptions>();

        if (loggingOptions?.Enabled == true &&
            !string.IsNullOrWhiteSpace(
                loggingOptions.ConnectionString))
        {
            services.AddDbContext<
                NotificationDbContext>(
                options =>
                    options.UseSqlServer(
                        loggingOptions.ConnectionString));

            services.AddScoped<
                INotificationLogService,
                NotificationLogService>();
        }

        services.AddSingleton<IEmailService,
            GraphEmailService>();

        services.AddSingleton<ISmsService, SmsProvider>();

        services.AddSingleton<IWhatsAppService,
            WhatsAppProvider>();

        services.AddSingleton<IOtpService,
            OtpService>();

        services.AddHttpClient<ISmsHttpClient, SmsHttpClient>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddSingleton<ISmsProvider, ConfigurableSmsProvider>();
        services.AddSingleton<IWhatsAppProvider, ConfigurableWhatsAppProvider>();

        services.AddSingleton<
            IWhatsAppProviderRegistry,
            WhatsAppProviderRegistry>();


        return services;
    }
}