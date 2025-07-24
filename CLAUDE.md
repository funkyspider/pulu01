# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 8 console application called PULU01 (savant.ulse.utility.client.PULU01) designed to clear holds placed on donation/product code combinations via API calls. The utility processes CSV files containing donation data and uses multi-threading for efficient processing.

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

## Project Structure

- `PULU01.sln` - Solution file
- `savant.ulse.utility.client.PULU01/` - Main project directory
  - `Program.cs` - Entry point (currently minimal Hello World)
  - `savant.ulse.utility.client.PULU01.csproj` - Project file (.NET 8, console app)
  - `plan/plan.md` - Detailed requirements and specifications

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

## Testing

Sample CSV files are included:
- `sample_data.csv` - 10 records with header
- `small_test.csv` - 3 records without header

Test with: `dotnet run -- --threads 3 --file sample_data.csv`