using System.Text.Json.Serialization;

namespace Savant.Pulse.Utility.Client.PULU01.enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorAreaAttribute : Attribute
    {
        public ErrorAreaAttribute(string area)
        {
            Area = area;
        }

        public string Area { get; }
    }

    public static class ErrorArea
    {
        public const string System = "System";
        public const string Session = "Session";
        public const string Stock = "Stock";
        public const string Reference = "Reference";
        public const string Address = "Address";
        public const string Balance = "Balance";
        public const string Report = "Report";
        public const string Print = "Print";
        public const string Donor = "Donor";
        public const string Laboratory = "Laboratory";
        public const string Utility = "Utility";
        public const string Order = "Order";
        public const string Outcome = "Outcome";
        public const string User = "User";
        
        // queue jobs
        public const string PULQ52 = "PULQ52";
        public const string PULQ40 = "PULQ40";
        
        // plugins
        public const string PULS23 = "PULS23";
    }

    /// <summary>
    /// Error Number Enumeration
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ErrorNum>))]
    public enum ErrorNum
    {
        /// <summary>
        /// Default when not set
        /// </summary>
        Unknown = 0,
        BookingError = 1,
        [ErrorArea(ErrorArea.System)]
        SysSerialisation = 10001,
        [ErrorArea(ErrorArea.System)]
        SysAuthentication = 10002,
        [ErrorArea(ErrorArea.System)]
        SysArgumentError = 10003,
        [ErrorArea(ErrorArea.System)]
        SysAutoMapperError = 10004,
        [ErrorArea(ErrorArea.System)]
        SysDatabaseError = 10005,
        [ErrorArea(ErrorArea.System)]
        SysDatabaseUpdateError = 10006,
        [ErrorArea(ErrorArea.System)]
        SysJsonPatchError = 10007,
        [ErrorArea(ErrorArea.System)]
        SysValidationError = 10008,
        [ErrorArea(ErrorArea.System)]
        SysLoqateError = 10009,
        [ErrorArea(ErrorArea.System)]
        SysUnauthorizedAccessError = 10010,
        [ErrorArea(ErrorArea.System)]
        SysMimerError = 10011,
        /// <summary>
        /// Error code for HttpRequestExceptions raised in code e.g. raised from HttpService when API is down
        /// </summary>
        [ErrorArea(ErrorArea.System)]
        SysHttpRequestError = 10012,
        [ErrorArea(ErrorArea.System)]
        SysInvalidEndpoint = 10013,
        [ErrorArea(ErrorArea.System)]
        SysResourceNotFound = 10014,
        [ErrorArea(ErrorArea.System)]
        SysDataError = 10015,
        /// <summary>
        /// Configuration exception errors
        /// </summary>
        [ErrorArea(ErrorArea.System)]
        SysConfiguration = 10016,
        /* Savant Business Errors -> Thrown with SavantException on API */
        [ErrorArea(ErrorArea.Reference)]
        PassDtoObjectNotEntity = 10,
        [ErrorArea(ErrorArea.Session)]
        NotLoadSessionGrid = 11,
        [ErrorArea(ErrorArea.Session)]
        NotLoadSessionDetailBySessionNum = 12,
        [ErrorArea(ErrorArea.Session)]
        NotLoadSessionDetailBySessionNumOnDate = 13,
        [ErrorArea(ErrorArea.Session)]
        NotLoadSitePrmForCompanyContact = 14,
        [ErrorArea(ErrorArea.Session)]
        CannotBookAppointmentNotConfiguredToAcceptAppointments = 15,
        [ErrorArea(ErrorArea.Session)]
        CannotBookAppointmentNotAvailableForBookings = 16,
        [ErrorArea(ErrorArea.Session)]
        SessionDateNotSetForDiaryRetrieval = 17,
        [ErrorArea(ErrorArea.Donor)]
        AppointmentDateInPast = 18,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateWithdrawn = 19,
        [ErrorArea(ErrorArea.Donor)]
        NotLoadSessionProcedure = 20,
        [ErrorArea(ErrorArea.Donor)]
        NoFreeSlotAtSession = 21,

        [ErrorArea(ErrorArea.Address)]
        CannotFindAddress = 22,
        [ErrorArea(ErrorArea.Address)]
        CannotRetrieveAddress = 23,
        [ErrorArea(ErrorArea.Address)]
        FailedToCalculateDistance = 24,
        [ErrorArea(ErrorArea.Address)]
        FailedToGetGridReferenceForPostCode = 25,
        [ErrorArea(ErrorArea.Address)]
        FailedToGetGridReferenceForLocationId = 26,
        [ErrorArea(ErrorArea.Address)]
        FailedToGetGridReferenceForPlaceName = 27,

        [ErrorArea(ErrorArea.Reference)]
        ReferenceDataNotFound = 28,

        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateSuspended = 29,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateTooSoon = 30,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateTooOldCatchAll = 31,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateTooYoung = 32,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateTooOld = 33,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateAgeQ1 = 34,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateAgeQ2 = 35,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateClashing = 36,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateNewSecond = 37,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateReturningSecond = 38,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToSessionDemandMet = 39,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToSiteDemandMet = 40,
        [ErrorArea(ErrorArea.Donor)]
        MasterDonorNotFound = 41,
        [ErrorArea(ErrorArea.Donor)]
        RedundantDonorNotFound = 42,
        [ErrorArea(ErrorArea.Donor)]
        DonorMergePairFound = 43,
        [ErrorArea(ErrorArea.Donor)]
        RedundantDonorMerged = 44,
        [ErrorArea(ErrorArea.Donor)]
        MasterRedundantDonorIdMatch = 45,
        [ErrorArea(ErrorArea.Donor)]
        DonorNotFound = 46,
        [ErrorArea(ErrorArea.Donor)]
        UnvalidatedEmailAddressRequired = 47,
        [ErrorArea(ErrorArea.Donor)]
        LocalPanelSearchNoGridRef = 48,
        [ErrorArea(ErrorArea.Donor)]
        InvalidReferenceDataSearchDistance = 49,
        [ErrorArea(ErrorArea.Donor)]
        NoPanelFoundMaxAttemptsExceeded = 50,
        [ErrorArea(ErrorArea.Donor)]
        InvalidDeferralCode = 51,
        [ErrorArea(ErrorArea.Donor)]
        DeferralNotInUse = 52,
        [ErrorArea(ErrorArea.Donor)]
        DeferralNotFound = 53,
        [ErrorArea(ErrorArea.Donor)]
        DeferralAlreadyLapsed = 54,
        [ErrorArea(ErrorArea.Donor)]
        DeferralCannotBeLifted = 55,
        [ErrorArea(ErrorArea.Donor)]
        DuplicateGeneralCommsRequest = 56,
        [ErrorArea(ErrorArea.Donor)]
        DonorCommsMailMergeInvalidTemplate = 57,
        [ErrorArea(ErrorArea.Donor)]
        DonorCommsMailMergeInvalidPath = 58,
        [ErrorArea(ErrorArea.Donor)]
        DonorCommsMailMergeInvalidLetterStore = 59,
        [ErrorArea(ErrorArea.Report)]
        DhcPrintTooEarly = 60,
        [ErrorArea(ErrorArea.Report)]
        DhcSessionDateInvalid = 61,
        [ErrorArea(ErrorArea.Donor)]
        DonorPermanentlyDeferred = 62,
        [ErrorArea(ErrorArea.Donor)]
        DonorPanelDoesNotAllowMailing = 63,
        [ErrorArea(ErrorArea.Donor)]
        DonorCommunicationCodeDoesNotExist = 64,
        [ErrorArea(ErrorArea.Donor)]
        DonorIdCardRejectEnrolled = 65,
        [ErrorArea(ErrorArea.Donor)]
        DonorInvalidBloodGroup = 66,
        [ErrorArea(ErrorArea.Donor)]
        DonorNewIdUsed = 67,
        [ErrorArea(ErrorArea.Donor)]
        ChangeProcedureTooYoung = 68,
        [ErrorArea(ErrorArea.Donor)]
        ChangeProcedureTooOld = 69,
        [ErrorArea(ErrorArea.Donor)]
        ChangeProcedureTooOldQ1 = 70,
        [ErrorArea(ErrorArea.Donor)]
        ChangeProcedureTooOldQ2 = 71,
        [ErrorArea(ErrorArea.Donor)]
        ChangePanelTypeTooYoung = 72,
        [ErrorArea(ErrorArea.Donor)]
        ChangePanelTypeTooOld = 73,
        [ErrorArea(ErrorArea.Donor)]
        ChangePanelTypeTooOldQ1 = 74,
        [ErrorArea(ErrorArea.Donor)]
        ChangePanelTypeTooOldQ2 = 75,
        [ErrorArea(ErrorArea.Report)]
        DhcDetailsInvalid = 76,
        [ErrorArea(ErrorArea.Report)]
        DhcPermissionToPrintFromPuld06 = 77,
        [ErrorArea(ErrorArea.Donor)]
        DonorNoActiveDeferrals = 78,
        [ErrorArea(ErrorArea.Donor)]
        DonorMoreThanOneActiveDeferrals = 79,
        [ErrorArea(ErrorArea.Donor)]
        DonorDeferralCannotBeOverriden = 80,
        [ErrorArea(ErrorArea.Donor)]
        DonorNoDONGMFILFound = 81,
        [ErrorArea(ErrorArea.Donor)]
        DonorNoDONGCOMFound = 82,
        [ErrorArea(ErrorArea.Stock)]
        UnitNumberNotFound = 83,
        [ErrorArea(ErrorArea.Donor)]
        DiscretionaryTestingIncidentAlreadyExists = 84,
        [ErrorArea(ErrorArea.Donor)]
        DiscretionaryTestingIncidentTypeAlreadyExists = 85,
        [ErrorArea(ErrorArea.Donor)]
        DiscretionaryTestingNotFound = 86,
        [ErrorArea(ErrorArea.Donor)]
        DiscretionaryTestNoRuleHoldCodeMissing = 87,
        [ErrorArea(ErrorArea.Stock)]
        FailedToGetNextMovementNumber = 88,
        [ErrorArea(ErrorArea.Stock)]
        HoldComponentsNotFound = 89,
        [ErrorArea(ErrorArea.Stock)]
        BatchProductDetailsNotFound = 90,
        [ErrorArea(ErrorArea.Stock)]
        InvalidHoldLocation = 91,
        [ErrorArea(ErrorArea.Stock)]
        StockTotalUpdateFailed = 92,
        [ErrorArea(ErrorArea.Stock)]
        HoldAlreadyExistsDonation = 93,
        [ErrorArea(ErrorArea.Stock)]
        HoldAlreadyExistsBatch = 94,
        [ErrorArea(ErrorArea.Stock)]
        ClearHoldFailedHoldDoesNotExist = 95,
        [ErrorArea(ErrorArea.Stock)]
        ClearHoldFailedIntendedFate = 96,
        [ErrorArea(ErrorArea.Stock)]
        ClearHoldFailedBlockPackDoesNotExist = 97,
        [ErrorArea(ErrorArea.Stock)]
        InvalidHoldCode = 98,
        [ErrorArea(ErrorArea.Stock)]
        InvalidProductCode = 99,
        [ErrorArea(ErrorArea.Stock)]
        InvalidProductDefaultLocation = 100,
        [ErrorArea(ErrorArea.Stock)]
        BatchOfBatchTooDeep = 101,
        [ErrorArea(ErrorArea.Stock)]
        ClearComponentHoldBatchPackDataInvalid = 102,
        [ErrorArea(ErrorArea.Donor)]
        DiscretionaryTestNoDonorId = 103,
        [ErrorArea(ErrorArea.Stock)]
        ClearHoldComponentsNotFound = 104,
        [ErrorArea(ErrorArea.Stock)]
        CreateFateComponentNotFound = 105,
        [ErrorArea(ErrorArea.Stock)] 
        CreateFateBiohazardExists = 106,
        [ErrorArea(ErrorArea.Stock)]
        CreateFateDiscardExists = 107,
        [ErrorArea(ErrorArea.Stock)]
        CreateFateInvalidCurrentLocation= 108,
        [ErrorArea(ErrorArea.Stock)]
        CreateFateIsInValidation = 109,
        [ErrorArea(ErrorArea.Stock)]
        CreateDiscardFateHoldRequired = 110,
        [ErrorArea(ErrorArea.Stock)]
        ClearFateBatchHoldDataMissing = 111,
        [ErrorArea(ErrorArea.Stock)]
        ClearFateBatchHoldNotFound = 112,
        [ErrorArea(ErrorArea.Stock)]
        ClearFateBatchHoldIsIndirect = 113,
        [ErrorArea(ErrorArea.Stock)]
        ClearFateComponentFateNotFound = 114,
        [ErrorArea(ErrorArea.Stock)]
        ClearFateInvalidUnitNumber = 115,
        [ErrorArea(ErrorArea.Stock)]
        ClearHoldBatchIntendedFateBiohazard = 116,
        [ErrorArea(ErrorArea.Stock)]
        ClearHoldBatchIndirectInvalid = 117,
        [ErrorArea(ErrorArea.Stock)] 
        PreValidationFailure = 118,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationRecordTypeInvalid = 119, 
        [ErrorArea(ErrorArea.Stock)]
        PreValidationDonorIdBlank = 120,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationTmFlagThreePos = 121,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationTmFlagFourNotNeg = 122,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationBledDateInFuture = 123,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationSampleTubesAreEmpty = 124,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationProductNotInUse = 125,
        [ErrorArea(ErrorArea.Stock)] 
        PreValidationNotAComponentProduct = 126,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationNotFitForIssue = 127,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationPackVolumeNotNormal = 128,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationProductHasExpired = 129,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationMicrobiologyPositive = 130,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationTestNotComplete = 131,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationInvalidBloodGroup = 132,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationTestingNotSetUp = 133,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationProductNotReadyForThisProcess = 134,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationSeriousBloodCharResult = 135,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationSeriousBloodCharResultOverRideTest = 136,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationDiscardHoldFailure = 137,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationPlasmaHoldFailure = 138,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationComponentNotFound = 139,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationRetrievalDateNotFound = 140,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationSampleMandatoryChecksFailed = 141,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationNoSampleCouldBeFound = 142,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationUnClearedHold = 143,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationSetToDiscard = 144,
        [ErrorArea(ErrorArea.Donor)]
        DiscretionaryTestingCannotBeUpdated = 145,
        [ErrorArea(ErrorArea.Donor)]
        DiscretionaryTestingCannotBeDeleted = 146,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationNoTissueProduct = 147,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationNoTissueProductInAudit = 148,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationTissueMicrobiologyPositive = 149,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationTissueMicrobiologyIncomplete = 150,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationTissueMicrobiologyNotReceived = 151,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationPlasmaOnly = 152,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationDiscretionaryTestingDiscard = 153,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationDiscretionaryCheckError = 154,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationStatusIncorrect = 155,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationNoMinimumTestRules = 156,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationNoDonorDetails = 157,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationOverRideTestNoResult = 158,
        [ErrorArea(ErrorArea.Stock)] 
        PreValidationHoldFailure = 159,
        [ErrorArea(ErrorArea.Reference)]
        ProductBarcodeInvalid = 160,
        [ErrorArea(ErrorArea.Reference)] 
        ProductNotFoundForBarcode = 161,
        [ErrorArea(ErrorArea.Stock)]
        MissingSelectionProfile = 162,
        [ErrorArea(ErrorArea.Reference)]
        TareWeightEmailAddressNotFound = 163,
        [ErrorArea(ErrorArea.Reference)]
        ProductDetailsNotFound = 164,
        [ErrorArea(ErrorArea.Stock)]
        ComponentDetailsNotFound = 165,
        [ErrorArea(ErrorArea.Stock)]
        BatchLocationDetailsNotFound = 166,
        [ErrorArea(ErrorArea.Laboratory)]
        ExtraTestingNoPrivilege = 167,
        [ErrorArea(ErrorArea.Utility)]
        VmsBatchJobSubmissionFailed = 168,
        [ErrorArea(ErrorArea.Laboratory)]
        ReEvaluateMultipleProducts = 169,
        [ErrorArea(ErrorArea.Laboratory)]
        ReEvaluateTestVsReference = 170,
        [ErrorArea(ErrorArea.Laboratory)]
        ReEvaluateNoAction = 171,
        [ErrorArea(ErrorArea.Laboratory)]
        ReEvaluateScreenVsReference = 172,
        [ErrorArea(ErrorArea.Donor)]
        DonationMissingDonorId = 173,
        [ErrorArea(ErrorArea.Stock)]
        PreValidationDonorDetailsMissing = 174,
        [ErrorArea(ErrorArea.Outcome)]
        OutcomeSessionNotFound = 175,
        [ErrorArea(ErrorArea.User)]
        UserNotFound = 176,
        [ErrorArea(ErrorArea.Stock)]
        DonationNotFound = 177,
        [ErrorArea(ErrorArea.Stock)]
        InvalidProductionDate = 178,
        [ErrorArea(ErrorArea.Stock)]
        InvalidProductLife = 179,
        [ErrorArea(ErrorArea.Stock)]
        InvalidProductTimescale = 180,
        [ErrorArea(ErrorArea.Donor)]
        OutcomeInvalidResultCode = 181,
        [ErrorArea(ErrorArea.Stock)]
        DiscardFailedBiohazardFateExists = 182,
        [ErrorArea(ErrorArea.Stock)]
        DiscardFailedBiohazardFateMustExist = 183,
        [ErrorArea(ErrorArea.Stock)]
        DiscardFailedHoldMustExist = 184,
        [ErrorArea(ErrorArea.Stock)] 
        DiscardFailedAlreadyDiscarded = 185,
        [ErrorArea(ErrorArea.Stock)]
        DiscardFailedNotAtCorrectSite = 186,
        [ErrorArea(ErrorArea.Print)]
        PrinterStatusError = 187,
        [ErrorArea(ErrorArea.Print)]
        UnableToGetResponseFromPrinter = 188,
        [ErrorArea(ErrorArea.Print)]
        PrinterConnectionTimeout = 189,
        [ErrorArea(ErrorArea.Print)]
        PrinterOperationCancelled = 190,
        [ErrorArea(ErrorArea.Print)]
        PrinterUsbConnectionError = 191,
        [ErrorArea(ErrorArea.Print)]
        PrinterSettingsError = 192,
        [ErrorArea(ErrorArea.Print)]
        PrinterLanguageUnknown = 193,
        [ErrorArea(ErrorArea.Print)]
        GeneralPrintingError = 194,
        [ErrorArea(ErrorArea.Stock)]
        BatchNotFound = 195,
        [ErrorArea(ErrorArea.Stock)]
        BatchComponentNotFound = 196,
        [ErrorArea(ErrorArea.Stock)]
        DiscardFailedDiscardReasonForTissue = 197,
        [ErrorArea(ErrorArea.Stock)]
        DiscardFailedBatchInvalidLocation = 198,
        [ErrorArea(ErrorArea.Stock)]
        DiscardModifyLocationFailedInvalidLocation = 199,
        [ErrorArea(ErrorArea.PULS23)]
        BiohazardDiscardRequired = 200,
        [ErrorArea(ErrorArea.Donor)]
        DonationNoIdNoneUkPlasma = 201,
        [ErrorArea(ErrorArea.Print)]
        UnableToRetrieveTemplateSource = 202,
        [ErrorArea(ErrorArea.Order)]
        RequestDonationDetailsNotFound = 203,
        [ErrorArea(ErrorArea.Order)]
        RequestComponentDetailsNotFound = 204,
        [ErrorArea(ErrorArea.Order)]
        RequestDetailsNotFound = 205,        
        [ErrorArea(ErrorArea.Order)]
        RequestDonorNumberNotFoundForDonation = 206,        
        [ErrorArea(ErrorArea.Order)]
        RequestDonorDetailsNotFound = 207,
        [ErrorArea(ErrorArea.Order)]
        RequestProductDetailsNotFound = 208,
        [ErrorArea(ErrorArea.Laboratory)]
        NoSamplingMachineDetailFound = 209,
        [ErrorArea(ErrorArea.Print)]
        LabelOdlFileLineMismatch = 210,
        [ErrorArea(ErrorArea.Laboratory)]
        DuplicateProfileId = 211,
        [ErrorArea(ErrorArea.Print)]
        UnableToRetrievePrinterSetting = 212,
        [ErrorArea(ErrorArea.Print)]
        PrinterInUseByAnotherApplication = 213,
        [ErrorArea(ErrorArea.Stock)]
        DiscardModifyLocationNoDiscard = 214,
        [ErrorArea(ErrorArea.Laboratory)]
        NoIfSpecSectionDHeaderRecord = 215,
        [ErrorArea(ErrorArea.Laboratory)]
        TrackingBatchIsClosed = 216,
        [ErrorArea(ErrorArea.Laboratory)]
        InvalidTrackingBatch = 217,
        [ErrorArea(ErrorArea.Laboratory)]
        TrackingBatchLifeExpired = 218,
        [ErrorArea(ErrorArea.Laboratory)]
        TrackingBatchMaxNumberLimit = 219, 
        [ErrorArea(ErrorArea.Laboratory)] 
        AllDonationsExcludedByTrackingBatchExclusion = 220,
        [ErrorArea(ErrorArea.Utility)]
        RenderLabelRequestFailed = 221,
        [ErrorArea(ErrorArea.Report)]
        InsertNotFoundUpdate = 222,
        [ErrorArea(ErrorArea.Report)]
        InsertNotFoundDelete = 223,
        [ErrorArea(ErrorArea.Report)]
        TemplateNotFoundDelete = 224,
        [ErrorArea(ErrorArea.Report)]
        TemplateNotFoundUpdate = 225,
        [ErrorArea(ErrorArea.Report)]
        NoReportTemplatesFound = 226,
        [ErrorArea(ErrorArea.Report)]
        ReportTemplateNotFound = 227,
        [ErrorArea(ErrorArea.Report)]
        ReportDataSourceNotFound = 228,
        [ErrorArea(ErrorArea.Laboratory)]
        SiteCodeNotFound = 229,
        [ErrorArea(ErrorArea.Donor)]
        InvalidSubPanel = 230,
        [ErrorArea(ErrorArea.Donor)]
        NotAppointmentSubPanel = 231,
        [ErrorArea(ErrorArea.Balance)]
        BalanceConnectionError = 232,
        [ErrorArea(ErrorArea.Balance)]
        BalanceDataError = 233,
        [ErrorArea(ErrorArea.Balance)]
        BalanceUnstable = 234,
        [ErrorArea(ErrorArea.Balance)]
        BalanceNotConnected = 235,
        [ErrorArea(ErrorArea.Balance)]
        BalancePolarityError = 236,
        [ErrorArea(ErrorArea.Balance)]
        BalanceTareError = 237,
        [ErrorArea(ErrorArea.Stock)]
        ComponentLabellingGenericProductsNotFound = 238,
        [ErrorArea(ErrorArea.Stock)]
        ComponentLabellingProductTypeInvalid = 239,
        [ErrorArea(ErrorArea.Stock)]
        ComponentLabellingComponentNotFound = 240,
        [ErrorArea(ErrorArea.Stock)]
        ComponentLabellingComponentWrongSite = 241,
        [ErrorArea(ErrorArea.Stock)]
        ComponentLabellingComponentInvalidStatus = 242,
        [ErrorArea(ErrorArea.Stock)]
        UnitLoadByDataFileDataMismatch = 243,
        [ErrorArea(ErrorArea.Print)]
        InvalidBarcodeFormat = 244,
        [ErrorArea(ErrorArea.Balance)]
        BalanceSocketError = 245,
        [ErrorArea(ErrorArea.Report)]
        OnlyNHSBTCanGenerateReport = 246,
        [ErrorArea(ErrorArea.Balance)]
        BalanceTimeoutError = 247,
        [ErrorArea(ErrorArea.Stock)]
        DiscardModifyLocationWrongSite = 248,
        [ErrorArea(ErrorArea.Session)]
        CannotSetSubPanelToSelectedState = 249,
        [ErrorArea(ErrorArea.Session)]
        SessionNotFound = 250,
        [ErrorArea(ErrorArea.Session)]
        SubPanelNotFound = 251,
        [ErrorArea(ErrorArea.Session)]
        CannotSetSessionToSelectedState = 252,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeInvalidProduct = 253,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeNoBatchedProductDetails= 254,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeInvalidSatelliteProduct = 255,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeInvalidConstitientProduct = 256,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeType1NotBloodGrouped = 257,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeSatelliteProductNotInUse = 258,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeSatelliteProductNotABatchedProduct = 259,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeSatelliteProductNotDefined = 260,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeProductCannotBeCreated = 261,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeProductIsSatellite = 262,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeNoBatchedProductDetailsConstituents = 263,
        [ErrorArea(ErrorArea.Stock)]
        ComponentBatchingNoPoolnumRow = 264,
        [ErrorArea(ErrorArea.Stock)]
        ComponentBatchingUnableToGeneratePoolNumber = 265,
        [ErrorArea(ErrorArea.Stock)]
        ComponentBatchingUnableToGenerateBatchCode = 266,
        [ErrorArea(ErrorArea.Stock)]
        ComponentBatchingType2BBatchCheckFailed = 267,
        [ErrorArea(ErrorArea.Stock)]
        BatchDonationAuditError = 268,
        [ErrorArea(ErrorArea.Stock)]
        BatchComponentsExist = 269,
        [ErrorArea(ErrorArea.Stock)]
        NoTissueExpiryLifeDetailsFound = 270,
        [ErrorArea(ErrorArea.Stock)]
        InvalidTissueUnitsInProductCodeLifeOrProductTypeLife = 271,
        [ErrorArea(ErrorArea.Stock)]
        InvalidTissueProductLifeDefined = 272,
        [ErrorArea(ErrorArea.Address)]
        FailedToCalculateDistanceGridReferencesNotAvailable = 273,
        [ErrorArea(ErrorArea.Stock)]
        BatchCannotOpenToExternal = 274,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotAddPrimaryProductNotExist = 275,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchNotAtUserSite = 276,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchNotOpen = 277,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchDiscard = 278,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddType1BatchOnHold = 279,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddType1BatchOnHoldIndirect = 280,
        [ErrorArea(ErrorArea.PULQ40)]
        FailedToFindTrackingBatchReferenceData = 281,
        [ErrorArea(ErrorArea.Reference)]
        ReferenceWeightsNotFound = 282,
        [ErrorArea(ErrorArea.Reference)]
        ReferenceWeightsInvalidForBatching = 283,
        [ErrorArea(ErrorArea.Reference)]
        ReferenceWeightInvalidForBatching = 284,
        [ErrorArea(ErrorArea.Stock)]
        BatchComponentsAlreadyClosed = 285,
        [ErrorArea(ErrorArea.Stock)]
        BatchRecipeNotFound = 286,
        [ErrorArea(ErrorArea.Stock)]
        IncorrectAmountOfConstituent = 287,
        [ErrorArea(ErrorArea.Stock)]
        BatchContainsTooManyProducts = 288,
        [ErrorArea(ErrorArea.Stock)]
        BatchContainsTooFewProducts = 289,
        [ErrorArea(ErrorArea.Stock)]
        MainBatchProductExpired = 290,
        [ErrorArea(ErrorArea.Stock)]
        SatelliteBatchProductExpired = 291,
        [ErrorArea(ErrorArea.Stock)]
        NoBatchExpiryDueToMissingAudits = 292,
        [ErrorArea(ErrorArea.Stock)]
        InvalidTissueUnits = 293,
        [ErrorArea(ErrorArea.Stock)]
        BatchExpiryNoConstituents = 294,
        [ErrorArea(ErrorArea.Stock)]
        InvalidConstituentProductTimeScale = 295,
        [ErrorArea(ErrorArea.Stock)]
        ConstituentBatchProductDoesNotExist = 296,
        [ErrorArea(ErrorArea.Stock)]
        InvalidExpiryForConstituentBatch = 297,
        [ErrorArea(ErrorArea.Stock)]
        ConstituentProductInvalidExpiryTime = 298,
        [ErrorArea(ErrorArea.Stock)]
        BatchConstituentStructureError = 299,
        [ErrorArea(ErrorArea.Stock)]
        NoComponentsForBatchedBatch = 300,
        [ErrorArea(ErrorArea.Stock)]
        BledDateOfBatchedComponentNotFound = 301,
        [ErrorArea(ErrorArea.Stock)]
        NoPackDetailsForBatchedComponent = 302,
        [ErrorArea(ErrorArea.Stock)]
        InvalidConstituentTypeFlag = 303,
        [ErrorArea(ErrorArea.Stock)]
        BatchNoComponents = 304,
        [ErrorArea(ErrorArea.Stock)]
        NoPackInformationForExpiryDate = 305,
        [ErrorArea(ErrorArea.Stock)]
        MissingPackExpiryDate = 306,
        [ErrorArea(ErrorArea.Stock)]
        NoProductionDateForPack = 307,
        [ErrorArea(ErrorArea.Stock)]
        ConstituentProductDoesNotExist = 308,
        [ErrorArea(ErrorArea.Stock)]
        InvalidExpiryForConstituentProduct = 309,
        [ErrorArea(ErrorArea.Stock)]
        ConstituentDonationDoesNotExist = 310,
        [ErrorArea(ErrorArea.Stock)]
        InvalidBatchProductTimescale = 311,
        [ErrorArea(ErrorArea.Stock)]
        MissingBatchOpenAudit = 312,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddExpiryDateMismatch = 313,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentExpired = 314,  
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentStatusInvalid = 315,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentBloodGroupInvalid = 316,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentOnHold = 317,
        [ErrorArea(ErrorArea.Address)]
        FailedToGetGridReferenceForInput = 318,
        [ErrorArea(ErrorArea.Stock)]
        NoLocationForBatch = 319,
        [ErrorArea(ErrorArea.Stock)]
        BatchOpenRemoveType1Invalid = 320,
        [ErrorArea(ErrorArea.Stock)]
        BatchOpenRemoveType3NotOpen = 321,
        [ErrorArea(ErrorArea.Stock)]
        BatchOpenRemoveInvalidStatus = 322,
        [ErrorArea(ErrorArea.Stock)]
        BatchOpenRemoveBlankingLabels = 323,
        [ErrorArea(ErrorArea.Stock)]
        BatchOpenRemoveDirectHolds = 324,
        [ErrorArea(ErrorArea.Stock)] 
        BatchLockedByUser = 325,
        [ErrorArea(ErrorArea.Stock)] 
        UnableToOpenNonTissueBatch = 326,
        [ErrorArea(ErrorArea.Stock)]
        BatchIsSplit = 327,
        [ErrorArea(ErrorArea.Stock)]
        InvalidBatchState = 328,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductHasIncident = 329,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchedProductHasIncident = 330,
        [ErrorArea(ErrorArea.Stock)]
        BatchUniqueMovementNumberCouldNotBeCreated = 331,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonationNumberNotUnique = 332,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddComponentDetailsNotFound = 333,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotAddUnitImportedWithoutDatafile = 334,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotAddNormalLifePackToShortLifeBatch = 335,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotAddShortLifePackToNormalLifeBatch = 336,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductAlreadyAddedToBatch = 337,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductAlreadyBatched = 338,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductExpired = 339,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddComponentAllocatedToSpecialistStockOrder = 340,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonorDetailsNotFound = 341,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonationIsWrongRecordType = 342,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPackSetToPlasmaOnly = 343,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddNoWeightRecordedForPack = 344,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductWillCauseBatchToExpire = 345,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddUnableToDetermineOriginalNonUkProduct = 346,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddImportDetailsNotFoundForNonUkProduct = 347,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddInvalidClinicalUseFlag = 348,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddErrorFoundDuringProductImport = 349,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddNonImportPackHasNoLinkedDonor = 350,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPreValidationPacksCannotBeBatchedAtCentre = 351,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPreValidationPacksCannotBeAddedToBatch = 352,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPreValidationPacksFromNewDonorCannotBeBatched = 353,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddTooManyInProcessProductsExist = 354,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPackStatusInvalidForBatching = 355,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddSamplingNotCompletedForUnit = 356,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductInUnfinishedValidationSession = 357,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonorMicrobiologyFlagsPositive = 358,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonationMicrobiologyFlagsPositive = 359,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonationHasBiohazardFate = 360,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonationHasDiscardFate = 361,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddAllPacksForDonationOnHold = 362,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPackOnHold = 363,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPackStatusHeldButNoHoldsFound = 364,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddNewIntendedFateFound = 365,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchedProductNotFound = 366,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonorHasNoAboOrRhResult = 367,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonationHasNoAboOrRh = 368,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddWrongDonationBloodGroup = 369,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonationHasWrongRhesusFactor = 370,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchHasNoRhesusValue = 371,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductDoesNotExistAtSite = 372,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPackIsInTransit = 373,
        [ErrorArea(ErrorArea.PULQ52)]
        SessionHasNoManufacturingSite = 374,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotBatchAutologousDonations = 375,
        [ErrorArea(ErrorArea.Stock)]
        PackLotNotRegistered = 376,
        [ErrorArea(ErrorArea.Stock)]
        UnitMissingProductHistoryAuditInformation = 377,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddUnitFailedMinimumTesting = 378,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddTissueDonationRecordNotFound = 379,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddTissueValidationFailed = 380,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotAddLivingDonationToDeceasedDonorBatch = 381,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotAddDeceasedDonationToLivingDonorBatch = 382,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchHasDonationsFromAnotherDonor = 383,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddDonorIdsDoNotMatch = 384,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotAddDirectedDonationsToNonDirectedBatch = 385,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotEvaluateDonationProductsAlreadyInBatch = 386,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddCannotEvaluateDonationProduct = 387,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddUnitFailedTwoYearDonationChecks = 388,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentUnitFailsTwoYearDonationChecks = 389,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddWouldCauseTooManyOfProductInBatch = 390,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddWouldCauseTooManyProductsInBatch = 391,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductRequiresPrimaryProductCheck = 392,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPrimaryProductNotFromSameUnitAsSecondaryProduct = 393,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentProductRequiresBaseCheckNotAllProductsAdded = 394,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentProductFailedBaseCheck = 395,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchOfTypeOnHold = 396,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchOfTypeHasDirectHold = 397,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchOfTypeHasIndirectHold = 398,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchBatchedIntoItself = 399,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentBatchExpired = 400,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddMainAndConstituentBatchBloodGroupsDoNotMatch = 401,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddMainAndConstituentBatchRhesusDoNotMatch = 402,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddOnlyValidatedBatchesCanBeAdded = 403,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPreValidationBatchesCannotBeAdded = 404,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentBatchIsInTransit = 405,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentBatchHasDiscardFate = 406,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentBatchOnHold = 407,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddConstituentBatchOnHoldViaSim = 408,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddProductWillCauseSatelliteBatchToExpire = 409,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddNoProductsFoundAtSiteOrLocation = 410,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveConstituentDonationProductNotInBatch = 411,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveConstituentDonationProductNoLongerExists = 412,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveProductNotBatched = 413,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveProductNotIssuedToBatch = 414,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveProductNotAtSite = 415,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveCannotRemovePrimaryUntilSecondaryRemoved = 416,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveConstituentBatchDetailsNoLongerExist = 417,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveNoProductsFoundAtSiteLocation = 418,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveProductNotPartOfMainBatch = 419,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveConstituentDonorDetailsNoLongerExist = 420,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveMainBatchDetailsNoLongerExist = 421,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveNoCreationDateFound = 422,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryProductDefinitionNotFound = 423,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryNoConstituentsFoundType4 = 424,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryBatchProductDetailsNotFound = 425,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryBatchHasNoComponents = 426,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryInvalidDonatedOnDates = 427,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryFutureDonatedOnDates = 428,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryMissingBadProductData = 429,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryInvalidProductionDates = 430,
        [ErrorArea(ErrorArea.Stock)]
        CalculateBatchExpiryFutureProductionDates = 431,
        [ErrorArea(ErrorArea.Stock)]
        LocationDetailsNotFound = 432,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveLocationTypeInvalid = 433,
        [ErrorArea(ErrorArea.Stock)]
        BatchNoLongerExistsAtSite = 434,
        [ErrorArea(ErrorArea.Stock)]
        BatchNoLongerExistsAtSiteAndLocation = 435,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchReopened = 436,
        [ErrorArea(ErrorArea.Stock)]
        NoPackVolume = 437,
        [ErrorArea(ErrorArea.Stock)]
        InvalidContainerWeight = 438,
        [ErrorArea(ErrorArea.Stock)]
        MainBatchContainerWeightInvalid = 439,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveConstituentBatchProductNotInBatch = 440,
        [ErrorArea(ErrorArea.Stock)]
        BatchRemoveConstituentBatchProductNoLongerExists = 441,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddPackStatusNotHeldWhenHoldsFound = 442,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeProductNotAssignedToSupergroup = 443,
        [ErrorArea(ErrorArea.Reference)]
        BatchRecipeSatelliteProductTimescaleIncompatible = 444,
        [ErrorArea(ErrorArea.Stock)]
        BatchRelabelBatchNotTissueOrInternalCommercial = 445,
        [ErrorArea(ErrorArea.Stock)]
        BatchRelabelBatchNotValidClosedCommercial = 446,
        [ErrorArea(ErrorArea.Stock)]
        BatchRelabelBatchExpiryEarlierThanRecorded = 447,
        [ErrorArea(ErrorArea.Stock)] 
        BatchCloseDonationHasDiscardFate = 448,
        [ErrorArea(ErrorArea.Stock)]
        BatchCloseDonationOnHold = 449,
        [ErrorArea(ErrorArea.Stock)]
        BatchCloseBatchHasDiscardFate = 450,
        [ErrorArea(ErrorArea.Stock)]
        BatchCloseBatchOnHold = 451,
        [ErrorArea(ErrorArea.Stock)]
        BatchCloseBatchClosedByAnotherUser = 452,
        [ErrorArea(ErrorArea.Stock)]
        BatchReopenBatchAlreadyOpen = 453,
        [ErrorArea(ErrorArea.Stock)]
        BatchReopenBatchDirectHold = 454,
        [ErrorArea(ErrorArea.Stock)]
        BatchCloseInvalidLocation = 455,
        [ErrorArea(ErrorArea.Stock)]
        BatchAddBatchBloodGroupInvalid = 456,
        [ErrorArea(ErrorArea.Session)]
        DssrEmailNotDefined = 457,
        [ErrorArea(ErrorArea.Session)]
        BulkAppointmentCancelMissingSessionNo = 458,
        [ErrorArea(ErrorArea.Session)]
        BulkAppointmentCancelMissingSessionDate = 459,
        [ErrorArea(ErrorArea.Session)]
        BulkAppointmentCancelNoAppointments = 460,
        [ErrorArea(ErrorArea.Stock)]
        BatchAlreadyDeleted = 461,
        [ErrorArea(ErrorArea.Session)]
        SlotOutsidePeriodTimes = 462,
        [ErrorArea(ErrorArea.Session)]
        DeleteSlotWithAppointment = 463,
        [ErrorArea(ErrorArea.Session)]
        SelectSessionNoRow = 464,
        [ErrorArea(ErrorArea.Session)]
        SelectSessionNoPanel = 465,
        [ErrorArea(ErrorArea.Session)]
        SelectSessionNoStatus = 466,
        [ErrorArea(ErrorArea.Session)]
        SelectSessionTriggerDate = 467,
        [ErrorArea(ErrorArea.Session)]
        SelectSessionPastStart = 468,
        [ErrorArea(ErrorArea.Session)]
        SessionDateInPast = 469,
        [ErrorArea(ErrorArea.Session)]
        SessionDoesNotAllowSlots = 470,
        [ErrorArea(ErrorArea.Session)]
        SessionInvalidSlotMachine = 471,
        [ErrorArea(ErrorArea.Session)]
        SessionSlotsNoPeriod = 472,
        [ErrorArea(ErrorArea.Donor)]
        GenderRuleDoesNotExist = 473,
        [ErrorArea(ErrorArea.Laboratory)]
        ReviewsDoesNotExist = 474,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02JobAbortedHeaderInvalid = 475,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02JobAbortedFooterInvalid = 476,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02JobAbortedResultInvalid = 477,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02JobAbortedUnitDefinitionInvalid = 478,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02JobAbortedSampleInvalid = 479,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02InvalidDataItem = 480,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02InvalidHeader = 481,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02InvalidFooter = 482,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02InvalidDataItemWithId = 483,
        [ErrorArea(ErrorArea.Laboratory)]
        Q02InvalidHeaderWithId = 484,
        [ErrorArea(ErrorArea.Laboratory)]
        W02FileParseFailure = 485,
        [ErrorArea(ErrorArea.Laboratory)]
        NumericEvaluationNoRules = 486,
        [ErrorArea(ErrorArea.Laboratory)]
        InvalidNumberOfSamplingStages = 487,
        [ErrorArea(ErrorArea.Laboratory)]
        NoTestingTypeSiteParameterForBacterialScreening = 488,
        [ErrorArea(ErrorArea.Laboratory)]
        NoLabTestingTypeSiteParameterForBacterialScreening = 489,
        [ErrorArea(ErrorArea.Reference)]
        DonorRequirementProfileNotFound = 490,
        [ErrorArea(ErrorArea.Reference)]
        WorkStreamNotFound = 491,
        [ErrorArea(ErrorArea.Laboratory)]
        BacterialNoProductOrUnitNumber = 492,
        [ErrorArea(ErrorArea.Stock)]
        InvalidCleaningTriggerDayOfWeek = 493,
        [ErrorArea(ErrorArea.Stock)]
        SiteYearSequenceNumberLimitExceeded = 494,
        [ErrorArea(ErrorArea.Stock)]
        ProductionBatchUnitAlreadyRecorded = 495,
        [ErrorArea(ErrorArea.Stock)]
        DonationNotRecordedAgainstThisWorkstream = 496,
        [ErrorArea(ErrorArea.Stock)]
        HierarchyCodeNotFound = 497,
        [ErrorArea(ErrorArea.Stock)]
        DonationHierarchyAuditNotFound = 498,
        [ErrorArea(ErrorArea.Stock)]
        ProductionBatchHierarchyIsProductSpecific = 499,
        [ErrorArea(ErrorArea.Stock)]
        UnitHierarchyAuditLatestNotFound = 500,
        [ErrorArea(ErrorArea.Stock)]
        ProductionBatchWorkstreamBatchesNotAllowed = 501,
        [ErrorArea(ErrorArea.Stock)]
        HierarchyRestrictionDoesNotAllowBatches = 502,
        [ErrorArea(ErrorArea.Stock)]
        ProductionBatchNoUnitsCanBeProcessed = 503,
        [ErrorArea(ErrorArea.Stock)]
        UnitWithinAnotherControlledProductionBatch = 504,
        [ErrorArea(ErrorArea.Stock)]
        UnitHasNotBeenManufactured = 505,
        [ErrorArea(ErrorArea.Stock)]
        ProductionBatchUnitStatusesNotFound = 506,
        [ErrorArea(ErrorArea.Stock)]
        ProductionBatchAllProductsAlreadyRecorded = 507,
        [ErrorArea(ErrorArea.Stock)]
        ProcessingCellNotFound = 508,
        [ErrorArea(ErrorArea.Stock)]
        ProcessingCellAlreadyExists = 509,
        [ErrorArea(ErrorArea.Reference)]
        ProductionBatchLinkedInspection = 510,
        [ErrorArea(ErrorArea.Reference)]
        DeleteEquipmentType = 511,
        [ErrorArea(ErrorArea.Reference)]
        ProductionBatchLinkedDeleteEquipmentItem = 512,

        // ComponentValidation
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDifferentSession = 600,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationSameSession = 601,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationUnitNotExistSessionActive = 602,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationUnitNotExist = 603,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationComponentNotExistSessionActive = 604,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationComponentNotExist = 605,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDTFailure = 606,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationNonUKNotFound = 607,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationNonUKNotForClinical = 608,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationNonUKNotLabelled = 609,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDonationNotLinked = 610,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDonorNotFound = 611,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidPackStatus = 612,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidPackSite = 613,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationPackInTransit = 614,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationPackBiohazard = 615,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationPackSingleDiscard = 616,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationPackAllDiscard = 617,        
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationPackOnHold = 618,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDonorTransfusionMicro = 619,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDonationTransfusionMicro = 620,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidRecordType = 621,        
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationNotForFractionator = 622,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationImportedAboRhBlank = 623,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDonationAboRhMismatch = 624,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationMinTestFailure = 625,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationOrderNotFound = 626,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationOrderNotCreated = 627,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationOrderNoPatient = 628,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDirectedDonationFailTests = 629,
        [ErrorArea((ErrorArea.Stock))]
        ComponentValidationUnitNotRegistered = 630,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidExpiryDateOrTime = 631,
        [ErrorArea(ErrorArea.Stock)]
        TissueExpiryCalculationInvalidStartDate = 632,
        [ErrorArea(ErrorArea.Stock)]
        TissueExpiryCalculationInvalidUnits = 633,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationComponentExpired = 634,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationTissueValidationFailure = 635,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBactiNotComplete = 636,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationDTProductReleaseRuleFail = 637,
        [ErrorArea(ErrorArea.Stock)]
        IgaMapMissing = 638,
        [ErrorArea(ErrorArea.Stock)]
        TissueRetrievalDate = 639,
        [ErrorArea(ErrorArea.Stock)]
        TissueStorageTempNotFound = 640,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationLabelDefinitionNotFound = 641,
        [ErrorArea(ErrorArea.Stock)]
        ProductBarcodeNotFound = 642,
        [ErrorArea(ErrorArea.Stock)]
        ProductBarcodeInvalid128 = 643,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchNoLocation = 644,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchMultipleLocations = 645,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchDiscarded = 646,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchInTransit = 647,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidBatchSite = 648,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidBatchStatus = 649,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidBatchType = 650,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchNoConstituents = 651,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchNoConstituentComponent = 652,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchNoConstituentNotBatched = 653,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchConstituentRecordTypeInvalid = 654,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchConstituentAboMismatch = 655,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchConstituentRhMismatch = 656,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchConstituentAboDonorMismatch = 657,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidTissueStorageTemp = 658,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationStorageTempNotSelected = 659,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationBatchHoldFlagSet = 660,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationNoReprintGroupSpecific = 661,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationNoReprintPrivilegeBlood = 662,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationNoReprintPrivilegeTissue = 663,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationShelfLifeDefaultLocation = 664,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationGeneralApiError = 665,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationSessionNotExist = 666,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationSessionAlreadyActive = 667,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationSessionUserLimit = 668,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationSessionUserLimitCreate = 669,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationSessionUserCreateActiveSession = 670,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationTissueMicroNoPrivilege = 672,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationLateAllocation = 673,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationTissueLocationNotFound = 674,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationTissueExpiryNotFound = 675,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationLabellingCancelledTissueMicro = 676,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationLabellingCancelledIncompleteTesting = 677,
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationClearanceFailedForSingleUnit = 678,        
        [ErrorArea(ErrorArea.Stock)]
        ComponentValidationInvalidLocationCode = 679,
        [ErrorArea(ErrorArea.Stock)]
        ValidationFailed = 680,
        [ErrorArea(ErrorArea.Stock)]
        InvalidProductForValidationInS28 = 681,
        [ErrorArea(ErrorArea.Utility)]
        AlarmpointServiceFailure = 682,

        [ErrorArea(ErrorArea.Laboratory)]
        InvalidBloodCharUpdateStatus = 683,

        // PULS28 Component Verification
        // Product
        [ErrorArea(ErrorArea.Stock)]
        ProductNotInUse = 700,
        [ErrorArea(ErrorArea.Stock)]
        ProductEnteredIsNotADonationProduct = 701,
        [ErrorArea(ErrorArea.Stock)]
        ProductEnteredIsNotABatchedProduct = 702,
        [ErrorArea(ErrorArea.Stock)]
        ProductMarkedAsNotFitForIssue = 703,
        [ErrorArea(ErrorArea.Stock)]
        ProductIsNotAllowedToBeStoredInSubLocations = 704,
        [ErrorArea(ErrorArea.Stock)]
        ValidationSessionCanOnlyProcessProductsThatExistInTheGenericProductSuperGroup = 705,
        [ErrorArea(ErrorArea.Stock)]
        ValidationSessionCanOnlyProcessProductsWithTheSameProductCodeAsTheGenericProductCode = 706,
        [ErrorArea(ErrorArea.Stock)]
        NonBloodGroupedBatchesCannotBeValidated = 707,
        [ErrorArea(ErrorArea.Stock)]
        InvalidProductTypeForTissueBatch = 708,
        // Below errors are handled by General Validation and Verification.
        // [ErrorArea(ErrorArea.Stock)]
        // ExternalProductsCannotBeValidated = 709,
        // [ErrorArea(ErrorArea.Stock)]
        // InvalidBatchTypeForValidationOnlyBatchTypes1And3CanBeValidated = 710,
        // [ErrorArea(ErrorArea.Stock)]
        // BatchedProductDetailsCannotBeFoundValidationAborted = 711
        // Concatenation
        [ErrorArea(ErrorArea.Stock)]
        ConcatenationWrongPack = 720,               // Scanned in Wrong Donation
        [ErrorArea(ErrorArea.Stock)]
        ConcatenationMismatchAndWrongPack = 721,    // Scanned in Wrong Donation with LHS RHS Mismatch
        [ErrorArea(ErrorArea.Stock)]
        ConcatenationMismatch = 722,                // LHS RHS Mismatch
        // Data Matrix Concatenation Field Errors.
        // [Data Structure 001]
        [ErrorArea(ErrorArea.Stock)]
        UnitNumberMismatch = 723, 
        // [Data Structure 002]
        [ErrorArea(ErrorArea.Stock)]
        BloodGroupMismatch = 724, 
        // [Data Structure 003]
        [ErrorArea(ErrorArea.Stock)]
        ProductCodeMismatch = 725,
        // [Data Structure 004]
        [ErrorArea(ErrorArea.Stock)]
        ExpirationDateMismatch = 726,
        // [Data Structure 005]
        [ErrorArea(ErrorArea.Stock)]
        ExpirationDateTimeMismatch = 727,
        // [Data Structure 006]
        [ErrorArea(ErrorArea.Stock)]
        CollectionDateMismatch = 728,
        // [Data Structure 007]
        [ErrorArea(ErrorArea.Stock)]
        CollectionDateTimeMismatch = 729,
        // [Data Structure 010]
        [ErrorArea(ErrorArea.Stock)]
        SpecialTestingGeneralMismatch = 730,
        // [Data Structure 012]
        [ErrorArea(ErrorArea.Stock)]
        SpecialTestingRedBloodCellAntigensMismatch = 731,
        // [Data Structure 014]
        [ErrorArea(ErrorArea.Stock)]
        SpecialTestingPlateletHLAPlateletSpecificAntigensMismatch = 732,
        // [Data Structure 027]
        [ErrorArea(ErrorArea.Stock)]
        TransfusionTransmittedInfectionMarkerMismatch = 733,
        [ErrorArea(ErrorArea.Stock)]
        InvalidFormatForExpiryDateBarcode = 734,
        [ErrorArea(ErrorArea.Stock)]
        ProductDoesNotPermitHospitalLabels = 735,
        [ErrorArea(ErrorArea.Stock)]
        ProductNotValidatedViaPuls28 = 736,
        [ErrorArea(ErrorArea.Stock)]
        InvalidProductTypeExternal = 737,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateWholeBloodOnly = 738,
        [ErrorArea(ErrorArea.Stock)]
        UnitNumberNotPresent = 739,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateSessionLimitDemandMet = 740,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateDonorTypeLimitNewDemandMet = 741,
        [ErrorArea(ErrorArea.Donor)]
        DonorIneligibleToDonateDonorTypeLimitReturningDemandMet = 742,
        [ErrorArea(ErrorArea.PULQ52)]
        DonorEligibilityNotSet = 743,

        // NFBB Labelling.
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingDonationNotFound = 750,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingDonationHasInvalidRecordType = 751,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingProductFrozenNotFound = 752,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingProductFrozenNotInUse = 753,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingComponentFrozenNotFound = 754,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingComponentFrozenNotImported = 755,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingU06ReferenceDataNotFound = 756,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingU06ReferenceDataHasNoThawedProduct = 757,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingInvalidComponentFrozenStatus = 758,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingComponentFrozenNotAuthorisedForIssue = 759,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingComponentFrozenNotAtUserSite = 760,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingDonationHasUnclearedHolds = 761,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingProductThawedNotFound = 762,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingUnitsDetailsNotFound = 763,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingSiteNotFound = 764,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingComponentFrozenNoLongerExists = 765,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingComponentThawedAlreadyExists = 766,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingProductHasInvalidDensity = 767,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingPackGrossWeightOutsideLimits = 768,
        [ErrorArea(ErrorArea.Stock)]
        NfbbLabellingPackNetWeightInvalid = 769,
    }
}
