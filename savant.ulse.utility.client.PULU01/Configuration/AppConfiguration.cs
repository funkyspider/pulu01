namespace savant.ulse.utility.client.PULU01.Configuration;

public class AppConfiguration
{
    public int ThreadCount { get; set; } = 1;
    public string FilePath { get; set; } = string.Empty;
    public string SuccessLogPath { get; set; } = "Hold_Clear_Ok.json";
    public string ErrorLogPath { get; set; } = "Hold_Clear_Errors.json";
    public int ProgressUpdateBatchSize { get; set; } = 20;
    public int FileWriteBatchSize { get; set; } = 100;
}