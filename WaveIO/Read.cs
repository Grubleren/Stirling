using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Threading;

namespace JH.Applications
{
    public partial class WaveIO
    {
        public bool ReadWaveFileInfo(BinaryReader reader)
        {
            uint head;
            ChunckInfo chunckInfo;

            try
            {
                Reset();
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                head = reader.ReadUInt32();
                if (head != StringToInt32("RIFF"))
                    throw new InvalidDataException();
                length = reader.ReadUInt32(); // not used but should be filelength - 8 bytes
                head = reader.ReadUInt32();
                if (head != StringToInt32("WAVE"))
                    throw new InvalidDataException();
                bool moreChuncks = true;
                uint nextChunk = 0;
                while (moreChuncks)
                {
                    reader.BaseStream.Seek(nextChunk, SeekOrigin.Current);
                    chunckInfo = new ChunckInfo();
                    chunckInfo.name = Int32ToString(reader.ReadUInt32());
                    chunckInfo.size = reader.ReadUInt32();
                    chunckInfo.contentPosition = reader.BaseStream.Position;
                    chuncks.Add(chunckInfo);
                    moreChuncks = chunckInfo.contentPosition + chunckInfo.size < reader.BaseStream.Length;
                    nextChunk = chunckInfo.size;
                }

                int index;

                if (ChunkAvailable("fmt ", chuncks, out index))
                {
                    reader.BaseStream.Seek(((ChunckInfo)chuncks[index]).contentPosition, SeekOrigin.Begin);
                    format = reader.ReadInt16();
                    numberOfChannels = reader.ReadUInt16();
                    samplingFrequency = reader.ReadInt32();
                    bytesPerSecond = reader.ReadUInt32();
                    blockAlign = reader.ReadUInt16();
                    bitsPerSample = reader.ReadUInt16();
                }
                else
                    throw new InvalidDataException();

                if (ChunkAvailable("data", chuncks, out index))
                {
                    dataSize = ((ChunckInfo)chuncks[index]).size;
                    dataStart = ((ChunckInfo)chuncks[index]).contentPosition;
                }
                else
                    throw new InvalidDataException();

                if (ChunkAvailable("swp ", chuncks, out index))
                {
                    reader.BaseStream.Seek(((ChunckInfo)chuncks[index]).contentPosition, SeekOrigin.Begin);
                    sweepType = (SweepTypes)(reader.ReadInt16());
                    sweepTime = reader.ReadDouble();
                    sweepStart = reader.ReadDouble();
                    sweepStop = reader.ReadDouble();
                    beta = reader.ReadDouble();
                }

                if (ChunkAvailable("lsw ", chuncks, out index))
                {
                    reader.BaseStream.Seek(((ChunckInfo)chuncks[index]).contentPosition, SeekOrigin.Begin);
                    sweepTime = reader.ReadDouble();
                    sweepStart = reader.ReadDouble();
                    sweepStop = reader.ReadDouble();
                    sweepType = SweepTypes.Linear;
                }

                if (ChunkAvailable("esw ", chuncks, out index))
                {
                    reader.BaseStream.Seek(((ChunckInfo)chuncks[index]).contentPosition, SeekOrigin.Begin);
                    sweepTime = reader.ReadDouble();
                    sweepStart = reader.ReadDouble();
                    beta = reader.ReadDouble();
                    sweepType = SweepTypes.Exponential;
                }

                if (ChunkAvailable("bkdk", chuncks, out index))
                {
                    reader.BaseStream.Seek(((ChunckInfo)chuncks[index]).contentPosition, SeekOrigin.Begin);
                    fileFormatVersion = ReadString(reader);
                    creationDateTime = ReadString(reader);
                    transducerName = ReadString(reader);
                    transducerSensitivity = ReadString(reader);
                    string dummy = ReadString(reader);
                    conditioningAmplifierGain = ReadString(reader);
                    dummy = ReadString(reader);
                    datRecorderGain = ReadString(reader);
                    dummy = ReadString(reader);
                    calibrationFactor = ReadString(reader);
                    CultureInfo cultureInfo = new CultureInfo("en-US");
                    if (fileFormatVersion == "02.10")
                        scalingFactor = double.Parse(calibrationFactor, cultureInfo.NumberFormat) / 32768;
                    normalizationFactor = ReadString(reader);
                    if (fileFormatVersion == "02.00")
                        scalingFactor = 1 / double.Parse(normalizationFactor, cultureInfo.NumberFormat);
                    fullScaleLevel = ReadString(reader);
                    if (fileFormatVersion == "02.10")
                        mode = ReadString(reader);
                    recordingEquipment = ReadString(reader);
                }

                if (ChunkAvailable("bkEx", chuncks, out index))
                {
                    reader.BaseStream.Seek(((ChunckInfo)chuncks[index]).contentPosition, SeekOrigin.Begin);
                    string dummy;
                    dummy = ReadLine(reader);
                    dummy = ReadLine(reader);
                    noChannels = ReadLine(reader);
                    dummy = ReadLine(reader);
                    unit = ReadLine(reader);
                    channelName = ReadLine(reader);
                    dummy = ReadLine(reader);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Reset()
        {
            chuncks = new ArrayList();

            length = 0;

            format = 0;
            numberOfChannels = 0;
            samplingFrequency = 0;
            bytesPerSecond = 0;
            blockAlign = 0;
            bitsPerSample = 0;

            dataSize = 0;
            dataStart = 0;

            sweepTime = 0;
            sweepStart = 0;
            sweepStop = 0;
            beta = 0;
            sweepType = SweepTypes.None;

            fileFormatVersion = "";
            creationDateTime = "";
            transducerName = "";
            transducerSensitivity = "";
            conditioningAmplifierGain = "";
            datRecorderGain = "";
            calibrationFactor = "";
            scalingFactor = 0;
            normalizationFactor = "";
            fullScaleLevel = "";
            mode = "";
            recordingEquipment = "";
            unit = "";
            noChannels = "";
            channelName = "";
        }

        private int StringToInt32(string s)
        {
            if (s.Length != 4)
                throw new InvalidOperationException();

            int value = 0;

            for (int i = 0; i < 4; i++)
                value += (byte)s[i] << 8 * i;

            return value;
        }

        private string Int32ToString(uint value)
        {
            string s = "";

            for (int i = 0; i < 4; i++)
            {
                s += (char)(value & 0xff);
                value >>= 8;
            }

            return s;
        }

        private bool ChunkAvailable(string chunckName, ArrayList chuncks, out int index)
        {
            bool found = false;
            index = 0;

            foreach (ChunckInfo chunkInfo in chuncks)
            {
                if (chunkInfo.name == chunckName)
                {
                    found = true;
                    break;
                }
                index++;
            }

            return found;
        }

        private string ReadString(BinaryReader reader)
        {
            char c;
            string s = "";

            do
            {
                c = reader.ReadChar();
                s += c;
            }
            while (c != 0);

            s = s.Substring(0, s.Length - 1);

            return s;
        }

        private string ReadLine(BinaryReader reader)
        {
            char c;
            string s = "";

            do
            {
                c = reader.ReadChar();
                s += c;
            }
            while (c != 10);

            s = s.Substring(0, s.Length - 1);

            return s;
        }

        public void ReadSamples(BinaryReader reader, double[] leftChannel)
        {
            ReadSamples(reader, leftChannel, null);
        }

        public void ReadSamples(BinaryReader reader, double[] leftChannel, double[] rightChannel)
        {
            int offset;
            byte[] buffer = reader.ReadBytes(leftChannel.Length * blockAlign);

            switch (blockAlign)
            {
                case 2: // 16 bit mono
                    offset = 0;
                    for (int i = 0; i < leftChannel.Length; i++)
                    {
                        leftChannel[i] = (int)(buffer[offset] << 16 | buffer[offset + 1] << 24);
                        offset += blockAlign;
                    }
                    break;
                case 3: // 24 bit mono
                    offset = 0;
                    for (int i = 0; i < leftChannel.Length; i++)
                    {
                        leftChannel[i] = (int)(buffer[offset] << 8 | buffer[offset + 1] << 16 | buffer[offset + 2] << 24);
                        offset += blockAlign;
                    }
                    break;
                case 4:
                    if (numberOfChannels == 2) // 16 bit stereo
                    {
                        if (rightChannel == null)
                        {
                            offset = 0;
                            for (int i = 0; i < leftChannel.Length; i++)
                            {
                                leftChannel[i] = (int)(buffer[offset] << 16 | buffer[offset + 1] << 24);
                                offset += blockAlign;
                            }
                        }
                        else
                        {
                            offset = 0;
                            for (int i = 0; i < leftChannel.Length; i++)
                            {
                                leftChannel[i] = (int)(buffer[offset] << 16 | buffer[offset + 1] << 24);
                                rightChannel[i] = (int)(buffer[offset + 2] << 16 | buffer[offset + 3] << 24);
                                offset += blockAlign;
                            }
                        }
                    }
                    else // 32 bit mono
                    {
                        offset = 0;
                        for (int i = 0; i < leftChannel.Length; i++)
                        {
                            leftChannel[i] = (int)(buffer[offset] | buffer[offset + 1] << 8 | buffer[offset + 2] << 16 | buffer[offset + 3] << 24);
                            offset += blockAlign;
                        }
                    }
                    break;
                case 6: // 24 bit stereo
                    if (rightChannel == null)
                    {
                        offset = 0;
                        for (int i = 0; i < leftChannel.Length; i++)
                        {
                            leftChannel[i] = (int)(buffer[offset] << 8 | buffer[offset + 1] << 16 | buffer[offset + 2] << 24);
                            offset += blockAlign;
                        }
                    }
                    else
                    {
                        offset = 0;
                        for (int i = 0; i < leftChannel.Length; i++)
                        {
                            leftChannel[i] = (int)(buffer[offset] << 8 | buffer[offset + 1] << 16 | buffer[offset + 2] << 24);
                            rightChannel[i] = (int)(buffer[offset + 3] << 8 | buffer[offset + 4] << 16 | buffer[offset + 5] << 24);
                            offset += blockAlign;
                        }
                    }
                    break;
                case 8: // 32 bit stereo
                    if (rightChannel == null)
                    {
                        offset = 0;
                        for (int i = 0; i < leftChannel.Length; i++)
                        {
                            leftChannel[i] = (int)(buffer[offset] | buffer[offset + 1] << 8 | buffer[offset + 2] << 16 | buffer[offset + 3] << 24);
                            offset += blockAlign;
                        }
                    }
                    else
                    {
                        offset = 0;
                        for (int i = 0; i < leftChannel.Length; i++)
                        {
                            leftChannel[i] = (int)(buffer[offset] | buffer[offset + 1] << 8 | buffer[offset + 2] << 16 | buffer[offset + 3] << 24);
                            rightChannel[i] = (int)(buffer[offset + 4] | buffer[offset + 5] << 8 | buffer[offset + 6] << 16 | buffer[offset + 7] << 24);
                            offset += blockAlign;
                        }
                    }
                    break;
            }
        }
    }
}
