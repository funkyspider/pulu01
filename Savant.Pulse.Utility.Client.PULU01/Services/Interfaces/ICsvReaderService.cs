using Savant.Pulse.Utility.Client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace Savant.Pulse.Utility.Client.PULU01.Services;

public interface ICsvReaderService
{
    Task<IEnumerable<DonationRecord>> ReadRecordsAsync(string filePath, CancellationToken cancellationToken = default);
    Task<IEnumerable<DiscardRecord>> ReadDiscardRecordsAsync(string filePath, CancellationToken cancellationToken = default);
}