using System;
using JH.Calculations;
using System.Threading;

namespace JH.Applications
{
    public partial class FftAnalysis : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public enum FftSignal
        {
            Timesignal = 0,
            InstSpectrum = 1,
            AutoSpectrum = 2,
            Autocorrelation = 3,
            Count = 4
        }

        public enum AveragingType
        {
            Linear,
            Exponential
        }

        Calculations calculations;
        AxisDescriptor axisDescriptor;
        DataObjectElement[] outputData;
        FftSetup setup;


        public FftAnalysis()
        {
            setup = new FftSetup();
            calculations = new Calculations();
            output = new DataObject();
            outputData = new DataObjectElement[5];
            outputData[0] = new DataObjectElement("Time",0);
            outputData[1] = new DataObjectElement("Inst",1);
            outputData[2] = new DataObjectElement("Autospectrum",2);
            outputData[3] = new DataObjectElement("Autocorrelation",3);
            outputData[4] = new DataObjectElement("Count",4);
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
                FftSetup s = value as FftSetup;

                if (setup == null ||
                    s.M != setup.M ||
                    s.averagingType != setup.averagingType ||
                    s.numberOfAverages != setup.numberOfAverages)
                {
                    calculations.averagingType = s.averagingType;
                    calculations.numberOfAverages = s.numberOfAverages;
                    calculations.Allocate(s.M, outputData);
                    calculations.Reset();
                    axisDescriptor = new AxisDescriptor();
                    axisDescriptor.axisType = DisplayComponent.AxisType.Lin;
                    axisDescriptor.min = 0;
                    axisDescriptor.max = 20000;
                    axisDescriptor.step = 1024.0 / (1 << s.M) * 50;
                    foreach (DataObjectElement element in outputData)
                    {
                        element.descriptors.Clear();
                        element.descriptors.Add(axisDescriptor);
                    }

                }
                setup.Copy(s);

            }
        }
    }

    public class FftSetup : ISetup
    {
        public int M;
        public FftAnalysis.AveragingType averagingType;
        public int numberOfAverages;
        public int type;
        

        public void Copy(FftSetup setup)
        {
            M = setup.M;
            averagingType = setup.averagingType;
            numberOfAverages = setup.numberOfAverages;
            type = setup.type;
        }

        public object Clone()
        {
            FftSetup s = new FftSetup();
            s.Copy(this);
            return s;
        }
    }
}
