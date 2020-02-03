using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EuclideanFilter
{

    /// <summary>
    /// 
    /// </summary>
    public static class GetAndSetPixels
    {
        /// <summary>
        /// Setpixel method, which will be used to set the RGB-values for the current row/col
        /// Either the RGB-value of the current pixel is set without change or set to black.
        /// This method is equal to bitmap.setpixel 
        /// </summary>
        /// <param name="imageArray"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="color"></param>
        public static void SetPixel(double[,,] imageArray, int row, int col, Color color)
        {
            imageArray[row, col, 0] = color.R;
            imageArray[row, col, 1] = color.G;
            imageArray[row, col, 2] = color.B;
        }

        /// <summary>
        /// Getpixel method, which gets the RGB-Value of the current pixel (row/col). This method is equal to bitmap.getpixel
        /// </summary>
        /// <param name="imageArray"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Color GetPixel(double[,,] imageArray, int row, int col)
        {
            double r = imageArray[row, col, 0];
            double g = imageArray[row, col, 1];
            double b = imageArray[row, col, 2];

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
