using AnujTiles.NotificationEngine.Configuration;
using AnujTiles.NotificationEngine.Logging;
using AnujTiles.NotificationEngine.Models;
using AnujTiles.NotificationEngine.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AnujTiles.NotificationEngine.Services;

public sealed class NotificationService : INotificationService
{
    private readonly IEmailService _email;
    private readonly ISmsService _sms;
    private readonly IWhatsAppService _whatsapp;
    private readonly IOtpService _otp;
    private readonly INotificationLogService? _logService;
    private readonly NotificationOptions _options;

    public NotificationService(
        IEmailService email,
        ISmsService sms,
        IWhatsAppService whatsapp,
        IOtpService otp,
        IOptions<NotificationOptions> options,
        IServiceProvider serviceProvider)
    {
        _email = email;
        _sms = sms;
        _whatsapp = whatsapp;
        _otp = otp;
        _options = options.Value;

        // Logging is optional.
        _logService =
            serviceProvider.GetService<INotificationLogService>();
    }

    public Task<NotificationResult> SendEmailAsync(
        EmailRequest request,
        CancellationToken cancellationToken = default)
    {
        return SendEmailWithLoggingAsync(
            request,
            cancellationToken);
    }

    public async Task<NotificationResult> SendOtpSmsAsync(
    string mobileNumber,
    string otp,
    string? templateId = null,
    CancellationToken cancellationToken = default)
    {
        var request = new SmsRequest
        {
            MobileNumber = mobileNumber,
            Message = NotificationTemplates.OtpSms(otp),
            TemplateId = templateId,
            MessageType = "OTP",
            Variables = new Dictionary<string, string>
            {
                ["otp"] = otp
            }
        };

        return await SendSmsAsync(request, cancellationToken);
    }

    public async Task<NotificationResult> SendOtpWhatsAppAsync(
    string mobileNumber,
    string otp,
    string templateName,
    string languageCode = "en",
    CancellationToken cancellationToken = default)
    {
        var request = new WhatsAppRequest
        {
            MobileNumber = mobileNumber,
            TemplateName = templateName,
            LanguageCode = languageCode,
            Parameters = [otp]
        };

        return await SendWhatsAppAsync(
            request,
            cancellationToken);
    }

    public Task<NotificationResult> SendSmsAsync(
        SmsRequest request,
        CancellationToken cancellationToken = default)
    {
        return SendSmsWithLoggingAsync(
            request,
            cancellationToken);
    }

    public Task<NotificationResult> SendWhatsAppAsync(
        WhatsAppRequest request,
        CancellationToken cancellationToken = default)
    {
        return SendWhatsAppWithLoggingAsync(
            request,
            cancellationToken);
    }

    public async Task<NotificationResult> SendAsync(
        NotificationChannel channel,
        object request,
        CancellationToken cancellationToken = default)
    {
        return channel switch
        {
            NotificationChannel.Email
                when request is EmailRequest email =>
                    await SendEmailAsync(
                        email,
                        cancellationToken),

            NotificationChannel.Sms
                when request is SmsRequest sms =>
                    await SendSmsAsync(
                        sms,
                        cancellationToken),

            NotificationChannel.WhatsApp
                when request is WhatsAppRequest whatsapp =>
                    await SendWhatsAppAsync(
                        whatsapp,
                        cancellationToken),

            _ => NotificationResult.Failed(
                $"Invalid request for {channel}.")
        };
    }

    public string GenerateOtp(int length = 6)
    {
        return _otp.Generate(length);
    }

    private async Task<NotificationResult>
        SendEmailWithLoggingAsync(
            EmailRequest request,
            CancellationToken cancellationToken)
    {
        long logId = 0;

        try
        {
            logId = await CreateLogAsync(
                channel: "Email",
                recipient: request.To,
                subject: request.Subject,
                cancellationToken);

            var result =
                await _email.SendAsync(
                    request,
                    cancellationToken);

            await CompleteLogAsync(
                logId,
                result,
                cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await CompleteLogExceptionAsync(
                logId,
                ex,
                cancellationToken);

            return NotificationResult.Failed(
                ex.Message);
        }
    }

    private async Task<NotificationResult>
        SendSmsWithLoggingAsync(
            SmsRequest request,
            CancellationToken cancellationToken)
    {
        long logId = 0;

        try
        {
            logId = await CreateLogAsync(
                channel: "SMS",
                recipient: request.MobileNumber,
                subject: null,
                cancellationToken);

            var result =
                await _sms.SendAsync(
                    request,
                    cancellationToken);

            await CompleteLogAsync(
                logId,
                result,
                cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await CompleteLogExceptionAsync(
                logId,
                ex,
                cancellationToken);

            return NotificationResult.Failed(
                ex.Message);
        }
    }

    private async Task<NotificationResult>
        SendWhatsAppWithLoggingAsync(
            WhatsAppRequest request,
            CancellationToken cancellationToken)
    {
        long logId = 0;

        try
        {
            logId = await CreateLogAsync(
                channel: "WhatsApp",
                recipient: request.MobileNumber,
                subject: request.TemplateName,
                cancellationToken);

            var result =
                await _whatsapp.SendAsync(
                    request,
                    cancellationToken);

            await CompleteLogAsync(
                logId,
                result,
                cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await CompleteLogExceptionAsync(
                logId,
                ex,
                cancellationToken);

            return NotificationResult.Failed(
                ex.Message);
        }
    }

    private async Task<long> CreateLogAsync(
        string channel,
        string recipient,
        string? subject,
        CancellationToken cancellationToken)
    {
        if (_logService == null)
            return 0;

        return await _logService.CreateAsync(
            new NotificationLog
            {
                ApplicationName =
                    _options.ApplicationName,

                Channel = channel,

                Recipient = recipient,

                Subject = subject,

                Status = "Sending"
            },
            cancellationToken);
    }

    private async Task CompleteLogAsync(
        long logId,
        NotificationResult result,
        CancellationToken cancellationToken)
    {
        if (_logService == null ||
            logId == 0)
            return;

        await _logService.CompleteAsync(
            logId,
            result.Success ? "Sent" : "Failed",
            result.ProviderMessageId,
            result.Error,
            cancellationToken);
    }

    private async Task CompleteLogExceptionAsync(
        long logId,
        Exception ex,
        CancellationToken cancellationToken)
    {
        if (_logService == null ||
            logId == 0)
            return;

        await _logService.CompleteAsync(
            logId,
            "Failed",
            null,
            ex.Message,
            cancellationToken);
    }
}