using Microsoft.Extensions.DependencyInjection;
using Savant.Pulse.Utility.Client.PULU01.Configuration;
using Savant.Pulse.Utility.Client.PULU01.Services;

namespace Savant.Pulse.Utility.Client.PULU01.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPulu01Services(this IServiceCollection services, AppConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration);
        
        // Core application services
        services.AddScoped<IApplicationService, ApplicationService>();
        
        // CSV processing
        services.AddScoped<ICsvReaderService, CsvReaderService>();
        
        // Processing and workers
        services.AddScoped<IProcessingWorkerService, ProcessingWorkerService>();
        
        // API client (mock implementation - replace with real API client when ready)
        services.AddScoped<IApiClientService, MockApiClientService>();
        
        // Processing persistence and progress tracking
        services.AddScoped<IProcessingPersistenceService, ProcessingPersistenceService>();
        services.AddScoped<IProgressTrackingService, ProgressTrackingService>();
        
        return services;
    }
}