using System;

namespace JH.Applications
{
    public partial class BBDetector : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public class Calculations
        {
            Detector detector;
            DataObjectElement[] outputData;

            public Calculations()
            {
            }
            public void Calculate(double[] input)
            {
                detector.input = input;
                detector.Compute();

                outputData[0].data = detector.rms[0];
                outputData[1].data = detector.leqTotal[0];

            }

            public void Reset()
            {
                detector.Reset();
            }

            public void Init(int numberOfaverages, int samplingFrequency)
            {
                detector = new Detector();
                detector.Init(numberOfaverages, samplingFrequency);
            }
            public void Allocate(DataObjectElement[] outputData)
            {
                this.outputData = outputData;
            }
        }
    }
}