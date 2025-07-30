namespace Savant.Pulse.Utility.WPF.Client.PULU01.Configuration
{
    public class AppConfiguration
    {
        public int ThreadCount { get; set; }
        public string FilePath { get; set; }
        public string SuccessLogPath { get; set; }
        public string ErrorLogPath { get; set; }
        public int ProgressUpdateBatchSize { get; set; }
        public int FileWriteBatchSize { get; set; }

        public AppConfiguration()
        {
            ThreadCount = 1;
            FilePath = string.Empty;
            SuccessLogPath = "Hold_Clear_Ok.json";
            ErrorLogPath = "Hold_Clear_Errors.json";
            ProgressUpdateBatchSize = 20;
            FileWriteBatchSize = 100;
        }
    }
}