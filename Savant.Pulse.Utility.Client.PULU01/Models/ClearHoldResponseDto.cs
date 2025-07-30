using Savant.Pulse.Utility.Client.PULU01.enums;

namespace Savant.Pulse.Utility.Client.PULU01.Models;

public class ClearHoldResponseDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string HoldCode { get; set; } = string.Empty;
    public string ClearCode { get; set; } = string.Empty;
    public ClearHoldStatus Status { set; get; }

    public ErrorNum ErrorNum { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
        
    public List<HoldComponentStatusResponse> HoldComponentStatusResponses { get; set; }

    public ClearHoldResponseDto()
    {
        HoldComponentStatusResponses = new List<HoldComponentStatusResponse>();
    }
    public DateTime ProcessedAt { get; set; } = DateTime.Now;
}


public class HoldComponentStatusResponse
{
    public string UnitNumber { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string SiteCode { get; set; } = string.Empty;
    public string LocationDescription { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
}