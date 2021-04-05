using System;
using System.Collections.Generic;
using System.Text;

namespace TD2_PSI
{
    public class Pixel
    {
        public byte r;
        public byte g;
        public byte b;

        public byte R
        {
            get { return (byte)r; }
            set { r = value; }
        }
        public byte G
        {
            get { return (byte)g; }
            set { g = value; }
        }
        public byte B
        {
            get { return (byte)b; }
            set { b = value; }
        }

        public byte MoyennePix()
        {
            return (byte)(((int)r + (int)g + (int)b) / 3);
        }


        public Pixel(byte r, byte g, byte b)
        {
            this.r = r;
            this.b = b;
            this.g = g;
        }

        

        // array form
        /*
        public Pixel(double[] arr)
        {
            r = arr[0];
            g = arr[1];
            b = arr[2];
        }
        */

        public  string toString()
        {
            return "(" + Convert.ToString(R) + " " + Convert.ToString(G) + " " + Convert.ToString(B) + ")";
        }




    }
}
