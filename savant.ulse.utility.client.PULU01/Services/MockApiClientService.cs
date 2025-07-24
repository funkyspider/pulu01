using Microsoft.Extensions.Logging;
using savant.ulse.utility.client.PULU01.Models;

namespace savant.ulse.utility.client.PULU01.Services;

public class MockApiClientService : IApiClientService
{
    private readonly ILogger<MockApiClientService> _logger;
    private readonly Random _random = new();

    public MockApiClientService(ILogger<MockApiClientService> logger)
    {
        _logger = logger;
    }

    public async Task<ProcessingResult> ClearHoldAsync(DonationRecord record, CancellationToken cancellationToken = default)
    {
        try
        {
            var delayMs = _random.Next(1000, 2001);
            await Task.Delay(delayMs, cancellationToken);

            var successRate = 0.95;
            var isSuccess = _random.NextDouble() < successRate;

            if (isSuccess)
            {
                // Removed debug logging for successful operations to clean up output
                
                return ProcessingResult.CreateSuccess(record);
            }
            else
            {
                var errorMessages = new[]
                {
                    "Hold not found",
                    "Donation already processed",
                    "Invalid product code",
                    "API timeout",
                    "Service temporarily unavailable"
                };
                
                var errorMessage = errorMessages[_random.Next(errorMessages.Length)];
                
                // Log errors at debug level to reduce console clutter - summary will be shown at the end
                _logger.LogDebug("Failed to clear hold for {DonationNumber}-{ProductCode}-{HoldCode}: {Error}", 
                    record.DonationNumber, record.ProductCode, record.HoldCode, errorMessage);
                
                return ProcessingResult.CreateFailure(record, errorMessage);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("API call cancelled for {DonationNumber}-{ProductCode}-{HoldCode}", 
                record.DonationNumber, record.ProductCode, record.HoldCode);
            
            return ProcessingResult.CreateFailure(record, "Operation cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error clearing hold for {DonationNumber}-{ProductCode}-{HoldCode}", 
                record.DonationNumber, record.ProductCode, record.HoldCode);
            
            return ProcessingResult.CreateFailure(record, $"Unexpected error: {ex.Message}");
        }
    }
}