using System;
using System.Collections;

namespace JH.Applications
{
    public partial class CpbAnalysis : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public class Calculations
        {
            CPBFilterBank filterBank;
            double[] input;
            double[][] output;
            DataObjectElement[] outputData;

            public Calculations()
            {
            }

            public void Calculate(double[] input)
            {
                for (int i = 0; i < input.Length; i++)
                    this.input[i] = input[i];

                filterBank.Compute();

                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = filterBank.filters[i].Output;
                    outputData[i].data = output[i];
                }
            }

            public int Init(int length, OctaveFilterType filterType, int lowFrequency, int highFrequency, int samplingFrequency)
            {
                filterBank = new CPBFilterBank(filterType, lowFrequency, highFrequency, samplingFrequency);
                filterBank.Init();
                input = new double[length];
                filterBank.Input = input;
                return filterBank.filters.Length;
            }

            public void Allocate(int nfilters, DataObject outputObj)
            {
                outputData = new DataObjectElement[nfilters + 1];
                for (int i = 0; i < outputData.Length - 1; i++)
                    if (filterBank.filterType == OctaveFilterType.ThirdOctave)
                        outputData[i] = new DataObjectElement(DisplayComponent.cpb3Freq[i + filterBank.lowIndex], i);
                    else
                        outputData[i] = new DataObjectElement(DisplayComponent.cpb1Freq[i + filterBank.lowIndex], i);
                outputData[outputData.Length - 1] = new DataObjectElement("All", outputData.Length-1);
                outputObj.dataElements = outputData;
                output = new double[nfilters][];
                outputData[nfilters].data = output;
            }
        }
    }
}

