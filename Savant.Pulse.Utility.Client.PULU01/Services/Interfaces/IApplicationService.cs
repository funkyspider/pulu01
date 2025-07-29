using Savant.Pulse.Utility.Client.PULU01.Configuration;

// ReSharper disable once CheckNamespace
namespace Savant.Pulse.Utility.Client.PULU01.Services;

public interface IApplicationService
{
    Task RunAsync(AppConfiguration configuration, CancellationToken cancellationToken = default);
}