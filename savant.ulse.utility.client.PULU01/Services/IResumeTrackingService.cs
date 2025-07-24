using savant.ulse.utility.client.PULU01.Models;

namespace savant.ulse.utility.client.PULU01.Services;

public interface IResumeTrackingService
{
    Task<HashSet<string>> LoadProcessedRecordsAsync(CancellationToken cancellationToken = default);
    Task SaveSuccessfulRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default);
    Task SaveFailedRecordsAsync(IEnumerable<ProcessingResult> results, CancellationToken cancellationToken = default);
    bool IsProcessed(DonationRecord record);
}