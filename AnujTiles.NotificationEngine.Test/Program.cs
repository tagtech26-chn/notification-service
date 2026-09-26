using AnujTiles.NotificationEngine;
using AnujTiles.NotificationEngine.Models;
using AnujTiles.NotificationEngine.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAnujTilesNotifications(
    builder.Configuration);

using var host = builder.Build();

var notifications =
    host.Services.GetRequiredService<INotificationService>();

var recipient =
    Environment.GetEnvironmentVariable(
        "ANUJ_TILES_TEST_RECIPIENT");

if (string.IsNullOrWhiteSpace(recipient))
{
    Console.WriteLine(
        "ERROR: ANUJ_TILES_TEST_RECIPIENT is not set.");

    return;
}

Console.WriteLine();
Console.WriteLine("======================================");
Console.WriteLine(" AnujTiles Notification Engine");
Console.WriteLine("======================================");

var otp = notifications.GenerateOtp();

Console.WriteLine($"Generated OTP: {otp}");
Console.WriteLine("Sending test email...");

var result =
    await notifications.SendEmailAsync(
        new EmailRequest
        {
            To = recipient,

            Subject =
                "Anuj Tiles Notification Engine Test",

            HtmlBody = $"""
                <div style="font-family:Arial,sans-serif">
                    <h2>Anuj Tiles Notification Engine</h2>

                    <p>
                        This email was sent through the
                        reusable notification engine.
                    </p>

                    <p>
                        <strong>Sender:</strong>
                        notifications@anujtiles.com
                    </p>

                    <p>
                        <strong>Test OTP:</strong>
                        {otp}
                    </p>
                </div>
                """,

            SaveToSentItems = true
        });

Console.WriteLine();

if (result.Success)
{
    Console.WriteLine(
        "======================================");

    Console.WriteLine(
        " EMAIL SENT SUCCESSFULLY");

    Console.WriteLine(
        "======================================");
}
else
{
    Console.WriteLine("EMAIL FAILED");
    Console.WriteLine(result.Error);
}
Console.WriteLine();
Console.WriteLine("Sending test SMS...");

var smsOtp = notifications.GenerateOtp();

var smsResult = await notifications.SendOtpSmsAsync(
    "919876543210",
    smsOtp);

if (smsResult.Success)
{
    Console.WriteLine("SMS SENT SUCCESSFULLY");
}
else
{
    Console.WriteLine($"SMS RESULT: {smsResult.Error}");
}

Console.WriteLine();
Console.WriteLine("Sending test WhatsApp...");

var whatsappOtp = notifications.GenerateOtp();

var whatsappResult = await notifications.SendOtpWhatsAppAsync(
    "919876543210",
    whatsappOtp,
    "otp_template");

if (whatsappResult.Success)
{
    Console.WriteLine("WHATSAPP SENT SUCCESSFULLY");
}
else
{
    Console.WriteLine($"WHATSAPP RESULT: {whatsappResult.Error}");
}