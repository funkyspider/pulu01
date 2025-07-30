using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using Savant.Pulse.Utility.Client.PULU01.Configuration;
using Savant.Pulse.Utility.Client.PULU01.Extensions;
using Savant.Pulse.Utility.Client.PULU01.Services;

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

var rootCommand = new RootCommand(GetHelpDescription())
{
    threadsOption,
    fileOption
};

rootCommand.SetHandler(async (threads, file) =>
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

    // Load configuration from appsettings.json
    var configBuilder = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
    
    var config = configBuilder.Build();
    
    // Create AppConfiguration with values from JSON and command line overrides
    var configuration = new AppConfiguration();
    config.Bind(configuration);
    
    // Override with command line parameters
    configuration.ThreadCount = threads;
    configuration.FilePath = file;

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
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning); // Only show warnings and errors
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
    
}, threadsOption, fileOption);

return await rootCommand.InvokeAsync(args);

static string GetHelpDescription()
{
    return @"PULU01 - Donation Hold Clearing Utility

DESCRIPTION:
    Processes CSV files containing donation records and clears holds by making API calls.
    Supports multi-threaded processing and can resume from where it left off if interrupted.

CSV FILE FORMAT:
    The CSV file must contain comma-separated values with a header row as the first line.
    
    Required column headers (case-insensitive):
    - DNTNO  : Donation Number (any length, will be trimmed of spaces)
    - HDATE  : Hold Date in YYYYMMDD format (e.g., 20240813)
    - HTIME  : Hold Time in HHMMSSzzz format (e.g., 142724591)
    - PRDCD  : Product Code (must be exactly 4 characters)
    - RSHLD  : Hold Code (must be more than 1 character, e.g., COS, RD)
    
    Additional columns in the CSV file will be ignored.

THREADING:
    Use --threads to specify concurrent processing threads (1-50).
    More threads = faster processing but higher system resource usage.
    Recommended: Start with 5-10 threads and adjust based on performance.

OUTPUT FILES:
    The utility creates JSON files to track processing results:
    
    Hold_Clear_Ok.json:
        Contains successfully processed records with timestamps.
        Used for resume functionality - processed records are skipped on restart.
    
    Hold_Clear_Errors.json:
        Contains failed records with error messages and timestamps.
        Each line is a separate JSON record for easy parsing.

RESUME FUNCTIONALITY:
    If Hold_Clear_Ok.json exists, already processed records will be skipped.
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