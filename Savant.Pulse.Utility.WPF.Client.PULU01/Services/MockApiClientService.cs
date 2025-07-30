using System;
using System.Threading;
using System.Threading.Tasks;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;
using Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services
{
    public class MockApiClientService : IApiClientService
    {
        private readonly Random _random = new Random();

        public MockApiClientService()
        {
        }

        public async Task<ProcessingResult> ClearHoldAsync(DonationRecord record, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var delayMs = _random.Next(1000, 2001);
                await Task.Delay(delayMs, cancellationToken);

                var successRate = 0.95;
                var isSuccess = _random.NextDouble() < successRate;

                if (isSuccess)
                {
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
                    return ProcessingResult.CreateFailure(record, errorMessage);
                }
            }
            catch (OperationCanceledException)
            {
                return ProcessingResult.CreateFailure(record, "Operation cancelled");
            }
            catch (Exception ex)
            {
                return ProcessingResult.CreateFailure(record, $"Unexpected error: {ex.Message}");
            }
        }
    }
}