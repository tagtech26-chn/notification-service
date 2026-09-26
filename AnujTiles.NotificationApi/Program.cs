using AnujTiles.NotificationApi.Middleware;
using AnujTiles.NotificationEngine;
using AnujTiles.NotificationEngine.Configuration;
using AnujTiles.NotificationEngine.Models;
using AnujTiles.NotificationEngine.Services;
using AnujTiles.NotificationEngine.Templates;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAnujTilesNotifications(builder.Configuration);
builder.Services.AddSingleton<NotificationOptionsAccessor>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "AnujTiles.NotificationApi",
    utc = DateTime.UtcNow
}));

app.MapPost("/api/notifications/email", async (
    EmailRequest request,
    INotificationService notifications,
    CancellationToken cancellationToken) =>
{
    var result = await notifications.SendEmailAsync(request, cancellationToken);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

app.MapPost("/api/notifications/sms", async (
    SmsRequest request,
    INotificationService notifications,
    CancellationToken cancellationToken) =>
{
    var result = await notifications.SendSmsAsync(request, cancellationToken);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

app.MapPost("/api/notifications/whatsapp", async (
    WhatsAppRequest request,
    INotificationService notifications,
    CancellationToken cancellationToken) =>
{
    var result = await notifications.SendWhatsAppAsync(request, cancellationToken);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

app.MapPost("/api/notifications/otp/email", async (
    OtpEmailRequest request,
    INotificationService notifications,
    NotificationOptionsAccessor options,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.To))
        return Results.BadRequest(new { success = false, error = "Email recipient is required." });

    var otp = notifications.GenerateOtp(request.Length);
    var html = NotificationTemplates.OtpEmail(
        otp,
        string.IsNullOrWhiteSpace(request.ApplicationName)
            ? options.ApplicationName
            : request.ApplicationName!);

    var result = await notifications.SendEmailAsync(
        new EmailRequest
        {
            To = request.To,
            ToName = request.ToName,
            Subject = request.Subject ?? "Your OTP",
            HtmlBody = html
        },
        cancellationToken);

    return result.Success
        ? Results.Ok(new { result.Success, result.ProviderMessageId, otp })
        : Results.BadRequest(result);
});

app.MapPost("/api/notifications/otp/sms", async (
    OtpSmsRequest request,
    INotificationService notifications,
    CancellationToken cancellationToken) =>
{
    var otp = notifications.GenerateOtp(request.Length);
    var result = await notifications.SendOtpSmsAsync(
        request.MobileNumber,
        otp,
        request.TemplateId,
        cancellationToken);

    return result.Success
        ? Results.Ok(new { result.Success, result.ProviderMessageId, otp })
        : Results.BadRequest(result);
});

app.MapPost("/api/notifications/otp/whatsapp", async (
    OtpWhatsAppRequest request,
    INotificationService notifications,
    CancellationToken cancellationToken) =>
{
    var otp = notifications.GenerateOtp(request.Length);
    var result = await notifications.SendOtpWhatsAppAsync(
        request.MobileNumber,
        otp,
        request.TemplateName,
        request.LanguageCode,
        cancellationToken);

    return result.Success
        ? Results.Ok(new { result.Success, result.ProviderMessageId, otp })
        : Results.BadRequest(result);
});

app.Run();

public sealed record OtpEmailRequest(
    string To,
    string? ToName = null,
    string? Subject = null,
    string? ApplicationName = null,
    int Length = 6);

public sealed record OtpSmsRequest(
    string MobileNumber,
    string? TemplateId = null,
    int Length = 6);

public sealed record OtpWhatsAppRequest(
    string MobileNumber,
    string TemplateName,
    string LanguageCode = "en",
    int Length = 6);

public sealed class NotificationOptionsAccessor
{
    private readonly Microsoft.Extensions.Options.IOptions<NotificationOptions> _options;

    public NotificationOptionsAccessor(Microsoft.Extensions.Options.IOptions<NotificationOptions> options)
        => _options = options;

    public string ApplicationName => _options.Value.ApplicationName;
}