using savant.ulse.utility.client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace savant.ulse.utility.client.PULU01.Services;

public interface IProcessingWorkerService
{
    Task ProcessRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default);
}