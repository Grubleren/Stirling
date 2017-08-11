using System;
using System.Collections.Generic;

namespace JH.Applications
{
    public partial class FftAdapter : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public class Calculations
        {
            Queue<double[]> queue;
            double[] extraBuffer;
            int extraBufferLength;
            int length;
            int overlap;

            public Calculations(Queue<double[]> queue)
            {
                this.queue = queue;
                extraBuffer = new double[0];
            }

            public void Calculate(double[] buffer)
            {
                double[] vector;

                int bufferLength = buffer.Length;
                int index;
                int vectorLength;

                int offset = 0;
                while (bufferLength - (offset - extraBufferLength) >= length)
                {
                    vector = new double[length];
                    vectorLength = vector.Length;
                    index = Math.Max(0, extraBufferLength - offset);
                    for (int j = 0; j < index; j++)
                    {
                        vector[j] = extraBuffer[offset + j];
                    }
                    for (int j = index; j < vectorLength; j++)
                    {
                        vector[j] = buffer[offset - extraBufferLength + j];
                    }
                    queue.Enqueue(vector);
                    offset += length / overlap;
                }

                index = Math.Max(0, extraBufferLength - offset);
                for (int j = 0; j < index; j++)
                {
                    extraBuffer[j] = extraBuffer[j + offset];
                }
                extraBufferLength += bufferLength - offset;
                for (int j = index; j < extraBufferLength; j++)
                {
                    extraBuffer[j] = buffer[j + bufferLength - extraBufferLength];
                }
            }

            public void Reset()
            {
                extraBufferLength = 0;
                queue.Clear();
            }

            public void Allocate(int length, int overlap)
            {
                extraBuffer = new double[length];
                this.length = length;
                this.overlap = overlap;
            }
        }
    }
}
 
