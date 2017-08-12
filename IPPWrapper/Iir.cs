using System;

namespace JH.Calculations
{
    public unsafe class IIR
    {
        IppState ippState;

        public IIR()
        {
            ippState.State = IntPtr.Zero;
        }

        public void Init(BiQuad[] biQuads, int noBiQuads)
        {
            double[,] tabVals = new double[biQuads.Length, 6];

            for (int i = 0; i < biQuads.Length; i++)
            {
                tabVals[i, 0] = biQuads[i].b0;
                tabVals[i, 1] = biQuads[i].b1;
                tabVals[i, 2] = biQuads[i].b2;
                tabVals[i, 3] = biQuads[i].a0;
                tabVals[i, 4] = biQuads[i].a1;
                tabVals[i, 5] = biQuads[i].a2;
            }

            fixed (double* pTabVals = tabVals)
            {
                IppWrapper.ippsIIRInitAlloc_BiQuad_64f(ref ippState, pTabVals, noBiQuads, null);
            }
        }

        public void Free()
        {
            IppWrapper.ippsIIRFree_64f(ippState);
        }

        public void Iir(double[] inData, double[] outData)
        {
            fixed (double* pSrc = inData)
            {
                fixed (double* pDest = outData)
                {
                    IppWrapper.ippsIIR_64f(pSrc, pDest, inData.Length, ippState);
                }
            }
        }
    }

    public struct BiQuad
    {
        public BiQuad(double b1, double b2, double a1, double a2)
        {
            this.b0 = 1;
            this.b1 = b1;
            this.b2 = b2;
            this.a0 = 1;
            this.a1 = a1;
            this.a2 = a2;
        }

        public double b0;
        public double b1;
        public double b2;
        public double a0;
        public double a1;
        public double a2;
    }
}
