using Savant.Pulse.Utility.Client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace Savant.Pulse.Utility.Client.PULU01.Services;

public interface IProcessingWorkerService
{
    Task ProcessRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default);
    Task ProcessRecordsAsync(IEnumerable<IProcessingRecord> records, CancellationToken cancellationToken = default);
}