namespace Savant.Pulse.Utility.Client.PULU01.Models;

public record DonationRecord(
    string DonationNumber,
    string ProductCode,
    string HoldCode)
{
    public string GetKey() => $"{DonationNumber}|{ProductCode}|{HoldCode}";
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(DonationNumber) && DonationNumber.Length == 14 &&
               !string.IsNullOrWhiteSpace(ProductCode) && ProductCode.Length == 4 &&
               !string.IsNullOrWhiteSpace(HoldCode) && HoldCode.Length == 3;
    }
}