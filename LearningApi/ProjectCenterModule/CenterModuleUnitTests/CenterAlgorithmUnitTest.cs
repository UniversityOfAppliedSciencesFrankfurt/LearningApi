using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearningFoundation;
using CenterModule;
using System.IO;
using System;

namespace CenterModuleUnitTests
{/// <summary>
/// This unitest shows that my Algorithm correct works and ist conectet to learningApi
/// </summary>
    [TestClass]
    public class CenterAlgorithmUnitTests
    {
        public double[][] LoadAndConvertToDoubleMatrix()
        {


            int digitNumber = 0;

            // Relative Paths to Dataset of images and their labels


            string pixelFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "train-images.idx3-ubyte");
            string labelFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "train-labels.idx1-ubyte");


            // Load one Digit Image

            DigitImage[] images = LoadDigitImage.LoadData(pixelFile, labelFile);




            //Make Mnist pictures in Double Array
            double[][] data = new double[images[digitNumber].Pixels.Length][];
            for (int i = 0; i < data.Length; i++)
            {

                data[i] = new double[images[digitNumber].Pixels.Length];
            }
            for (int i = 0; i < images[digitNumber].Pixels.Length; i++)
            {
                for (int j = 0; j < images[digitNumber].Pixels.Length; j++)
                {
                    data[i][j] = images[digitNumber].Pixels[i][j];
                }
            }


            return data;
        }

        [TestMethod]


        public void TestRun()
        {
            LearningApi api = new LearningApi();
            CenterAlgorithm center = new CenterAlgorithm();
            api.UseActionModule<double[][], double[][]>((input, ctx) =>
            {
                double[][] data = LoadAndConvertToDoubleMatrix();
                return data;


            });

            api.AddModule(center);
            double[][] output = api.Run() as double[][];
            Save save = new Save();
            save.SaveFile(output);
            Assert.IsNotNull(output);

        }

    }

}
