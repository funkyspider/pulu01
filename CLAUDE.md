# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 8 console application called PULU01 (Savant.Pulse.Utility.Client.PULU01) designed to process donation records via API calls. The utility supports two processing modes:

1. **Hold Clearing**: Clear holds placed on donation/product code combinations
2. **Discard Fate Clearing**: Clear discard fates for donation components

The utility processes CSV files containing donation data and uses multi-threading for efficient processing.

## Key Requirements & Architecture

### Core Functionality
- **Dual Mode Processing**: Hold clearing and discard fate clearing modes
- Processes CSV files with 30,000+ rows containing donation data
- Multi-threaded processing with configurable thread count (1-50)
- **Mode-specific resumption**: Uses mode-aware log files for tracking progress
  - Hold mode: `Hold_Clear_Ok.json`, `Hold_Clear_Errors.json`
  - Discard mode: `Discard_Clear_Ok.json`, `Discard_Clear_Errors.json`
- Error tracking with failure logging including detailed reasons
- Batch updates to prevent excessive file writes
- **Mode-aware console output** for user confidence

### Command Line Parameters
- `--mode MODE`: Processing mode - 'hold' (default) or 'discard'
- `--threads N`: Number of active threads (1-50, default: 1)
- `--file filename`: CSV file to process (required)
- `--clearcode XYZ`: Pulse clear code (required for hold mode, optional for discard mode, 2-3 characters)

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

# Run with parameters (hold mode - default)
dotnet run -- --threads 5 --file holds.csv --clearcode "ABC"

# Run in discard mode
dotnet run -- --mode discard --threads 5 --file discards.csv

# Run in hold mode explicitly
dotnet run -- --mode hold --threads 5 --file holds.csv --clearcode "ABC"

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
  - `Models/` - Domain models (DonationRecord, DiscardRecord, ProcessingResult, ProcessingStatus, ProcessingMode)
  - `Services/` - Core business logic services
    - `Interfaces/` - Service contracts
    - `ApplicationService.cs` - Main orchestration service
    - `CsvReaderService.cs` - CSV parsing with header detection
    - `ProcessingWorkerService.cs` - Multi-threaded processing coordination
    - `ProcessingPersistenceService.cs` - JSON file persistence for resume functionality
    - `ProgressTrackingService.cs` - Real-time progress reporting with ETA
    - `HttpApiClientService.cs` - Production API client for clearing holds and discard fates
    - `MockApiClientService.cs` - API client simulation (95% success rate, 1-2s delays)
  - `enums/` - Enumeration types (HoldType, ClearHoldStatus, ErrorNum, ProcessingMode, DiscardFate, ClearFateStatus)
  - `JsonContext.cs` - Source-generated JSON serialization context
  - `Utilities/ConsoleHelper.cs` - Console UI helpers and formatting
  - `plan/plan.md` - Original requirements specification

## Current Implementation Status

✅ **Production Ready**: Full working implementation with all core features:
- Domain models (DonationRecord, ProcessingResult, ProcessingStatus, DTOs)
- Command line argument parsing with System.CommandLine including required clearcode parameter
- Dependency injection with Microsoft.Extensions.DependencyInjection
- CSV reader service with header detection and validation
- Resume tracking service with JSON persistence (mode-aware: `Hold_Clear_Ok.json`/`Discard_Clear_Ok.json`, `Hold_Clear_Errors.json`/`Discard_Clear_Errors.json`)
- Production HTTP API client service for real hold clearing and discard fate clearing operations
- Mock API client service with 1-2 second random delays and 95% success rate
- Progress tracking service with real-time console updates and ETA
- Multi-threaded worker service with configurable thread count
- Thread-safe processing with proper locking and error handling
- Enhanced logging with NLog for structured output

## Development Notes

- Target framework: .NET 8.0
- Output type: Console application
- Nullable reference types enabled
- Implicit usings enabled
- Uses dependency injection pattern throughout
- Implements producer-consumer pattern for multi-threading
- Batch file writing to prevent excessive I/O operations
- Comprehensive logging with NLog and Microsoft.Extensions.Logging
- Thread-safe progress reporting with visual progress bars
- Source-generated JSON serialization for performance
- Resume functionality by tracking processed records
- Graceful shutdown handling with Ctrl+C support

## Key Architecture Patterns

- **Producer-Consumer**: `ProcessingWorkerService` uses a thread-safe queue with configurable worker threads
- **Service Layer**: All business logic isolated in services with dependency injection
- **Persistence Strategy**: Mode-aware JSON files for tracking processed records (hold: `Hold_Clear_Ok.json`/`Hold_Clear_Errors.json`, discard: `Discard_Clear_Ok.json`/`Discard_Clear_Errors.json`)
- **Progress Reporting**: Batch-style updates (every 20 operations) with real-time ETA calculations
- **Graceful Shutdown**: Ctrl+C handling with proper resource cleanup and summary display

## Testing & Sample Data

