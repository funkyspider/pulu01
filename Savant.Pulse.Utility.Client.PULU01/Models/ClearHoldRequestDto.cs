namespace Savant.Pulse.Utility.Client.PULU01.Models;

public class ClearHoldRequestDto
{
    private string _productCode;
    private string _unitNumber;
    public string ProductCode
    {
        get => _productCode;
        set => _productCode = value.ToUpper().Trim();
    }

    public string UnitNumber
    {
        get => _unitNumber;
        set => _unitNumber = value.ToUpper().Trim();
    }

    public string HoldCode { get; set; }
    public string ClearCode { get; set; }
    public string ProgramId { get; set; }
    public DateTime DateTimePlaced { get; set; }
    public string IncidentNumber { get; set; }
    public bool ClearConstituents { get; set; }
    public HoldType ClearType { get; set; }

    public bool CalledFromDT { get; set; }
    public bool CalledFromValidation { get; set; }

    public bool IsInStockIncident => !string.IsNullOrEmpty(IncidentNumber);
    public bool IsAllHold => !string.IsNullOrEmpty(ProductCode) && ProductCode.ToUpper().Trim() == "ALL";

    public static ClearHoldRequestDto FromDonationRecord(DonationRecord record, string clearCode)
    {
        return new ClearHoldRequestDto
        {
            UnitNumber = record.DonationNumber,
            ProductCode = record.ProductCode,
            HoldCode = record.HoldCode,
            DateTimePlaced = record.HoldDateTime.Value,
            IncidentNumber = string.Empty,
            ClearType = HoldType.Direct,
            ClearConstituents = true,
            CalledFromDT = false,
            CalledFromValidation = false,
            ProgramId = "PULS11",
            ClearCode = clearCode
        };
    }
}