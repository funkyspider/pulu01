using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Savant.Pulse.Utility.WPF.Client.PULU01.Configuration;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services
{
    public class ProcessingPersistenceService : IProcessingPersistenceService
    {
        private readonly AppConfiguration _configuration;
        private readonly HashSet<string> _processedRecords = new HashSet<string>();
        private readonly ConcurrentQueue<DonationRecord> _successBatch = new ConcurrentQueue<DonationRecord>();
        private readonly ConcurrentQueue<ProcessingResult> _errorBatch = new ConcurrentQueue<ProcessingResult>();
        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);

        public ProcessingPersistenceService(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HashSet<string>> LoadProcessedRecordsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (File.Exists(_configuration.SuccessLogPath))
                {
                    Console.WriteLine($"Loading processed records from {_configuration.SuccessLogPath}");
                    
                    var json = await ReadAllTextAsync(_configuration.SuccessLogPath, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var records = JsonConvert.DeserializeObject<List<ProcessedRecord>>(json) ?? new List<ProcessedRecord>();
                        
                        foreach (var record in records)
                        {
                            _processedRecords.Add(record.Key);
                        }
                        
                        Console.WriteLine($"Loaded {records.Count:N0} processed records for resume functionality");
                    }
                }
                else
                {
                    Console.WriteLine("No existing success log found, starting fresh");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load processed records from {_configuration.SuccessLogPath}: {ex.Message}");
            }

            return _processedRecords;
        }

        public bool IsProcessed(DonationRecord record)
        {
            return _processedRecords.Contains(record.GetKey());
        }

        public async Task SaveSuccessfulRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var record in records)
            {
                _successBatch.Enqueue(record);
                _processedRecords.Add(record.GetKey());
            }

            if (GetQueueCount(_successBatch) >= _configuration.FileWriteBatchSize)
            {
                await FlushSuccessfulRecordsAsync(cancellationToken);
            }
        }

        public async Task SaveFailedRecordsAsync(IEnumerable<ProcessingResult> results, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Write failures immediately since they're rare
            foreach (var result in results)
            {
                await WriteFailedRecordImmediately(result, cancellationToken);
            }
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await FlushSuccessfulRecordsAsync(cancellationToken);
            // No need to flush failed records since they're written immediately
        }

        private async Task FlushSuccessfulRecordsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsQueueEmpty(_successBatch)) return;

            await _writeSemaphore.WaitAsync(cancellationToken);
            try
            {
                var recordsToWrite = new List<DonationRecord>();
                DonationRecord record;
                while (_successBatch.TryDequeue(out record))
                {
                    recordsToWrite.Add(record);
                }

                if (recordsToWrite.Count == 0) return;

                var existingRecords = new List<ProcessedRecord>();
                if (File.Exists(_configuration.SuccessLogPath))
                {
                    var existingJson = await ReadAllTextAsync(_configuration.SuccessLogPath, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        existingRecords = JsonConvert.DeserializeObject<List<ProcessedRecord>>(existingJson) ?? new List<ProcessedRecord>();
                    }
                }

                var newRecords = recordsToWrite.Select(r => new ProcessedRecord
                {
                    Key = r.GetKey(),
                    DonationNumber = r.DonationNumber,
                    ProductCode = r.ProductCode,
                    HoldCode = r.HoldCode,
                    ProcessedAt = DateTime.UtcNow
                });

                existingRecords.AddRange(newRecords);

                var json = JsonConvert.SerializeObject(existingRecords, Formatting.Indented);
                await WriteAllTextAsync(_configuration.SuccessLogPath, json, cancellationToken);

                Console.WriteLine($"Saved {recordsToWrite.Count:N0} successful records to {_configuration.SuccessLogPath}");
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        private async Task WriteFailedRecordImmediately(ProcessingResult result, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _writeSemaphore.WaitAsync(cancellationToken);
            try
            {
                var failedRecord = new FailedRecord
                {
                    Key = result.Record.GetKey(),
                    DonationNumber = result.Record.DonationNumber,
                    ProductCode = result.Record.ProductCode,
                    HoldCode = result.Record.HoldCode,
                    ErrorMessage = result.ErrorMessage ?? "Unknown error",
                    FailedAt = result.ProcessedAt
                };

                // Append single record as JSON line to avoid reading entire file
                var json = JsonConvert.SerializeObject(failedRecord, Formatting.None);
                await AppendAllTextAsync(_configuration.ErrorLogPath, json + Environment.NewLine, cancellationToken);

                Console.WriteLine($"Saved failed record to {_configuration.ErrorLogPath}: {failedRecord.Key}");
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        // Helper methods for .NET 4.8 compatibility (no File.ReadAllTextAsync, etc.)
        private async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
        {
            return await Task.Run(() => File.ReadAllText(path), cancellationToken);
        }

        private async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken)
        {
            await Task.Run(() => File.WriteAllText(path, content), cancellationToken);
        }

        private async Task AppendAllTextAsync(string path, string content, CancellationToken cancellationToken)
        {
            await Task.Run(() => File.AppendAllText(path, content), cancellationToken);
        }

        // .NET 4.8 doesn't have ConcurrentQueue<T>.IsEmpty or Count properties
        private bool IsQueueEmpty<T>(ConcurrentQueue<T> queue)
        {
            return queue.IsEmpty;
        }

        private int GetQueueCount<T>(ConcurrentQueue<T> queue)
        {
            return queue.Count;
        }

        private class ProcessedRecord
        {
            public string Key { get; set; } = string.Empty;
            public string DonationNumber { get; set; } = string.Empty;
            public string ProductCode { get; set; } = string.Empty;
            public string HoldCode { get; set; } = string.Empty;
            public DateTime ProcessedAt { get; set; }
        }

        private class FailedRecord
        {
            public string Key { get; set; } = string.Empty;
            public string DonationNumber { get; set; } = string.Empty;
            public string ProductCode { get; set; } = string.Empty;
            public string HoldCode { get; set; } = string.Empty;
            public string ErrorMessage { get; set; } = string.Empty;
            public DateTime FailedAt { get; set; }
        }
    }
}