using JH.Calculations;
using System;

namespace JH.Applications
{
    public partial class Afilter : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public class Calculations
        {
            IIR iir;
            double[] output;

            public Calculations()
            {

            }

            public void Calculate(double[] input)
            {
                iir.Iir(input, output);
            }

            public void Reset()
            {
            }
            public void Allocate(int outputLength, IIR iir, DataObjectElement[] outputData)
            {
                this.iir = iir;
                output = new double[outputLength];
                outputData[0].data = output;
            }
        }
    }
}