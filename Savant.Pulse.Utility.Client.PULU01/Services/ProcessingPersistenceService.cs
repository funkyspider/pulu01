using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using Savant.Pulse.Utility.Client.PULU01.Configuration;
using Savant.Pulse.Utility.Client.PULU01.Models;

namespace Savant.Pulse.Utility.Client.PULU01.Services;

public class ProcessingPersistenceService : IProcessingPersistenceService
{
    private readonly ILogger<ProcessingPersistenceService> _logger;
    private readonly AppConfiguration _configuration;
    private readonly HashSet<string> _processedRecords = new();
    private readonly ConcurrentQueue<DonationRecord> _successBatch = new();
    private readonly ConcurrentQueue<ProcessingResult> _errorBatch = new();
    private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

    public ProcessingPersistenceService(ILogger<ProcessingPersistenceService> logger, AppConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<HashSet<string>> LoadProcessedRecordsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (File.Exists(_configuration.SuccessLogPath))
            {
                _logger.LogInformation("Loading processed records from {FilePath}", _configuration.SuccessLogPath);
                
                var json = await File.ReadAllTextAsync(_configuration.SuccessLogPath, cancellationToken);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var records = JsonSerializer.Deserialize<List<ProcessedRecord>>(json) ?? new List<ProcessedRecord>();
                    
                    foreach (var record in records)
                    {
                        _processedRecords.Add(record.Key);
                    }
                    
                    _logger.LogInformation("Loaded {Count} processed records for resume functionality", records.Count);
                }
            }
            else
            {
                _logger.LogInformation("No existing success log found, starting fresh");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load processed records from {FilePath}", _configuration.SuccessLogPath);
        }

        return _processedRecords;
    }

    public bool IsProcessed(DonationRecord record)
    {
        return _processedRecords.Contains(record.GetKey());
    }

    public async Task SaveSuccessfulRecordsAsync(IEnumerable<DonationRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            _successBatch.Enqueue(record);
            _processedRecords.Add(record.GetKey());
        }

        if (_successBatch.Count >= _configuration.FileWriteBatchSize)
        {
            await FlushSuccessfulRecordsAsync(cancellationToken);
        }
    }

    public async Task SaveFailedRecordsAsync(IEnumerable<ProcessingResult> results, CancellationToken cancellationToken = default)
    {
        // Write failures immediately since they're rare
        foreach (var result in results)
        {
            await WriteFailedRecordImmediately(result, cancellationToken);
        }
    }

    public async Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        await FlushSuccessfulRecordsAsync(cancellationToken);
        // No need to flush failed records since they're written immediately
    }

    private async Task FlushSuccessfulRecordsAsync(CancellationToken cancellationToken = default)
    {
        if (_successBatch.IsEmpty) return;

        await _writeSemaphore.WaitAsync(cancellationToken);
        try
        {
            var recordsToWrite = new List<DonationRecord>();
            while (_successBatch.TryDequeue(out var record))
            {
                recordsToWrite.Add(record);
            }

            if (recordsToWrite.Count == 0) return;

            var existingRecords = new List<ProcessedRecord>();
            if (File.Exists(_configuration.SuccessLogPath))
            {
                var existingJson = await File.ReadAllTextAsync(_configuration.SuccessLogPath, cancellationToken);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    existingRecords = JsonSerializer.Deserialize<List<ProcessedRecord>>(existingJson) ?? new List<ProcessedRecord>();
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

            var json = JsonSerializer.Serialize(existingRecords, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_configuration.SuccessLogPath, json, cancellationToken);

            _logger.LogDebug("Saved {Count} successful records to {FilePath}", recordsToWrite.Count, _configuration.SuccessLogPath);
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    private async Task WriteFailedRecordImmediately(ProcessingResult result, CancellationToken cancellationToken = default)
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
            var json = JsonSerializer.Serialize(failedRecord, new JsonSerializerOptions { WriteIndented = false });
            await File.AppendAllTextAsync(_configuration.ErrorLogPath, json + Environment.NewLine, cancellationToken);

            _logger.LogDebug("Saved failed record to {FilePath}: {Key}", _configuration.ErrorLogPath, failedRecord.Key);
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    private async Task FlushFailedRecordsAsync(CancellationToken cancellationToken = default)
    {
        // This method is now obsolete since we write failures immediately
        // Kept for compatibility but does nothing
        await Task.CompletedTask;
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