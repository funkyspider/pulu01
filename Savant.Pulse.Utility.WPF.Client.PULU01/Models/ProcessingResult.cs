using System;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Models
{
    public class ProcessingResult
    {
        public DonationRecord Record { get; set; }
        public ProcessingStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; }

        public ProcessingResult()
        {
            ProcessedAt = DateTime.UtcNow;
        }

        public ProcessingResult(DonationRecord record, ProcessingStatus status, string errorMessage = null, DateTime? processedAt = null)
        {
            Record = record;
            Status = status;
            ErrorMessage = errorMessage;
            ProcessedAt = processedAt ?? DateTime.UtcNow;
        }

        public static ProcessingResult CreateSuccess(DonationRecord record)
        {
            return new ProcessingResult(record, ProcessingStatus.Success, null, DateTime.UtcNow);
        }

        public static ProcessingResult CreateFailure(DonationRecord record, string errorMessage)
        {
            return new ProcessingResult(record, ProcessingStatus.Failed, errorMessage, DateTime.UtcNow);
        }

        public static ProcessingResult CreateSkipped(DonationRecord record)
        {
            return new ProcessingResult(record, ProcessingStatus.Skipped, null, DateTime.UtcNow);
        }

        public bool IsSuccess => Status == ProcessingStatus.Success;
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        // Override Equals and GetHashCode for proper comparison (replacing record functionality)
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (ProcessingResult)obj;
            return Equals(Record, other.Record) &&
                   Status == other.Status &&
                   ErrorMessage == other.ErrorMessage &&
                   ProcessedAt == other.ProcessedAt;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + (Record?.GetHashCode() ?? 0);
                hash = hash * 23 + Status.GetHashCode();
                hash = hash * 23 + (ErrorMessage?.GetHashCode() ?? 0);
                hash = hash * 23 + ProcessedAt.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"ProcessingResult {{ Record = {Record}, Status = {Status}, ErrorMessage = {ErrorMessage}, ProcessedAt = {ProcessedAt} }}";
        }
    }
}