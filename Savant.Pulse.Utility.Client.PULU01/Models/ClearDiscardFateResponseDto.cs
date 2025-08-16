using Savant.Pulse.Utility.Client.PULU01.enums;

namespace Savant.Pulse.Utility.Client.PULU01.Models;

public interface IClearFateResponseDto
{
    string UnitNumber { get; set; }
    string ProductCode { get; set; }
    ClearFateStatus Status { get; set; }
    ErrorNum ErrorNum { get; set; }
    string ErrorMessage { get; set; }
}

public class ClearDiscardFateResponseDto : IClearFateResponseDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public ClearFateStatus Status { get; set; } = ClearFateStatus.Error;
    public ErrorNum ErrorNum { get; set; } = ErrorNum.Unknown;
    public string ErrorMessage { get; set; } = string.Empty;
    public List<HoldComponentStatusResponse> HoldComponentStatusResponses { get; set; } = new();
}