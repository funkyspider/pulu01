namespace Savant.Pulse.Utility.Client.PULU01.Models;

public class ClearHoldResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}