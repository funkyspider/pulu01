namespace Savant.Pulse.Utility.Client.PULU01.Models;

public record ProcessingResult(
    DonationRecord Record,
    ProcessingStatus Status,
    string? ErrorMessage = null,
    DateTime ProcessedAt = default)
{
    public static ProcessingResult CreateSuccess(DonationRecord record)
        => new(record, ProcessingStatus.Success, null, DateTime.UtcNow);
    
    public static ProcessingResult CreateFailure(DonationRecord record, string errorMessage)
        => new(record, ProcessingStatus.Failed, errorMessage, DateTime.UtcNow);
    
    public static ProcessingResult CreateSkipped(DonationRecord record)
        => new(record, ProcessingStatus.Skipped, null, DateTime.UtcNow);
    
    public bool IsSuccess => Status == ProcessingStatus.Success;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
}