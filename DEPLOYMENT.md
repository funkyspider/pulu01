# PULU01 Deployment Guide

## Simplified Single-File Deployment (Recommended)

### For Developers: Creating the Deployment

**Option 1: Single File (No .NET Runtime Required on Server)**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -o deploy-single
```

**Option 2: Framework-Dependent (Requires .NET 8 Runtime on Server)**
```bash
dotnet publish -c Release -r win-x64 --self-contained false -o deploy-framework
```

### Deployment Result

#### Option 1 (Self-Contained) - User sees:
```
📁 PULU01/
├── 📄 PULU01.exe           (Single executable ~80MB)
├── ⚙️ appsettings.json     (Configuration file)
├── ⚙️ NLog.config          (Logging configuration)  
└── 📖 README.md            (User instructions)
```

#### Option 2 (Framework-Dependent) - User sees:
```
📁 PULU01/
├── 📄 PULU01.exe           (Small executable ~200KB)
├── 📄 *.dll                (Required libraries)
├── ⚙️ appsettings.json     (Configuration file)
├── ⚙️ NLog.config          (Logging configuration)
└── 📖 README.md            (User instructions)
```

## Deployment Instructions for IT/Server Admin

### Step 1: Copy Files to Server
Create folder: `C:\PULU01\`
Copy all files from the deployment folder.

### Step 2: For Framework-Dependent Only
Install .NET 8 Runtime: https://dotnet.microsoft.com/download/dotnet/8.0

### Step 3: Configure API Settings
Edit `C:\PULU01\appsettings.json`:
```json
{
  "Api": {
    "BaseUrl": "http://your-actual-api-server/DonationAPI/",
    "Headers": {
      "XUserId": "PULSE",
      "XAppName": "PULS11",
      "XEnvironment": "production"
    }
  }
}
```

### Step 4: Test Installation
```cmd
cd C:\PULU01
PULU01.exe --help
```

### Step 5: Set Permissions
- **Read/Execute**: PULU01.exe, appsettings.json, NLog.config
- **Write**: logs\ directory (auto-created)
- **Read**: CSV input files directory

## User Experience

### What Users See and Need to Know:

#### Simple Folder Structure:
- **PULU01.exe** - The main application (just double-click or run from command line)
- **appsettings.json** - Configuration (IT configured, users don't need to touch)
- **README.md** - Complete user instructions

#### What Users Run:
```cmd
# Navigate to folder
cd C:\PULU01

# Run with their CSV file
PULU01.exe --threads 5 --file "C:\Data\MyFile.csv" --clearcode "ABC"
```

#### Files Created During Use:
- `logs\pulu01-YYYY-MM-DD.log` - Daily log files
- `Hold_Clear_Ok.json` - Successfully processed records
- `Hold_Clear_Errors.json` - Failed records for review

## Recommendations

### For Production Deployment:
✅ **Use Self-Contained** - No .NET runtime dependency  
✅ **Single executable** - Clean, simple for users  
✅ **Include README.md** - Complete user documentation  
✅ **Pre-configure appsettings.json** - Users don't need to edit  

### For Development/Testing:
✅ **Use Framework-Dependent** - Smaller files, easier updates  
✅ **Keep DLLs visible** - Easier debugging if needed  

## File Size Comparison

- **Self-Contained**: ~80MB single file + configs (~80MB total)
- **Framework-Dependent**: ~5MB total (requires .NET 8 Runtime)

The self-contained approach is recommended for production as it provides the cleanest user experience with no dependencies.