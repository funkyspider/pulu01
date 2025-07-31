using System.Text.Json.Serialization;

namespace Savant.Pulse.Utility.Client.PULU01.enums;

public enum ClearHoldStatus
{
    [JsonPropertyName("cleared")]
    Cleared,
    
    [JsonPropertyName("ignoredAlreadyCleared")]
    IgnoredAlreadyCleared,
    
    [JsonPropertyName("notClearedIndirectHoldDoesNotExist")]
    NotClearedIndirectHoldDoesNotExist,
    
    [JsonPropertyName("error")]
    Error
}