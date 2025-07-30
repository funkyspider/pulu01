using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Savant.Pulse.Utility.Client.PULU01.Configuration;
using Savant.Pulse.Utility.Client.PULU01.Models;

namespace Savant.Pulse.Utility.Client.PULU01.Services;

public class HttpApiClientService : IApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly AppConfiguration _configuration;
    private readonly ILogger<HttpApiClientService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public HttpApiClientService(
        HttpClient httpClient,
        IOptions<AppConfiguration> configuration,
        ILogger<HttpApiClientService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration.Value;
        _logger = logger;
        
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        if (!string.IsNullOrEmpty(_configuration.Api.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_configuration.Api.BaseUrl);
        }
        
        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.Api.TimeoutSeconds);
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-UserId", _configuration.Api.Headers.XUserId);
        _httpClient.DefaultRequestHeaders.Add("X-AppName", _configuration.Api.Headers.XAppName);
        _httpClient.DefaultRequestHeaders.Add("X-Environment", _configuration.Api.Headers.XEnvironment);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<ProcessingResult> ClearHoldAsync(DonationRecord record, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Clearing hold for record: {RecordKey}", record.GetKey());

            var request = ClearHoldRequestDto.FromDonationRecord(record);
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_configuration.Api.ClearHoldEndpoint, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var responseDto = JsonSerializer.Deserialize<ClearHoldResponseDto>(responseContent, JsonOptions);

                if (responseDto?.Success == true)
                {
                    _logger.LogDebug("Successfully cleared hold for record: {RecordKey}", record.GetKey());
                    return new ProcessingResult(record, ProcessingStatus.Success, responseDto.Message);
                }
                else
                {
                    var errorMessage = responseDto?.Message ?? "Unknown API error";
                    _logger.LogWarning("API returned failure for record {RecordKey}: {ErrorMessage}", record.GetKey(), errorMessage);
                    return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var errorMessage = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {errorContent}";
                _logger.LogError("HTTP error clearing hold for record {RecordKey}: {ErrorMessage}", record.GetKey(), errorMessage);
                return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            var errorMessage = $"HTTP request failed: {ex.Message}";
            _logger.LogError(ex, "HTTP request exception clearing hold for record {RecordKey}", record.GetKey());
            return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            var errorMessage = "Request timeout";
            _logger.LogError(ex, "Timeout clearing hold for record {RecordKey}", record.GetKey());
            return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
        }
        catch (TaskCanceledException ex)
        {
            var errorMessage = "Request cancelled";
            _logger.LogWarning(ex, "Request cancelled for record {RecordKey}", record.GetKey());
            return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
        }
        catch (JsonException ex)
        {
            var errorMessage = $"JSON serialization/deserialization error: {ex.Message}";
            _logger.LogError(ex, "JSON error clearing hold for record {RecordKey}", record.GetKey());
            return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Unexpected error: {ex.Message}";
            _logger.LogError(ex, "Unexpected error clearing hold for record {RecordKey}", record.GetKey());
            return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
        }
    }
}