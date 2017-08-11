using System;

namespace JH.Applications
{
    public class Sine : IGenerator
    {
        GeneratorAbstraction abstraction;
        double phi;
        double delta;

        public Sine()
        {
            phi = 0;
        }

        public GeneratorAbstraction BaseClass
        {
            set
            {
                abstraction = value;
                delta = abstraction.sine.Length * abstraction.setup.frequency / abstraction.setup.samplingFrequency;
            }
        }

        public void GenerateNextBuffer()
        {
            for (int i = 0; i < abstraction.buffer.Length; i++)
            {
                int mod = (int)phi;
                int n = (int)(mod & 0xff);
                double frac = phi - mod;
                int n1 = (n + 1) & 0xff;

                abstraction.buffer[i] = abstraction.setup.level * (abstraction.sine[n] * (1 - frac) + abstraction.sine[n1] * frac);
                phi += delta;
            }

        }

        public void Init()
        {

        }


    }
}
