using Savant.Pulse.Utility.Client.PULU01.enums;

namespace Savant.Pulse.Utility.Client.PULU01.Models;

public interface IClearFateRequestDto
{
    string ProductCode { get; set; }
    string UnitNumber { get; set; }
}

public interface IClearDiscardFateRequestDto : IClearFateRequestDto
{
    DiscardFate DiscardFate { get; set; }      // only required for clearing batch fates
    string HoldCode { get; set; }              // only required for clearing batch fates
    DateTime DateTimePlaced { get; set; }
    string ProgramId { get; set; }
}

public class ClearDiscardFateRequestDto : IClearDiscardFateRequestDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public DiscardFate DiscardFate { get; set; } = DiscardFate.None;
    public string HoldCode { get; set; } = string.Empty;
    public DateTime DateTimePlaced { get; set; } = DateTime.MinValue;
    public string ProgramId { get; set; } = string.Empty;

    public static ClearDiscardFateRequestDto FromDiscardRecord(DiscardRecord record, string clearCode)
    {
        return new ClearDiscardFateRequestDto
        {
            UnitNumber = record.DonationNumber,
            ProductCode = record.ProductCode,
            DateTimePlaced = record.DateTimePlaced ?? DateTime.MinValue,
            HoldCode = record.HoldCode,
            ProgramId = "PULS11",
            DiscardFate = DiscardFate.Discard // Default to Discard fate
        };
    }
}