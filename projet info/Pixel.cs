using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projet_info
{
    public class Pixel
    {
        byte blue;
        byte green;
        byte red;
        public byte rgb;

        public Pixel(byte r, byte g, byte b)
        {
            this.red = r;
            this.green = g;
            this.blue = b;
            this.rgb = (byte)((r + g + b) / 3);
        }

        #region Propriétés 

        public byte Blue
        {
            get { return this.blue; }
            set { this.blue = value; }
        }

        public byte Green
        {
            get { return this.green; }
            set { this.green = value; }
        }

        public byte Red
        {
            get { return this.red; }
            set { this.red = value; }
        }

        #endregion


    }
}