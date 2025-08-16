using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System.CommandLine;
using Savant.Pulse.Utility.Client.PULU01.Configuration;
using Savant.Pulse.Utility.Client.PULU01.Extensions;
using Savant.Pulse.Utility.Client.PULU01.Services;
using Savant.Pulse.Utility.Client.PULU01.Models;

var threadsOption = new Option<int>(
    "--threads",
    getDefaultValue: () => 1,
    description: "Number of concurrent processing threads (1-50). Higher values process records faster but use more system resources. Default: 1");

var fileOption = new Option<string>(
    "--file",
    description: "Path to the CSV file containing donation records to process. File must have comma-separated values with a header row.")
{
    IsRequired = true
};

var clearCodeOption = new Option<string>(
    "--clearcode",
    description: "Pulse clear code (required for hold mode, optional for discard mode)");

var modeOption = new Option<string>(
    "--mode",
    getDefaultValue: () => "hold",
    description: "Processing mode: 'hold' for donation hold clearing (default) or 'discard' for discard fate clearing");

modeOption.AddValidator(result =>
{
    var value = result.GetValueOrDefault<string>()?.ToLowerInvariant();
    if (value != "hold" && value != "discard")
    {
        result.ErrorMessage = "Mode must be either 'hold' or 'discard'";
    }
});

var rootCommand = new RootCommand(GetHelpDescription())
{
    threadsOption,
    fileOption,
    clearCodeOption,
    modeOption
};

rootCommand.SetHandler(async (threads, file, clearCode, mode) =>
{
    if (!File.Exists(file))
    {
        Console.WriteLine($"Error: File '{file}' not found.");
        Environment.Exit(1);
    }

    if (threads < 1 || threads > 50)
    {
        Console.WriteLine("Error: Thread count must be between 1 and 50.");
        Environment.Exit(1);
    }

    // Validate clearcode based on mode
    var normalizedMode = mode.ToLowerInvariant();
    if (normalizedMode == "hold")
    {
        if (string.IsNullOrEmpty(clearCode) || clearCode.Length < 2 || clearCode.Length > 3)
        {
            Console.WriteLine("Error: Clear code is required for hold mode and must be between 2 and 3 characters.");
            Environment.Exit(1);
        }
    }
    else if (normalizedMode == "discard")
    {
        // For discard mode, clearcode is optional
        if (!string.IsNullOrEmpty(clearCode) && (clearCode.Length < 2 || clearCode.Length > 3))
        {
            Console.WriteLine("Error: If provided, clear code must be between 2 and 3 characters.");
            Environment.Exit(1);
        }
    }

    // Load configuration from appsettings.json
    var basePath = AppContext.BaseDirectory;
    var configBuilder = new ConfigurationBuilder()
        .SetBasePath(basePath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
    
    var config = configBuilder.Build();
    
    // Create AppConfiguration with values from JSON and command line overrides
    var configuration = new AppConfiguration();
    
    // Use explicit binding for better self-contained deployment compatibility
    configuration.SuccessLogPath = config.GetValue<string>("SuccessLogPath") ?? "Hold_Clear_Ok.json";
    configuration.ErrorLogPath = config.GetValue<string>("ErrorLogPath") ?? "Hold_Clear_Errors.json";
    configuration.ProgressUpdateBatchSize = config.GetValue<int>("ProgressUpdateBatchSize", 20);
    configuration.FileWriteBatchSize = config.GetValue<int>("FileWriteBatchSize", 100);
    
    configuration.Api.BaseUrl = config.GetValue<string>("Api:BaseUrl") ?? string.Empty;
    configuration.Api.ClearHoldEndpoint = config.GetValue<string>("Api:ClearHoldEndpoint") ?? string.Empty;
    configuration.Api.ClearDiscardEndpoint = config.GetValue<string>("Api:ClearDiscardEndpoint") ?? "/api/v43/ComponentHold/clear-discard-fate";
    configuration.Api.TimeoutSeconds = config.GetValue<int>("Api:TimeoutSeconds", 30);
    
    configuration.Api.Headers.XUserId = config.GetValue<string>("Api:Headers:XUserId") ?? string.Empty;
    configuration.Api.Headers.XAppName = config.GetValue<string>("Api:Headers:XAppName") ?? string.Empty;
    configuration.Api.Headers.XEnvironment = config.GetValue<string>("Api:Headers:XEnvironment") ?? string.Empty;
    
    // Parse and set processing mode
    var processingMode = mode.ToLowerInvariant() switch
    {
        "hold" => ProcessingMode.Hold,
        "discard" => ProcessingMode.Discard,
        _ => ProcessingMode.Hold // Default to hold
    };

    // Override with command line parameters
    configuration.ThreadCount = threads != configuration.ThreadCount ? threads : configuration.ThreadCount;
    configuration.FilePath = file;
    configuration.ClearCode = clearCode ?? string.Empty; // Handle null clearCode for discard mode
    configuration.Mode = processingMode;
    
    // Set mode-specific log file paths
    configuration.SuccessLogPath = configuration.GetSuccessLogPath();
    configuration.ErrorLogPath = configuration.GetErrorLogPath();

    var host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(builder => 
        {
            builder.Sources.Clear();
            builder.AddConfiguration(config);
        })
        .ConfigureServices((context, services) =>
        {
            services.AddPulu01Services(configuration);
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddNLog(); // Add NLog
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug); // Allow debug level for detailed API logging
        })
        .Build();

    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        cts.Cancel();
        Console.WriteLine("\nShutdown requested, stopping gracefully...");
    };

    try
    {
        var appService = host.Services.GetRequiredService<IApplicationService>();
        await appService.RunAsync(configuration, cts.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Application stopped by user request.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Application failed: {ex.Message}");
        Environment.Exit(1);
    }
    
}, threadsOption, fileOption, clearCodeOption, modeOption);

