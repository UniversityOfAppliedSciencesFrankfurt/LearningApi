using GaussianAndMeanFilter;
using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace LearningFoundation.Test
{
    [TestClass]
    public class GaussianAndMeanFilterTest
    {

        private string appPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()))) + "/GaussianAndMeanFilter";

        private string outputPath;

        public GaussianAndMeanFilterTest()
        {
            outputPath = appPath + "/outputPic/";

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

        }

        private void SaveImage(Bitmap bitmap, string filename)
        {
            Image img = (Image)bitmap;

            img.Save(outputPath + filename );
        }

        /// <summary>
        /// Unit test for Mean Filter
        /// </summary>
        [TestMethod]
        public void MeanF()
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                Bitmap myBitmap = new Bitmap($"{appPath}/TestPicture/HandWritten1.jpg");

                double[,,] data = new double[myBitmap.Width, myBitmap.Height, 3];

                for (int x = 0; x < myBitmap.Width; x++)
                {
                    for (int y = 0; y < myBitmap.Height; y++)
                    {
                        Color pixelColor = myBitmap.GetPixel(x, y);

                        data[x, y, 0] = pixelColor.R;
                        data[x, y, 1] = pixelColor.G;
                        data[x, y, 2] = pixelColor.B;
                    }
                }
                return data;
            });

            lApi.AddModule(new GaussianFilter());

            double[,,] result = lApi.Run() as double[,,];

            Assert.IsTrue(result != null);

            Bitmap blurBitmap = new Bitmap(result.GetLength(0), result.GetLength(1));

            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    Color pixelColor = Color.FromArgb((int)result[x, y, 0], (int)result[x, y, 1], (int)result[x, y, 2]);

                    blurBitmap.SetPixel(x, y, pixelColor);
                }
            }

            SaveImage(blurBitmap, "Mean.jpg");

        }

        /// <summary>
        /// Unit test for Gaussian Filter
        /// </summary>
        [TestMethod]
        public void Gaussian()
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                Bitmap myBitmap = new Bitmap($"{appPath}/TestPicture/test.gif");

                double[,,] data = new double[myBitmap.Width, myBitmap.Height, 3];

                for (int x = 0; x < myBitmap.Width; x++)
                {
                    for (int y = 0; y < myBitmap.Height; y++)
                    {
                        Color pixelColor = myBitmap.GetPixel(x, y);

                        data[x, y, 0] = pixelColor.R;
                        data[x, y, 1] = pixelColor.G;
                        data[x, y, 2] = pixelColor.B;
                    }
                }
                return data;
            });

            lApi.AddModule(new GaussianFilter());

            double[,,] result = lApi.Run() as double[,,];

            Assert.IsTrue(result != null);

            Bitmap blurBitmap = new Bitmap(result.GetLength(0), result.GetLength(1));

            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    Color pixelColor = Color.FromArgb((int)result[x, y, 0], (int)result[x, y, 1], (int)result[x, y, 2]);

                    blurBitmap.SetPixel(x, y, pixelColor);
                }
            }

            SaveImage(blurBitmap, "Gaussian.jpg");

          
        }
        /// <summary>
        /// Unit test for Gaussian and Mean Filter together
        /// </summary>
        [TestMethod]
        public void GaussianAndMean()
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                Bitmap myBitmap = new Bitmap($"{appPath}/TestPicture/test.gif");

                double[,,] data = new double[myBitmap.Width, myBitmap.Height, 3];

                for (int x = 0; x < myBitmap.Width; x++)
                {
                    for (int y = 0; y < myBitmap.Height; y++)
                    {
                        Color pixelColor = myBitmap.GetPixel(x, y);

                        data[x, y, 0] = pixelColor.R;
                        data[x, y, 1] = pixelColor.G;
                        data[x, y, 2] = pixelColor.B;
                    }
                }
                return data;
            });

            lApi.AddModule(new GaussianFilter());

            lApi.AddModule(new MeanFilter());

            double[,,] result = lApi.Run() as double[,,];

            Assert.IsTrue(result != null);

            Bitmap blurBitmap = new Bitmap(result.GetLength(0), result.GetLength(1));

            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    Color pixelColor = Color.FromArgb((int)result[x, y, 0], (int)result[x, y, 1], (int)result[x, y, 2]);

                    blurBitmap.SetPixel(x, y, pixelColor);
                }
            }

            SaveImage(blurBitmap, "GaussianAndMean.jpg");
        }
    }
}
