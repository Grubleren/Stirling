using System;
using System.Runtime.InteropServices;

using JH.Calculations;

namespace JH.Applications
{
    public enum FilterType
    {
        LP,
        HP,
        BP
    }

    public enum OctaveFilterType
    {
        Octave,
        ThirdOctave,
        TwelfthOctave,
        TwentyfourthOctave
    }

    public unsafe class ButterworthFilter
    {
        public double[] Input
        {
            set 
            { 
                input = value;
                output = new double[input.Length];
            }
        }

        public double[] Output
        {
            get { return output; }
        }

        public ButterworthFilter(FilterType filterType, int order, double f0, double fs)
        {
            this.f0 = f0;
            double omega0 = Math.Tan(Math.PI * f0 / fs);

            if (filterType == FilterType.LP)
                CalculateFilterCoefficientsLP(order, omega0);
            else if (filterType == FilterType.HP)
                CalculateFilterCoefficientsHP(order, omega0);
            else
                throw new NotImplementedException("Filter type not supported");
        }

        public ButterworthFilter(FilterType filterType, int order, double f0, double f1, double fs)
        {
            this.f0 = f0;
            this.f1 = f1;
            this.fc = Math.Sqrt(f0 * f1); ;
            double omega0 = Math.Tan(Math.PI * f0 / fs);
            double omega1 = Math.Tan(Math.PI * f1 / fs);

            if (filterType == FilterType.BP)
                CalculateFilterCoefficientsBP(order, omega0, omega1);
            else
                throw new NotImplementedException("Filter type not supported");
        }

        public void Init()
        {
            iir = new IIR();

            iir.Init(biQuads, biQuads.Length);
        }

        public void Free()
        {
            iir.Free();
        }

        public void Compute()
        {
            iir.Iir(input, output);
        }

        private void CalculateFilterCoefficientsLP(int order, double omega0)
        {
            Complex[] poles = GetComplexPolesLP(order, omega0);

            BilinearTransform(poles);

            biQuads = new BiQuad[(order + 1) / 2];

            for (int i = 0; i < poles.Length; i++)
                biQuads[i] = new BiQuad(2, 1, -2 * poles[i].re, Complex.AbsSqr(poles[i]));

            if (order % 2 != 0)
            {
                double pole = BilinearTransform(-omega0);
                biQuads[poles.Length] = new BiQuad(1, 0, -pole, 0);
            }

            NormalizeBiQuads(biQuads, 0);
        }

        private void CalculateFilterCoefficientsHP(int order, double omega0)
        {
            Complex[] poles = GetComplexPolesHP(order, omega0);

            BilinearTransform(poles);

            biQuads = new BiQuad[(order + 1) / 2];

            for (int i = 0; i < poles.Length; i++)
                biQuads[i] = new BiQuad(-2, 1, -2 * poles[i].re, Complex.AbsSqr(poles[i]));

            if (order % 2 != 0)
            {
                double pole = BilinearTransform(-omega0);
                biQuads[poles.Length] = new BiQuad(-1, 0, -pole, 0);
            }

            NormalizeBiQuads(biQuads, Math.PI);
        }

        private void CalculateFilterCoefficientsBP(int order, double omega0, double omega1)
        {
            Complex[] poles = GetComplexPolesBP(order, omega0, omega1);

            BilinearTransform(poles);

            biQuads = new BiQuad[order];

            for (int i = 0; i < poles.Length; i++)
                biQuads[i] = new BiQuad(0, -1, -2 * poles[i].re, Complex.AbsSqr(poles[i]));

            if (order % 2 != 0)
            {
                double omega2 = Math.Sqrt(omega0 * omega1);
                double p = -(omega1 - omega0) / omega2;
                Complex pole;
                if (Math.Abs(p) > 2)
                {
                    double u = Math.Sqrt(-4 + Math.Pow(p, 2));
                    pole = 0.5 * omega2 * new Complex(p-u, 0);

                }
                else
                {
                    double u = Math.Sqrt(4 - Math.Pow(p, 2));
                    pole = 0.5 * omega2 * new Complex(p, u);
                }
                pole = BilinearTransform(pole);
                biQuads[poles.Length] = new BiQuad(0, -1, -2 * pole.re, Complex.AbsSqr(pole));
            }

            double omegaRef = 2 * Math.Atan(Math.Sqrt(omega0 * omega1));

            NormalizeBiQuads(biQuads, omegaRef);
        }

