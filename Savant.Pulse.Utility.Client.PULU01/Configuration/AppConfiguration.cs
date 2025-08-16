using Savant.Pulse.Utility.Client.PULU01.Models;

namespace Savant.Pulse.Utility.Client.PULU01.Configuration;

public class AppConfiguration
{
    public int ThreadCount { get; set; } = 1;
    public string FilePath { get; set; } = string.Empty;
    public string ClearCode { get; set; } = string.Empty;
    public ProcessingMode Mode { get; set; } = ProcessingMode.Hold;

    public string SuccessLogPath { get; set; } = "Hold_Clear_Ok.json";
    public string ErrorLogPath { get; set; } = "Hold_Clear_Errors.json";
    public int ProgressUpdateBatchSize { get; set; } = 20;
    public int FileWriteBatchSize { get; set; } = 100;

    // Mode-specific file paths - will be set dynamically based on Mode
    public string GetSuccessLogPath() => Mode == ProcessingMode.Hold ? "Hold_Clear_Ok.json" : "Discard_Clear_Ok.json";
    public string GetErrorLogPath() => Mode == ProcessingMode.Hold ? "Hold_Clear_Errors.json" : "Discard_Clear_Errors.json";
    
    public ApiConfiguration Api { get; set; } = new();
}

public class ApiConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClearHoldEndpoint { get; set; } = string.Empty;
    public string ClearDiscardEndpoint { get; set; } = "/api/v43/ComponentHold/clear-discard-fate";
    public ApiHeaders Headers { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 30;
}

public class ApiHeaders
{
    public string XUserId { get; set; } = "";
    public string XAppName { get; set; } = "";
    public string XEnvironment { get; set; } = "";
}