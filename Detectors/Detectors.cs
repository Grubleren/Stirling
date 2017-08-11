using System;

namespace JH.Applications
{
    public class Detectors : IFunctionBlock
    {
        public DetectorSetup setup;
        private CpbData input;
        private DetectorData output;
        private int noDetectors;
        private int blockSize;
        private int noBlocks;
        private int N;
        private double alpha;

        public Detectors()
        {
            output = new DetectorData();
            N = 250;
        }

        public void Compute()
        {
            int count = 0;
            output.newData = false;
            if (input.newData)
            {
                for (int i = 0; i < input.timeSignal.Length; i++)
                {
                    double rms = output.rmsFast[i][noBlocks - 1];
                    count = output.count;
                    for (int j = 0; j < noBlocks; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < blockSize; k++)
                            sum += input.timeSignal[i][k + j * blockSize] * input.timeSignal[i][k + j * blockSize];
                        sum /= blockSize;
                        alpha = 1.0 / (count + 1);
                        output.leqTotal[i] = (1 - alpha) * output.leqTotal[i] + alpha * sum;
                        if (count < N)
                            alpha = 1.0 / (count + 1);
                        else
                            alpha = 2.0 / N;
                        rms = (1 - alpha) * rms + alpha * sum;
                        output.rmsFast[i][j] = rms;
                        count++;
                    }
                }
                output.count = count;
                output.newData = true;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < noDetectors; i++)
                output.leqTotal[i] = 0;
            output.count = 0;
        }

        public void UpdateSettings()
        {
            noDetectors = input.timeSignal.Length;
            blockSize = input.samplingFrequency / 1000;
            noBlocks = input.timeSignal[0].Length / blockSize;
            output.Allocate(noDetectors, noBlocks);
        }

        public void SetInput(FunctionBlockData measureData)
        {
            input = (CpbData)measureData;
        }

        public FunctionBlockData GetOutput()
        {
            return output;
        }
    }

    public class DetectorData : FunctionBlockData
    {
        public void Allocate(int noDetectors, int noBlocks)
        {
            leqTotal = new double[noDetectors];
            rmsFast = new double[noDetectors][];
            for (int i = 0; i < noDetectors; i++)
                rmsFast[i] = new double[noBlocks];
        }

        public double[] leqTotal;
        public double[][] rmsFast;
        public int count;
    }

    public class DetectorSetup
    {
    }
}
