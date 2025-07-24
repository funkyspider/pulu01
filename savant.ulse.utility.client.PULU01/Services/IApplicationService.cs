using savant.ulse.utility.client.PULU01.Configuration;

namespace savant.ulse.utility.client.PULU01.Services;

public interface IApplicationService
{
    Task RunAsync(AppConfiguration configuration, CancellationToken cancellationToken = default);
}