        private Complex[] GetComplexPolesLP(int order, double omega0)
        {
            Complex[] poles = new Complex[order / 2];

            for (int i = 0; i < poles.Length; i++)
            {
                double angle = (double)(2 * i + 1) / order * Math.PI / 2;
                poles[i] = omega0 * new Complex(-Math.Sin(angle), Math.Cos(angle));
            }

            return poles;
        }

        private Complex[] GetComplexPolesHP(int order, double omega0)
        {
            Complex[] poles = new Complex[order / 2];

            for (int i = 0; i < poles.Length; i++)
            {
                double angle = (double)(2 * i + 1) / order * Math.PI / 2;
                poles[i] = omega0 / new Complex(-Math.Sin(angle), Math.Cos(angle));
            }

            return poles;
        }

        private Complex[] GetComplexPolesBP(int order, double omega0, double omega1)
        {
            Complex[] poles = new Complex[2 *(order / 2)];

            double omega2 = Math.Sqrt(omega0 * omega1);

            for (int i = 0; i < poles.Length / 2; i++)
            {
                double angle = (double)(2 * i + 1) / order * Math.PI / 2;
                Complex p = (omega1 - omega0) / omega2 * new Complex(-Math.Sin(angle), Math.Cos(angle));
                
                double q = 0.5 * (4 - Math.Pow(p.re, 2) + Math.Pow(p.im, 2));
                double u = Math.Sqrt(q + Math.Sqrt(Math.Pow(q, 2) + Math.Pow(p.re * p.im, 2)));
                double v = p.re * p.im / u;
                
                poles[i] = 0.5 * omega2 * new Complex(p.re + v, p.im + u);
                poles[i + poles.Length / 2] = 0.5 * omega2 * new Complex(p.re - v, -p.im + u);
            }

            return poles;
        }

        private void BilinearTransform(Complex[] poles)
        {
            for (int i = 0; i < poles.Length; i++)
            {
                poles[i] = BilinearTransform(poles[i]);
            }
        }

        private Complex BilinearTransform(Complex pole)
        {
            return (1 + pole) / (1 - pole);
        }

        private double BilinearTransform(double pole)
        {
            return (1 + pole) / (1 - pole);
        }

        private void NormalizeBiQuads(BiQuad[] biQuads, double omegaRef)
        {
            Complex z_1 = new Complex(Math.Cos(omegaRef), -Math.Sin(omegaRef));
            double scale;

            for (int i = 0; i < biQuads.Length; i++)
            {
                Complex numerator = (biQuads[i].b0 + biQuads[i].b1 * z_1 + biQuads[i].b2 * z_1 * z_1);
                Complex denominator = (biQuads[i].a0 + biQuads[i].a1 * z_1 + biQuads[i].a2 * z_1 * z_1);
                scale = 1 / Math.Sqrt(Complex.AbsSqr(numerator / denominator));
                biQuads[i].b0 *= scale;
                biQuads[i].b1 *= scale;
                biQuads[i].b2 *= scale;
            }
        }

        public double FrequencyResponse(double f, double fs)
        {
            double omega = 2 * Math.PI * f / fs;
            Complex Z_1 = new Complex(Math.Cos(omega), -Math.Sin(omega));
            double frequencyResponse = 1;

            for (int i = 0; i < biQuads.Length; i++)
            {
                Complex numerator = (biQuads[i].b0 + biQuads[i].b1 * Z_1 + biQuads[i].b2 * Z_1 * Z_1);
                Complex denominator = (biQuads[i].a0 + biQuads[i].a1 * Z_1 + biQuads[i].a2 * Z_1 * Z_1);

                frequencyResponse *= Math.Sqrt(Complex.AbsSqr(numerator / denominator));
            }

            return frequencyResponse;
        }

        public double f0;
        public double f1;
        public double fc;

        private BiQuad[] biQuads;
        private double[] input;
        private double[] output;
        private IIR iir;

    }
}
