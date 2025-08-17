# Plan

**Note: This is the original specification for the hold clearing feature. The application has been extended to support dual-mode processing (hold clearing and discard fate clearing). See clear_discard_plan.md for the discard feature specification.**

## Original Hold Clearing Specification

A utility which will clear holds placed on a donation/product code combination by making an API call to an endpoint.
Te donations to process will be provided in a .csv file.
The file could containind 30,000+ rows
Each row will contain

-- Donation Number (Character 14) L Example format G091224000001A
-- Product code (Character 4)
-- Hold Code (Character 3)

Items will be comma seperated on each line.  If an exel csv it may contain header row which should be ignored 

The utility should accept command line parameters
--threads N : Number of active threads
--file nnnnnnnnnnnnnn : .csv file

The program should run N number of threads continuously until all donations have been processed.

I want to be able to resume from where the job stopped.  For this I need a list of successfully processed. This should be stored in a file with a hard coded name "Hold_Clear_Ok.json". On start the code should look for this file, read and do not process any donation / product / Hold combinations which already processed.  The Hold_clear_ok file should be updated for successful api calls.  Write in blocks to prevent execessive file writes

A list of donations which failed is required.  Once again a file.  Include the failure reason message along with the main identifiers

## Tech stack
.net 8 console application
c#
Use best practices including seperation of concerns - Creae a worker service for api call
callback to update the console with progress.  This should manage batch style updates, increment progress every 20 etc
Dependency injection for the service
Multi-threaded with callbacks and locking

