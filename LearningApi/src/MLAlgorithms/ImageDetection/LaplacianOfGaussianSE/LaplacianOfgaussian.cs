using System;
using LearningFoundation;

namespace LaplacianOfGaussianSE
{
    /// <summary>
    /// image edge detection with laplacian of gaussian by ipipline algorithm
    /// </summary>
    public class LaplacianOfgaussian : IPipelineModule<double[,,],double[,,]>
    {
        /// <summary>
        /// kernel filter of laplacian3x3 matrix
        /// </summary>
        /* public static double[,] Laplacian3x3
         {
             get
             {
                 return new double[,]
                 { { -1, -1, -1, },
                   { -1,  8, -1, },
                   { -1, -1, -1, },
                 };
             }
         }*/

        /// <summary>
        /// kernel filter of gaussian 3x3 matrix
        /// </summary>
        /* public static double[,] Gaussian3x3
         {
             get
             {
                 return new double[,]
                 { { 1, 2, 1, },
                   { 2, 4, 2, },
                   { 1, 2, 1, }
                 };
             }
         }*/

        /// <summary>
        /// kernel filter of Laplacian and Gaussian 3x3 matrix, kernel for pixel changes
        /// </summary>

        public static double[,] LaplacianOfGaussian
        {
            get
            {
                return new double[,]
                { {  0,  0, -1,  0,  0 },
                  {  0, -1, -2, -1,  0 },
                  { -1, -2, 16, -2, -1 },
                  {  0, -1, -2, -1,  0 },
                  {  0,  0, -1,  0,  0 }
                };
            }
        }
        
        /// <summary>
        /// This method takes the LoG kernel and applies convolution filter and also converts double[,,] image into byte[] and vice versa 
        /// </summary>
        /// <param name="sourceimagebitmap"></param>
        /// <param name="Matrixfilter"></param>
        /// <param name="factor"></param>
        /// <param name="bias"></param>
        /// <param name="grayscale"></param>
        /// <returns></returns>
        public double[,,] Convolutionfilter(double[,,] sourceimagebitmap,
                                              double[,] Matrixfilter,
                                                  double factor = 1,
                                                  int bias = 0,
                                                  bool grayscale = true)
        {
            int filterimagewidth = sourceimagebitmap.GetLength(0);
            int filterimageHeight = sourceimagebitmap.GetLength(1);
            int filterimageStride = filterimagewidth * 4;

            // Converting double[,,] Source Image to byte[] for further processing
            byte[] pixelBuffer = new byte[filterimageStride * filterimageHeight];
            byte[] resultBufferdata = new byte[filterimageStride * filterimageHeight];
            
            for (int i = 0; i < filterimageHeight; i++)
            {
                for (int j = 0; j < filterimagewidth; j++)
                {
                    int r = (int)sourceimagebitmap[j, i, 0];
                    int g = (int)sourceimagebitmap[j, i, 1];
                    int b = (int)sourceimagebitmap[j, i, 2];
                    pixelBuffer[filterimagewidth * 4 * i + 4 * j] = (byte)b;
                    pixelBuffer[filterimagewidth * 4 * i + 4 * j + 1] = (byte)g;
                    pixelBuffer[filterimagewidth * 4 * i + 4 * j + 2] = (byte)r;
                    pixelBuffer[filterimagewidth * 4 * i + 4 * j + 3] = 255;
                }
            }

            // convertion of input image to gray scale 
            if (grayscale == true)
            {
                float redgreenblue = 0;

                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    redgreenblue = pixelBuffer[k] * 0.11f;
                    redgreenblue += pixelBuffer[k + 1] * 0.59f;
                    redgreenblue += pixelBuffer[k + 2] * 0.3f;


                    pixelBuffer[k] = (byte)redgreenblue;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            // variables RED GREEN AND BLUE colors
            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            int filterWidth = Matrixfilter.GetLength(1);
            int filterHeight = Matrixfilter.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY <
                filterimageHeight - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    filterimagewidth - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    byteOffset = offsetY *
                                 filterimageStride +
                                 offsetX * 4;

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {

                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * filterimageStride);

                            blue += (double)(pixelBuffer[calcOffset]) *
                                    Matrixfilter[filterY + filterOffset,
                                                        filterX + filterOffset];

                            green += (double)(pixelBuffer[calcOffset + 1]) *
                                     Matrixfilter[filterY + filterOffset,
                                                        filterX + filterOffset];

                            red += (double)(pixelBuffer[calcOffset + 2]) *
                                   Matrixfilter[filterY + filterOffset,
                                                      filterX + filterOffset];
                        }
                    }

                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;
                    // giving values either 0 or 255
                    if (blue > 255)
                    { blue = 255; }
                    else if (blue < 0)
                    { blue = 0; }

                    if (green > 255)
                    { green = 255; }
                    else if (green < 0)
                    { green = 0; }

                    if (red > 255)
                    { red = 255; }
                    else if (red < 0)
                    { red = 0; }

                    resultBufferdata[byteOffset] = (byte)(blue);
                    resultBufferdata[byteOffset + 1] = (byte)(green);
                    resultBufferdata[byteOffset + 2] = (byte)(red);
                    resultBufferdata[byteOffset + 3] = 255;
                }
            }

            // Converting the  byte[] to double[,,] Result Image
            double[,,] resultImageDouble = new double[filterimagewidth, filterimageHeight, 3];
            for (int i = 0; i < filterimageHeight; i++)
            {
                for (int j = 0; j < filterimagewidth; j++)
                {
                    resultImageDouble[j, i, 0] = resultBufferdata[filterimagewidth * 4 * i + 4 * j + 2];
                    resultImageDouble[j, i, 1] = resultBufferdata[filterimagewidth * 4 * i + 4 * j + 1];
                    resultImageDouble[j, i, 2] = resultBufferdata[filterimagewidth * 4 * i + 4 * j ];
                }
            }

            // return the processed image
            return resultImageDouble;

        }
        /// <summary>
        /// Interface IPipeline run method
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public double[,,] Run(double[,,] data, IContext ctx)
        {

            return Convolutionfilter(data, LaplacianOfGaussian);

        }
    }
}

    
