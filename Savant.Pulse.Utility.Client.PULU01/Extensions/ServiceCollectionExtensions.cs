using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Savant.Pulse.Utility.Client.PULU01.Configuration;
using Savant.Pulse.Utility.Client.PULU01.Services;

namespace Savant.Pulse.Utility.Client.PULU01.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPulu01Services(this IServiceCollection services, AppConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration);
        services.Configure<AppConfiguration>(opts =>
        {
            opts.ThreadCount = configuration.ThreadCount;
            opts.FilePath = configuration.FilePath;
            opts.SuccessLogPath = configuration.SuccessLogPath;
            opts.ErrorLogPath = configuration.ErrorLogPath;
            opts.ProgressUpdateBatchSize = configuration.ProgressUpdateBatchSize;
            opts.FileWriteBatchSize = configuration.FileWriteBatchSize;
            opts.Api = configuration.Api;
        });
        
        // API Client - HTTP for production, Mock for testing
        services.AddHttpClient<IApiClientService, HttpApiClientService>();
        // services.AddScoped<IApiClientService, MockApiClientService>();
        
        // Core application services
        services.AddScoped<IApplicationService, ApplicationService>();
        
        // CSV processing
        services.AddScoped<ICsvReaderService, CsvReaderService>();
        
        // Processing and workers
        services.AddScoped<IProcessingWorkerService, ProcessingWorkerService>();
        
        // Processing persistence and progress tracking
        services.AddScoped<IProcessingPersistenceService, ProcessingPersistenceService>();
        services.AddScoped<IProgressTrackingService, ProgressTrackingService>();
        
        return services;
    }
}