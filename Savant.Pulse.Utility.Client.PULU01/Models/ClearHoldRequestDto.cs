using System.Text.Json;
using System.Text.Json.Serialization;

namespace Savant.Pulse.Utility.Client.PULU01.Models;

public class ClearHoldRequestDto
{
    private string _productCode = string.Empty;
    private string _unitNumber = string.Empty;
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

    public string HoldCode { get; set; } = string.Empty;
    public string ClearCode { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime DateTimePlaced { get; set; }
    public string IncidentNumber { get; set; } = string.Empty;
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
            DateTimePlaced = record.HoldDateTime ?? DateTime.MinValue,
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

/// <summary>
/// Custom DateTime converter to ensure the API gets the exact format it expects: yyyy-MM-ddTHH:mm:ss.fff
/// </summary>
public class DateTimeConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "yyyy-MM-ddTHH:mm:ss.fff";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (DateTime.TryParseExact(dateString, DateFormat, null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }
        return DateTime.TryParse(dateString, out date) ? date : default;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}