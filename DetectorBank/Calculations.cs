using System;

namespace JH.Applications
{
    public partial class DetectorBank : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public class Calculations
        {
            Detector[] detectors;
            double[] leqTotal;
            double[] rms;

            public Calculations()
            {
            }
            public void Calculate(double[][] input)
            {
                for (int i = 0; i < detectors.Length; i++)
                {
                    detectors[i].input = input[i] as double[];
                    detectors[i].Compute();
                    rms[i] = detectors[i].rms[0];
                    leqTotal[i] = detectors[i].leqTotal[0];
                }
            }

            public void Reset()
            {
                for (int i = 0; i < detectors.Length; i++)
                    detectors[i].Reset();
            }

            public void Init(int nDetectors, int numberOfaverages, int samplingFrequency)
            {
                detectors = new Detector[nDetectors];
                for (int i = 0; i < nDetectors; i++)
                {
                    detectors[i] = new Detector();
                    detectors[i].Init(numberOfaverages, samplingFrequency);
                }

            }
            public void Allocate(int nDetectors, DataObjectElement[] outputData)
            {
                leqTotal = new double[nDetectors];
                rms = new double[nDetectors];
                outputData[0].data = rms;
                outputData[1].data = leqTotal;
            }
        }
    }
}