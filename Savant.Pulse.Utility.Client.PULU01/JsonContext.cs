using System.Text.Json.Serialization;
using Savant.Pulse.Utility.Client.PULU01.Models;
using Savant.Pulse.Utility.Client.PULU01.Services;
using Savant.Pulse.Utility.Client.PULU01.enums;

namespace Savant.Pulse.Utility.Client.PULU01;

[JsonSerializable(typeof(ClearHoldRequestDto))]
[JsonSerializable(typeof(ClearHoldResponseDto))]
[JsonSerializable(typeof(DonationRecord))]
[JsonSerializable(typeof(ProcessingResult))]
[JsonSerializable(typeof(HoldComponentStatusResponse))]
[JsonSerializable(typeof(List<ProcessingPersistenceService.ProcessedRecord>))]
[JsonSerializable(typeof(ProcessingPersistenceService.ProcessedRecord))]
[JsonSerializable(typeof(ProcessingPersistenceService.FailedRecord))]
[JsonSerializable(typeof(ClearHoldStatus))]
public partial class AppJsonContext : JsonSerializerContext
{
}