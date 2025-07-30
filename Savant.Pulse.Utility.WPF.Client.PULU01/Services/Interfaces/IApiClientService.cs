using System.Threading;
using System.Threading.Tasks;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces
{
    public interface IApiClientService
    {
        Task<ProcessingResult> ClearHoldAsync(DonationRecord record, CancellationToken cancellationToken = default(CancellationToken));
    }
}