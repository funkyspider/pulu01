using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces
{
    public interface IProcessingPersistenceService
    {
        Task<HashSet<string>> LoadProcessedRecordsAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task SaveSuccessfulRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveFailedRecordsAsync(IEnumerable<ProcessingResult> results, CancellationToken cancellationToken = default(CancellationToken));
        Task FlushAllAsync(CancellationToken cancellationToken = default(CancellationToken));
        bool IsProcessed(DonationRecord record);
    }
}