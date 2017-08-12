using System;

namespace JH.Calculations
{
    public struct IppState
    {
        public IntPtr State;
    }

    public enum FFTFactor
    {
        IPP_FFT_DIV_FWD_BY_N = 1,
        IPP_FFT_DIV_INV_BY_N = 2,
        IPP_FFT_DIV_BY_SQRTN = 4,
        IPP_FFT_NODIV_BY_ANY = 8
    }

    public enum IppHintAlgorithm
    {
        ippAlgHintNone,
        ippAlgHintFast,
        ippAlgHintAccurate
    }

    public enum Status
    {
        StsNotSupportedModeErr = -9999,
        StsCpuNotSupportedErr = -9998,
        StsSizeMatchMatrixErr = -204,
        StsCountMatrixErr = -203,
        StsRoiShiftMatrixErr = -202,
        StsResizeNoOperationErr = -201,
        StsSrcDataErr = -200,
        StsMaxLenHuffCodeErr = -199,
        StsCodeLenTableErr = -198,
        StsFreqTableErr = -197,
        StsIncompleteContextErr = -196,
        StsSingularErr = -195,
        StsSparseErr = -194,
        StsBitOffsetErr = -193,
        StsQPErr = -192,
        StsVLCErr = -191,
        StsRegExpOptionsErr = -190,
        StsRegExpErr = -189,
        StsRegExpMatchLimitErr = -188,
        StsRegExpQuantifierErr = -187,
        StsRegExpGroupingErr = -186,
        StsRegExpBackRefErr = -185,
        StsRegExpChClassErr = -184,
        StsRegExpMetaChErr = -183,
        StsStrideMatrixErr = -182,
        StsCTRSizeErr = -181,
        StsJPEG2KCodeBlockIsNotAttached = -180,
        StsNotPosDefErr = -179,
        StsEphemeralKeyErr = -178,
        StsMessageErr = -177,
        StsShareKeyErr = -176,
        StsIvalidPublicKey = -175,
        StsIvalidPrivateKey = -174,
        StsOutOfECErr = -173,
        StsECCInvalidFlagErr = -172,
        StsMP3FrameHeaderErr = -171,
        StsMP3SideInfoErr = -170,
        StsBlockStepErr = -169,
        StsMBStepErr = -168,
        StsAacPrgNumErr = -167,
        StsAacSectCbErr = -166,
        StsAacSfValErr = -164,
        StsAacCoefValErr = -163,
        StsAacMaxSfbErr = -162,
        StsAacPredSfbErr = -161,
        StsAacPlsDataErr = -160,
        StsAacGainCtrErr = -159,
        StsAacSectErr = -158,
        StsAacTnsNumFiltErr = -157,
        StsAacTnsLenErr = -156,
        StsAacTnsOrderErr = -155,
        StsAacTnsCoefResErr = -154,
        StsAacTnsCoefErr = -153,
        StsAacTnsDirectErr = -152,
        StsAacTnsProfileErr = -151,
        StsAacErr = -150,
        StsAacBitOffsetErr = -149,
        StsAacAdtsSyncWordErr = -148,
        StsAacSmplRateIdxErr = -147,
        StsAacWinLenErr = -146,
        StsAacWinGrpErr = -145,
        StsAacWinSeqErr = -144,
        StsAacComWinErr = -143,
        StsAacStereoMaskErr = -142,
        StsAacChanErr = -141,
        StsAacMonoStereoErr = -140,
        StsAacStereoLayerErr = -139,
        StsAacMonoLayerErr = -138,
        StsAacScalableErr = -137,
        StsAacObjTypeErr = -136,
        StsAacWinShapeErr = -135,
        StsAacPcmModeErr = -134,
        StsVLCUsrTblHeaderErr = -133,
        StsVLCUsrTblUnsupportedFmtErr = -132,
        StsVLCUsrTblEscAlgTypeErr = -131,
        StsVLCUsrTblEscCodeLengthErr = -130,
        StsVLCUsrTblCodeLengthErr = -129,
        StsVLCInternalTblErr = -128,
        StsVLCInputDataErr = -127,
        StsVLCAACEscCodeLengthErr = -126,
        StsNoiseRangeErr = -125,
        StsUnderRunErr = -124,
        StsPaddingErr = -123,
        StsCFBSizeErr = -122,
        StsPaddingSchemeErr = -121,
        StsInvalidCryptoKeyErr = -120,
        StsLengthErr = -119,
        StsBadModulusErr = -118,
        StsLPCCalcErr = -117,
        StsRCCalcErr = -116,
        StsIncorrectLSPErr = -115,
        StsNoRootFoundErr = -114,
        StsJPEG2KBadPassNumber = -113,
        StsJPEG2KDamagedCodeBlock = -112,
        StsH263CBPYCodeErr = -111,
        StsH263MCBPCInterCodeErr = -110,
        StsH263MCBPCIntraCodeErr = -109,
        StsNotEvenStepErr = -108,
        StsHistoNofLevelsErr = -107,
        StsLUTNofLevelsErr = -106,
        StsMP4BitOffsetErr = -105,
        StsMP4QPErr = -104,
        StsMP4BlockIdxErr = -103,
        StsMP4BlockTypeErr = -102,
        StsMP4MVCodeErr = -101,
        StsMP4VLCCodeErr = -100,
        StsMP4DCCodeErr = -99,
        StsMP4FcodeErr = -98,
        StsMP4AlignErr = -97,
        StsMP4TempDiffErr = -96,
        StsMP4BlockSizeErr = -95,
        StsMP4ZeroBABErr = -94,
        StsMP4PredDirErr = -93,
        StsMP4BitsPerPixelErr = -92,
        StsMP4VideoCompModeErr = -91,
        StsMP4LinearModeErr = -90,
        StsH263PredModeErr = -83,
        StsH263BlockStepErr = -82,
        StsH263MBStepErr = -81,
        StsH263FrameWidthErr = -80,
        StsH263FrameHeightErr = -79,
        StsH263ExpandPelsErr = -78,
        StsH263PlaneStepErr = -77,
        StsH263QuantErr = -76,
        StsH263MVCodeErr = -75,
        StsH263VLCCodeErr = -74,
        StsH263DCCodeErr = -73,
        StsH263ZigzagLenErr = -72,
        StsFBankFreqErr = -71,
        StsFBankFlagErr = -70,
        StsFBankErr = -69,
        StsNegOccErr = -67,
        StsCdbkFlagErr = -66,
        StsSVDCnvgErr = -65,
        StsJPEGHuffTableErr = -64,
        StsJPEGDCTRangeErr = -63,
        StsJPEGOutOfBufErr = -62,
        StsDrawTextErr = -61,
        StsChannelOrderErr = -60,
        StsZeroMaskValuesErr = -59,
        StsQuadErr = -58,
        StsRectErr = -57,
        StsCoeffErr = -56,
        StsNoiseValErr = -55,
        StsDitherLevelsErr = -54,
        StsNumChannelsErr = -53,
        StsCOIErr = -52,
        StsDivisorErr = -51,
        StsAlphaTypeErr = -50,
        StsGammaRangeErr = -49,
        StsGrayCoefSumErr = -48,
        StsChannelErr = -47,
        StsToneMagnErr = -46,
        StsToneFreqErr = -45,
        StsTonePhaseErr = -44,
        StsTrnglMagnErr = -43,
        StsTrnglFreqErr = -42,
        StsTrnglPhaseErr = -41,
        StsTrnglAsymErr = -40,
        StsHugeWinErr = -39,
        StsJaehneErr = -38,
        StsStrideErr = -37,
        StsEpsValErr = -36,
        StsWtOffsetErr = -35,
        StsAnchorErr = -34,
        StsMaskSizeErr = -33,
        StsShiftErr = -32,
        StsSampleFactorErr = -31,
        StsSamplePhaseErr = -30,
        StsFIRMRFactorErr = -29,
        StsFIRMRPhaseErr = -28,
        StsRelFreqErr = -27,
        StsFIRLenErr = -26,
        StsIIROrderErr = -25,
        StsDlyLineIndexErr = -24,
        StsResizeFactorErr = -23,
        StsInterpolationErr = -22,
        StsMirrorFlipErr = -21,
        StsMoment00ZeroErr = -20,
        StsThreshNegLevelErr = -19,
        StsThresholdErr = -18,
        StsContextMatchErr = -17,
        StsFftFlagErr = -16,
        StsFftOrderErr = -15,
        StsStepErr = -14,
        StsScaleRangeErr = -13,
        StsDataTypeErr = -12,
        StsOutOfRangeErr = -11,
        StsDivByZeroErr = -10,
        StsMemAllocErr = -9,
        StsNullPtrErr = -8,
        StsRangeErr = -7,
        StsSizeErr = -6,
        StsBadArgErr = -5,
        StsNoMemErr = -4,
        StsSAReservedErr3 = -3,
        StsErr = -2,
        StsSAReservedErr1 = -1,
        StsNoErr = 0,
        StsNoOperation = 1,
        StsMisalignedBuf = 2,
        StsSqrtNegArg = 3,
        StsInvZero = 4,
        StsEvenMedianMaskSize = 5,
        StsDivByZero = 6,
        StsLnZeroArg = 7,
        StsLnNegArg = 8,
        StsNanArg = 9,
        StsJPEGMarker = 10,
        StsResFloor = 11,
        StsOverflow = 12,
        StsLSFLow = 13,
        StsLSFHigh = 14,
        StsLSFLowAndHigh = 15,
        StsZeroOcc = 16,
        StsUnderflow = 17,
        StsSingularity = 18,
        StsDomain = 19,
        StsNonIntelCpu = 20,
        StsCpuMismatch = 21,
        StsNoIppFunctionFound = 22,
        StsDllNotFoundBestUsed = 23,
        StsNoOperationInDll = 24,
        StsInsufficientEntropy = 25,
        StsOvermuchStrings = 26,
        StsOverlongString = 27,
        StsAffineQuadChanged = 28,
        StsWrongIntersectROI = 29,
        StsWrongIntersectQuad = 30,
        StsSmallerCodebook = 31,
        StsSrcSizeLessExpected = 32,
        StsDstSizeLessExpected = 33,
        StsStreamEnd = 34,
        StsDoubleSize = 35,
        StsNotSupportedCpu = 36,
        StsUnknownCacheSize = 37
    }
}