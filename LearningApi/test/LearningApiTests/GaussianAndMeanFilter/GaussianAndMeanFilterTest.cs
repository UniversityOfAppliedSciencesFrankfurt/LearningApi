using GaussianAndMeanFilter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using LearningApiTests.GaussianAndMeanFilter;

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
        [DataTestMethod]
        [DataRow("/TestPicture/test1.gif", "/TestPicture/test1.gif")]
        [DataRow("/TestPicture/test2.jpg", "/ExpectedPicture/Mean/test2.jpg")]
        [DataRow("/TestPicture/test3.png", "/ExpectedPicture/Mean/test3.png")]
        [DataRow("/TestPicture/test4.png", "/ExpectedPicture/Mean/test4.png")]
        [DataRow("/TestPicture/test5.jpg", "/ExpectedPicture/Mean/test5.jpg")]
        [DataRow("/TestPicture/test6.jpg", "/ExpectedPicture/Mean/test6.jpg")]
        [DataRow("/TestPicture/test7.jpg", "/ExpectedPicture/Mean/test7.jpg")]
        public void MeanFilter_ImageBlur_FilterSuccessfullyApplied(string inputImageFileName, string expectedImageFileName)
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule((Func<double[,,], IContext, double[,,]>)((input, ctx) => 
                GetDataArrayFromImage(inputImageFileName)));

            lApi.AddModule(new MeanFilter());

            double[,,] result = lApi.Run() as double[,,];

            ValidateBitmap(result, expectedImageFileName);
        }

        /// <summary>
        /// Unit test for Gaussian Filter
        /// </summary>
        [DataTestMethod]
        [DataRow("/TestPicture/test1.gif", "/TestPicture/test1.gif")]
        [DataRow("/TestPicture/test2.jpg", "/ExpectedPicture/Gaussian/test2.jpg")]
        [DataRow("/TestPicture/test3.png", "/ExpectedPicture/Gaussian/test3.png")]
        [DataRow("/TestPicture/test4.png", "/ExpectedPicture/Gaussian/test4.png")]
        [DataRow("/TestPicture/test5.jpg", "/ExpectedPicture/Gaussian/test5.jpg")]
        [DataRow("/TestPicture/test6.jpg", "/ExpectedPicture/Gaussian/test6.jpg")]
        [DataRow("/TestPicture/test7.jpg", "/ExpectedPicture/Gaussian/test7.jpg")]
        public void GaussianFilter_ImageBlur_FilterSuccessfullyApplied(string inputImageFileName, string expectedImageFileName)
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule((Func<double[,,], IContext, double[,,]>)((input, ctx) =>
                GetDataArrayFromImage(inputImageFileName)));

            lApi.AddModule(new GaussianFilter());

            double[,,] result = lApi.Run() as double[,,];

            ValidateBitmap(result, expectedImageFileName);
        }
        /// <summary>
        /// Unit test for Gaussian and Mean Filter together
        /// </summary>
        [DataTestMethod]
        [DataRow("/TestPicture/test1.gif", "/TestPicture/test1.gif")]
        [DataRow("/TestPicture/test2.jpg", "/ExpectedPicture/GaussianAndMean/test2.jpg")]
        [DataRow("/TestPicture/test3.png", "/ExpectedPicture/GaussianAndMean/test3.png")]
        [DataRow("/TestPicture/test4.png", "/ExpectedPicture/GaussianAndMean/test4.png")]
        [DataRow("/TestPicture/test5.jpg", "/ExpectedPicture/GaussianAndMean/test5.jpg")]
        [DataRow("/TestPicture/test6.jpg", "/ExpectedPicture/GaussianAndMean/test6.jpg")]
        [DataRow("/TestPicture/test7.jpg", "/ExpectedPicture/GaussianAndMean/test7.jpg")]
        public void GaussianAndMeanFilter_ImageBlur_FilterSuccessfullyApplied(string inputImageFileName, string expectedImageFileName)
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule((Func<double[,,], IContext, double[,,]>)((input, ctx) =>
                GetDataArrayFromImage(inputImageFileName)));

            lApi.AddModule(new GaussianFilter());

            lApi.AddModule(new MeanFilter());

            double[,,] result = lApi.Run() as double[,,];

            ValidateBitmap(result, expectedImageFileName);
        }

        private void GenerateExpectedImage(double[,,] result, string expectedImageFileName)
        {
            Assert.IsTrue(result != null);
            Bitmap resultBitmap = GenerateResultBitmap(result);
            SaveImage(resultBitmap, expectedImageFileName);
        }

        private void ValidateBitmap(double[,,] result, string expectedImageFileName)
        {
            Assert.IsTrue(result != null);
            Bitmap resultBitmap = GenerateResultBitmap(result);

            Bitmap expectedBitmap = new Bitmap($"{appPath}{expectedImageFileName}");

            var expectedByteArray = expectedBitmap.GetBytes();
            var resultByteArray = resultBitmap.GetBytes();

            Assert.IsTrue(resultByteArray.SequenceEqual(expectedByteArray));
        }

        private static Bitmap GenerateResultBitmap(double[,,] result)
        {
            Bitmap resultBitmap = new Bitmap(result.GetLength(0), result.GetLength(1));

            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    Color pixelColor = Color.FromArgb((int)result[x, y, 0], (int)result[x, y, 1], (int)result[x, y, 2]);
                    resultBitmap.SetPixel(x, y, pixelColor);
                }
            }

            return resultBitmap;
        }

        private double[,,] GetDataArrayFromImage(string inputImageFileName)
        {
            Bitmap inputBitmap = new Bitmap($"{appPath}{inputImageFileName}");

            double[,,] data = new double[inputBitmap.Width, inputBitmap.Height, 3];

            for (int x = 0; x < inputBitmap.Width; x++)
            {
                for (int y = 0; y < inputBitmap.Height; y++)
                {
                    Color pixelColor = inputBitmap.GetPixel(x, y);

                    data[x, y, 0] = pixelColor.R;
                    data[x, y, 1] = pixelColor.G;
                    data[x, y, 2] = pixelColor.B;
                }
            }
            return data;
        }
    }
}
