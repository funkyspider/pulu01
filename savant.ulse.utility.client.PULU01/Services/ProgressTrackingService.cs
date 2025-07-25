using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using savant.ulse.utility.client.PULU01.Configuration;
using savant.ulse.utility.client.PULU01.Models;
using savant.ulse.utility.client.PULU01.Utilities;

namespace savant.ulse.utility.client.PULU01.Services;

public class ProgressTrackingService : IProgressTrackingService
{
    private readonly ILogger<ProgressTrackingService> _logger;
    private readonly AppConfiguration _configuration;
    private readonly object _lockObject = new();
    
    private int _totalRecords;
    private int _processedCount;
    private int _successCount;
    private int _failedCount;
    private int _skippedCount;
    private int _lastReportedCount;
    private DateTime _startTime;
    private DateTime _lastUpdateTime;

    public event EventHandler<ProgressEventArgs>? ProgressUpdated;

    public ProgressTrackingService(ILogger<ProgressTrackingService> logger, AppConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void Initialize(int totalRecords)
    {
        lock (_lockObject)
        {
            _totalRecords = totalRecords;
            _processedCount = 0;
            _successCount = 0;
            _failedCount = 0;
            _skippedCount = 0;
            _lastReportedCount = 0;
            _startTime = DateTime.UtcNow;
            _lastUpdateTime = _startTime;
            
            _logger.LogDebug("Progress tracking initialized for {TotalRecords} records", totalRecords);
            DisplayProgress(force: true);
        }
    }

    public void ReportProgress(ProcessingResult result)
    {
        lock (_lockObject)
        {
            _processedCount++;
            
            switch (result.Status)
            {
                case ProcessingStatus.Success:
                    _successCount++;
                    break;
                case ProcessingStatus.Failed:
                    _failedCount++;
                    break;
                case ProcessingStatus.Skipped:
                    _skippedCount++;
                    break;
            }

            var shouldUpdate = _processedCount - _lastReportedCount >= _configuration.ProgressUpdateBatchSize ||
                              _processedCount == _totalRecords;

            if (shouldUpdate)
            {
                DisplayProgress();
                _lastReportedCount = _processedCount;
                
                var args = new ProgressEventArgs
                {
                    ProcessedCount = _processedCount,
                    TotalCount = _totalRecords,
                    SuccessCount = _successCount,
                    FailedCount = _failedCount,
                    SkippedCount = _skippedCount,
                    PercentComplete = _totalRecords > 0 ? (double)_processedCount / _totalRecords * 100 : 0
                };
                
                ProgressUpdated?.Invoke(this, args);
            }
        }
    }

    public void DisplayFinalSummary()
    {
        lock (_lockObject)
        {
            var endTime = DateTime.UtcNow;
            var totalDuration = endTime - _startTime;
            var recordsPerSecond = totalDuration.TotalSeconds > 0 ? _processedCount / totalDuration.TotalSeconds : 0;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            if (_processedCount < _totalRecords)
            {
                    Console.WriteLine($"  {ConsoleHelper.Icons.Stopped}  Processing Stopped (Ctrl+C pressed)");
            }
            else
            {
                Console.WriteLine($"  {ConsoleHelper.Icons.Complete} Processing Complete!");
            }
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine($"  {ConsoleHelper.Icons.Stats} Total Records:    {_totalRecords:N0}");
            Console.WriteLine($"  {ConsoleHelper.Icons.Success} Successful:       {_successCount:N0} ({GetPercentage(_successCount, _processedCount):F1}%)");
            Console.WriteLine($"  {ConsoleHelper.Icons.Failed} Failed:           {_failedCount:N0} ({GetPercentage(_failedCount, _processedCount):F1}%)");
            Console.WriteLine($"  {ConsoleHelper.Icons.Skipped} Skipped:          {_skippedCount:N0} ({GetPercentage(_skippedCount, _processedCount):F1}%)");
            Console.WriteLine();
            Console.WriteLine($"  {ConsoleHelper.Icons.Duration}  Duration:         {FormatDuration(totalDuration)}");
            Console.WriteLine($"  {ConsoleHelper.Icons.Speed} Processing Rate:  {recordsPerSecond:F1} records/second");
            Console.WriteLine();

            if (_failedCount > 0)
            {
                Console.WriteLine($"  {ConsoleHelper.Icons.ErrorLog} Error Log:        {_configuration.ErrorLogPath}");
            }
            
            Console.WriteLine($"  {ConsoleHelper.Icons.SuccessLog} Success Log:      {_configuration.SuccessLogPath}");
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            _logger.LogInformation("Processing completed. Success: {SuccessCount}, Failed: {FailedCount}, Skipped: {SkippedCount}, Duration: {Duration}",
                _successCount, _failedCount, _skippedCount, totalDuration);
        }
    }

    private void DisplayProgress(bool force = false)
    {
        var now = DateTime.UtcNow;
        
        if (!force && (now - _lastUpdateTime).TotalMilliseconds < 500) // Limit console updates
            return;

        _lastUpdateTime = now;
        
        var percentage = _totalRecords > 0 ? (double)_processedCount / _totalRecords * 100 : 0;
        var elapsed = now - _startTime;
        var recordsPerSecond = elapsed.TotalSeconds > 0 ? _processedCount / elapsed.TotalSeconds : 0;
        
        var eta = recordsPerSecond > 0 && _processedCount < _totalRecords 
            ? TimeSpan.FromSeconds((_totalRecords - _processedCount) / recordsPerSecond)
            : TimeSpan.Zero;

        var progressBar = CreateProgressBar(percentage);
        
        Console.Write($"\r{progressBar} {percentage:F1}% " +
                     $"({_processedCount:N0}/{_totalRecords:N0}) " +
                     $"{ConsoleHelper.Icons.Success} {_successCount:N0} | {ConsoleHelper.Icons.Failed} {_failedCount:N0} | {ConsoleHelper.Icons.Skipped} {_skippedCount:N0} " +
                     $"| {ConsoleHelper.Icons.Speed} {recordsPerSecond:F1}/s");
        
        if (eta > TimeSpan.Zero && _processedCount < _totalRecords)
        {
            Console.Write($" | ETA: {FormatDuration(eta)}");
        }
    }

    private string CreateProgressBar(double percentage)
    {
        const int width = 30;
        var filled = (int)(percentage / 100 * width);
        var empty = width - filled;
        
        return "[" + new string(ConsoleHelper.Icons.ProgressFilled, filled) + new string(ConsoleHelper.Icons.ProgressEmpty, empty) + "]";
    }

    private double GetPercentage(int value, int total)
    {
        return total > 0 ? (double)value / total * 100 : 0;
    }

    private string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalHours >= 1)
            return $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        else
            return $"{duration.Minutes:D2}:{duration.Seconds:D2}";
    }
}