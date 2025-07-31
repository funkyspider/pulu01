using System.Text.Json.Serialization;
using Savant.Pulse.Utility.Client.PULU01.Models;

namespace Savant.Pulse.Utility.Client.PULU01;

[JsonSerializable(typeof(ClearHoldRequestDto))]
[JsonSerializable(typeof(ClearHoldResponseDto))]
[JsonSerializable(typeof(DonationRecord))]
[JsonSerializable(typeof(ProcessingResult))]
[JsonSerializable(typeof(HoldComponentStatusResponse))]
[JsonSerializable(typeof(List<object>))]
public partial class AppJsonContext : JsonSerializerContext
{
}