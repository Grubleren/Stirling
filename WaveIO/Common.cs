using System;
using System.Collections;

namespace JH.Applications
{
    public partial class WaveIO
    {
        public struct ChunckInfo
        {
            public string name;
            public uint size;
            public long contentPosition;
        }

        public enum SweepTypes
        {
            None,
            Linear,
            Exponential
        }

        ArrayList chuncks;

        uint length;

        short format;
        ushort numberOfChannels;
        int samplingFrequency;
        uint bytesPerSecond;
        ushort blockAlign;
        ushort bitsPerSample;

        uint dataSize;
        long dataStart;

        SweepTypes sweepType;
        double sweepTime;
        double sweepStart;
        double sweepStop;
        double beta;

        string fileFormatVersion;
        string creationDateTime;
        string transducerName;
        string transducerSensitivity;
        string conditioningAmplifierGain;
        string datRecorderGain;
        string calibrationFactor;
        string normalizationFactor;
        double scalingFactor;
        string fullScaleLevel;
        string mode;
        string recordingEquipment;

        string unit;
        string noChannels;
        string channelName;

        public WaveIO()
        {
        }

        public WaveIO(int samplingFrequency,
                      SweepTypes sweepType, double sweepTime, double sweepStart, double sweepStop, double beta)
        {
            this.samplingFrequency = samplingFrequency;
            this.sweepType = sweepType;
            this.sweepTime = sweepTime;
            this.sweepStart = sweepStart;
            this.sweepStop = sweepStop;
            this.beta = beta;
        }

        public ArrayList Chuncks
        {
            get { return chuncks; }
        }

        public uint Length
        {
            get { return length; }
        }

        public short Format
        {
            get { return format; }
        }

        public ushort NumberOfChannels
        {
            get { return numberOfChannels; }
        }

        public int SamplingFrequency
        {
            get { return samplingFrequency; }
            set { samplingFrequency = value; }
        }

        public uint BytesPerSecond
        {
            get { return bytesPerSecond; }
        }

        public ushort BlockAlign
        {
            get { return blockAlign; }
        }

        public ushort BitsPerSample
        {
            get { return bitsPerSample; }
        }

        public uint DataSize
        {
            get { return dataSize; }
        }

        public long DataStart
        {
            get { return dataStart; }
        }

        public SweepTypes SweepType
        {
            get { return sweepType; }
            set { sweepType = value; }
        }

        public double SweepTime
        {
            get { return sweepTime; }
            set { sweepTime = value; }
        }

        public double SweepStart
        {
            get { return sweepStart; }
            set { sweepStart = value; }
        }

        public double SweepStop
        {
            get { return sweepStop; }
            set { sweepStop = value; }
        }

        public double Beta
        {
            get { return beta; }
            set { beta = value; }
        }

        public string FileFormatVersion
        {
            get {return fileFormatVersion;}
        }

        public string CreationDateTime
        {
            get {return creationDateTime;}
        }

        public string TransducerName
        {
            get {return transducerName;}
        }

        public string TransducerSensitivity
        {
            get {return transducerSensitivity;}
        }

        public string ConditioningAmplifierGain
        {
            get {return conditioningAmplifierGain;}
        }

        public string DatRecorderGain
        {
            get {return datRecorderGain;}
        }

        public string CalibrationFactor
        {
            get {return calibrationFactor;}
        }

        public string NormalizationFactor
        {
            get {return normalizationFactor;}
        }

        public double ScalingFactor
        {
            get {return scalingFactor;}
        }

        public string FullScaleLevel
        {
            get {return fullScaleLevel;}
        }

        public string Mode
        {
            get {return mode;}
        }

        public string RecordingEquipment
        {
            get { return recordingEquipment; }
        }

        public string Unit
        {
            get { return unit; }
        }

        public string NoChannels
        {
            get { return noChannels; }
        }

        public string ChannelName
        {
            get { return channelName; }
        }

    }
}