return await rootCommand.InvokeAsync(args);

static string GetHelpDescription()
{
    return @"PULU01 - Donation Processing Utility

DESCRIPTION:
    Processes CSV files and makes API calls to clear holds or discard fates.
    Supports multi-threaded processing and can resume from where it left off if interrupted.

PROCESSING MODES:
    --mode hold    : Clear donation holds (default)
    --mode discard : Clear discard fates

CSV FILE FORMAT (HOLD MODE):
    Required column headers (case-insensitive):
    - DNTNO  : Donation Number (any length, will be trimmed of spaces)
    - HDATE  : Hold Date in YYYYMMDD format (e.g., 20240813)
    - HTIME  : Hold Time in HHMMSSzzz format (e.g., 142724591)
    - PRDCD  : Product Code (must be exactly 4 characters)
    - RSHLD  : Hold Code (must be more than 1 character, e.g., COS, RD)

CSV FILE FORMAT (DISCARD MODE):
    Required column headers (case-insensitive):
    - DNTNO  : Donation Number (any length, will be trimmed of spaces)
    - PRDCD  : Product Code (must be exactly 4 characters)
    - LOCCD  : Location Code (required for discard processing)
    
    Optional column headers:
    - HDATE  : Hold Date in YYYYMMDD format (optional)
    - HTIME  : Hold Time in HHMMSSzzz format (optional)
    - RSHLD  : Hold Code (optional)

CLEAR CODE:
    Use --clearcode to define the Pulse clear code (required for hold mode, optional for discard mode).
    This is not validated against the database. Please ensure the code is correct.

THREADING:
    Use --threads to specify concurrent processing threads (1-50).
    More threads = faster processing but higher system resource usage.
    Recommended: Start with 5-10 threads and adjust based on performance.

OUTPUT FILES:
    The utility creates JSON files to track processing results (mode-specific names):
    
    Hold mode:
        Hold_Clear_Ok.json     - Successfully processed records
        Hold_Clear_Errors.json - Failed records with error messages
    
    Discard mode:
        Discard_Clear_Ok.json     - Successfully processed records
        Discard_Clear_Errors.json - Failed records with error messages

RESUME FUNCTIONALITY:
    If the success file exists, already processed records will be skipped.
    This allows you to resume processing after stopping or if errors occur.
    The utility will show how many records were skipped on startup.

STOPPING THE PROCESS:
    Press Ctrl+C to gracefully stop processing at any time.
    The utility will save progress and display a summary before exiting.
    You can then restart to resume from where you left off.

EXAMPLES:
    Process file with 1 thread:
        PULU01 --file donations.csv
    
    Process file with 5 threads:
        PULU01 --threads 5 --file donations.csv";
}