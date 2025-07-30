namespace Savant.Pulse.Utility.Client.PULU01.Models;

public class ClearHoldRequestDto
{
    public string DonationNumber { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string HoldCode { get; set; } = string.Empty;
    public DateTime? HoldDateTime { get; set; }

    public static ClearHoldRequestDto FromDonationRecord(DonationRecord record)
    {
        return new ClearHoldRequestDto
        {
            DonationNumber = record.DonationNumber,
            ProductCode = record.ProductCode,
            HoldCode = record.HoldCode,
            HoldDateTime = record.HoldDateTime
        };
    }
}