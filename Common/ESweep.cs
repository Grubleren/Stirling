using System;

namespace BK.BasicEnv.Applications
{
    public class ESweep : RTGeneratorBase
    {
        public ESweep()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            phaseFactor = 1;
        }

        public override void InitializeTables(double f0, double f1, double T)
        {
            samplingFrequency = 48000;
            bufferSize = 0x00010000;

            double omega0 = 2 * Math.PI * f0;
            double omega1 = 2 * Math.PI * f1;

            int sweepLength = (int)(samplingFrequency * T);
            noBlocksToPlay = (int)Math.Ceiling((double)sweepLength / bufferSize);
            bufferSizeLast = sweepLength % bufferSize;
            if (bufferSizeLast == 0)
                bufferSizeLast = bufferSize;

            phiTable = new double[bufferSize];
            double alpha = 1.0 / T * Math.Log(omega1 / omega0);
            double tau = alpha / samplingFrequency;
            blockPhaseFactor = Math.Exp(tau * bufferSize);
            startPhase = omega0 / alpha / (2 * Math.PI) * bufferSize;
            for (int n = 0; n < bufferSize; n++)
                phiTable[n] = startPhase * Math.Exp(tau * n);

            sinTable = new short[bufferSize];
            double sinPhase = 2 * Math.PI / bufferSize;
            for (int n = 0; n < bufferSize; n++)
                sinTable[n] = (short)Math.Round(32765 * Math.Sin(sinPhase * n) + 2 * ((random.RandomDouble() - 0.5) + (random.RandomDouble() - 0.5)));

            buffer = new short[2][];
            buffer[0] = new short[bufferSize];
            buffer[1] = new short[bufferSize];
            Initialize();
        }

        public override void GenerateNextBuffer()
        {
            short[] buf = buffer[activeBuffer];

            for (int n = 0; n < bufferSize; n++)
            {
                double phi = phaseFactor * phiTable[n] - startPhase;
                buf[n] = sinTable[(ushort)((uint)phi)];
            }

            phaseFactor *= blockPhaseFactor;
            blockCounter++;
            activeBuffer = activeBuffer == 0 ? 1 : 0;
        }

        private double phaseFactor;
        private double startPhase;
        private double blockPhaseFactor;
        private double[] phiTable;
        private short[] sinTable;

    }
}
