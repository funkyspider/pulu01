using savant.ulse.utility.client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace savant.ulse.utility.client.PULU01.Services;

public interface IApiClientService
{
    Task<ProcessingResult> ClearHoldAsync(DonationRecord record, CancellationToken cancellationToken = default);
}