Available test files:
- `HolsToClear_3.csv` - 3 records with proper hold format (DNTNO, HDATE, HTIME, PRDCD, RSHLD, etc.)
- `HoldsToClear.csv` - ~9,264 records for hold performance testing
- `DiscardsToClear.csv` - Sample discard records with format (DNTNO, PRDCD, LOCCD, HDATE, HTIME, RSHLD)

Test commands:
```bash
# Show help information
dotnet run -- --help

# Hold mode tests
dotnet run -- --mode hold --threads 3 --file HolsToClear_3.csv --clearcode "ABC"
dotnet run -- --threads 10 --file HoldsToClear.csv --clearcode "XYZ"

# Discard mode tests
dotnet run -- --mode discard --threads 3 --file DiscardsToClear.csv
dotnet run -- --mode discard --threads 5 --file DiscardsToClear.csv --clearcode "XYZ"

# Resume functionality test (run, stop with Ctrl+C, then re-run)
dotnet run -- --mode hold --threads 5 --file HolsToClear_3.csv --clearcode "ABC"
```

## CSV Format Requirements

### Hold Mode CSV Format
Required headers for hold clearing:
- `DNTNO` - Donation Number (maps to DonationNumber, trimmed) - flexible length for different batch codes
- `HDATE` - Hold Date in YYYYMMDD format  
- `HTIME` - Hold Time in HHMMSSzzz format
- `PRDCD` - Product Code (maps to ProductCode) - must be exactly 4 characters
- `RSHLD` - Hold Code (maps to HoldCode) - must be more than 1 character

### Discard Mode CSV Format
Required headers for discard fate clearing:
- `DNTNO` - Donation Number (maps to DonationNumber, trimmed) - flexible length
- `PRDCD` - Product Code (maps to ProductCode) - must be exactly 4 characters or "ALL"
- `LOCCD` - Location Code (maps to LocationCode) - must be exactly 4 characters

Optional headers for discard mode:
- `HDATE` - Hold Date in YYYYMMDD format (00000000 if not applicable)
- `HTIME` - Hold Time in HHMMSSzzz format (000000 if not applicable)
- `RSHLD` - Hold Code (maps to HoldCode) - can be empty

Additional columns are ignored. HDATE and HTIME are combined into a single DateTime field when provided.

## Validation Rules

### Hold Mode Validation
- **Donation Number**: Any non-empty string (flexible length to accommodate different batch code formats)
- **Product Code**: Exactly 4 characters
- **Hold Code**: More than 1 character (allows 2, 3, or more character codes)
- **Clear Code**: Required parameter, 2-3 characters

### Discard Mode Validation
- **Donation Number**: Any non-empty string (flexible length to accommodate different batch code formats)
- **Product Code**: Exactly 4 characters or "ALL" (case-insensitive)
- **Location Code**: Exactly 4 characters
- **Hold Code**: Optional (can be empty)
- **Clear Code**: Optional parameter

## Dependencies

Key NuGet packages:
- `System.CommandLine` (2.0.0-beta4) - Command line parsing
- `Microsoft.Extensions.DependencyInjection` (8.0.0) - DI container
- `Microsoft.Extensions.Hosting` (8.0.0) - Host builder pattern
- `Microsoft.Extensions.Logging` (8.0.0) - Structured logging
- `Microsoft.Extensions.Http` (8.0.0) - HTTP client services
- `Microsoft.Extensions.Configuration` (8.0.0) - Configuration framework
- `NLog` (5.3.4) - Advanced logging framework
- `NLog.Extensions.Logging` (5.3.14) - NLog integration with Microsoft logging

## Configuration

The application uses `appsettings.json` for configuration:
- API settings (BaseUrl, Headers for authentication)
- Logging configuration
- Environment-specific settings

Key configuration sections:
- `Api.BaseUrl` - Base URL for the donation API
- `Api.ClearHoldEndpoint` - Endpoint for hold clearing operations
- `Api.ClearDiscardEndpoint` - Endpoint for discard fate clearing operations
- `Api.Headers` - Required headers (XUserId, XAppName, XEnvironment)
- `Api.TimeoutSeconds` - HTTP request timeout (default: 30 seconds)
- Logging configuration is handled via NLog.config

## API Integration

The application integrates with two API endpoints:

### Hold Clearing API
- **Endpoint**: `api/v43/ComponentHold/clear-hold`
- **Method**: POST
- **Request**: ClearHoldRequestDto with donation number, product code, hold code, clear code
- **Response**: ClearHoldResponseDto with status and error information

### Discard Fate Clearing API
- **Endpoint**: `api/v43/ComponentHold/clear-discard-fate` 
- **Method**: POST
- **Request**: ClearDiscardFateRequestDto with donation number, product code, location code, discard fate
- **Response**: ClearDiscardFateResponseDto with status and error information

Both endpoints require authentication headers (XUserId, XAppName, XEnvironment) and return structured responses for success/failure tracking.