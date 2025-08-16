using System.Text.Json.Serialization;
using Savant.Pulse.Utility.Client.PULU01.Models;
using Savant.Pulse.Utility.Client.PULU01.Services;
using Savant.Pulse.Utility.Client.PULU01.enums;

namespace Savant.Pulse.Utility.Client.PULU01;

[JsonSerializable(typeof(ClearHoldRequestDto))]
[JsonSerializable(typeof(ClearHoldResponseDto))]
[JsonSerializable(typeof(ClearDiscardFateRequestDto))]
[JsonSerializable(typeof(ClearDiscardFateResponseDto))]
[JsonSerializable(typeof(DonationRecord))]
[JsonSerializable(typeof(DiscardRecord))]
[JsonSerializable(typeof(IProcessingRecord))]
[JsonSerializable(typeof(ProcessingResult))]
[JsonSerializable(typeof(ProcessingMode))]
[JsonSerializable(typeof(DiscardFate))]
[JsonSerializable(typeof(HoldComponentStatusResponse))]
[JsonSerializable(typeof(List<ProcessingPersistenceService.ProcessedRecord>))]
[JsonSerializable(typeof(ProcessingPersistenceService.ProcessedRecord))]
[JsonSerializable(typeof(ProcessingPersistenceService.FailedRecord))]
[JsonSerializable(typeof(ClearHoldStatus))]
[JsonSerializable(typeof(ErrorNum))]
public partial class AppJsonContext : JsonSerializerContext
{
}