using AnujTiles.NotificationEngine.Models;

namespace AnujTiles.NotificationEngine.Services;

public interface IEmailService
{
    Task<NotificationResult> SendAsync(
        EmailRequest request,
        CancellationToken cancellationToken = default);
}

public interface ISmsService
{
    Task<NotificationResult> SendAsync(
        SmsRequest request,
        CancellationToken cancellationToken = default);
}


public interface IWhatsAppService
{
    Task<NotificationResult> SendAsync(
        WhatsAppRequest request,
        CancellationToken cancellationToken = default);
}

public interface IOtpService
{
    string Generate(int length = 6);
}

public interface INotificationService
{
    Task<NotificationResult> SendEmailAsync(
        EmailRequest request,
        CancellationToken cancellationToken = default);

    Task<NotificationResult> SendSmsAsync(
        SmsRequest request,
        CancellationToken cancellationToken = default);

    Task<NotificationResult> SendWhatsAppAsync(
        WhatsAppRequest request,
        CancellationToken cancellationToken = default);

    Task<NotificationResult> SendOtpSmsAsync(
    string mobileNumber,
    string otp,
    string? templateId = null,
    CancellationToken cancellationToken = default);

    Task<NotificationResult> SendOtpWhatsAppAsync(
    string mobileNumber,
    string otp,
    string templateName,
    string languageCode = "en",
    CancellationToken cancellationToken = default);

    Task<NotificationResult> SendAsync(
        NotificationChannel channel,
        object request,
        CancellationToken cancellationToken = default);

    string GenerateOtp(int length = 6);
}