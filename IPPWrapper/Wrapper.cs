using System;
using System.Runtime.InteropServices;
using System.Security;

namespace JH.Calculations
{
    public static unsafe class IppWrapper
    {
        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsFFTInitAlloc_C_64fc(ref IppState ppFFTSpec, int order, FFTFactor flag, IppHintAlgorithm hint);

        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsFFTFwd_CToC_64fc_I(Complex* pSrcDst, IppState pFFTSpec, byte* pBuffer);

        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsFFTInv_CToC_64fc_I(Complex* pSrcDst, IppState pFFTSpec, byte* pBuffer);

        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsFFTFree_C_64fc(IppState pFFTSpec);


        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsIIRInitAlloc_BiQuad_64f(ref IppState ppIIRState, double* pTaps, int numBq, int* dummy);

        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsIIRFree_64f(IppState pState);

        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsIIR_64f(double* pSrc, double* pDst, int len, IppState pState);

        
        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsFIRInitAlloc_64f(ref IppState pState, double* pTaps, int tapsLen, double* pDlyLine);

        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsFIRFree_64f(IppState pState);
        
        [SuppressUnmanagedCodeSecurityAttribute()]
        [DllImport("ipps-8.0.dll")]
        public static extern Status ippsFIR_64f(double* pSrc, double* pDst, int numIters, IppState pState);

        [DllImport("FFTC.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr Init_Fft_JH(int M);

        [DllImport("FFTC.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Free_Fft_JH(IntPtr state);

        [DllImport("FFTC.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FftFwd_JH(int M, Complex* X, IntPtr state);

        [DllImport("FFTC.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FftInv_JH(int M, Complex* X, IntPtr state);

    }
}
