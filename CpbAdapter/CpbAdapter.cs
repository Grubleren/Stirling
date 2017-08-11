using System;
using System.Collections;

namespace JH.Applications
{
    public class CpbAdapter : IFunctionBlock, IObservable<double[]>, IObserver<double[]>
    {
        public double[] output;
        public CpbAdapterSetup setup;

        private ArrayList subscribers = new ArrayList();
        private IIterator iterator;

        public CpbAdapter(IIterator iterator)
        {
            this.iterator = iterator;
        }
        public IIterator CreateIterator(int type)
        {
        }
        public IIterator GetIterator()
        {
            return iterator;
        }
        public void SetIterator(IIterator iterator)
        {
            this.iterator = iterator;
        }
        public IDisposable Subscribe(IObserver<double[]> observer)
        {
            subscribers.Add(observer);
            return new Cancel(observer, this);
        }
        public void Unsubscribe(object observer)
        {
            subscribers.Remove((IObserver<double[]>)observer);
        }
        public void OnNext(double[] input)
        {
            {
                for (int j = 0; j < output.Length; j++)
                {
                    output[j] = input[3];
                }
            }
        }
        public void OnCompleted()
        {
        }
        public void OnError(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        public void Reset()
        {
        }

        public void UpdateSettings()
        {
            output = new double[setup.bufferSize];
        }

        public void SetSetup(CpbAdapterSetup setup)
        {
            this.setup = setup;
        }

    }
    public class CpbAdapterSetup
    {
        public int bufferSize;
    }


}


