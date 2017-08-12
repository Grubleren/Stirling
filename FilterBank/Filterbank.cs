using System;

namespace JH.Applications
{
    public class CPBFilterBank
    {
        public CPBFilterBank(OctaveFilterType filterType, double lowFc, double highFc, int fs)
        {
            this.filterType = filterType;
            int bw;

            switch (filterType)
            {
                case OctaveFilterType.Octave:
                        bw = 1;
                    break;
                case OctaveFilterType.ThirdOctave:
                        bw = 3;
                    break;
                case OctaveFilterType.TwelfthOctave:
                        bw = 12;
                    break;
                case OctaveFilterType.TwentyfourthOctave:
                        bw = 24;
                    break;
                default: bw = 0;
                    break;
            }
            lowIndex = (int)Math.Round((Math.Log10(lowFc / 1000) / Math.Log10(2) + 10) * bw);
            highIndex = (int)Math.Round((Math.Log10(highFc / 1000) / Math.Log10(2) + 10) * bw);
            noFilters = highIndex - lowIndex + 1;
            filters = new ButterworthFilter[noFilters];
            double bandwidthFactor = Math.Pow(2, 0.5 / bw);
            double rel = (bandwidthFactor - 1 / bandwidthFactor) / (Math.PI / 3) / 2;
            bandwidthFactor = rel + Math.Sqrt(Math.Pow(rel, 2) + 1);

            for (int i = 0; i < noFilters; i++)
            {
                double fCenter = 1000 * Math.Pow(Math.Pow(2, 1.0 / bw), i + lowIndex - 10 * bw);
                double fLower = fCenter / bandwidthFactor;
                double fUpper = fCenter * bandwidthFactor;
                filters[i] = new ButterworthFilter(FilterType.BP, 3, fLower, fUpper, fs);
            }
        }

        public void Init()
        {
            for (int i = 0; i < filters.Length; i++)
                filters[i].Init();
        }

        public double[] Input
        {
            set
            {
                for (int i = 0; i < filters.Length; i++)
                    filters[i].Input = value;
            }
        }

        public void Compute()
        {
            for (int i = 0; i < filters.Length; i++)
                filters[i].Compute();
        }

        public double CenterFrequency(int index)
        {
            switch (filterType)
            {
                case OctaveFilterType.Octave:
                    return octaves[index + lowIndex];
                case OctaveFilterType.ThirdOctave:
                    return thirdOctaves[index + lowIndex];
                case OctaveFilterType.TwelfthOctave:
                    return Math.Pow(2, (index + lowIndex) / 12.0);
                case OctaveFilterType.TwentyfourthOctave:
                    return Math.Pow(2, (index + lowIndex) / 24.0);
                default: return 0;
            }
        }

        public static readonly double[] octaves = { 1, 2, 4, 8, 16, 31.5, 63, 125, 250, 500, 1000, 2000, 4000, 8000, 16000 };
        public static readonly double[] thirdOctaves = { 1, 1.2, 1.6, 2, 2.5, 3.1, 4, 5, 6.3, 8, 10, 12.5, 16, 20, 25, 31.5, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000, 10000, 12500, 16000, 20000 };

        public OctaveFilterType filterType;

        public int lowIndex;
        public int highIndex;
        public int noFilters;

        public ButterworthFilter[] filters;

    }
}
