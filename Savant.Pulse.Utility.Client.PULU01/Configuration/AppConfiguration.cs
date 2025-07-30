namespace Savant.Pulse.Utility.Client.PULU01.Configuration;

public class AppConfiguration
{
    public int ThreadCount { get; set; } = 1;
    public string FilePath { get; set; } = string.Empty;
    public string SuccessLogPath { get; set; } = "Hold_Clear_Ok.json";
    public string ErrorLogPath { get; set; } = "Hold_Clear_Errors.json";
    public int ProgressUpdateBatchSize { get; set; } = 20;
    public int FileWriteBatchSize { get; set; } = 100;
    
    public ApiConfiguration Api { get; set; } = new();
}

public class ApiConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClearHoldEndpoint { get; set; } = "/api/v43/ComponentHold/clear-hold";
    public ApiHeaders Headers { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 30;
}

public class ApiHeaders
{
    public string XUserId { get; set; } = "PULSE";
    public string XAppName { get; set; } = "PULU01";
    public string XEnvironment { get; set; } = "test";
}