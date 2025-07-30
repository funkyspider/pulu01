using System.Threading;
using System.Threading.Tasks;
using Savant.Pulse.Utility.WPF.Client.PULU01.Configuration;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces
{
    public interface IApplicationService
    {
        Task RunAsync(AppConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken));
    }
}