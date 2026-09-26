namespace AnujTiles.NotificationEngine.Templates;

public static class NotificationTemplates
{
    public static string OtpEmail(
        string otp,
        string applicationName = "Anuj Tiles")
    {
        return $"""
        <div style="font-family:Arial,sans-serif">
            <h2>{applicationName}</h2>

            <p>Your verification code is:</p>

            <div style="
                font-size:32px;
                font-weight:bold;
                letter-spacing:6px;
                margin:20px 0;">
                {otp}
            </div>

            <p>
                This code is for verification.
                Do not share it with anyone.
            </p>
        </div>
        """;
    }

    public static string OtpSms(string otp, string applicationName = "Anuj Tiles")
    {
        return $"{applicationName}: Your verification OTP is {otp}. Do not share this code with anyone.";
    }

    public static string WelcomeEmail(
        string customerName,
        string applicationName = "Anuj Tiles")
    {
        return $"""
        <div style="font-family:Arial,sans-serif">
            <h2>Welcome to {applicationName}</h2>

            <p>
                Dear {customerName},
            </p>

            <p>
                Your account has been created successfully.
            </p>
        </div>
        """;
    }
}