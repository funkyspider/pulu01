using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Savant.Pulse.Utility.WPF.Client.PULU01.Models;
using Savant.Pulse.Utility.WPF.Client.PULU01.Services.Interfaces;

namespace Savant.Pulse.Utility.WPF.Client.PULU01.Services
{
    public class CsvReaderService : ICsvReaderService
    {
        public async Task<IEnumerable<DonationRecord>> ReadRecordsAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine($"Reading CSV file: {filePath}");
            
            var records = new List<DonationRecord>();
            var lineNumber = 0;
            var validRecords = 0;
            var invalidRecords = 0;

            using (var reader = new StreamReader(filePath))
            {
                var firstLine = await reader.ReadLineAsync();
                
                if (firstLine == null)
                {
                    Console.WriteLine("CSV file is empty");
                    return records;
                }

                var headerInfo = DetectHeaderFormat(firstLine);
                if (headerInfo.HasHeader)
                {
                    Console.WriteLine($"Detected header row: {(headerInfo.IsNewFormat ? "New format" : "Legacy format")}");
                    lineNumber++;
                    
                    if (headerInfo.IsNewFormat && !headerInfo.IsValid)
                    {
                        Console.WriteLine("Invalid header format. Missing required fields. Expected: DNTNO, HDATE, HTIME, PRDCD, RSHLD");
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

                    DonationRecord record;
                    if (headerInfo.IsNewFormat)
                    {
                        record = ParseNewFormatLine(line, headerInfo.FieldMapping, ++lineNumber);
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

                Console.WriteLine($"CSV parsing completed. Valid records: {validRecords}, Invalid records: {invalidRecords}");

                return records;
            }
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

        private DonationRecord ParseNewFormatLine(string line, Dictionary<string, int> fieldMapping, int lineNumber)
        {
            try
            {
                var parts = SplitCsvLine(line);
                
                if (parts.Length < fieldMapping.Values.Max() + 1)
                {
                    Console.WriteLine($"Line {lineNumber}: Expected at least {fieldMapping.Values.Max() + 1} fields, found {parts.Length}");
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
                    Console.WriteLine($"Line {lineNumber}: Invalid record format - Donation: {donationNumber} ({donationNumber.Length}), Product: {productCode} ({productCode.Length}), Hold: {holdCode} ({holdCode.Length})");
                    return null;
                }

                if (holdDateTime == null)
                {
                    Console.WriteLine($"Line {lineNumber}: Invalid date/time format - HDATE: {hdate}, HTIME: {htime}");
                }

                return record;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Line {lineNumber}: Error parsing line: {line} - {ex.Message}");
                return null;
            }
        }

        private DonationRecord ParseLegacyLine(string line, int lineNumber)
        {
            try
            {
                var parts = SplitCsvLine(line);
                
                if (parts.Length != 3)
                {
                    Console.WriteLine($"Line {lineNumber}: Expected 3 fields, found {parts.Length}");
                    return null;
                }

                var donationNumber = parts[0].Trim();
                var productCode = parts[1].Trim();
                var holdCode = parts[2].Trim();

                var record = new DonationRecord(donationNumber, productCode, holdCode);
                
                if (!record.IsValid())
                {
                    Console.WriteLine($"Line {lineNumber}: Invalid record format - Donation: {donationNumber} ({donationNumber.Length}), Product: {productCode} ({productCode.Length}), Hold: {holdCode} ({holdCode.Length})");
                    return null;
                }

                return record;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Line {lineNumber}: Error parsing line: {line} - {ex.Message}");
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

        private class HeaderInfo
        {
            public bool HasHeader { get; set; }
            public bool IsNewFormat { get; set; }
            public bool IsValid { get; set; }
            public Dictionary<string, int> FieldMapping { get; set; }
        }
    }
}