using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Savant.Pulse.Utility.WPF.Client.PULU01.Configuration;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services
{
    public class ProcessingWorkerService : IProcessingWorkerService
    {
        private readonly AppConfiguration _configuration;
        private readonly IApiClientService _apiClientService;
        private readonly IProcessingPersistenceService _processingPersistenceService;
        private readonly IProgressTrackingService _progressTrackingService;

        public ProcessingWorkerService(
            AppConfiguration configuration,
            IApiClientService apiClientService,
            IProcessingPersistenceService processingPersistenceService,
            IProgressTrackingService progressTrackingService)
        {
            _configuration = configuration;
            _apiClientService = apiClientService;
            _processingPersistenceService = processingPersistenceService;
            _progressTrackingService = progressTrackingService;
        }

        public async Task ProcessRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default(CancellationToken))
        {
            var recordList = records.ToList();
            
            // Load processed records for resume functionality
            var processedKeys = await _processingPersistenceService.LoadProcessedRecordsAsync(cancellationToken);
            
            // Filter out already processed records
            var unprocessedRecords = recordList.Where(r => !_processingPersistenceService.IsProcessed(r)).ToList();
            var skippedCount = recordList.Count - unprocessedRecords.Count;
            
            if (skippedCount > 0)
            {
                Console.WriteLine($"[SUCCESS] Found {skippedCount:N0} already processed records (resuming from previous run)");
            }

            if (unprocessedRecords.Count == 0)
            {
                Console.WriteLine("[OK] All records have already been processed!");
                return;
            }

            Console.WriteLine($"[RATE] Processing {unprocessedRecords.Count:N0} remaining records using {_configuration.ThreadCount} threads");
            Console.WriteLine();

            // Initialize progress tracking
            _progressTrackingService.Initialize(recordList.Count);

            // Report skipped records to progress tracker
            for (int i = 0; i < skippedCount; i++)
            {
                var skippedResult = ProcessingResult.CreateSkipped(recordList[i]);
                _progressTrackingService.ReportProgress(skippedResult);
            }

            // Create a concurrent queue for work distribution
            var workQueue = new ConcurrentQueue<DonationRecord>();
            foreach (var record in unprocessedRecords)
            {
                workQueue.Enqueue(record);
            }

            // Collections for batch processing
            var successfulRecords = new ConcurrentBag<DonationRecord>();
            var failedResults = new ConcurrentBag<ProcessingResult>();

            // Create and start worker tasks
            var workers = new List<Task>();
            var semaphore = new SemaphoreSlim(_configuration.ThreadCount, _configuration.ThreadCount);

            // Create worker tasks
            for (int i = 0; i < _configuration.ThreadCount; i++)
            {
                var workerId = i + 1;
                var worker = Task.Run(async () =>
                {
                    await ProcessWorkerQueue(workerId, workQueue, successfulRecords, failedResults, semaphore, cancellationToken);
                }, cancellationToken);
                
                workers.Add(worker);
            }

            // Start batch saving task
            var batchSaver = Task.Run(async () =>
            {
                await BatchSaveResults(successfulRecords, failedResults, cancellationToken);
            }, cancellationToken);

            // Wait for all workers to complete
            try
            {
                await Task.WhenAll(workers);
                await batchSaver;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Processing was cancelled by user request");
            }
            finally
            {
                // Final flush of any remaining results (use CancellationToken.None to ensure cleanup completes)
                try
                {
                    // Save any remaining records to the batches first
                    if (successfulRecords.Count > 0)
                    {
                        await _processingPersistenceService.SaveSuccessfulRecordsAsync(successfulRecords.ToList(), CancellationToken.None);
                    }
                    
                    if (failedResults.Count > 0)
                    {
                        await _processingPersistenceService.SaveFailedRecordsAsync(failedResults.ToList(), CancellationToken.None);
                    }
                    
                    // Force flush all remaining batched records to disk
                    await _processingPersistenceService.FlushAllAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving final results during cleanup: {ex.Message}");
                }

                // Always display final summary, even if cancelled
                _progressTrackingService.DisplayFinalSummary();
                
                Console.WriteLine($"Processing completed. Successful: {successfulRecords.Count}, Failed: {failedResults.Count}");
            }
        }

        private async Task ProcessWorkerQueue(
            int workerId,
            ConcurrentQueue<DonationRecord> workQueue,
            ConcurrentBag<DonationRecord> successfulRecords,
            ConcurrentBag<ProcessingResult> failedResults,
            SemaphoreSlim semaphore,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Worker {workerId} started");

            try
            {
                DonationRecord record;
                while (!cancellationToken.IsCancellationRequested && workQueue.TryDequeue(out record))
                {
                    await semaphore.WaitAsync(cancellationToken);
                    
                    try
                    {
                        var result = await _apiClientService.ClearHoldAsync(record, cancellationToken);
                        
                        if (result.IsSuccess)
                        {
                            successfulRecords.Add(record);
                        }
                        else
                        {
                            failedResults.Add(result);
                            // Write failures immediately since they're rare
                            await _processingPersistenceService.SaveFailedRecordsAsync(new[] { result }, cancellationToken);
                        }
                        
                        _progressTrackingService.ReportProgress(result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Worker {workerId} failed to process record {record.GetKey()}: {ex.Message}");
                        
                        var errorResult = ProcessingResult.CreateFailure(record, $"Worker error: {ex.Message}");
                        failedResults.Add(errorResult);
                        // Write failures immediately since they're rare
                        await _processingPersistenceService.SaveFailedRecordsAsync(new[] { errorResult }, CancellationToken.None);
                        _progressTrackingService.ReportProgress(errorResult);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Worker {workerId} cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Worker {workerId} encountered an unexpected error: {ex.Message}");
            }

            Console.WriteLine($"Worker {workerId} completed");
        }

        private async Task BatchSaveResults(
            ConcurrentBag<DonationRecord> successfulRecords,
            ConcurrentBag<ProcessingResult> failedResults,
            CancellationToken cancellationToken)
        {
            var lastSuccessCount = 0;
            var lastFailedCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(5000, cancellationToken); // Check every 5 seconds

                    var currentSuccessCount = successfulRecords.Count;
                    var currentFailedCount = failedResults.Count;

                    // Save successful records if batch size reached
                    if (currentSuccessCount - lastSuccessCount >= _configuration.FileWriteBatchSize)
                    {
                        var recordsToSave = new List<DonationRecord>();
                        for (int i = 0; i < currentSuccessCount - lastSuccessCount; i++)
                        {
                            DonationRecord record;
                            if (successfulRecords.TryTake(out record))
                            {
                                recordsToSave.Add(record);
                            }
                        }
                        
                        if (recordsToSave.Count > 0)
                        {
                            await _processingPersistenceService.SaveSuccessfulRecordsAsync(recordsToSave, cancellationToken);
                            lastSuccessCount = currentSuccessCount;
                        }
                    }

                    // Save failed records if batch size reached
                    if (currentFailedCount - lastFailedCount >= _configuration.FileWriteBatchSize)
                    {
                        var resultsToSave = new List<ProcessingResult>();
                        for (int i = 0; i < currentFailedCount - lastFailedCount; i++)
                        {
                            ProcessingResult result;
                            if (failedResults.TryTake(out result))
                            {
                                resultsToSave.Add(result);
                            }
                        }
                        
                        if (resultsToSave.Count > 0)
                        {
                            await _processingPersistenceService.SaveFailedRecordsAsync(resultsToSave, cancellationToken);
                            lastFailedCount = currentFailedCount;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in batch save task: {ex.Message}");
                }
            }
        }
    }
}