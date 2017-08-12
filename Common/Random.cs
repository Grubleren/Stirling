using System;

namespace BK.BasicEnv.Applications
{
    /// <summary>
    /// A simple 32 bit random generator
    /// </summary>
    public class Random
    {
        /// <summary>
        /// Used to generate a random seed
        /// </summary>
        public Random()
        {
            r = (uint)DateTime.Now.Ticks;
        } // Random

        /// <summary>
        /// Used when a fixed seed is wanted
        /// </summary>
        /// <param name="seed"></param>
        public Random(uint seed)
        {
            r = seed;
        } // Random

        /// <summary>
        /// Here we generate the 32 bit random number
        /// </summary>
        private void rnd()
        {
            for (int j = 0; j < 32; j++)
            {
                uint s1 = (r & 0x1) ^ ((r >> 2) & 0x1) ^ ((r >> 7) & 0x1) ^ ((r >> 16) & 0x1);
                r >>= 1;
                r |= s1 << 31;
            }
        } // rnd

        /// <summary>
        /// Scales the random number to a double between 0 and 1
        /// </summary>
        /// <returns> The random number between 0 and 1</returns>
        public double RandomDouble()
        {
            rnd();
            return r * c;
        } // RandomDouble

        public double RandomGauss()
        {
            double sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += RandomDouble();
            }
            sum -= 6;
            return sum;
        }

        private const double c = 1.0 / 0x100000000;
        private uint r;
    } // Random


}
