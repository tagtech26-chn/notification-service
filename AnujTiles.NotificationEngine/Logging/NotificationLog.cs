namespace AnujTiles.NotificationEngine.Logging;

public sealed class NotificationLog
{
    public long Id { get; set; }

    public string ApplicationName { get; set; } = "";

    public string Channel { get; set; } = "";

    public string Recipient { get; set; } = "";

    public string? Subject { get; set; }

    public string? TemplateName { get; set; }

    public string Status { get; set; } = "";

    public string? ProviderMessageId { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}