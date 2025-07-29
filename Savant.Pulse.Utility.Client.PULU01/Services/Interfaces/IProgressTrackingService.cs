using Savant.Pulse.Utility.Client.PULU01.Models;

// ReSharper disable once CheckNamespace
namespace Savant.Pulse.Utility.Client.PULU01.Services;

public interface IProgressTrackingService
{
    void Initialize(int totalRecords);
    void ReportProgress(ProcessingResult result);
    void DisplayFinalSummary();
    event EventHandler<ProgressEventArgs>? ProgressUpdated;
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