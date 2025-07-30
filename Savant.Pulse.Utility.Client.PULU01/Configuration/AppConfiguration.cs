namespace Savant.Pulse.Utility.Client.PULU01.Configuration;

public class AppConfiguration
{
    public int ThreadCount { get; set; } = 1;
    public string FilePath { get; set; } = string.Empty;
    public string ClearCode { get; set; } = string.Empty;

    public string SuccessLogPath { get; set; } = "Hold_Clear_Ok.json";
    public string ErrorLogPath { get; set; } = "Hold_Clear_Errors.json";
    public int ProgressUpdateBatchSize { get; set; } = 20;
    public int FileWriteBatchSize { get; set; } = 100;

    
    public ApiConfiguration Api { get; set; } = new();
}

public class ApiConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ClearHoldEndpoint { get; set; } = string.Empty;
    public ApiHeaders Headers { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 30;
}

public class ApiHeaders
{
    public string XUserId { get; set; } = "";
    public string XAppName { get; set; } = "";
    public string XEnvironment { get; set; } = "";
}