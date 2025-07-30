using Savant.Pulse.Utility.Client.PULU01.enums;

namespace Savant.Pulse.Utility.Client.PULU01.Models;

public class ClearHoldResponseDto
{
    public string UnitNumber { get; set; }
    public string ProductCode { get; set; }
    public string HoldCode { get; set; }
    public string ClearCode { get; set; }
    public ClearHoldStatus Status { set; get; }

    public ErrorNum ErrorNum { get; set; }

    public string ErrorMessage { get; set; }
        
    public List<HoldComponentStatusResponse> HoldComponentStatusResponses { get; set; }

    public ClearHoldResponseDto()
    {
        HoldComponentStatusResponses = new List<HoldComponentStatusResponse>();
    }
    public DateTime ProcessedAt { get; set; } = DateTime.Now;
}


public class HoldComponentStatusResponse
{
    public string UnitNumber { get; set; }
    public string ProductCode { get; set; }
    public string Status { get; set; }
    public string LocationCode { get; set; }
    public string SiteCode { get; set; }
    public string LocationDescription { get; set; }
    public string ProductDescription { get; set; }
}