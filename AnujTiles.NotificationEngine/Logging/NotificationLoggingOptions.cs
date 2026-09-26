namespace AnujTiles.NotificationEngine.Logging;

public sealed class NotificationLoggingOptions
{
    public const string SectionName =
        "AnujTilesNotifications:Logging";

    public bool Enabled { get; set; } = true;

    public string ApplicationName { get; set; } = "";

    public string ConnectionString { get; set; } = "";
}