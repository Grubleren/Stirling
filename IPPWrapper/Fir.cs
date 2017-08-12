using System;

namespace JH.Calculations
{
    public unsafe class FIR
    {
        IppState ippState;

        public FIR()
        {
            ippState.State = IntPtr.Zero;
        }

        public void Init(double[] tabsVals, int noTabsVals)
        {
            fixed (double* pTabs = tabsVals)
            {
                IppWrapper.ippsFIRInitAlloc_64f(ref ippState, pTabs, noTabsVals, null);
            }
        }

        public void Free()
        {
            IppWrapper.ippsFIRFree_64f(ippState);
        }

        public void Fir(double[] inData, double[] outData, int length)
        {
            fixed (double* pSrc = inData)
            {
                fixed (double* pDest = outData)
                {
                    IppWrapper.ippsFIR_64f(pSrc, pDest, length, ippState);
                }
            }
        }
    }
}
