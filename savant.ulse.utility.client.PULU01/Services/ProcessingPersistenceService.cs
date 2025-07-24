using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using savant.ulse.utility.client.PULU01.Configuration;
using savant.ulse.utility.client.PULU01.Models;

namespace savant.ulse.utility.client.PULU01.Services;

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
        foreach (var result in results)
        {
            _errorBatch.Enqueue(result);
        }

        if (_errorBatch.Count >= _configuration.FileWriteBatchSize)
        {
            await FlushFailedRecordsAsync(cancellationToken);
        }
    }

    public async Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        await FlushSuccessfulRecordsAsync(cancellationToken);
        await FlushFailedRecordsAsync(cancellationToken);
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

    private async Task FlushFailedRecordsAsync(CancellationToken cancellationToken = default)
    {
        if (_errorBatch.IsEmpty) return;

        await _writeSemaphore.WaitAsync(cancellationToken);
        try
        {
            var errorsToWrite = new List<ProcessingResult>();
            while (_errorBatch.TryDequeue(out var error))
            {
                errorsToWrite.Add(error);
            }

            if (errorsToWrite.Count == 0) return;

            var existingErrors = new List<FailedRecord>();
            if (File.Exists(_configuration.ErrorLogPath))
            {
                var existingJson = await File.ReadAllTextAsync(_configuration.ErrorLogPath, cancellationToken);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    existingErrors = JsonSerializer.Deserialize<List<FailedRecord>>(existingJson) ?? new List<FailedRecord>();
                }
            }

            var newErrors = errorsToWrite.Select(r => new FailedRecord
            {
                Key = r.Record.GetKey(),
                DonationNumber = r.Record.DonationNumber,
                ProductCode = r.Record.ProductCode,
                HoldCode = r.Record.HoldCode,
                ErrorMessage = r.ErrorMessage ?? "Unknown error",
                FailedAt = r.ProcessedAt
            });

            existingErrors.AddRange(newErrors);

            var json = JsonSerializer.Serialize(existingErrors, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_configuration.ErrorLogPath, json, cancellationToken);

            _logger.LogDebug("Saved {Count} failed records to {FilePath}", errorsToWrite.Count, _configuration.ErrorLogPath);
        }
        finally
        {
            _writeSemaphore.Release();
        }
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