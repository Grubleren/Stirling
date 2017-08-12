using System;

namespace BK.BasicEnv.Applications
{
    public class WNoise : RTGeneratorBase
    {
        public WNoise()
        {
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
                buf[n] = (short)(20000 * random.RandomGauss());
            }
            blockCounter++;
            activeBuffer = activeBuffer == 0 ? 1 : 0;
        }
    }
}
