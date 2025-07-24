using savant.ulse.utility.client.PULU01.Models;

namespace savant.ulse.utility.client.PULU01.Services;

public interface ICsvReaderService
{
    Task<IEnumerable<DonationRecord>> ReadRecordsAsync(string filePath, CancellationToken cancellationToken = default);
}