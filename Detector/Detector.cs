using System;

namespace JH.Applications
{
    public class Detector
    {
        int blockSize;
        int nBlocks;
        int N;
        int count;
        public double[] leqTotal;
        public double[] rms;
        public double[] input;

        public Detector()
        {
        }

        public void Compute()
        {
            if (rms == null)
            {
                nBlocks = input.Length / blockSize;
                rms = new double[nBlocks];
                leqTotal = new double[1];

            }
            double alpha;
            double rmsLast = rms[nBlocks - 1];
            for (int j = 0; j < nBlocks; j++)
            {
                double sum = 0;
                for (int k = 0; k < blockSize; k++)
                    sum += input[k + j * blockSize] * input[k + j * blockSize];
                sum /= blockSize;
                alpha = 1.0 / (count + 1);
                leqTotal[0] = (1 - alpha) * this.leqTotal[0] + alpha * sum;
                if (count < N)
                    alpha = 1.0 / (count + 1);
                else
                    alpha = 2.0 / N;
                rmsLast = (1 - alpha) * rmsLast + alpha * sum;
                rms[j] = rmsLast;
                count++;
            }
        }

        public void Reset()
        {
            if (leqTotal != null)
                leqTotal[0] = 0;
            count = 0;
        }

        public void Init(int N, int samplingFrequency)
        {
            this.N = N;
            blockSize = samplingFrequency / 1000;

            Reset();
        }


    }


}
