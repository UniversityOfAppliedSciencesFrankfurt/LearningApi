using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Sobel_Detection;

namespace Sobel_Detection
{
    public class ConversionHelper
    {
        /// <summary>
        /// convert the image into 3 dimentional array
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public double[,,] ConvertBitmapToDouble( Bitmap bitmap)
        {

            int imgWidth = bitmap.Width;
            int imgHeight = bitmap.Height;

            
            double[,,] imageArray = new double[imgWidth, imgHeight, 3];

            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    imageArray[i, j, 0] = color.R;
                    imageArray[i, j, 1] = color.G;
                    imageArray[i, j, 2] = color.B;
                }
            }

            return imageArray;
        }

        /// <summary>
        /// convert 3 dimentional array to bitmap
        /// </summary>
        /// <param name="imageAray"></param>
        /// <returns></returns>
        public Bitmap ConvertDoubleToBitmap(double[,,] imageAray)
        {

            Bitmap resbitmap = new Bitmap(imageAray.GetLength(0), imageAray.GetLength(1));

            for (int i = 0; i < imageAray.GetLength(0); i++)
            {
                for (int j = 0; j < imageAray.GetLength(1); j++)
                {
                    int r = (int)imageAray[i, j, 0];
                    int g = (int)imageAray[i, j, 1];
                    int b = (int)imageAray[i, j, 2];
                    resbitmap.SetPixel(i, j, Color.FromArgb(255, r, g, b));
                }
               
            }
            return resbitmap;
        }

        

        }
    }
    

