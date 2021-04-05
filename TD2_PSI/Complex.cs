using System;
using System.Collections.Generic;
using System.Text;

namespace TD2_PSI
{
    class Complex
    {
        double real;
        double imaginary;

        public double Getreal
        {
            get { return real; }
        }
        public double Getimaginary
        {
            get { return imaginary; }
        }
        public Complex(double real, double imaginary)
            {
            this.real = real;
            this.imaginary = imaginary;
            }

        public void Square()
        {
            double tmp = real * real - imaginary * imaginary ;
            imaginary = 2 * imaginary * real  ;
            real = tmp;
        }
        public double Magnitude()  //norm
        {
            return Math.Sqrt((real * real) + (imaginary * imaginary));
        }
       
        public double Test()  //norm
        {
            return (real * real) - (imaginary * imaginary);
        }

        public void Add(Complex c)
        {
            real += c.real ;
            imaginary += c.imaginary ;
        }
    }
}
