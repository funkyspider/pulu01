# Clear Discard Plan - PULU01 Feature Enhancement

## ✅ Implementation Status: COMPLETED

**Feature Status**: The discard fate clearing functionality has been successfully implemented and tested.

**Completion Date**: August 2025

**Key Achievements**:
- ✅ Dual-mode processing (hold and discard) fully implemented
- ✅ Command line interface with `--mode` parameter working
- ✅ CSV processing for both hold and discard formats functional
- ✅ API integration with discard fate clearing endpoint successful
- ✅ Mode-aware resumption with separate JSON files working
- ✅ Console output updated to reflect current processing mode
- ✅ Production HTTP API client tested and deployed
- ✅ All architectural patterns maintained and extended properly

## Overview

This document outlines the plan to add **discard fate clearing** functionality to the existing PULU01 console application. This is an **additive feature** that extends the current hold clearing capabilities without modifying or breaking existing functionality.

## Current Application State

The PULU01 application is a production-ready .NET 8 console utility that processes CSV files to clear holds on donation/product combinations via API calls. Key existing features:

- Multi-threaded processing (1-50 configurable threads)
- Resumable processing with JSON persistence
- Progress tracking with ETA calculations
- Command line interface with System.CommandLine
- Thread-safe operations with proper locking
- Both production and mock API clients
- Comprehensive logging with NLog

## New Feature Requirements

### Functional Requirements

1. **Mode Selection**: Add `--mode` command line parameter accepting values:
   - `"hold"` (default) - existing hold clearing functionality
   - `"discard"` - new discard fate clearing functionality

2. **CSV File Format**: Support new discard CSV format with headers:
   - **Required fields**: `DNTNO`, `PRDCD`, `LOCCD`
   - **Optional fields**: `HDATE`, `HTIME`, `RSHLD` (read if non-blank, non-zero)
   - **Ignored fields**: Any other columns

3. **API Integration**: Call new discard clearing endpoint:
   - **URL**: `http://w19api6/DonationAPI/api/v43/ComponentHold/clear-discard-fate`
   - **Method**: HTTP POST
   - **Content-Type**: application/json

4. **File Processing**: Mirror existing hold processing behaviour:
   - Multi-threaded processing
   - Resume functionality with JSON tracking
   - Separate OK/Error files for discard operations
   - Progress reporting with ETA

### Technical Requirements

1. **Maintain Existing Architecture**: Use established patterns and services
2. **Thread Safety**: Ensure new functionality is thread-safe
3. **Error Handling**: Healthcare-grade exception handling and logging
4. **Testing**: Support both production and mock API modes
5. **Performance**: Handle large CSV files efficiently (30,000+ rows)

## New Data Models

### Core Models

```csharp
// New discard-specific record model
public class DiscardRecord
{
    public string DonationNumber { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public DateTime? DateTimePlaced { get; set; }
    public string HoldCode { get; set; } = string.Empty;
}

// Enhanced processing mode enumeration
public enum ProcessingMode
{
    Hold,
    Discard
}

// Discard fate enumeration
public enum DiscardFate
{
    [Description("")]
    None,
    [Description("0003")]
    Biohazard,
    [Description("0004")]
    Discard
}
```

### DTOs for API Communication

```csharp
// Base DTO interface
public interface IClearDiscardFateRequestDto
{
    string ProductCode { get; set; }
    string UnitNumber { get; set; }
    DiscardFate DiscardFate { get; set; }
    string HoldCode { get; set; }
    DateTime DateTimePlaced { get; set; }
    string ProgramId { get; set; }
}

// Request DTO for discard clearing
public class ClearDiscardFateRequestDto : IClearDiscardFateRequestDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public DiscardFate DiscardFate { get; set; } = DiscardFate.None;
    public string HoldCode { get; set; } = string.Empty;
    public DateTime DateTimePlaced { get; set; } = DateTime.MinValue;
    public string ProgramId { get; set; } = string.Empty;
}

// Response DTO (likely similar to existing hold response)
public class ClearDiscardFateResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
}
```

## Architecture Changes

### 1. Command Line Interface Enhancement

**File**: `Program.cs`

- Add `--mode` option to existing command line parser
- Validate mode parameter accepts only "hold" or "discard"
- Default to "hold" when not specified
- Pass mode to ApplicationService via configuration

### 2. Configuration Model Extension

**File**: `Configuration/AppConfiguration.cs`

```csharp
public class AppConfiguration
{
    // Existing properties...
    public ProcessingMode Mode { get; set; } = ProcessingMode.Hold;
    
    // New API endpoints
    public string DiscardApiEndpoint { get; set; } = "/api/v43/ComponentHold/clear-discard-fate";
}
```

### 3. Service Layer Enhancements

#### A. CSV Reader Service Enhancement

**File**: `Services/CsvReaderService.cs`

