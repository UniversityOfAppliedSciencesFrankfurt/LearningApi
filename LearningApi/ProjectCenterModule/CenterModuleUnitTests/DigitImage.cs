using System;
using System.Collections.Generic;
using System.Text;

namespace CenterModuleUnitTests
{ /// <summary>
  ///  This Class declare the size of the MNIST- Images.
  ///  The size is always 28x28.
  ///  It shows the individual Digits of the Datasets.
  /// </summary>
   public class DigitImage
    {
        public int Width; // 28
        public int Height; // 28
        public byte[][] Pixels; // 0(white) - 255(black)
        public byte Label; // '0' - '9'
        

        public DigitImage(int width, int height,
          byte[][] pixels, byte label)
        {
            Width = width;
            Height = height;
            Pixels = new byte[height][];
            for (int i = 0; i < this.Pixels.Length; ++i)
                Pixels[i] = new byte[width];
            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                    Pixels[i][j] = pixels[i][j];
                    Label = label;
        }
    }
}

