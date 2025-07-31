# PULU01 - Donation Hold Clearing Utility

A .NET 8 console application for clearing holds on donation/product code combinations via API calls. Processes CSV files with multi-threading support and comprehensive logging.

## 📋 Requirements

- **Windows Server 2019** or later
- **.NET 8 Runtime** (Console Apps) - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Network access** to the donation API server

## 🚀 Quick Start

### 1. Verify .NET 8 Installation
Open command prompt and run:
```cmd
dotnet --version
```
You should see version 8.0.x or later.

### 2. Configure API Settings
Edit `appsettings.json` in the application folder:
```json
{
  "Api": {
    "BaseUrl": "http://your-api-server/DonationAPI/",
    "Headers": {
      "XUserId": "PULSE",
      "XAppName": "PULS11",
      "XEnvironment": "production"
    }
  }
}
```

### 3. Prepare Your CSV File
Your CSV file must contain these columns (case-insensitive):
- **DNTNO** - Donation Number (any length)
- **HDATE** - Hold Date (YYYYMMDD format, e.g., 20240813)  
- **HTIME** - Hold Time (HHMMSSzzz format, e.g., 142724591)
- **PRDCD** - Product Code (exactly 4 characters)
- **RSHLD** - Hold Code (2+ characters, e.g., COS, RD)

Additional columns are ignored.

### 4. Run the Application
```cmd
PULU01.exe --threads 5 --file "path\to\your\file.csv" --clearcode "XYZ"
```

**Parameters:**
- `--threads` - Number of concurrent processing threads (1-50). Default: 1
- `--file` - Path to your CSV file (required)
- `--clearcode` - Pulse clear code (2-3 characters, required)

## 📊 Examples

**Basic usage with 1 thread:**
```cmd
PULU01.exe --file donations.csv --clearcode "ABC"
```

**High-performance processing with 10 threads:**
```cmd
PULU01.exe --threads 10 --file "C:\Data\HoldsToClear.csv" --clearcode "XYZ"
```

**Processing with spaces in file path:**
```cmd
PULU01.exe --threads 5 --file "C:\Hold Files\donations 2024.csv" --clearcode "DEF"
```

## 📁 Output Files

The application creates tracking files in the same directory:

### `Hold_Clear_Ok.json`
- Contains successfully processed records with timestamps
- Used for **resume functionality** - processed records are skipped on restart
- Each line is a separate JSON record

### `Hold_Clear_Errors.json`
- Contains failed records with error messages and timestamps
- Each line is a separate JSON record for easy parsing
- Review this file to identify and fix data issues

### `logs/pulu01-YYYY-MM-DD.log`
- Daily log files with detailed operation logs
- Contains structured success/failure information:
  - ✅ `Hold cleared successfully - Unit: G123, Product: SP54, Hold: COS`
  - ⚠️ `Hold clear failed - Unit: G124, Product: SP54, Hold: RD, Error: Unit not found`

## 🔄 Resume Functionality

If processing is interrupted (Ctrl+C or system restart):
1. Simply **run the same command again**
2. The application will automatically skip already processed records
3. Processing continues from where it left off
4. Shows how many records were skipped at startup

## ⏹️ Stopping the Process

- Press **Ctrl+C** to gracefully stop processing
- The application saves progress and displays a summary
- All processed records are saved before exit

## 🔧 Troubleshooting

### "File not found" Error
- Verify the file path is correct
- Use quotes around paths with spaces
- Ensure the file exists and is accessible

### "Thread count must be between 1 and 50" Error
- Use a valid thread count: `--threads 5`
- More threads = faster processing but higher resource usage

### "Clear code is required" Error  
- Provide a valid clear code: `--clearcode "ABC"`
- Clear code must be 2-3 characters

### API Connection Issues
- Verify `BaseUrl` in `appsettings.json` is correct
- Check network connectivity to the API server
- Review logs for detailed error messages

### CSV Format Issues
- Ensure your CSV has the required column headers
- Check that Product Codes are exactly 4 characters
- Verify Hold Codes are 2+ characters
- Review the error log for validation details

## 📈 Performance Recommendations

### Thread Count Guidelines:
- **Small files (< 1,000 records):** 1-3 threads
- **Medium files (1,000-10,000 records):** 5-10 threads  
- **Large files (10,000+ records):** 10-20 threads
- **Very large files:** Start with 10 threads and increase if needed

### Monitoring Performance:
- Watch the progress display for records/second rate
- Monitor server CPU and network usage
- Adjust thread count based on performance

## 🛡️ Data Validation

The application validates each record:
- **Donation Number:** Any non-empty string
- **Product Code:** Exactly 4 characters
- **Hold Code:** More than 1 character
- **Date/Time:** Valid YYYYMMDD and HHMMSSzzz formats

Invalid records are logged but do not stop processing.

## 📞 Support

For issues or questions:
1. Check the log files in the `logs/` directory
2. Review error records in `Hold_Clear_Errors.json`
3. Verify your CSV file format matches requirements
4. Contact your system administrator with specific error messages

---

**Version:** 1.0  
**Last Updated:** July 2025  
**Compatible with:** .NET 8, Windows Server 2019+