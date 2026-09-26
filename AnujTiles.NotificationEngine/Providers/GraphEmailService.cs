using AnujTiles.NotificationEngine.Configuration;
using AnujTiles.NotificationEngine.Models;
using AnujTiles.NotificationEngine.Services;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace AnujTiles.NotificationEngine.Providers;

public sealed class GraphEmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly GraphServiceClient _graph;

    public GraphEmailService(
        IOptions<NotificationOptions> options)
    {
        _options = options.Value.Email;

        ValidateConfiguration();

        var credential = new ClientSecretCredential(
            _options.TenantId,
            _options.ClientId,
            _options.ClientSecret);

        _graph = new GraphServiceClient(
            credential,
            new[]
            {
                "https://graph.microsoft.com/.default"
            });
    }

    public async Task<NotificationResult> SendAsync(
        EmailRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.To))
            return NotificationResult.Failed(
                "Email recipient is required.");

        if (string.IsNullOrWhiteSpace(request.Subject))
            return NotificationResult.Failed(
                "Email subject is required.");

        if (string.IsNullOrWhiteSpace(request.HtmlBody))
            return NotificationResult.Failed(
                "Email HTML body is required.");

        try
        {
            var message = new Message
            {
                Subject = request.Subject,

                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = request.HtmlBody
                },

                ToRecipients =
                [
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = request.To,
                            Name = request.ToName
                        }
                    }
                ]
            };

            await _graph
                .Users[_options.SenderAddress]
                .SendMail
                .PostAsync(
                    new SendMailPostRequestBody
                    {
                        Message = message,
                        SaveToSentItems =
                            request.SaveToSentItems
                    },
                    cancellationToken: cancellationToken);

            return NotificationResult.Ok();
        }
        catch (Exception ex)
        {
            return NotificationResult.Failed(ex.Message);
        }
    }

    private void ValidateConfiguration()
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(_options.TenantId))
            missing.Add("TenantId");

        if (string.IsNullOrWhiteSpace(_options.ClientId))
            missing.Add("ClientId");

        if (string.IsNullOrWhiteSpace(_options.ClientSecret))
            missing.Add("ClientSecret");

        if (string.IsNullOrWhiteSpace(_options.SenderAddress))
            missing.Add("SenderAddress");

        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                "Microsoft 365 email configuration is missing: " +
                string.Join(", ", missing));
        }
    }
}