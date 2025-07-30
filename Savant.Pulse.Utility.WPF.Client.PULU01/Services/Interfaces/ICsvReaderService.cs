using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces
{
    public interface ICsvReaderService
    {
        Task<IEnumerable<DonationRecord>> ReadRecordsAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken));
    }
}