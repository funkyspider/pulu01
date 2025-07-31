using System.Text.Json.Serialization;

namespace Savant.Pulse.Utility.Client.PULU01.enums;

[JsonConverter(typeof(JsonStringEnumConverter<ClearHoldStatus>))]
public enum ClearHoldStatus
{
    Cleared,
    IgnoredAlreadyCleared,
    NotClearedIndirectHoldDoesNotExist,
    Error
}