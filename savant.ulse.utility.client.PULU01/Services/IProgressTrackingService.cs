using savant.ulse.utility.client.PULU01.Models;

namespace savant.ulse.utility.client.PULU01.Services;

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