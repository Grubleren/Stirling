using System;

namespace JH.Applications
{
    public partial class Upsampling : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        int upFactor;
        int downFactor;
        Calculations calculations;
        UpsamplingSetup setup;
        DataObjectElement[] outputData;

        public Upsampling()
        {
            upFactor = 16;
            downFactor = 15;
            calculations = new Calculations(upFactor, downFactor);
            setup = new UpsamplingSetup();
            output = new DataObject();
            outputData = new DataObjectElement[1];
            outputData[0] = new DataObjectElement("",0);
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
                UpsamplingSetup s = value as UpsamplingSetup;

                if (setup == null ||
                    s.length != setup.length)
                {
                    int outputlength = s.length * upFactor / downFactor;

                    calculations.Allocate(outputlength, outputData);
                }

                setup.Copy(s);
            }
        }

    }

    public class UpsamplingSetup : ISetup, ICloneable
    {
        public int length;
        

        public void Copy(UpsamplingSetup setup)
        {
            length = setup.length;
        }

        public object Clone()
        {
            UpsamplingSetup s = new UpsamplingSetup();
            s.Copy(this);
            return s;
        }
    }
}

