using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Sobel_Detection
{


    /// <summary>
    /// Main class for the sobel filter algorithm using Ipipeline
    /// /// <param name="double[,,]" this is input to Ipipeline as Three-D doube  data></param>
    /// <param name="double[,,]" this is output to Ipipleine as Three-D doube data></param>
    /// </summary>
    public class SobelConvolutionFilter : IPipelineModule<double[,,], double[,,]>
    {

        /// <summary>
        /// sobel operator for Horizontal pixel Image
        /// </summary>
        public static double[,] XSobel
        {
            get
            {
                return new double[,]
                {
                    { -1, 0, 1 },
                    { -2, 0, 2 },
                    { -1, 0, 1 }
                };
            }
        }


        /// <summary>
        /// Sobel operator kernel for vertical pixel changes
        /// </summary>
        public static double[,] YSobel
        {
            get
            {
                return new double[,]
                {
                    {  1,  2,  1 },
                    {  0,  0,  0 },
                    { -1, -2, -1 }
                };
            }
        }

        /// <summary>
        /// Method of Interface Ipipline
        /// </summary>
        /// <param name="data" this is the double data coming from unitest.></param>
        /// <param name="ctx" this define the Interface IContext for Data descriptor.></param>
        /// <returns It Run convolutional filter with parameter "data" X sobel and y sobel as written as double data> </returns>
        public double[,,] Run(double[,,] data,IContext ctx)
        {
            return ConvolutionFilter(data, XSobel, YSobel);
            
        }

        /// <summary>
        /// Taking the image data in double double array and applying kernel convoution to image data(pixel value) and after converting into grayscale and saving in to array
        /// and returing double data again after necessary conversion.
        /// </summary>
        /// <param name="data" coming from run method of Ipipeline interface></param>
        /// <param name="Xsobel" this is a sobel Kernel for Horizontal direction ></param>
        /// <param name="Ysobel" this is a sobel kernel for Vertical direction></param>
        /// <param name="grayscale" this is boolean datatype used for converting image into grayscale if set to true></param>
        /// <returns it return data in  three dimensional array> </returns>
        private double[,,] ConvolutionFilter(double[,,] data, double[,] Xsobel, double[,] Ysobel, bool grayscale=false)
        {
            Bitmap bitmap = new Bitmap(data.GetLength(0), data.GetLength(1));
            
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    int r = (int)data[i, j, 0];
                    int g = (int)data[i, j, 1];
                    int b = (int)data[i, j, 2];
                    bitmap.SetPixel(i, j, Color.FromArgb(255, r, g, b));
                }
            }
            
            int width = bitmap.Width;
            int height = bitmap.Height;


            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData inpdata = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = inpdata.Stride * inpdata.Height;

            byte[] pixelarray = new byte[bytes];
            byte[] resultArray = new byte[bytes];
            IntPtr addroffirstpixel = inpdata.Scan0;
            Marshal.Copy(addroffirstpixel, pixelarray, 0, bytes);
            bitmap.UnlockBits(inpdata);
            // convert the image into gray scale
            if (grayscale == true)
            {
                float rgb = 0;
                for (int i = 0; i < pixelarray.Length; i += 4)
                {
                    rgb = pixelarray[i] * .21f;
                    rgb += pixelarray[i + 1] * .71f;
                    rgb += pixelarray[i + 2] * .071f;
                    pixelarray[i] = (byte)rgb;
                    pixelarray[i + 1] = pixelarray[i];
                    pixelarray[i + 2] = pixelarray[i];
                    pixelarray[i + 3] = 255;
                }
            }

            //crete varibale for pixel data for each kernel

            double xr = 0.0;
            double xg = 0.0;
            double xb = 0.0;
            double yr = 0.0;
            double yg = 0.0;
            double yb = 0.0;
            double rt = 0.0;
            double gt = 0.0;
            double bt = 0.0;

            // This is how much  center pixel is offset from the border of the kernel
            //Sobel is 3x3, so center is 1 pixel from the kernel border
            int filterOffset = 1;
            int calcOffset = 0;
            int byteOffset = 0;


            for (int OffsetY = filterOffset; OffsetY < height - filterOffset; OffsetY++)
            {
                for (int OffsetX = filterOffset; OffsetX < width - filterOffset; OffsetX++)
                {
                    //reset rgb values to 0
                    xr = xg = xb = yr = yg = yb = 0;
                    rt = gt = bt = 0.0;

                    //position of the kernel center pixel
                    byteOffset = OffsetY * inpdata.Stride + OffsetX * 4;

                    //kernel calculations
                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + filterX * 4 + filterY * inpdata.Stride;

                            xb += (double)(pixelarray[calcOffset]) * Xsobel[filterY + filterOffset, filterX + filterOffset];
                            xg += (double)(pixelarray[calcOffset + 1]) * Xsobel[filterY + filterOffset, filterX + filterOffset];
                            xr += (double)(pixelarray[calcOffset + 2]) * Xsobel[filterY + filterOffset, filterX + filterOffset];
                            yb += (double)(pixelarray[calcOffset]) * Ysobel[filterY + filterOffset, filterX + filterOffset];
                            yg += (double)(pixelarray[calcOffset + 1]) * Ysobel[filterY + filterOffset, filterX + filterOffset];
                            yr += (double)(pixelarray[calcOffset + 2]) * Ysobel[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    //total rgb values for this pixel
                    bt = Math.Sqrt((xb * xb) + (yb * yb));
                    gt = Math.Sqrt((xg * xg) + (yg * yg));
                    rt = Math.Sqrt((xr * xr) + (yr * yr));

                    //set limits, bytes can hold values from 0 up to 255;
                    if (bt > 255) bt = 255;
                    else if (bt < 0) bt = 0;
                    if (gt > 255) gt = 255;
                    else if (gt < 0) gt = 0;
                    if (rt > 255) rt = 255;
                    else if (rt < 0) rt = 0;

                    //set new data in the other byte array for your image data
                    resultArray[byteOffset] = (byte)bt;
                    resultArray[byteOffset + 1] = (byte)gt;
                    resultArray[byteOffset + 2] = (byte)rt;
                    resultArray[byteOffset + 3] = 255;
                }
            }

            //Create new bitmap which will hold the processed data
            Bitmap resultImage = new Bitmap(width, height);

            // Lock bits into system memory
            BitmapData resultData = resultImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Copy from byte array that holds processed data to bitmap
            Marshal.Copy(resultArray, 0, resultData.Scan0, resultArray.Length);

            // Unlock bits from system memory
            resultImage.UnlockBits(resultData);

            // Return processed image

            double[,,] resultImageDouble = new double[width, height, 3];
           //convert the data in to double using conversion helper class
            ConversionHelper helper = new ConversionHelper();
            resultImageDouble = helper.ConvertBitmapToDouble(resultImage);

            return resultImageDouble;

        }
    }
}