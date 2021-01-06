using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace DimensionalityReduction.PCA.Utils
{
    /// <summary>
    /// This class contains methods to deal with Bitmap (.bmp) images
    /// </summary>
    public class ImageUtils
    {

        /// <summary>
        /// Read (.bmp) gray scale image from a file path and output double[][] array
        /// </summary>
        /// <param name="filePath">full path to the bmp image</param>
        /// <returns>double[][] array represent the image pixel</returns>
        public static double[][] getImageAsDoubleArray(string filePath)
        {
            if (!filePath.EndsWith(".bmp"))
            {
                throw new InvalidDataException("Only bitmap image (.bmp) is supported at the moment");
            }

            Bitmap srcImg = new Bitmap(filePath);
            int XBound = srcImg.Width;
            int YBound = srcImg.Height;

            double[][] result = new double[YBound][];
            for (int y = 0; y < YBound; y++)
            {
                result[y] = new double[XBound];
                for (int x = 0; x < XBound; x++)
                {
                    result[y][x] = srcImg.GetPixel(x, y).R; //; gray scale bitmap image have A=255, R = G = B
                }
            }

            return result;
        }

        /// <summary>
        /// store the data from double[][] into a bmp image
        /// </summary>
        /// <param name="filePath">full file path</param>
        /// <param name="imageData">double[][] array represent pixel of the image</param>
        public static void saveDoubleArrayToImage(string filePath, double[][] imageData)
        {
            if (!filePath.EndsWith(".bmp"))
            {
                throw new InvalidDataException("Only bitmap image (.bmp) is supported at the moment");
            }

            int height = imageData.GetLength(0);
            int width = imageData[0].GetLength(0);

            Bitmap destImage = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int r = Convert.ToInt32(imageData[y][x]);
                    r = r >= 0 ? r : 0;
                    r = r <= 255 ? r : 255; 
                    int g = r;
                    int b = r;
                    destImage.SetPixel(x, y, Color.FromArgb(255, r, g, b));
                }
            }
            
            destImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);   
        }
    }
}