- Create overloaded method `ReadDiscardRecordsAsync()` for discard CSV parsing
- Implement header detection for discard format (DNTNO, PRDCD, LOCCD)
- Handle optional fields (HDATE, HTIME, RSHLD) with null checking
- Validate required fields are present and properly formatted
- Convert HDATE/HTIME to DateTime when both present and non-zero

#### B. API Client Service Enhancement

**File**: `Services/HttpApiClientService.cs`

- Add `ClearDiscardFateAsync()` method for discard API calls
- Map DiscardRecord to ClearDiscardFateRequestDto
- Handle discard-specific HTTP responses and error codes
- Maintain existing error handling patterns

**File**: `Services/MockApiClientService.cs`

- Add mock implementation of `ClearDiscardFateAsync()`
- Simulate 95% success rate with 1-2 second delays
- Generate realistic mock responses for testing

#### C. Processing Worker Service Enhancement

**File**: `Services/ProcessingWorkerService.cs`

- Create mode-aware processing logic
- Route to appropriate API client methods based on mode
- Maintain thread-safe operations for both hold and discard processing
- Ensure progress callbacks work consistently for both modes

#### D. Persistence Service Enhancement

**File**: `Services/ProcessingPersistenceService.cs`

- Generate mode-specific file names:
  - Hold mode: `Hold_Clear_Ok.json`, `Hold_Clear_Errors.json`
  - Discard mode: `Discard_Clear_Ok.json`, `Discard_Clear_Errors.json`
- Support both DonationRecord and DiscardRecord serialization
- Maintain resume functionality for both processing modes

### 4. Validation Enhancements

#### A. CSV Validation

- **Discard CSV Requirements**:
  - DNTNO: Non-empty string (flexible length)
  - PRDCD: Exactly 4 characters or "ALL" (case-insensitive)
  - LOCCD: Valid location code (non-empty string)
  - HDATE: Optional, YYYYMMDD format when present
  - HTIME: Optional, HHMMSSzzz format when present
  - RSHLD: Optional, 2+ characters when present

#### B. Mode-Specific File Validation

- Detect CSV format automatically based on headers
- Validate file format matches specified mode
- Provide clear error messages for format mismatches

## File Structure Changes

### New Files to Create

```
Services/
├── Interfaces/
│   └── IClearDiscardFateService.cs     # New interface for discard operations
Models/
├── DiscardRecord.cs                    # New discard record model
├── ClearDiscardFateRequestDto.cs       # New discard request DTO
├── ClearDiscardFateResponseDto.cs      # New discard response DTO
└── ProcessingMode.cs                   # New processing mode enumeration
Enums/
└── DiscardFate.cs                      # New discard fate enumeration
```

### Files to Modify

```
Program.cs                              # Add --mode command line option
Configuration/AppConfiguration.cs      # Add Mode and DiscardApiEndpoint
Services/CsvReaderService.cs           # Add ReadDiscardRecordsAsync method
Services/HttpApiClientService.cs       # Add ClearDiscardFateAsync method
Services/MockApiClientService.cs       # Add mock discard clearing
Services/ProcessingWorkerService.cs    # Add mode-aware processing
Services/ProcessingPersistenceService.cs # Add mode-specific file naming
Services/ApplicationService.cs         # Add mode routing logic
JsonContext.cs                         # Add new types for serialization
```

## Implementation Strategy

### Phase 1: Core Infrastructure
1. Add new data models and enumerations
2. Extend command line interface with --mode parameter
3. Update configuration model and dependency injection

### Phase 2: Service Layer Extensions
1. Enhance CSV reader service for discard format
2. Extend API client services (both production and mock)
3. Update persistence service for mode-specific files

### Phase 3: Processing Logic Integration
1. Modify worker service for mode-aware processing
2. Update application service orchestration
3. Ensure thread-safety across all new functionality

### Phase 4: Testing and Validation
1. Create unit tests for new services and models
2. Test with sample discard CSV files
3. Validate multi-threaded processing with both modes
4. Test resume functionality for discard processing

## Testing Requirements

### Unit Tests (XUnit with FluentAssertions and NSubstitute)

```csharp
// Example test structure
public class DiscardCsvReaderServiceTests
{
    [Fact]
    public async Task ReadDiscardRecordsAsync_WithValidFile_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = "DNTNO,PRDCD,LOCCD,HDATE,HTIME,RSHLD\n...";
        
        // Act
        var result = await csvReaderService.ReadDiscardRecordsAsync(csvContent);
        
        // Assert
        result.Should().NotBeEmpty();
        result.First().DonationNumber.Should().NotBeEmpty();
    }
}

public class HttpApiClientServiceTests
{
    [Fact]
    public async Task ClearDiscardFateAsync_WithValidRequest_ShouldSucceed()
    {
        // Test implementation for discard API calls
    }
}
```

### Integration Tests

- Test complete discard processing workflow
- Validate mode switching between hold and discard
- Test error handling and file persistence
- Verify multi-threaded processing performance

