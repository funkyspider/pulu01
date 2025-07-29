# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 8 console application called PULU01 (Savant.Pulse.Utility.Client.PULU01) designed to clear holds placed on donation/product code combinations via API calls. The utility processes CSV files containing donation data and uses multi-threading for efficient processing.

## Key Requirements & Architecture

### Core Functionality
- Processes CSV files with 30,000+ rows containing donation numbers (14 chars), product codes (4 chars), and hold codes (3 chars)
- Multi-threaded processing with configurable thread count
- Resumable processing using `Hold_Clear_Ok.json` for successful records
- Error tracking with failure logging including reasons
- Batch updates to prevent excessive file writes

### Command Line Parameters
- `--threads N`: Number of active threads
- `--file filename`: CSV file to process

### Architecture Requirements
- Separation of concerns with dedicated worker service for API calls
- Dependency injection for services
- Multi-threaded implementation with callbacks and proper locking
- Progress callbacks with batch-style updates (increment every 20 operations)
- Console progress management

## Build and Development Commands

Since this is a .NET 8 project:

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Run with parameters
dotnet run -- --threads 5 --file data.csv

# Build for release
dotnet build --configuration Release

# Clean build artifacts
dotnet clean
```

## Project Structure & Architecture

- `PULU01.sln` - Solution file
- `Savant.Pulse.Utility.Client.PULU01/` - Main project directory
  - `Program.cs` - Entry point with System.CommandLine integration and DI setup
  - `Configuration/AppConfiguration.cs` - Configuration model
  - `Extensions/ServiceCollectionExtensions.cs` - DI container setup
  - `Models/` - Domain models (DonationRecord, ProcessingResult, ProcessingStatus)
  - `Services/` - Core business logic services
    - `Interfaces/` - Service contracts
    - `ApplicationService.cs` - Main orchestration service
    - `CsvReaderService.cs` - CSV parsing with header detection
    - `ProcessingWorkerService.cs` - Multi-threaded processing coordination
    - `ProcessingPersistenceService.cs` - JSON file persistence for resume functionality
    - `ProgressTrackingService.cs` - Real-time progress reporting with ETA
    - `MockApiClientService.cs` - API client simulation (95% success rate, 1-2s delays)
  - `Utilities/ConsoleHelper.cs` - Console UI helpers and formatting
  - `plan/plan.md` - Original requirements specification

## Current Implementation Status

✅ **Stage 1-5 Complete**: Full working implementation with all core features:
- Domain models (DonationRecord, ProcessingResult, ProcessingStatus)
- Command line argument parsing with System.CommandLine
- Dependency injection with Microsoft.Extensions.DependencyInjection
- CSV reader service with header detection and validation
- Resume tracking service with JSON persistence (`Hold_Clear_Ok.json`, `Hold_Clear_Errors.json`)
- Mock API client service with 1-2 second random delays and 95% success rate
- Progress tracking service with real-time console updates and ETA
- Multi-threaded worker service with configurable thread count
- Thread-safe processing with proper locking and error handling

## Development Notes

- Target framework: .NET 8.0
- Output type: Console application
- Nullable reference types enabled
- Implicit usings enabled
- Uses dependency injection pattern throughout
- Implements producer-consumer pattern for multi-threading
- Batch file writing to prevent excessive I/O operations
- Comprehensive logging with Microsoft.Extensions.Logging
- Thread-safe progress reporting with visual progress bars
- Resume functionality by tracking processed records
- Graceful shutdown handling with Ctrl+C support

## Key Architecture Patterns

- **Producer-Consumer**: `ProcessingWorkerService` uses a thread-safe queue with configurable worker threads
- **Service Layer**: All business logic isolated in services with dependency injection
- **Persistence Strategy**: JSON files for tracking processed records (`Hold_Clear_Ok.json`, `Hold_Clear_Errors.json`)
- **Progress Reporting**: Batch-style updates (every 20 operations) with real-time ETA calculations
- **Graceful Shutdown**: Ctrl+C handling with proper resource cleanup and summary display

## Testing & Sample Data

Available test files:
- `HolsToClear_3.csv` - 3 records with proper header format (DNTNO, HDATE, HTIME, PRDCD, RSHLD, etc.)
- `HoldsToClear.csv` - ~9,264 records for performance testing with proper header format

Test commands:
```bash
# Quick test with small dataset
dotnet run -- --threads 3 --file HolsToClear_3.csv

# Performance test with larger dataset
dotnet run -- --threads 10 --file HoldsToClear.csv

# Resume functionality test (run, stop with Ctrl+C, then re-run)
dotnet run -- --threads 5 --file HolsToClear_3.csv
```

## CSV Format Requirements

The application now supports the customer-supplied format with these required headers:
- `DNTNO` - Donation Number (maps to DonationNumber, trimmed) - flexible length for different batch codes
- `HDATE` - Hold Date in YYYYMMDD format  
- `HTIME` - Hold Time in HHMMSSzzz format
- `PRDCD` - Product Code (maps to ProductCode) - must be exactly 4 characters
- `RSHLD` - Hold Code (maps to HoldCode) - must be more than 1 character

Additional columns are ignored. HDATE and HTIME are combined into a single DateTime field.

## Validation Rules

- **Donation Number**: Any non-empty string (flexible length to accommodate different batch code formats)
- **Product Code**: Exactly 4 characters
- **Hold Code**: More than 1 character (allows 2, 3, or more character codes)

## Dependencies

Key NuGet packages:
- `System.CommandLine` (2.0.0-beta4) - Command line parsing
- `Microsoft.Extensions.DependencyInjection` (8.0.0) - DI container
- `Microsoft.Extensions.Hosting` (8.0.0) - Host builder pattern
- `Microsoft.Extensions.Logging` (8.0.0) - Structured logging