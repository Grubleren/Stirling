using System;

namespace JH.Applications
{
    public partial class CpbAnalysis : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        Calculations calculations;
        CpbSetup setup;

        public CpbAnalysis()
        {
            setup = new CpbSetup();
            calculations = new Calculations();
            output = new DataObject();
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

        public override ISetup Settings
        {
            get
            {
                return (ISetup)setup.Clone();
            }
            set
            {
                CpbSetup s = value as CpbSetup;

                if (setup == null ||
                    s.lowFrequency != setup.lowFrequency ||
                    s.highFrequency != setup.highFrequency ||
                    s.filterType != setup.filterType ||
                    s.samplingFrequency != setup.samplingFrequency ||
                    s.length != setup.length)
                {
                    int nFilters = calculations.Init(s.length, s.filterType, s.lowFrequency, s.highFrequency, s.samplingFrequency);
                    calculations.Allocate(nFilters, output);

                    AxisDescriptor axisDescriptor = new AxisDescriptor();
                    axisDescriptor.axisType = s.filterType == OctaveFilterType.Octave ? DisplayComponent.AxisType.Cpb1 : DisplayComponent.AxisType.Cpb3;
                    axisDescriptor.min = DisplayComponent.Frequency2Index1(axisDescriptor.axisType, s.lowFrequency);
                    axisDescriptor.max = DisplayComponent.Frequency2Index1(axisDescriptor.axisType, s.highFrequency);
                    axisDescriptor.step = 1;
                    foreach (DataObjectElement element in output.dataElements)
                    {
                        element.descriptors.Clear();
                        element.descriptors.Add(axisDescriptor);
                    }
                }

                setup.Copy(s);

            }
        }
    }

    public class CpbSetup : ISetup
    {
        public int lowFrequency;
        public int highFrequency;
        public OctaveFilterType filterType;
        public int samplingFrequency;
        public int length;
        

        public void Copy(CpbSetup setup)
        {
            lowFrequency = setup.lowFrequency;
            highFrequency = setup.highFrequency;
            filterType = setup.filterType;
            samplingFrequency = setup.samplingFrequency;
            length = setup.length;
        }

        public object Clone()
        {
            CpbSetup s = new CpbSetup();
            s.Copy(this);
            return s;
        }
    }
}
