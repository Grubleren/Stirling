using System;

namespace JH.Calculations
{
    public struct Complex
    {
        public double re;
        public double im;

        public Complex(double re, double im)
        {
            this.re = re;
            this.im = im;
        }

        public static Complex Conjugate(Complex c)
        {
            return new Complex(c.re, -c.im);
        }

        public static double AbsSqr(Complex c)
        {
            return c.re * c.re + c.im * c.im;
        }

        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.re + c2.re, c1.im + c2.im);
        }

        public static Complex operator +(double d, Complex c)
        {
            return new Complex(d, 0) + c;
        }

        public static Complex operator +(Complex c, double d)
        {
            return d + c;
        }

        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.re - c2.re, c1.im - c2.im);
        }

        public static Complex operator -(double d, Complex c)
        {
            return new Complex(d, 0) - c;
        }

        public static Complex operator -(Complex c, double d)
        {
            return c - new Complex(d, 0);
        }

        public static Complex operator -(Complex c)
        {
            return new Complex(-c.re, -c.im);
        }

        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.re * c2.re - c1.im * c2.im, c1.re * c2.im + c1.im * c2.re);
        }

        public static Complex operator *(double d, Complex c)
        {
            return new Complex(c.re * d, c.im * d);
        }

        public static Complex operator *(Complex c, double d)
        {
            return new Complex(c.re * d, c.im * d);
        }

        public static Complex operator /(Complex c1, Complex c2)
        {
            Complex n = c1 * Complex.Conjugate(c2);

            return n / (c2.re * c2.re + c2.im * c2.im);
        }

        public static Complex operator /(double d, Complex c)
        {
            return Complex.Conjugate(c) * d / (c.re * c.re + c.im * c.im);
        }

        public static Complex operator /(Complex c, double d)
        {
            return new Complex(c.re / d, c.im / d);
        }
    }
}
