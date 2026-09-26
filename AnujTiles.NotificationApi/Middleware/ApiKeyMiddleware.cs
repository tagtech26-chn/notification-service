using System.Security.Cryptography;
using System.Text;

namespace AnujTiles.NotificationApi.Middleware;

public sealed class ApiKeyMiddleware
{
    private const string HeaderName = "X-Notification-Api-Key";
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var expected = _configuration["NotificationApi:ApiKey"];

        if (string.IsNullOrWhiteSpace(expected))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Notification API key is not configured."
            });
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var supplied) ||
            !FixedTimeEquals(supplied.ToString(), expected))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Invalid notification API key."
            });
            return;
        }

        await _next(context);
    }

    private static bool FixedTimeEquals(string supplied, string expected)
    {
        var left = Encoding.UTF8.GetBytes(supplied);
        var right = Encoding.UTF8.GetBytes(expected);

        return left.Length == right.Length &&
               CryptographicOperations.FixedTimeEquals(left, right);
    }
}
