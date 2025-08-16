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

        var headerInfo = DetectHeaderFormat(firstLine);
        if (headerInfo.HasHeader)
        {
            _logger.LogInformation("Detected header row: {HeaderType}", headerInfo.IsNewFormat ? "New format" : "Legacy format");
            lineNumber++;
            
            if (headerInfo.IsNewFormat && !headerInfo.IsValid)
            {
                _logger.LogError("Invalid header format. Missing required fields. Expected: DNTNO, HDATE, HTIME, PRDCD, RSHLD");
                throw new InvalidOperationException("CSV file missing required headers: DNTNO, HDATE, HTIME, PRDCD, RSHLD");
            }
        }
        else
        {
            // No header, process first line as data (legacy format)
            var record = ParseLegacyLine(firstLine, ++lineNumber);
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

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            DonationRecord? record;
            if (headerInfo.IsNewFormat)
            {
                record = ParseNewFormatLine(line, headerInfo.FieldMapping!, ++lineNumber);
            }
            else
            {
                record = ParseLegacyLine(line, ++lineNumber);
            }

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

    public async Task<IEnumerable<DiscardRecord>> ReadDiscardRecordsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reading discard CSV file: {FilePath}", filePath);
        
        var records = new List<DiscardRecord>();
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

        var headerInfo = DetectDiscardHeaderFormat(firstLine);
        if (headerInfo.HasHeader)
        {
            _logger.LogInformation("Detected discard header row");
            lineNumber++;
            
            if (!headerInfo.IsValid)
            {
                _logger.LogError("Invalid discard header format. Missing required fields. Expected: DNTNO, PRDCD, LOCCD");
                throw new InvalidOperationException("CSV file missing required headers: DNTNO, PRDCD, LOCCD");
            }
        }
        else
        {
            // No header, process first line as data
            var record = ParseDiscardLine(firstLine, headerInfo.FieldMapping!, ++lineNumber);
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

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var record = ParseDiscardLine(line, headerInfo.FieldMapping!, ++lineNumber);

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

        _logger.LogInformation("Discard CSV parsing completed. Valid records: {ValidRecords}, Invalid records: {InvalidRecords}", 
            validRecords, invalidRecords);

        return records;
    }

    private HeaderInfo DetectHeaderFormat(string firstLine)
    {
        var parts = SplitCsvLine(firstLine);
        
        // Check for new format headers
        var requiredNewHeaders = new[] { "DNTNO", "HDATE", "HTIME", "PRDCD", "RSHLD" };
        var headerParts = parts.Select(p => p.Trim().ToUpper()).ToArray();
        
        if (requiredNewHeaders.All(h => headerParts.Contains(h)))
        {
            // New format with all required headers
            var fieldMapping = new Dictionary<string, int>();
            for (int i = 0; i < headerParts.Length; i++)
            {
                fieldMapping[headerParts[i]] = i;
            }
            
            return new HeaderInfo
            {
                HasHeader = true,
                IsNewFormat = true,
                IsValid = true,
                FieldMapping = fieldMapping
            };
        }
        
        // Check for legacy 3-field format header first
        if (parts.Length == 3 && parts.Any(p => !char.IsDigit(p.Trim().FirstOrDefault()) && !p.Trim().StartsWith("G")))
        {
            return new HeaderInfo
            {
                HasHeader = true,
                IsNewFormat = false,
                IsValid = true,
                FieldMapping = null
            };
        }
        
        // Check if it looks like a header but missing required fields (for new format)
        if (parts.Any(p => !char.IsDigit(p.Trim().FirstOrDefault()) && !p.Trim().StartsWith("G")))
        {
            return new HeaderInfo
            {
                HasHeader = true,
                IsNewFormat = true,
                IsValid = false,
                FieldMapping = null
            };
        }
        
        
        // No header detected - treat as data
        return new HeaderInfo
        {
            HasHeader = false,
            IsNewFormat = parts.Length > 3,
            IsValid = true,
            FieldMapping = null
        };
    }

    private DonationRecord? ParseNewFormatLine(string line, Dictionary<string, int> fieldMapping, int lineNumber)
    {
        try
        {
            var parts = SplitCsvLine(line);
            
            if (parts.Length < fieldMapping.Values.Max() + 1)
            {
                _logger.LogWarning("Line {LineNumber}: Expected at least {ExpectedFields} fields, found {FieldCount}", 
                    lineNumber, fieldMapping.Values.Max() + 1, parts.Length);
                return null;
            }

            var donationNumber = parts[fieldMapping["DNTNO"]].Trim();
            var productCode = parts[fieldMapping["PRDCD"]].Trim();
            var holdCode = parts[fieldMapping["RSHLD"]].Trim();
            var hdate = parts[fieldMapping["HDATE"]].Trim();
            var htime = parts[fieldMapping["HTIME"]].Trim();

            var holdDateTime = DonationRecord.ParseHoldDateTime(hdate, htime);
            var record = new DonationRecord(donationNumber, productCode, holdCode, holdDateTime);
            
            if (!record.IsValid())
            {
                _logger.LogWarning("Line {LineNumber}: Invalid record format - Donation: {DonationNumber} ({Length}), Product: {ProductCode} ({Length2}), Hold: {HoldCode} ({Length3})",
                    lineNumber, donationNumber, donationNumber.Length, productCode, productCode.Length, holdCode, holdCode.Length);
                return null;
            }

            if (holdDateTime == null)
            {
                _logger.LogWarning("Line {LineNumber}: Invalid date/time format - HDATE: {HDate}, HTIME: {HTime}", 
                    lineNumber, hdate, htime);
            }

            return record;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Line {LineNumber}: Error parsing line: {Line}", lineNumber, line);
            return null;
        }
    }

    private DonationRecord? ParseLegacyLine(string line, int lineNumber)
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

    private HeaderInfo DetectDiscardHeaderFormat(string firstLine)
    {
        var parts = SplitCsvLine(firstLine);
        
        // Check for required discard headers
        var requiredDiscardHeaders = new[] { "DNTNO", "PRDCD", "LOCCD" };
        var optionalDiscardHeaders = new[] { "HDATE", "HTIME", "RSHLD" };
        var headerParts = parts.Select(p => p.Trim().ToUpper()).ToArray();
        
        if (requiredDiscardHeaders.All(h => headerParts.Contains(h)))
        {
            // Discard format with all required headers
            var fieldMapping = new Dictionary<string, int>();
            for (int i = 0; i < headerParts.Length; i++)
            {
                fieldMapping[headerParts[i]] = i;
            }
            
            return new HeaderInfo
            {
                HasHeader = true,
                IsNewFormat = true,
                IsValid = true,
                FieldMapping = fieldMapping
            };
        }
        
        // Check if it looks like a header but missing required fields
        if (parts.Any(p => !char.IsDigit(p.Trim().FirstOrDefault()) && !p.Trim().StartsWith("G")))
        {
            return new HeaderInfo
            {
                HasHeader = true,
                IsNewFormat = true,
                IsValid = false,
                FieldMapping = null
            };
        }
        
        // No header detected - treat as data (assume format: DNTNO, PRDCD, LOCCD, HDATE, HTIME, RSHLD)
        var defaultMapping = new Dictionary<string, int>
        {
            ["DNTNO"] = 0,
            ["PRDCD"] = 1,
            ["LOCCD"] = 2
        };
        
        // Add optional fields if they exist
        if (parts.Length > 3) defaultMapping["HDATE"] = 3;
        if (parts.Length > 4) defaultMapping["HTIME"] = 4;
        if (parts.Length > 5) defaultMapping["RSHLD"] = 5;
        
        return new HeaderInfo
        {
            HasHeader = false,
            IsNewFormat = true,
            IsValid = true,
            FieldMapping = defaultMapping
        };
    }

    private DiscardRecord? ParseDiscardLine(string line, Dictionary<string, int> fieldMapping, int lineNumber)
    {
        try
        {
            var parts = SplitCsvLine(line);
            
            if (parts.Length < 3)
            {
                _logger.LogWarning("Line {LineNumber}: Expected at least 3 fields for discard format, found {FieldCount}", 
                    lineNumber, parts.Length);
                return null;
            }

            var donationNumber = parts[fieldMapping["DNTNO"]].Trim();
            var productCode = parts[fieldMapping["PRDCD"]].Trim();
            var locationCode = parts[fieldMapping["LOCCD"]].Trim();
            
            // Optional fields
            var hdate = "";
            var htime = "";
            var holdCode = "";
            
            if (fieldMapping.ContainsKey("HDATE") && fieldMapping["HDATE"] < parts.Length)
                hdate = parts[fieldMapping["HDATE"]].Trim();
                
            if (fieldMapping.ContainsKey("HTIME") && fieldMapping["HTIME"] < parts.Length)
                htime = parts[fieldMapping["HTIME"]].Trim();
                
            if (fieldMapping.ContainsKey("RSHLD") && fieldMapping["RSHLD"] < parts.Length)
                holdCode = parts[fieldMapping["RSHLD"]].Trim();

            // Parse date/time if both are present and not zero/empty
            DateTime? dateTimePlaced = null;
            if (!string.IsNullOrWhiteSpace(hdate) && !string.IsNullOrWhiteSpace(htime) && 
                hdate != "00000000" && htime != "000000")
            {
                dateTimePlaced = DiscardRecord.ParseHoldDateTime(hdate, htime);
            }

            var record = new DiscardRecord(donationNumber, productCode, locationCode, dateTimePlaced, holdCode);
            
            if (!record.IsValid())
            {
                _logger.LogWarning("Line {LineNumber}: Invalid discard record format - Donation: {DonationNumber} ({Length}), Product: {ProductCode} ({Length2}), Location: {LocationCode} ({Length3}), Hold: {HoldCode}",
                    lineNumber, donationNumber, donationNumber.Length, productCode, productCode.Length, locationCode, locationCode.Length, holdCode);
                return null;
            }

            if (!string.IsNullOrWhiteSpace(hdate) && !string.IsNullOrWhiteSpace(htime) && 
                hdate != "00000000" && htime != "000000" && dateTimePlaced == null)
            {
                _logger.LogWarning("Line {LineNumber}: Invalid date/time format - HDATE: {HDate}, HTIME: {HTime}", 
                    lineNumber, hdate, htime);
            }

            return record;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Line {LineNumber}: Error parsing discard line: {Line}", lineNumber, line);
            return null;
        }
    }

    private class HeaderInfo
    {
        public bool HasHeader { get; set; }
        public bool IsNewFormat { get; set; }
        public bool IsValid { get; set; }
        public Dictionary<string, int>? FieldMapping { get; set; }
    }
}