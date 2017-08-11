using System;
using JH.Calculations;
using System.IO;

namespace JH.Applications
{
    public partial class FftAnalysis : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public class Calculations
        {
            Complex[] temp;
            double[] hanning;
            double factor;
            double alpha;
            double beta;
            int autoSpectrumLength;
            FFT fft;
            public AveragingType averagingType;
            public int numberOfAverages;
            double[] timeSignal;
            Complex[] instSpectrum;
            double[] autoSpectrum;
            double[] autocorrelation;
            DataObjectElement pCount;

            public void Calculate(double[] input)
            {
                int count = (int)pCount.data;
                if (averagingType == AveragingType.Linear || count < numberOfAverages)
                    alpha = (double)count / (count + 1);
                else
                    alpha = 1 - 2.0 / (numberOfAverages + 1);
                beta = 1 - alpha;

                for (int i = 0; i < input.Length; i++)
                {
                    timeSignal[i] = input[i];
                    temp[i].re = input[i] * hanning[i];
                    temp[i].im = 0;
                }

                fft.FftForward(temp);

                for (int i = 0; i < temp.Length; i++)
                    instSpectrum[i] = temp[i];
                for (int i = 0; i < autoSpectrumLength; i++)
                {
                    double power = Complex.AbsSqr(temp[i]) * factor;
                    if (i == 0)
                        power *= 0.5;
                    autoSpectrum[i] = autoSpectrum[i] * alpha + power * beta;
                }

                int N = temp.Length;
                for (int i = 0; i < N; i++)
                {
                    temp[i] = new Complex(0, 0);
                }
                for (int i = 0; i < autoSpectrum.Length; i++)
                {
                    temp[i].re = Math.Sqrt(autoSpectrum[i]);
                }


                double min = double.MaxValue;
                for (int i = 0; i <= N / 2; i++)
                    if (temp[i].re > 0 && temp[i].re < min)
                        min = temp[i].re;

                for (int i = 0; i <= N / 2; i++)
                    if (temp[i].re == 0)
                        temp[i].re = min;

                for (int i = 0; i <= N / 2; i++)
                    temp[i].re = 2 * Math.Log(temp[i].re);

                //int n0 = (int)(20.0 / 51200 * N);
                //for (int i = 0; i <= n0; i++)
                //    temp[i].re *= 0.5 * (1 - Math.Cos(Math.PI / n0 * i));

                //int n2 = (int)(10000.0 / 51200 * N);
                //for (int i = n2; i <= N / 2; i++)
                //    temp[i].re *= 0.5 * (1 + Math.Cos(Math.PI / (N / 2 - n2) * (i - n2)));

                for (int i = 1; i < N / 2; i++)
                    temp[N - i].re = temp[i].re;

                fft.FftInverse(temp);

                int n3 = N / 2 + 1;
                int n4 = N / 2 + 1;
                for (int i = n3; i < n4; i++)
                {
                    temp[i] *= 0.5 * (1 + Math.Cos(Math.PI / (n4 - n3) * (i - n3)));
                }

                for (int i = n4; i < N; i++)
                {
                    temp[i] = new Complex(0, 0);
                }

                n4 = 1;
                for (int i = 0; i < n4; i++)
                {
                    double fact = 0.5 * (1 + Math.Sin(Math.PI / 2 / n4 * i));
                    temp[i] *= fact;
                    temp[N / 2 - i] *= fact;
                }

                fft.FftForward(temp);

                for (int i = 0; i < N / 2; i++)
                {
                    temp[i] = Math.Exp(temp[i].re) * new Complex(Math.Cos(temp[i].im), Math.Sin(temp[i].im));
                    temp[i + N / 2] = new Complex(0, 0);
                }

                //for (int i = 0; i <= n0; i++)
                //    temp[i] = new Complex(0, 0);

                //for (int i = n2; i < N; i++)
                //    temp[i] = new Complex(0, 0);


                fft.FftInverse(temp);

                int skip = (int)(0.005 * 51200);
                double max = double.MinValue;
                for (int i = skip; i < N; i++)
                {
                    if (Math.Abs(temp[i].re) > max)
                        max = Math.Abs(temp[i].re);
                }
                double factor1 = 30000 / max;
                for (int i = skip; i < N; i++)
                    temp[i - skip] = temp[i] * factor1;
                for (int i = N - skip; i < N; i++)
                    temp[i] = new Complex(0, 0);

                for (int i = 0; i < N / 2; i++)
                {
                    autocorrelation[i] = (autocorrelation[i]*count+temp[i].re)/(count+1);
                }
                count++;
                if (false)
                {
                    WaveIO waveIO = new WaveIO();
                    waveIO.SamplingFrequency = 51200;
                    waveIO.SaveRealData("c:/users/jhee/impavg.wav", autocorrelation, 1);
                }
                pCount.data = count;
            }

            public void Reset()
            {
                for (int i = 0; i < autoSpectrumLength; i++)
                    autoSpectrum[i] = 0;

                for (int i = 0; i < autocorrelation.Length; i++)
                    autocorrelation[i] = 0;

                pCount.data = 0;
            }

            public void Allocate(int M, DataObjectElement[] outputData)
            {
                int N = 1 << M;
                autoSpectrumLength = (int)(N / 2.56) +1;

                factor = 2.0 / N / N;

                temp = new Complex[N];
                hanning = new double[N];

                for (int i = 0; i < N; i++)
                    hanning[i] = (1 - Math.Cos(2 * Math.PI / N * i));

                timeSignal = new double[N];
                instSpectrum = new Complex[N];
                autoSpectrum = new double[autoSpectrumLength];
                autocorrelation = new double[N];
                outputData[(int)FftSignal.Timesignal].data = timeSignal;
                outputData[(int)FftSignal.InstSpectrum].data = instSpectrum;
                outputData[(int)FftSignal.AutoSpectrum].data = autoSpectrum;
                outputData[(int)FftSignal.Autocorrelation].data = autocorrelation;
                pCount = outputData[(int)FftSignal.Count];

                fft = new FFT();
                fft.Free();
                fft.Init(M, FFTFactor.IPP_FFT_DIV_INV_BY_N);

            }
        }
    }
}