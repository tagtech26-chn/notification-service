namespace AnujTiles.NotificationEngine.Models;

public enum NotificationChannel
{
    Email,
    Sms,
    WhatsApp
}

public sealed class EmailRequest
{
    public string To { get; init; } = "";
    public string? ToName { get; init; }
    public string Subject { get; init; } = "";
    public string HtmlBody { get; init; } = "";
    public bool SaveToSentItems { get; init; } = true;
}

public sealed class SmsRequest
{
    public string MobileNumber { get; init; } = "";
    public string Message { get; init; } = "";

    // Provider/DLT template identifier.
    // This will be used when we connect MSG91 or another provider.
    public string? TemplateId { get; init; }

    // Optional provider template variables.
    // Example: OTP, customer name, invoice number, etc.
    public IReadOnlyDictionary<string, string> Variables { get; init; }
        = new Dictionary<string, string>();

    public string? MessageType { get; init; }
}

public sealed class WhatsAppRequest
{
    public string MobileNumber { get; init; } = "";
    public string TemplateName { get; init; } = "";
    public string LanguageCode { get; init; } = "en";
    public IReadOnlyList<string> Parameters { get; init; } = [];
}

public sealed class NotificationResult
{
    public bool Success { get; init; }
    public string? ProviderMessageId { get; init; }
    public string? Error { get; init; }

    public static NotificationResult Ok(string? id = null) =>
        new()
        {
            Success = true,
            ProviderMessageId = id
        };

    public static NotificationResult Failed(string error) =>
        new()
        {
            Success = false,
            Error = error
        };
}