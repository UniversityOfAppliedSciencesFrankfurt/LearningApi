// Copyright (c) daenet GmbH / Frankfurt University of Applied Sciences. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Drawing;
using LearningFoundation.EuclideanColorFilter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearningFoundation;
using System.IO;
using System;

namespace LearningFoundation.EuclideanColorFilterTests
{
    [TestClass]
    public class EuclideanFilterModuleTester
    {
        /// <summary>
        /// Converts Bitmap into 3 dimensional array
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static double[,,] ConvertFromBitmapTo3dArray(Bitmap bitmap)
        {
            int imgWidth = bitmap.Width;
            int imgHeight = bitmap.Height;

            //0 -> R, 1 -> G, 2 -> B
            double[,,] imageArray = new double[imgWidth, imgHeight , 3];

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
        /// Converts the 3 dimensional array back to Bitmap
        /// </summary>
        /// <param name="imageArray"></param>
        /// <returns></returns>
        public static Bitmap ConvertFrom3dArrayToBitmap(double[,,] imageArray)
        {
            int imgWidth = imageArray.GetLength(0);
            int imgHeight = imageArray.GetLength(1);

            //0 -> R, 1 -> G, 2 -> B
            Bitmap bitmap = new Bitmap(imgWidth, imgHeight);

            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    Color color = GetAndSetPixels.GetPixel(imageArray, i, j);
                    bitmap.SetPixel(i, j, color);
                }
            }

            return bitmap;
        }
                   
            /// <summary>
            /// Load Method for loading a Bitmap, which will be converted to double[,,]
            /// </summary>
            /// <param name="filename"></param>
            /// <returns></returns>
            public static double[,,] Load(string filename)
        {
            Bitmap bitmap = new Bitmap(filename);

            return ConvertFromBitmapTo3dArray(bitmap);
        }

        /// <summary>
        /// Save Method after running the Algorithm. The double[,,] will be converted back to bitmap
        /// </summary>
        /// <param name="imageArray"></param>
        /// <param name="filename"></param>
        public static void Save(double[,,] imageArray, string filename)
        {
            Bitmap bitmap = ConvertFrom3dArrayToBitmap(imageArray);

            bitmap.Save(filename);
        }

        /// <summary>
        /// Tests the Algorithm. Bitmap will be loaded and converted to double[,,]. After that the euclidean filter algorithm will be executed.
        /// Then the filtered Picture will be converted back to Bitmap and saved in FilteredImage folder.
        /// Please notice you have to specify the Parameters in EuclideanFilterModule Instance (Color Center and float radius).
        /// </summary>
        [TestMethod]
        public void RunApi()
        {
            LearningApi api = new LearningApi();
            EuclideanFilterModule module = new EuclideanFilterModule(Color.FromArgb(255, 250, 150, 100), 180.0f);

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDirectory, "TestPictures\\11.jpg");
                double[,,] data = Load(path);
                return data;
            });

            api.AddModule(module);

            double[,,] output = api.Run() as double[,,];

            Assert.IsNotNull(output);

            string baseDirectory2 = AppDomain.CurrentDomain.BaseDirectory;
            string outPath = Path.Combine(baseDirectory2, "FilteredImage\\FilteredImage_Nr11.jpg");
            Save(output, outPath);
        }

        /// <summary>
        /// This TestMethod gerneretes an 2x2 bitmap. The center Color is red, and the radius is 1.0.
        /// So every other color, which is not exactly red (255,0,0), should be set to black (in this case the last two Asserts)
        /// </summary>
        [TestMethod]
        public void GenerateImageAndRunApi()
        {
            LearningApi api = new LearningApi();
            EuclideanFilterModule module = new EuclideanFilterModule(Color.FromArgb(255, 255, 0, 0), 1.0f);
            Bitmap bitmap = new Bitmap(2, 2);

            bitmap.SetPixel(0, 0, Color.FromArgb(255, 0, 0));
            bitmap.SetPixel(0, 1, Color.FromArgb(255, 0, 0));
            bitmap.SetPixel(1, 0, Color.FromArgb(0, 255, 0));
            bitmap.SetPixel(1, 1, Color.FromArgb(0, 0, 255));

            double[,,] data = ConvertFromBitmapTo3dArray(bitmap);

            api.UseActionModule<double[,,], double[,,]>((input, ctx) => { return data; });

            api.AddModule(module);

            double[,,] output = api.Run() as double[,,];

            Bitmap outputAsBitmap = ConvertFrom3dArrayToBitmap(output);
            
            Assert.AreEqual(Color.FromArgb(255, 0, 0), outputAsBitmap.GetPixel(0, 0));
            Assert.AreEqual(Color.FromArgb(255, 0, 0), outputAsBitmap.GetPixel(0, 1));
            Assert.AreEqual(Color.FromArgb(0, 0, 0), outputAsBitmap.GetPixel(1, 0));
            Assert.AreEqual(Color.FromArgb(0, 0, 0), outputAsBitmap.GetPixel(1, 1));
        }
               
    }
}