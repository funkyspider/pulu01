using System.Text.Json.Serialization;

namespace Savant.Pulse.Utility.Client.PULU01.Models;

public interface IClearFateRequestDto
{
    string ProductCode { get; set; }
    string UnitNumber { get; set; }
}

public interface IClearDiscardFateRequestDto : IClearFateRequestDto
{
    string DiscardFate { get; set; }      // API expects string value like "discard"
    string HoldCode { get; set; }         // only required for clearing batch fates
    DateTime DateTimePlaced { get; set; }
    string ProgramId { get; set; }
}

public class ClearDiscardFateRequestDto : IClearDiscardFateRequestDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string DiscardFate { get; set; } = "discard";  // Default to "discard" as expected by API
    public string HoldCode { get; set; } = string.Empty;
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime DateTimePlaced { get; set; } = DateTime.MinValue;
    public string ProgramId { get; set; } = string.Empty;

    public static ClearDiscardFateRequestDto FromDiscardRecord(DiscardRecord record, string clearCode)
    {
        return new ClearDiscardFateRequestDto
        {
            UnitNumber = record.DonationNumber,
            ProductCode = record.ProductCode,
            DiscardFate = "discard",  // API expects lowercase string "discard"
            HoldCode = record.HoldCode,
            DateTimePlaced = record.DateTimePlaced ?? DateTime.MinValue,
            ProgramId = "PULS11"
        };
    }
}