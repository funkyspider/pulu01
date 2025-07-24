using System.Text;

const int recordCount = 20000;
const string outputFileName = "test_data_20k.csv";

Console.WriteLine($"Generating {recordCount:N0} test records...");

var random = new Random();
var productCodes = new[] { "PROD", "GIFT", "DONA", "MEMB", "EVEN", "ITEM" };
var holdCodes = new[] { "H01", "H02", "H03", "H04", "H05", "SUS", "REV", "CHK" };

// Characters for the last position: 0-9, A-Z
var lastChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

using var writer = new StreamWriter(outputFileName);

// Write header
writer.WriteLine("DonationNumber,ProductCode,HoldCode");

for (int i = 1; i <= recordCount; i++)
{
    // Build donation number: G095625 + 6-digit sequence + last char
    var sequenceNumber = i.ToString("D6");
    var lastChar = lastChars[random.Next(lastChars.Length)];
    var donationNumber = $"G095625{sequenceNumber}{lastChar}";
    
    // Random product and hold codes
    var productCode = productCodes[random.Next(productCodes.Length)];
    var holdCode = holdCodes[random.Next(holdCodes.Length)];
    
    writer.WriteLine($"{donationNumber},{productCode},{holdCode}");
    
    // Progress indicator
    if (i % 5000 == 0)
    {
        Console.WriteLine($"Generated {i:N0} records...");
    }
}

Console.WriteLine($"Successfully generated {recordCount:N0} records in {outputFileName}");
Console.WriteLine($"File size: {new FileInfo(outputFileName).Length / 1024:N0} KB");

// Show first few and last few records as samples
Console.WriteLine("\nFirst 5 records:");
ShowSampleRecords(outputFileName, 1, 5);

Console.WriteLine("\nLast 5 records:");
ShowSampleRecords(outputFileName, recordCount - 4, 5);

static void ShowSampleRecords(string fileName, int startLine, int count)
{
    var lines = File.ReadAllLines(fileName);
    for (int i = startLine; i < Math.Min(startLine + count, lines.Length); i++)
    {
        if (i == 0) // Header
            Console.WriteLine($"  Header: {lines[i]}");
        else
            Console.WriteLine($"  {i:D6}: {lines[i]}");
    }
}
