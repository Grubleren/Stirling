using System;

namespace JH.Calculations
{
    public unsafe class FFT
    {
        IppState ippState;

        public FFT()
        {
            ippState.State = IntPtr.Zero;
        }

        public void Init(int order, FFTFactor factor)
        {
            IppWrapper.ippsFFTInitAlloc_C_64fc(ref ippState,order,factor, IppHintAlgorithm.ippAlgHintNone);
        }

        public void Free()
        {
            IppWrapper.ippsFFTFree_C_64fc(ippState);
        }

        public void FftForward(Complex[] data)
        {
            fixed (Complex* pData = data)
            {
                IppWrapper.ippsFFTFwd_CToC_64fc_I(pData, ippState, null);
            }
        }

        public void FftInverse(Complex[] data)
        {
            fixed (Complex* pData = data)
            {
                IppWrapper.ippsFFTInv_CToC_64fc_I(pData, ippState, null);
            }
        }
    }
}
