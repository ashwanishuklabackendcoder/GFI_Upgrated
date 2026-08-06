using System.Net.Http.Json;
using System.Text.Json;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.UI.State;

namespace GFI_Upgrated.UI.Services;

public abstract class ApiClientBase
{
    protected readonly HttpClient _httpClient;
    protected readonly AppSessionState _sessionState;

    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected ApiClientBase(HttpClient httpClient, AppSessionState sessionState)
    {
        _httpClient = httpClient;
        _sessionState = sessionState;
    }

    protected void AddAuthHeader()
    {
        if (_sessionState.CurrentUser?.Token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionState.CurrentUser.Token);
        }
    }

    protected async Task<T?> GetEnvelopeAsync<T>(string url, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.GetAsync(url, cancellationToken);
        
        await EnsureSuccessAsync(response, cancellationToken);

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(payload)) return default;

        var envelope = JsonSerializer.Deserialize<ApiEnvelope<T>>(payload, JsonOptions);
        return envelope is { Success: true } ? envelope.Data : default;
    }

    protected async Task<TResponse> PostEnvelopeAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
        
        await EnsureSuccessAsync(response, cancellationToken);

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(payload)) return default!;

        var envelope = JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);
        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    protected async Task<TResponse> DeleteEnvelopeAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        
        await EnsureSuccessAsync(response, cancellationToken);

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(payload)) return default!;

        var envelope = JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);
        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    protected string BuildQuery(string path, PagedRequest request, params (string key, string? value)[] extraParams)
    {
        var query = new List<string>
        {
            $"CurrentPage={request.CurrentPage}",
            $"RecordPerPage={request.RecordPerPage}"
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn))
            query.Add($"SortColumn={Uri.EscapeDataString(request.SortColumn)}");
        
        if (!string.IsNullOrWhiteSpace(request.SortType))
            query.Add($"SortType={Uri.EscapeDataString(request.SortType)}");

        foreach (var (key, value) in extraParams)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                query.Add($"{key}={Uri.EscapeDataString(value)}");
            }
        }

        return $"{path}?{string.Join('&', query)}";
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        
        if (!string.IsNullOrWhiteSpace(payload))
        {
            try
            {
                using var doc = JsonDocument.Parse(payload);
                var root = doc.RootElement;

                // 1. Standard ApiEnvelope check
                if ((root.TryGetProperty("message", out var msgProp) || root.TryGetProperty("Message", out msgProp)) && !string.IsNullOrWhiteSpace(msgProp.GetString()))
                {
                    throw new ApiException(msgProp.GetString()!);
                }

                // 2. ASP.NET Core ValidationProblemDetails ("errors": { "Field": ["Error 1"] })
                if (root.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Object)
                {
                    var errorMessages = new List<string>();
                    foreach (var prop in errorsProp.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var err in prop.Value.EnumerateArray())
                            {
                                if (!string.IsNullOrWhiteSpace(err.GetString()))
                                    errorMessages.Add(err.GetString()!);
                            }
                        }
                    }
                    if (errorMessages.Any())
                    {
                        throw new ApiException(string.Join("; ", errorMessages));
                    }
                }

                // 3. Fallback title or detail
                if (root.TryGetProperty("title", out var titleProp) && !string.IsNullOrWhiteSpace(titleProp.GetString()))
                {
                    throw new ApiException(titleProp.GetString()!);
                }
            }
            catch (ApiException)
            {
                throw;
            }
            catch (JsonException)
            {
                // Payload wasn't JSON
            }
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _sessionState.NotifySessionExpired();
            throw new ApiException("Your session has expired or is unauthorized. Please log in again.");
        }

        throw new ApiException($"An unexpected error occurred (HTTP {(int)response.StatusCode}).");
    }
}
