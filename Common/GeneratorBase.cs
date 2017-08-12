using System;

namespace BK.BasicEnv.Applications
{
    public abstract class RTGeneratorBase : IRTGenerator
    {
        public RTGeneratorBase()
        {
        }

        public virtual void Initialize()
        {
            activeBuffer = 0;
            blockCounter = 0;
        }

        public abstract void InitializeTables(double f0, double f1, double T);

        public abstract void GenerateNextBuffer();

        public int BufferSize
        {
            get { return bufferSize; }
        }
        public int BufferSizeLast
        {
            get { return bufferSizeLast; }
        }
        public int NoBlocksToPlay
        {
            get { return noBlocksToPlay; }
        }
        public int BlockCounter
        {
            get { return blockCounter; }
        }
        public short[][] Buffer
        {
            get { return buffer; }
        }
        public int ActiveBuffer
        {
            get { return activeBuffer; }
        }
        public int SamplingFrequency
        {
            get { return samplingFrequency; }
        }

        protected int bufferSize;
        protected int bufferSizeLast;
        protected int noBlocksToPlay;
        protected int blockCounter;
        protected short[][] buffer;
        protected int activeBuffer;
        protected int samplingFrequency;
        protected Random random = new Random();

    }
}
