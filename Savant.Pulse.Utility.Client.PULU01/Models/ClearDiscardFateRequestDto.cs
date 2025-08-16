using Savant.Pulse.Utility.Client.PULU01.enums;

namespace Savant.Pulse.Utility.Client.PULU01.Models;

public interface IClearDiscardFateRequestDto
{
    string ProductCode { get; set; }
    string UnitNumber { get; set; }
    DiscardFate DiscardFate { get; set; }
    string HoldCode { get; set; }
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
}