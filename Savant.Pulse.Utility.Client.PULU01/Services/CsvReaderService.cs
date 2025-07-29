using Microsoft.Extensions.Logging;
using Savant.Pulse.Utility.Client.PULU01.Models;

namespace Savant.Pulse.Utility.Client.PULU01.Services;

public class CsvReaderService : ICsvReaderService
{
    private readonly ILogger<CsvReaderService> _logger;

    public CsvReaderService(ILogger<CsvReaderService> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<DonationRecord>> ReadRecordsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reading CSV file: {FilePath}", filePath);
        
        var records = new List<DonationRecord>();
        var lineNumber = 0;
        var validRecords = 0;
        var invalidRecords = 0;

        using var reader = new StreamReader(filePath);
        var firstLine = await reader.ReadLineAsync();
        
        if (firstLine == null)
        {
            _logger.LogWarning("CSV file is empty");
            return records;
        }

        var hasHeader = DetectHeader(firstLine);
        if (!hasHeader)
        {
            var record = ParseLine(firstLine, ++lineNumber);
            if (record != null)
            {
                records.Add(record);
                validRecords++;
            }
            else
            {
                invalidRecords++;
            }
        }
        else
        {
            _logger.LogInformation("Detected header row, skipping first line");
            lineNumber++;
        }

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var record = ParseLine(line, ++lineNumber);
            if (record != null)
            {
                records.Add(record);
                validRecords++;
            }
            else
            {
                invalidRecords++;
            }
        }

        _logger.LogInformation("CSV parsing completed. Valid records: {ValidRecords}, Invalid records: {InvalidRecords}", 
            validRecords, invalidRecords);

        return records;
    }

    private bool DetectHeader(string firstLine)
    {
        var parts = SplitCsvLine(firstLine);
        if (parts.Length != 3) return false;

        return parts.Any(p => !char.IsDigit(p.FirstOrDefault()) && !p.StartsWith("G"));
    }

    private DonationRecord? ParseLine(string line, int lineNumber)
    {
        try
        {
            var parts = SplitCsvLine(line);
            
            if (parts.Length != 3)
            {
                _logger.LogWarning("Line {LineNumber}: Expected 3 fields, found {FieldCount}", lineNumber, parts.Length);
                return null;
            }

            var donationNumber = parts[0].Trim();
            var productCode = parts[1].Trim();
            var holdCode = parts[2].Trim();

            var record = new DonationRecord(donationNumber, productCode, holdCode);
            
            if (!record.IsValid())
            {
                _logger.LogWarning("Line {LineNumber}: Invalid record format - Donation: {DonationNumber} ({Length}), Product: {ProductCode} ({Length2}), Hold: {HoldCode} ({Length3})",
                    lineNumber, donationNumber, donationNumber.Length, productCode, productCode.Length, holdCode, holdCode.Length);
                return null;
            }

            return record;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Line {LineNumber}: Error parsing line: {Line}", lineNumber, line);
            return null;
        }
    }

    private string[] SplitCsvLine(string line)
    {
        var parts = new List<string>();
        var current = "";
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                parts.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        
        parts.Add(current);
        return parts.ToArray();
    }
}