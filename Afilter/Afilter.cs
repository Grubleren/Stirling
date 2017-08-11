using System;
using JH.Calculations;

namespace JH.Applications
{
    partial class Afilter : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        Calculations calculations;
        BiQuad[] biQuads;
        IIR iir;
        DataObjectElement[] outputData;
        AfilterSetup setup;

        public Afilter()
        {
            calculations = new Calculations();
            setup = new AfilterSetup();
            AFilterCoefficients();
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

        public override ISetup Settings
        {
            get
            {
                return (ISetup)setup.Clone();
            }
            set
            {
                AfilterSetup s = value as AfilterSetup;

                if (setup == null ||
                    s.length != setup.length)
                {
                    calculations.Allocate(s.length, iir, outputData);
                }

                setup.Copy(s);
            }
        }

        void AFilterCoefficients()
        {
            iir = new IIR();
            biQuads = new BiQuad[3];
            for (int i = 0; i < biQuads.Length; i++)
            {
                biQuads[i] = new BiQuad();
            }

            biQuads[0].a0 = 1;
            biQuads[0].a1 = -1.34730722798;
            biQuads[0].a1 = -1.34730722798;
            biQuads[0].a2 = 0.349057529796;
            biQuads[0].b0 = 0.965250965250;
            biQuads[0].b1 = -1.34730163086;
            biQuads[0].b2 = 0.382050665614;

            biQuads[1].a0 = 1;
            biQuads[1].a1 = -1.89387049481;
            biQuads[1].a2 = 0.895159769170;
            biQuads[1].b0 = 0.946969696969;
            biQuads[1].b1 = -1.89393939393;
            biQuads[1].b2 = 0.946969696969;

            biQuads[2].a0 = 1;
            biQuads[2].a1 = -1.34730722798;
            biQuads[2].a2 = 0.349057529796;
            biQuads[2].b0 = 0.646665428100;
            biQuads[2].b1 = -0.38362237137;
            biQuads[2].b2 = -0.26304305672;

            iir.Init(biQuads, biQuads.Length);
        }

    }

    public class AfilterSetup : ISetup
    {
        public int length;
        

        public void Copy(AfilterSetup setup)
        {
            length = setup.length;
        }

        public object Clone()
        {
            AfilterSetup s = new AfilterSetup();
            s.Copy(this);
            return s;
        }
    }
}
