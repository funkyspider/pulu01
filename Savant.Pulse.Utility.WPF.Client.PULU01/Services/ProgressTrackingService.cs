using System;
using System.Collections.Concurrent;
using Savant.Pulse.Utility.WPF.Client.PULU01.Configuration;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services
{
    public class ProgressTrackingService : IProgressTrackingService
    {
        private readonly AppConfiguration _configuration;
        private readonly object _lockObject = new object();
        
        private int _totalRecords;
        private int _processedCount;
        private int _successCount;
        private int _failedCount;
        private int _skippedCount;
        private int _lastReportedCount;
        private DateTime _startTime;
        private DateTime _lastUpdateTime;

        public event EventHandler<ProgressEventArgs> ProgressUpdated;

        public ProgressTrackingService(AppConfiguration configuration)
        {
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
                
                // Output initialization instead of logging
                Console.WriteLine($"Progress tracking initialized for {totalRecords:N0} records");
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
                    
                    OnProgressUpdated(args);
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
                    Console.WriteLine("  [STOP] Processing Stopped (Ctrl+C pressed)");
                }
                else
                {
                    Console.WriteLine("  [DONE] Processing Complete!");
                }
                Console.WriteLine("═══════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine($"  [STATS] Total Records:    {_totalRecords:N0}");
                Console.WriteLine($"  [OK] Successful:       {_successCount:N0} ({GetPercentage(_successCount, _processedCount):F1}%)");
                Console.WriteLine($"  [ERR] Failed:           {_failedCount:N0} ({GetPercentage(_failedCount, _processedCount):F1}%)");
                Console.WriteLine($"  [SKIP] Skipped:          {_skippedCount:N0} ({GetPercentage(_skippedCount, _processedCount):F1}%)");
                Console.WriteLine();
                Console.WriteLine($"  [TIME] Duration:         {FormatDuration(totalDuration)}");
                Console.WriteLine($"  [RATE] Processing Rate:  {recordsPerSecond:F1} records/second");
                Console.WriteLine();

                if (_failedCount > 0)
                {
                    Console.WriteLine($"  [ERRORS] Error Log:        {_configuration.ErrorLogPath}");
                }
                
                Console.WriteLine($"  [SUCCESS] Success Log:      {_configuration.SuccessLogPath}");
                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════════════════════════════");

                Console.WriteLine($"Processing completed. Success: {_successCount}, Failed: {_failedCount}, Skipped: {_skippedCount}, Duration: {totalDuration}");
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
                         $"[OK] {_successCount:N0} | [ERR] {_failedCount:N0} | [SKIP] {_skippedCount:N0} " +
                         $"| [RATE] {recordsPerSecond:F1}/s");
            
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
            
            return "[" + new string('=', filled) + new string('-', empty) + "]";
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

        protected virtual void OnProgressUpdated(ProgressEventArgs e)
        {
            ProgressUpdated?.Invoke(this, e);
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public int ProcessedCount { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }
        public double PercentComplete { get; set; }
    }
}