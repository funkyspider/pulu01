using savant.ulse.utility.client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace savant.ulse.utility.client.PULU01.Services;

public interface ICsvReaderService
{
    Task<IEnumerable<DonationRecord>> ReadRecordsAsync(string filePath, CancellationToken cancellationToken = default);
}