using Savant.Pulse.Utility.Client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace Savant.Pulse.Utility.Client.PULU01.Services;

public interface IApiClientService
{
    Task<ProcessingResult> ClearHoldAsync(DonationRecord record, CancellationToken cancellationToken = default);
}