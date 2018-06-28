using LearningFoundation;
using MLPerceptron;
using MLPerceptron.NeuralNetworkCore;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xunit;
using System.Diagnostics;
using System.Globalization;
using NeuralNet.MLPerceptron;
using ImageBinarizer;

namespace test.MLPerceptron
{
    /// <summary>
    /// Class MLPerceptronUnitTests contains the unit test cases to test the ML Perceptron algorithm
    /// </summary>
    public class MLPImgRecognitionUnitTests
    {
        /// <summary>
        /// MLPerceptronUnitTests Default constructor
        /// </summary>
        static MLPImgRecognitionUnitTests()
        {

        }
   

        private DataDescriptor getImgRecognitionDescriptor(int numOfFeatures)
        {
            DataDescriptor desc = new DataDescriptor();
            desc.Features = new LearningFoundation.DataMappers.Column[numOfFeatures];
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "X",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 1;

            return desc;
        }

        private string readImageData(string imageName, int width, int height)
        {
            string trainingImagesPath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "MLPerceptron"), "TrainingImages");
            Binarizer bizer = new Binarizer(targetHeight: height, targetWidth: width);
            return bizer.GetBinary(Path.Combine(trainingImagesPath, imageName));
        }

        [Fact]
        //[InlineData(new int[] { 6, 3 })]
        public void ImageRecognitionTest()
        {
            int size = 64;

            var context = getImgRecognitionDescriptor(size * size);

            LearningApi api = new LearningApi(context);

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                return getImageData(size, $"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TrainingImages");
            });

            var hiddenLayerNeurons = new int[] { size*size/2, size, size/2, 5 };

            api.UseMLPerceptron(0.5, 10, hiddenLayerNeurons);

            Stopwatch sw = new Stopwatch();

            sw.Start();
            IScore score = api.Run() as IScore;
            sw.Stop();
            Trace.WriteLine($"Duration:{(double)(sw.ElapsedMilliseconds / 1000 /60)} min");

            var testImageData = getImageData(size, $"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestingImages");

            MLPerceptronResult res = api.Algorithm.Predict(testImageData, api.Context) as MLPerceptronResult;

        }

        private double[][] getImageData(int size, string imageFolder)
        {
            var trainingImages = Directory.GetFiles(imageFolder);

            double[][] data = new double[trainingImages.Length][];

            int indx = 0;

            foreach (var file in trainingImages)
            {
                string imgData = readImageData(file, size, size);

                List<double> imgTrainingRow = new List<double>();

                foreach (var rowInImg in imgData.Split('\n'))
                {
                    for (int k = 0; k < rowInImg.Length; k++)
                    {
                        if (rowInImg[k] == '1')
                            imgTrainingRow.Add(1);
                        else if (rowInImg[k] == '0')
                            imgTrainingRow.Add(0);
                        else if (rowInImg[k] == '\r' || rowInImg[k] == '\n')
                            continue;
                        else
                            throw new InvalidDataException($"'1' or '0' is supported only. We detected {rowInImg[k]}");
                    }
                }

                if(file.Contains("positive"))
                    imgTrainingRow.Add(1);  //Label 1
                else
                    imgTrainingRow.Add(0); ; //Label 0

                data[indx] = imgTrainingRow.ToArray();

                indx++;
            }

            return data;
        }
    }

}




