using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AnujTiles.NotificationEngine.Logging;

public sealed class NotificationLogService
    : INotificationLogService
{
    private readonly NotificationDbContext _db;
    private readonly NotificationLoggingOptions _options;

    public NotificationLogService(
        NotificationDbContext db,
        IOptions<NotificationLoggingOptions> options)
    {
        _db = db;
        _options = options.Value;
    }

    public async Task<long> CreateAsync(
        NotificationLog log,
        CancellationToken cancellationToken = default)
    {
        log.ApplicationName =
            string.IsNullOrWhiteSpace(log.ApplicationName)
                ? _options.ApplicationName
                : log.ApplicationName;

        log.CreatedAt = DateTime.Now;

        _db.NotificationLogs.Add(log);

        await _db.SaveChangesAsync(
            cancellationToken);

        return log.Id;
    }

    public async Task CompleteAsync(
        long id,
        string status,
        string? providerMessageId = null,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        var log =
            await _db.NotificationLogs
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (log == null)
            return;

        log.Status = status;
        log.ProviderMessageId =
            providerMessageId;
        log.ErrorMessage =
            errorMessage;
        log.CompletedAt = DateTime.Now;

        await _db.SaveChangesAsync(
            cancellationToken);
    }
}