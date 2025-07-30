using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Savant.Pulse.Utility.WPF.Client.PULU01.Configuration;
using Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly ICsvReaderService _csvReaderService;
        private readonly IProcessingWorkerService _processingWorkerService;

        public ApplicationService(
            ICsvReaderService csvReaderService,
            IProcessingWorkerService processingWorkerService)
        {
            _csvReaderService = csvReaderService;
            _processingWorkerService = processingWorkerService;
        }

        public async Task RunAsync(AppConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var records = await _csvReaderService.ReadRecordsAsync(configuration.FilePath, cancellationToken);
                var recordList = records.ToList();

                if (recordList.Count == 0)
                {
                    throw new InvalidOperationException("No valid records found in the CSV file.");
                }

                await _processingWorkerService.ProcessRecordsAsync(recordList, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Application processing failed: {ex.Message}", ex);
            }
        }
    }
}