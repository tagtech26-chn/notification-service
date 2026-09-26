using System.Net.Http.Json;

namespace AnujTiles.NotificationEngine.Providers;

public sealed class SmsHttpClient : ISmsHttpClient
{
    private readonly HttpClient _httpClient;

    public SmsHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> PostAsync(
        string url,
        Dictionary<string, string> headers,
        object payload,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            url);

        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(
                header.Key,
                header.Value);
        }

        request.Content = JsonContent.Create(payload);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"SMS provider returned HTTP {(int)response.StatusCode}: {responseBody}");
        }

        return responseBody;
    }
}