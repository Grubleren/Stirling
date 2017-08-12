using System;
using System.IO;

using JH.Calculations;

namespace JH.Applications
{
    public partial class WaveIO
    {
        public void SaveRealData(string path, double[] result, double scale)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            SaveWaveHeader(writer, result.Length, 1);
            for (int i = 0; i < result.Length; i++)
                writer.Write((short)(result[i] * scale));
            AddSweepChunck(writer);
            writer.Close();
        }

        public void SaveComplexData(string path, Complex[] result, double scale)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            SaveWaveHeader(writer, result.Length, 2);
            for (int i = 0; i < result.Length; i++)
            {
                writer.Write((short)(result[i].re * scale));
                writer.Write((short)(result[i].im * scale));
            }
            AddSweepChunck(writer);
            writer.Close();
        }

        public void SaveRealPart(string path, Complex[] result, double scale)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            SaveWaveHeader(writer, result.Length, 1);
            for (int i = 0; i < result.Length; i++)
                writer.Write((short)(result[i].re * scale));
            AddSweepChunck(writer);
            writer.Close();
        }

        public void SaveImagPart(string path, Complex[] result, double scale)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            SaveWaveHeader(writer, result.Length, 1);
            for (int i = 0; i < result.Length; i++)
                writer.Write((short)(result[i].im * scale));
            AddSweepChunck(writer);
            writer.Close();
        }

        public void SaveMagnitude(string path, Complex[] data, double offset)
        {
            double[] result = new double[data.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 10 * Math.Log10(Math.Pow(data[i].re, 2) + Math.Pow(data[i].im, 2)) * 500 + offset * 500;
            }

            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            SaveWaveHeader(writer, result.Length, 1);
            for (int i = 0; i < result.Length; i++)
            {
                double d = result[i];
                short s;
                if (double.IsInfinity(d) || double.IsNaN(d) || d < short.MinValue)
                    s = short.MinValue;
                else
                    s = (short)d;
                writer.Write(s);
            }
            AddSweepChunck(writer);
            writer.Close();
        }

        public void SavePhase(string path, Complex[] data, double scale)
        {
            double[] phase = new double[data.Length];
            int skip = 0;
            for (int i = 0; i < skip; i++)
                phase[i] = Math.Atan2(data[i].im, data[i].re);
            

            double pOld = Math.Atan2(data[skip].im, data[skip].re);
            phase[skip] = pOld;
            for (int i = skip+1; i < phase.Length; i++)
            {
                double p = Math.Atan2(data[i].im, data[i].re);
                phase[i] = phase[i - 1] + (p - pOld);
                if (p - pOld > Math.PI)
                    phase[i] -= 2 * Math.PI;
                if (pOld - p > Math.PI)
                    phase[i] += 2 * Math.PI;
                pOld = p;
            }
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            SaveWaveHeader(writer, phase.Length, 1);
            for (int i = 0; i < phase.Length; i++)
                writer.Write((short)(phase[i] / Math.PI * 30000 / scale));
            AddSweepChunck(writer);
            writer.Close();
        }

        public void SaveWrapedPhase(string path, Complex[] data)
        {
            double[] phase = new double[data.Length];
            for (int i = 1; i < phase.Length; i++)
            {
                phase[i] = Math.Atan2(data[i].im, data[i].re);
            }
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
            SaveWaveHeader(writer, phase.Length, 1);
            for (int i = 0; i < phase.Length; i++)
                writer.Write((short)(phase[i] / Math.PI * 30000));
            AddSweepChunck(writer);
            writer.Close();
        }

        public void SaveWaveHeader(BinaryWriter writer, int dataLength, short noChannels)
        {
            SaveWaveHeader(writer, dataLength, 16, noChannels);
        }

        public void SaveWaveHeader(BinaryWriter writer, int dataLength, short bitsPerSample, short noChannels)
        {
            short blockAllign = (short)(noChannels * bitsPerSample / 8);
            int size = blockAllign * dataLength;

            writer.Write(StringToInt32("RIFF"));            // "RIFF"
            writer.Write(36 + size);                        // Total file size - 8, (Header length - 8 = 36)
            writer.Write(StringToInt32("WAVE"));            // "WAVE"
            writer.Write(StringToInt32("fmt "));            // "fmt "
            writer.Write((uint)16);                         // "fmt " chunck size = 16
            writer.Write((ushort)1);                        // Format (no compression = 1)
            writer.Write(noChannels);
            writer.Write(samplingFrequency);
            writer.Write(blockAllign * samplingFrequency);
            writer.Write(blockAllign);
            writer.Write(bitsPerSample);
            writer.Write(StringToInt32("data"));            // "data"
            writer.Write((uint)(size));                     // "data" chunck size
        }

        public void AddSweepChunck(BinaryWriter writer)
        {
            SaveSweepChunck(writer, sweepType, sweepTime, sweepStart, sweepStop, beta);
        }

        public void SaveSweepChunck(BinaryWriter writer, SweepTypes sweepType, double sweepTime, double sweepStart, double sweepStop, double beta)
        {
            writer.Write(StringToInt32("swp "));
            writer.Write((uint)34);             // "swp " chunck size = 34
            writer.Write((short)sweepType);
            writer.Write(sweepTime);
            writer.Write(sweepStart);
            writer.Write(sweepStop);
            writer.Write(beta);
            int size = (int)writer.BaseStream.Length - 8;
            writer.Seek(4, SeekOrigin.Begin);
            writer.Write(size);
        }

        public void SaveESweepChunck(BinaryWriter writer, double sweepTime, double sweepStart, double beta)
        {
            writer.Write(StringToInt32("esw "));
            writer.Write((uint)24);             // "esw " chunck size = 24
            writer.Write(sweepTime);
            writer.Write(sweepStart);
            writer.Write(beta);
            int size = (int)writer.BaseStream.Length - 8;
            writer.Seek(4, SeekOrigin.Begin);
            writer.Write(size);
        }

        public void SaveLSweepChunck(BinaryWriter writer, double sweepTime, double sweepStart, double sweepStop)
        {
            writer.Write(StringToInt32("lsw "));
            writer.Write((uint)24);             // "lsw " chunck size = 24
            writer.Write(sweepTime);
            writer.Write(sweepStart);
            writer.Write(sweepStop);
            int size = (int)writer.BaseStream.Length - 8;
            writer.Seek(4, SeekOrigin.Begin);
            writer.Write(size);
        }
    }
}
