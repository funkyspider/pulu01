using Savant.Pulse.Utility.Client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace Savant.Pulse.Utility.Client.PULU01.Services;

public interface IProcessingPersistenceService
{
    Task<HashSet<string>> LoadProcessedRecordsAsync(CancellationToken cancellationToken = default);
    Task SaveSuccessfulRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default);
    Task SaveFailedRecordsAsync(IEnumerable<ProcessingResult> results, CancellationToken cancellationToken = default);
    bool IsProcessed(DonationRecord record);
}