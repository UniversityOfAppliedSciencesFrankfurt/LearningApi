using LaplacianOfGaussianSE;
using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;

namespace LOGTest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// to bring the bitmap from the 3D array
        /// </summary>
        /// <param name="filterimageArray"></param>
        /// <returns></returns>
        public static Bitmap changeFrom3dArrayToBitmap(double[,,] filterimageArray)
        {

            Bitmap bitmapreslut = new Bitmap(filterimageArray.GetLength(0), filterimageArray.GetLength(1));

            for (int i = 0; i < filterimageArray.GetLength(0); i++)
            {
                for (int j = 0; j < filterimageArray.GetLength(1); j++)
                {
                    int red = (int)filterimageArray[i, j, 0];
                    int green = (int)filterimageArray[i, j, 1];
                    int blue = (int)filterimageArray[i, j, 2];
                    bitmapreslut.SetPixel(i, j, Color.FromArgb(255, red, green, blue));
                }

            }
            return bitmapreslut;
        }

        /// <summary>
        /// here to covert the bitmap image to 3D array
        /// </summary>
        /// <param name="bitmapimage"></param>
        /// <returns></returns>
        public static double[,,] BitmapTo3dArray(Bitmap bitmapimage)
        {
            int resultingimgWidth = bitmapimage.Width;
            int resultingimgHeight = bitmapimage.Height;

            //Required values for RGB 0 -> Red, 1 -> Green, 2 -> Blue
            double[,,] imageArray = new double[resultingimgWidth, resultingimgHeight, 3];

            for (int i = 0; i < resultingimgWidth; i++)
            {
                for (int j = 0; j < resultingimgHeight; j++)
                {
                    Color variablecolor = bitmapimage.GetPixel(i, j);
                    imageArray[i, j, 0] = variablecolor.R;
                    imageArray[i, j, 1] = variablecolor.G;
                    imageArray[i, j, 2] = variablecolor.B;
                }
            }

            return imageArray;
        }
        
        /// <summary>
        /// Load image Method for bitmap to converted to double[,,]
        /// </summary>
        /// <param name="sourcefile"></param>
        /// <returns></returns>
        public static double[,,] UploadImage(string sourcefile)
        {
            Bitmap sourceimage = new Bitmap(sourcefile);

            return BitmapTo3dArray(sourceimage);
        }

        /// <summary>
        /// The double[,,] will be converted back to bitmap and save the  method after runnig the algorithm
        /// </summary>
        /// <param name="bitmapimageArray"></param>
        /// <param name="sourcefile"></param>
        public static void SaveImage(double[,,] bitmapimageArray, string sourcefile)
        {
            Bitmap lockimage = changeFrom3dArrayToBitmap(bitmapimageArray);

            lockimage.Save(sourcefile);
        }

        /// <summary>
        /// This method is to  get the input image from InputImages and image will load in 3D array.
        ///  outcome of image will be saved in OutputImages
        ///  intiating method here
        /// </summary>
        [TestMethod]
        public void RunApi()
        {
            LearningApi api = new LearningApi();
            LaplacianOfgaussian LOG = new LaplacianOfgaussian();
            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                string LocalDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagepath = Path.Combine(LocalDirectory, "InputImages\\parrot.jpg");
                double[,,] imagedata = UploadImage(imagepath);
                return imagedata;
            });

            api.UseLaplacianOfgaussian();

            double[,,] imageoutput = api.Run() as double[,,];

            Assert.IsNotNull(imageoutput);

            string Directorybase = AppDomain.CurrentDomain.BaseDirectory;
            string outputPath = Path.Combine(Directorybase, "OutputImages\\parrot.jpg");
            SaveImage(imageoutput, outputPath);
        }

        

    }
}


