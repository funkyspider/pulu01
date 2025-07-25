using savant.ulse.utility.client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace savant.ulse.utility.client.PULU01.Services;

public interface IProcessingPersistenceService
{
    Task<HashSet<string>> LoadProcessedRecordsAsync(CancellationToken cancellationToken = default);
    Task SaveSuccessfulRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default);
    Task SaveFailedRecordsAsync(IEnumerable<ProcessingResult> results, CancellationToken cancellationToken = default);
    bool IsProcessed(DonationRecord record);
}