using System;

namespace JH.Applications
{
    partial class BBDetector : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        Calculations calculations;
        Detector detector;
        DataObjectElement[] outputData;
        BBDetectorSetup setup;

        public enum DetectorSignal
        {
            Rms = 0,
            leqTotal = 1,
        }

        public BBDetector()
        {
            BBDetectorSetup bb = new BBDetectorSetup();
            calculations = new Calculations();
            setup = new BBDetectorSetup();
            detector = new Detector();
            output = new DataObject();
            outputData = new DataObjectElement[2];
            outputData[0] = new DataObjectElement("Exponential",0);
            outputData[1] = new DataObjectElement("Linear",1);
            output.dataElements = outputData;
        }

        public void OnNext(DataObject data)
        {
            if (!error)
            {
                try
                {
                    double[] input = data.dataElements[dataSelector].data as double[];

                    calculations.Calculate(input);

                    TraverseSubscribers();
                }
                catch
                {
                    error = true;
                    TraverseError();
                }
            }
        }

        public void OnCompleted()
        {
            foreach (IObserver<DataObject> subscriber in subscribers)
                subscriber.OnCompleted();
        }

        public void OnError(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        public override void Reset()
        {
            error = false;
            calculations.Reset();
        }

        public override ISetup Settings
        {
            get
            {
                return (ISetup)setup.Clone();
            }
            set
            {
                BBDetectorSetup s = value as BBDetectorSetup;

                if (setup == null ||
                    s.samplingFrequency != setup.samplingFrequency ||
                    s.N != setup.N)
                {
                    calculations.Init(s.N, s.samplingFrequency);
                    calculations.Allocate(outputData);
                }

                setup.Copy(s);
            }
        }
    }

    public class BBDetectorSetup : ISetup
    {
        public int samplingFrequency;
        public int N;
        

        public void Copy(BBDetectorSetup setup)
        {
            samplingFrequency = setup.samplingFrequency;
            N = setup.N;
        }

        public object Clone()
        {
            BBDetectorSetup s = new BBDetectorSetup();
            s.Copy(this);
            return s;
        }
    }
}
