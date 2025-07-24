using Microsoft.Extensions.DependencyInjection;
using savant.ulse.utility.client.PULU01.Configuration;
using savant.ulse.utility.client.PULU01.Services;

namespace savant.ulse.utility.client.PULU01.Extensions;

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
        
        // Resume and progress tracking
        services.AddScoped<IResumeTrackingService, ResumeTrackingService>();
        services.AddScoped<IProgressTrackingService, ProgressTrackingService>();
        
        return services;
    }
}