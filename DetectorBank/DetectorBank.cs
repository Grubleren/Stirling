using System;

namespace JH.Applications
{
    public partial class DetectorBank : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public enum DetectorBankSignal
        {
            Rms = 0,
            leqTotal = 1,
        }

        Calculations calculations;
        DataObjectElement[] outputData;
        DetectorBankSetup setup;

        public DetectorBank()
        {
            setup = new DetectorBankSetup();
            calculations = new Calculations();
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
                    double[][] input = data.dataElements[dataSelector].data as double[][];
                    if (input == null)
                        return;

                    calculations.Calculate(input);

                    for (int i = 0; i < outputData.Length; i++)
                        outputData[i].descriptors.Add(data.dataElements[dataSelector].descriptors[0]);

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
                DetectorBankSetup s = value as DetectorBankSetup;

                if (setup == null ||
                    s.nDetectors != setup.nDetectors ||
                    s.samplingFrequency != setup.samplingFrequency ||
                    s.N != setup.N)
                {
                    calculations.Init(s.nDetectors, s.N, s.samplingFrequency);
                    calculations.Allocate(s.nDetectors, outputData);
                }

                setup.Copy(s);
            }
        }
    }

    public class DetectorBankSetup : ISetup
    {
        public int nDetectors;
        public int samplingFrequency;
        public int N;
        

        public void Copy(DetectorBankSetup setup)
        {
            nDetectors = setup.nDetectors;
            samplingFrequency = setup.samplingFrequency;
            N = setup.N;
        }

        public object Clone()
        {
            DetectorBankSetup s = new DetectorBankSetup();
            s.Copy(this);
            return s;
        }
    }
}