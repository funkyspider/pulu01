using System;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces
{
    public interface IProgressTrackingService
    {
        void Initialize(int totalRecords);
        void ReportProgress(ProcessingResult result);
        void DisplayFinalSummary();
        event EventHandler<ProgressEventArgs> ProgressUpdated;
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