using savant.ulse.utility.client.PULU01.Models;

namespace savant.ulse.utility.client.PULU01.Services;

public interface IApiClientService
{
    Task<ProcessingResult> ClearHoldAsync(DonationRecord record, CancellationToken cancellationToken = default);
}