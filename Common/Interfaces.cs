using System;

namespace BK.BasicEnv.Applications
{
    public interface IRTGenerator
    {
        void Initialize();
        void InitializeTables(double f0, double f1, double T);
        void GenerateNextBuffer();
        int BufferSize { get; }
        int BufferSizeLast { get; }
        int NoBlocksToPlay { get; }
        int BlockCounter { get; }
        short[][] Buffer { get; }
        int ActiveBuffer { get; }
        int SamplingFrequency { get; }
    }
}
