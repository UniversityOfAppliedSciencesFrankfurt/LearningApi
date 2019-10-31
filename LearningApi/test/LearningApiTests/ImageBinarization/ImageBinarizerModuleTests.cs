using LearningFoundation;
using LearningFoundation.ImageBinarizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace LearningFoundation.Test.ImageBinarizerTest
{
/// <summary>
/// Main class for testing of Image Binarization algorithm using IPipeline
/// </summary>
    [TestClass]
    public class ImageBinarizerModuleTests
    {
        /// <summary>
        /// This method is used to Test the Algorithm by taking the Input Image and setting the parameters
        /// Bitmap image will be loaded from Image Folder and converted into double[,,].
        /// After that Image Binarization will be executed and hence the Binarized Image will be formed.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {

            Dictionary<String, int> imageParams = new Dictionary<string, int>();
            imageParams.Add("imageWidth", 60);
            imageParams.Add("imageHeight", 60);
            imageParams.Add("redThreshold", 0);
            imageParams.Add("greenThreshold", 0);
            imageParams.Add("blueThreshold", 0);

            var api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                string path = Path.Combine(AppContext.BaseDirectory, "Images\\a.png");

                Bitmap bitmap = new Bitmap(path);

                int imgWidth = bitmap.Width;
                int imgHeight = bitmap.Height;
                double[,,] data = new double[imgWidth, imgHeight, 3];

                for (int i = 0; i < imgWidth; i++)
                {
                    for (int j = 0; j < imgHeight; j++)
                    {
                        Color color = bitmap.GetPixel(i, j);
                        data[i, j, 0] = color.R;
                        data[i, j, 1] = color.G;
                        data[i, j, 2] = color.B;
                    }
                }
                return data;
            });

            api.UseImageBinarizer(imageParams);
            var result = api.Run() as double[,,];

            StringBuilder stringArray = new StringBuilder();
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    stringArray.Append(result[i, j, 0]);
                }
                stringArray.AppendLine();
            }
            using (StreamWriter writer = File.CreateText(Path.Combine(AppContext.BaseDirectory, "Images\\a.txt")))
            {
                writer.Write(stringArray.ToString());
            }
        }
        /// <summary>
        /// This method is used to Test the Algorithm by taking a Black and White Image and setting the parameters
        /// Bitmap image will be loaded from Image Folder and converted into double[,,].
        /// After that Image Binarization will be executed and hence the Binarized Image will be formed.
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {

            Dictionary<String, int> imageParams = new Dictionary<string, int>();
            imageParams.Add("imageWidth", 60);
            imageParams.Add("imageHeight", 60);
            imageParams.Add("redThreshold", 0);
            imageParams.Add("greenThreshold", 0);
            imageParams.Add("blueThreshold", 0);

            var api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                string path = Path.Combine(AppContext.BaseDirectory, "Images\\black and white.jpg");

                Bitmap bitmap = new Bitmap(path);

                int imgWidth = bitmap.Width;
                int imgHeight = bitmap.Height;
                double[,,] data = new double[imgWidth, imgHeight, 3];

                for (int i = 0; i < imgWidth; i++)
                {
                    for (int j = 0; j < imgHeight; j++)
                    {
                        Color color = bitmap.GetPixel(i, j);
                        data[i, j, 0] = color.R;
                        data[i, j, 1] = color.G;
                        data[i, j, 2] = color.B;
                    }
                }
                return data;
            });

            api.UseImageBinarizer(imageParams);
            var result = api.Run() as double[,,];

            StringBuilder stringArray = new StringBuilder();
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    stringArray.Append(result[i, j, 0]);
                }
                stringArray.AppendLine();
            }
            using (StreamWriter writer = File.CreateText(Path.Combine(AppContext.BaseDirectory, "Images\\black and white.txt")))
            {
                writer.Write(stringArray.ToString());
            }
        }
        /// <summary>
        /// This method is used to Test the Algorithm by taking a Coloured Image and setting the parameters
        /// Bitmap image will be loaded from Image Folder and converted into double[,,].
        /// After that Image Binarization will be executed and hence the Binarized Image will be formed.
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {

            Dictionary<String, int> imageParams = new Dictionary<string, int>();
            imageParams.Add("imageWidth", 60);
            imageParams.Add("imageHeight", 60);
            imageParams.Add("redThreshold", 0);
            imageParams.Add("greenThreshold", 0);
            imageParams.Add("blueThreshold", 0);

            var api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                string path = Path.Combine(AppContext.BaseDirectory, "Images\\coloured.jpg");

                Bitmap bitmap = new Bitmap(path);

                int imgWidth = bitmap.Width;
                int imgHeight = bitmap.Height;
                double[,,] data = new double[imgWidth, imgHeight, 3];

                for (int i = 0; i < imgWidth; i++)
                {
                    for (int j = 0; j < imgHeight; j++)
                    {
                        Color color = bitmap.GetPixel(i, j);
                        data[i, j, 0] = color.R;
                        data[i, j, 1] = color.G;
                        data[i, j, 2] = color.B;
                    }
                }
                return data;
            });

            api.UseImageBinarizer(imageParams);
            var result = api.Run() as double[,,];

            StringBuilder stringArray = new StringBuilder();
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    stringArray.Append(result[i, j, 0]);
                }
                stringArray.AppendLine();
            }
            using (StreamWriter writer = File.CreateText(Path.Combine(AppContext.BaseDirectory, "Images\\coloured.txt")))
            {
                writer.Write(stringArray.ToString());
            }
        }
        /// <summary>
        /// This method is used to Test the Algorithm by taking a Grayscale Image and setting the parameters
        /// Bitmap image will be loaded from Image Folder and converted into double[,,].
        /// After that Image Binarization will be executed and hence the Binarized Image will be formed.
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {

            Dictionary<String, int> imageParams = new Dictionary<string, int>();
            imageParams.Add("imageWidth", 60);
            imageParams.Add("imageHeight", 60);
            imageParams.Add("redThreshold", 0);
            imageParams.Add("greenThreshold", 0);
            imageParams.Add("blueThreshold", 0);

            var api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                string path = Path.Combine(AppContext.BaseDirectory, "Images\\grayscale.jpg");

                Bitmap bitmap = new Bitmap(path);

                int imgWidth = bitmap.Width;
                int imgHeight = bitmap.Height;
                double[,,] data = new double[imgWidth, imgHeight, 3];

                for (int i = 0; i < imgWidth; i++)
                {
                    for (int j = 0; j < imgHeight; j++)
                    {
                        Color color = bitmap.GetPixel(i, j);
                        data[i, j, 0] = color.R;
                        data[i, j, 1] = color.G;
                        data[i, j, 2] = color.B;
                    }
                }
                return data;
            });

            api.UseImageBinarizer(imageParams);
            var result = api.Run() as double[,,];

            StringBuilder stringArray = new StringBuilder();
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    stringArray.Append(result[i, j, 0]);
                }
                stringArray.AppendLine();
            }
            using (StreamWriter writer = File.CreateText(Path.Combine(AppContext.BaseDirectory, "Images\\grayscale.txt")))
            {
                writer.Write(stringArray.ToString());
            }
        }
    }
}

