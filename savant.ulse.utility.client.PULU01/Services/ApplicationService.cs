using Microsoft.Extensions.Logging;
using savant.ulse.utility.client.PULU01.Configuration;

namespace savant.ulse.utility.client.PULU01.Services;

public class ApplicationService : IApplicationService
{
    private readonly ILogger<ApplicationService> _logger;
    private readonly ICsvReaderService _csvReaderService;
    private readonly IProcessingWorkerService _processingWorkerService;

    public ApplicationService(
        ILogger<ApplicationService> logger,
        ICsvReaderService csvReaderService,
        IProcessingWorkerService processingWorkerService)
    {
        _logger = logger;
        _csvReaderService = csvReaderService;
        _processingWorkerService = processingWorkerService;
    }

    public async Task RunAsync(AppConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting PULU01 utility with {ThreadCount} threads processing file: {FilePath}",
            configuration.ThreadCount, configuration.FilePath);

        try
        {
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("  PULU01 - Donation Hold Clearing Utility");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine();
            
            Console.Write("Reading CSV file... ");
            var records = await _csvReaderService.ReadRecordsAsync(configuration.FilePath, cancellationToken);
            var recordList = records.ToList();

            if (recordList.Count == 0)
            {
                Console.WriteLine("❌ No valid records found in the CSV file.");
                return;
            }

            Console.WriteLine($"✅ {recordList.Count:N0} records loaded");
            Console.WriteLine();

            await _processingWorkerService.ProcessRecordsAsync(recordList, cancellationToken);
            
            _logger.LogInformation("Application processing completed successfully");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Application processing was cancelled");
            Console.WriteLine("Processing was cancelled by user request.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application failed with error: {ErrorMessage}", ex.Message);
            Console.WriteLine($"Application failed: {ex.Message}");
            throw;
        }
    }
}