### Test Data

```csv
# Sample DiscardsToClear.csv
DNTNO,PRDCD,LOCCD,HDATE,HTIME,RSHLD
DN12345678,PLAT,001,20250816,120000000,ABC
DN87654321,CRYO,002,20250816,130000000,XYZ
DN11111111,ALL,003,,,
```

## Command Line Usage Examples

```bash
# Existing hold clearing (default mode)
dotnet run -- --threads 5 --file HoldsToClear.csv --clearcode "ABC"

# Explicit hold mode
dotnet run -- --mode hold --threads 5 --file HoldsToClear.csv --clearcode "ABC"

# New discard clearing mode
dotnet run -- --mode discard --threads 3 --file DiscardsToClear.csv --clearcode "XYZ"

# Help showing new option
dotnet run -- --help
```

## Error Handling and Logging

### Healthcare-Grade Safety Requirements

1. **Comprehensive Exception Handling**:
   - Wrap all discard API calls in try-catch blocks
   - Log all exceptions with full context
   - Never silently fail or lose data

2. **Detailed Audit Trail**:
   - Log every discard clearing attempt
   - Record success/failure with timestamps
   - Include donation number, product code, and location code in logs

3. **Data Integrity**:
   - Validate all input data before processing
   - Ensure atomic operations where possible
   - Maintain referential integrity in persistence files

### Logging Enhancements

```csharp
// Example logging for discard operations
logger.LogInformation("Starting discard clearing for donation {DonationNumber}, product {ProductCode}, location {LocationCode}",
    record.DonationNumber, record.ProductCode, record.LocationCode);

logger.LogError(exception, "Failed to clear discard fate for donation {DonationNumber}: {ErrorMessage}",
    record.DonationNumber, exception.Message);
```

## Configuration Changes

### appsettings.json

```json
{
  "Api": {
    "BaseUrl": "http://w19api6/DonationAPI",
    "HoldEndpoint": "/api/v43/ComponentHold/clear",
    "DiscardEndpoint": "/api/v43/ComponentHold/clear-discard-fate",
    "Headers": {
      "XUserId": "PULU01",
      "XAppName": "PulseUtility",
      "XEnvironment": "Production"
    }
  }
}
```

## Performance Considerations

1. **Memory Usage**: DiscardRecord objects should be lightweight
2. **Thread Safety**: All new services must be thread-safe
3. **I/O Optimization**: Batch file writes for discard results
4. **API Throttling**: Respect existing rate limiting patterns

## Backwards Compatibility

- **Zero Breaking Changes**: All existing functionality remains intact
- **Default Behaviour**: Application defaults to hold mode when --mode not specified
- **File Formats**: Existing hold CSV format continues to work unchanged
- **API Calls**: Existing hold clearing API calls remain unmodified

## Security Considerations

1. **Input Validation**: Sanitise all CSV input data
2. **API Security**: Maintain existing authentication headers
3. **File Access**: Validate file paths and permissions
4. **Data Exposure**: Ensure sensitive data is properly logged/masked

## Success Criteria

1. **Functional**: Discard clearing works identically to hold clearing
2. **Performance**: No degradation in existing hold clearing performance
3. **Reliability**: Healthcare-grade error handling and logging
4. **Maintainability**: Clean separation between hold and discard logic
5. **Testing**: Comprehensive unit and integration test coverage
6. **Documentation**: Updated README and inline code comments

## Rollback Plan

If issues arise:
1. **Feature Flag**: Disable discard mode via configuration
2. **Code Isolation**: New discard functionality is isolated from existing hold logic
3. **Database Impact**: No database schema changes required
4. **API Dependencies**: New API endpoint is independent of existing hold endpoint

## Future Considerations

- **Additional Modes**: Architecture supports adding more processing modes
- **Enhanced Validation**: More sophisticated CSV validation rules
- **Monitoring**: Enhanced telemetry and health checks
- **Performance Optimisation**: Bulk API operations if supported by backend

---

## Implementation Notes for AI Coding Assistant

1. **Preserve Existing Code**: Do not modify working hold clearing functionality
2. **Follow Existing Patterns**: Use established service patterns and dependency injection
3. **Maintain Architecture**: Keep clean separation of concerns
4. **Healthcare Safety**: Implement robust error handling and logging
5. **Thread Safety**: Ensure all new code is thread-safe
6. **Testing**: Write comprehensive unit tests using XUnit, FluentAssertions, and NSubstitute
7. **SOLID Principles**: Follow established SOLID patterns in existing codebase
8. **Async/Await**: All database and API operations must be async
9. **SonarLint Compliance**: Ensure all new code passes SonarLint analysis
10. **UK Standards**: Use UK metric formats for any dates/measurements

This feature extends the existing PULU01 application to support discard fate clearing while maintaining all existing functionality and architectural patterns.