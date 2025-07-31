using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Savant.Pulse.Utility.Client.PULU01.Configuration;
using Savant.Pulse.Utility.Client.PULU01.enums;
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
        WriteIndented = false,
        TypeInfoResolver = AppJsonContext.Default
    };

    public HttpApiClientService(
        HttpClient httpClient,
        AppConfiguration configuration,
        ILogger<HttpApiClientService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        if (!string.IsNullOrEmpty(_configuration.Api.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_configuration.Api.BaseUrl);
            _logger.LogInformation("HTTP Client Base Address: {BaseAddress}", _httpClient.BaseAddress);
        }
    
        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.Api.TimeoutSeconds);
        _logger.LogInformation("HTTP Client Timeout: {Timeout}s", _configuration.Api.TimeoutSeconds);
    
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-UserId", _configuration.Api.Headers.XUserId);
        _httpClient.DefaultRequestHeaders.Add("X-AppName", _configuration.Api.Headers.XAppName);
        _httpClient.DefaultRequestHeaders.Add("X-Environment", _configuration.Api.Headers.XEnvironment);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        _logger.LogInformation("API Endpoint: {Endpoint}", _configuration.Api.ClearHoldEndpoint);
        _logger.LogInformation("API Headers configured: X-UserId={XUserId}, X-AppName={XAppName}, X-Environment={XEnvironment}", 
            _configuration.Api.Headers.XUserId, 
            _configuration.Api.Headers.XAppName, 
            _configuration.Api.Headers.XEnvironment);
    }

public async Task<ProcessingResult> ClearHoldAsync(DonationRecord record, string clearCode, CancellationToken cancellationToken = default)
{
    try
    {
        _logger.LogDebug("Clearing hold for record: {RecordKey}", record.GetKey());

        var request = ClearHoldRequestDto.FromDonationRecord(record, clearCode);
        var json = JsonSerializer.Serialize(request, JsonOptions);
        
        // Log the request details for debugging
        _logger.LogDebug("API Request URL: {Url}", $"{_httpClient.BaseAddress}{_configuration.Api.ClearHoldEndpoint}");
        _logger.LogDebug("API Request Headers: {Headers}", 
            string.Join(", ", _httpClient.DefaultRequestHeaders.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));
        _logger.LogDebug("API Request Body: {RequestBody}", json);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_configuration.Api.ClearHoldEndpoint, content, cancellationToken);

        // Log response details
        _logger.LogDebug("API Response Status: {StatusCode} {ReasonPhrase}", 
            (int)response.StatusCode, response.ReasonPhrase);
        _logger.LogDebug("API Response Headers: {Headers}", 
            string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("API Response Body: {ResponseBody}", responseContent);

        if (response.IsSuccessStatusCode)
        {
            var responseDto = JsonSerializer.Deserialize<ClearHoldResponseDto>(responseContent, JsonOptions);

            if (responseDto?.Status == ClearHoldStatus.Cleared)
            {
                _logger.LogInformation("Hold cleared successfully - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}", 
                    record.DonationNumber, record.ProductCode, record.HoldCode);
                _logger.LogDebug("Successfully cleared hold for record: {RecordKey}", record.GetKey());
                return new ProcessingResult(record, ProcessingStatus.Success, string.Empty);
            }
            else
            {
                var errorMessage = responseDto?.ErrorMessage ?? "Unknown API error";
                _logger.LogWarning("Hold clear failed - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}, Error: {ErrorMessage}", 
                    record.DonationNumber, record.ProductCode, record.HoldCode, errorMessage);
                _logger.LogWarning("API returned failure for record {RecordKey}: {ErrorMessage}", record.GetKey(), errorMessage);
                return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
            }
        }
        else
        {
            // Enhanced error handling for status code 477
            if ((int)response.StatusCode == 477)
            {
                _logger.LogWarning("HTTP 477 error - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}, Error: Business rule violation or data validation error", 
                    record.DonationNumber, record.ProductCode, record.HoldCode);
                _logger.LogError("API returned custom error 477 for record {RecordKey}. This may indicate a business rule violation or data validation error.", record.GetKey());
            }
            else
            {
                _logger.LogWarning("HTTP error - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}, Status: {StatusCode} {ReasonPhrase}", 
                    record.DonationNumber, record.ProductCode, record.HoldCode, (int)response.StatusCode, response.ReasonPhrase);
            }

            var errorMessage = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {responseContent}";
            _logger.LogError("HTTP error clearing hold for record {RecordKey}: {ErrorMessage}", record.GetKey(), errorMessage);
            return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
        }
    }
    catch (HttpRequestException ex)
    {
        var errorMessage = $"HTTP request failed: {ex.Message}";
        _logger.LogWarning("Request failed - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}, Error: {ErrorMessage}", 
            record.DonationNumber, record.ProductCode, record.HoldCode, ex.Message);
        _logger.LogError(ex, "HTTP request exception clearing hold for record {RecordKey}", record.GetKey());
        return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
    }
    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
    {
        var errorMessage = "Request timeout";
        _logger.LogWarning("Request timeout - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}", 
            record.DonationNumber, record.ProductCode, record.HoldCode);
        _logger.LogError(ex, "Timeout clearing hold for record {RecordKey}", record.GetKey());
        return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
    }
    catch (TaskCanceledException ex)
    {
        var errorMessage = "Request cancelled";
        _logger.LogWarning("Request cancelled - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}", 
            record.DonationNumber, record.ProductCode, record.HoldCode);
        _logger.LogWarning(ex, "Request cancelled for record {RecordKey}", record.GetKey());
        return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
    }
    catch (JsonException ex)
    {
        var errorMessage = $"JSON serialization/deserialization error: {ex.Message}";
        _logger.LogWarning("JSON error - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}, Error: {ErrorMessage}", 
            record.DonationNumber, record.ProductCode, record.HoldCode, ex.Message);
        _logger.LogError(ex, "JSON error clearing hold for record {RecordKey}", record.GetKey());
        return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
    }
    catch (Exception ex)
    {
        var errorMessage = $"Unexpected error: {ex.Message}";
        _logger.LogWarning("Unexpected error - Unit: {UnitNumber}, Product: {ProductCode}, Hold: {HoldCode}, Error: {ErrorMessage}", 
            record.DonationNumber, record.ProductCode, record.HoldCode, ex.Message);
        _logger.LogError(ex, "Unexpected error clearing hold for record {RecordKey}", record.GetKey());
        return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage);
    }
}
}