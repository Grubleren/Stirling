using System;

namespace BK.BasicEnv.Applications
{
    public class LSweep : RTGeneratorBase
    {
        public LSweep()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            blockStart = 0;
        }

        public override void InitializeTables(double f0, double f1, double T)
        {
            samplingFrequency = 48000;
            bufferSize = 0x00010000;

            int sweepLength = (int)(samplingFrequency * T);
            noBlocksToPlay = (int)Math.Ceiling((double)sweepLength / bufferSize);
            bufferSizeLast = sweepLength % bufferSize;
            if (bufferSizeLast == 0)
                bufferSizeLast = bufferSize;

            double bufferDuration = (double)bufferSize / samplingFrequency;
            startFrequency = f0 * bufferDuration;
            double stopFrequency = f1 * bufferDuration;
            sweepRateFactor = 0.5 * (stopFrequency - startFrequency) / sweepLength;

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
                int n1 = n + blockStart;
                double phi = (sweepRateFactor * n1 + startFrequency) * n1;
                buf[n] = sinTable[(ushort)((uint)phi)];
            }

            blockStart += bufferSize;
            blockCounter++;
            activeBuffer = activeBuffer == 0 ? 1 : 0;
        }

        private int blockStart;
        private double sweepRateFactor;
        private double startFrequency;
        private short[] sinTable;

    }
}
