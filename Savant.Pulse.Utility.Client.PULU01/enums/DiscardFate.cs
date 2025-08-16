using System.ComponentModel;

namespace Savant.Pulse.Utility.Client.PULU01.enums;

public enum DiscardFate
{
    [Description("")]
    None,
    
    [Description("0003")]
    Biohazard,
    
    [Description("0004")]
    Discard
}