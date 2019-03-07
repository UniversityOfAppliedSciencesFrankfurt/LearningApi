using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectCenterModule
{
    public class DigitImage
    {
        public int width; // 28
        public int height; // 28
        public byte[][] pixels; // 0(white) - 255(black)
        public byte label; // '0' - '9'
        /*
         This Class declare the size of the MNIST- Images.
         The size is always 28x28.
         It shows the individual Digits of the Datasets.

      
             */
        public DigitImage()
        {
        }

        public DigitImage(int width, int height,
          byte[][] pixels, byte label)
        {
            this.width = width;
            this.height = height;
            this.pixels = new byte[height][];
            for (int i = 0; i < this.pixels.Length; ++i)
                this.pixels[i] = new byte[width];
            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                    this.pixels[i][j] = pixels[i][j];
            this.label = label;
        }
    }
}
