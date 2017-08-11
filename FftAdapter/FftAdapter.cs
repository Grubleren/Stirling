using System;
using System.Collections.Generic;

namespace JH.Applications
{
    public partial class FftAdapter : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        Calculations calculations;
        Queue<double[]> queue;
        DataObjectElement[] outputData;
        FftAdapterSetup setup;

        public FftAdapter()
        {
            setup = new FftAdapterSetup();
            queue = new Queue<double[]>();
            calculations = new Calculations(queue);
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

                    for (; ; )
                    {
                        if (queue.Count == 0)
                            break;
                        outputData[0].data = queue.Dequeue();

                        TraverseSubscribers();
                    }
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
                FftAdapterSetup s = value as FftAdapterSetup;

                if (setup == null ||
                    s.length != setup.length ||
                    s.overlap != setup.overlap
                    )
                {
                    calculations.Reset();
                    calculations.Allocate(s.length, s.overlap);
                }

                setup.Copy(s);
            }
        }

    }

    public class FftAdapterSetup : ISetup
    {
        public int length;
        public int overlap;
        

        public void Copy(FftAdapterSetup setup)
        {
            length = setup.length;
            overlap = setup.overlap;
        }

        public object Clone()
        {
            FftAdapterSetup s = new FftAdapterSetup();
            s.Copy(this);
            return s;
        }
    }
}
