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
    description: "Number of active threads");

var fileOption = new Option<string>(
    "--file",
    description: "CSV file to process")
{
    IsRequired = true
};

var rootCommand = new RootCommand("PULU01 - Donation Hold Clearing Utility")
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

    var configuration = new AppConfiguration
    {
        ThreadCount = threads,
        FilePath = file
    };

    var host = Host.CreateDefaultBuilder()
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