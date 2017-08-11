using System;

namespace JH.Applications
{
    public class WhiteNoise : IGenerator
    {
        GeneratorAbstraction abstraction;

        public WhiteNoise()
        {
        }

        public GeneratorAbstraction BaseClass
        {
            set { abstraction = value; }
        }

        public void GenerateNextBuffer()
        {
            for (int i = 0; i < abstraction.buffer.Length; i++)
            {
                abstraction.buffer[i] = abstraction.setup.level * abstraction.random.RandomGauss();

            }
        }

        public void Init()
        {

        }
    }
}
