namespace AnujTiles.NotificationEngine.Configuration;

public sealed class NotificationOptions
{
    public const string SectionName = "AnujTilesNotifications";
 
    public string ApplicationName { get; set; } = "";

    public EmailOptions Email { get; set; } = new();
    public SmsOptions Sms { get; set; } = new();
    public WhatsAppOptions WhatsApp { get; set; } = new();
    
}

public sealed class EmailOptions
{
    public string TenantId { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string SenderAddress { get; set; } =
        "notifications@anujtiles.com";
}

public sealed class SmsOptions
{
    public bool Enabled { get; set; } = false;

    public string Provider { get; set; } = "";

    public string BaseUrl { get; set; } = "";

    public string ApiKey { get; set; } = "";

    public string SenderId { get; set; } = "";

    public string Route { get; set; } = "";

    public string DefaultTemplateId { get; set; } = "";
}
public sealed class WhatsAppOptions
{
    public bool Enabled { get; set; } = false;

    public string Provider { get; set; } = "";

    public string BaseUrl { get; set; } = "";

    public string AccessToken { get; set; } = "";

    public string PhoneNumberId { get; set; } = "";

    public string BusinessAccountId { get; set; } = "";

    public string DefaultTemplateName { get; set; } = "";

    public string DefaultLanguageCode { get; set; } = "en";
}