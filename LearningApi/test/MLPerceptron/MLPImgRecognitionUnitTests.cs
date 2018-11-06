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
            Binarizer bizer = new Binarizer(targetHeight: height, targetWidth: width);
            return bizer.GetBinary(imageName);
        }

        [Fact]
        //[InlineData(new int[] { 6, 3 })]
        public void ImageRecognitionTest()
        {
            int size = 64;
            int numberOfOutputs = 1;
            int numberOfInputs = size*size;
        

            var context = getImgRecognitionDescriptor(numberOfInputs);

            LearningApi api = new LearningApi(context);

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                return getImageData(size, $"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TrainingImages");
                //return getSomOtherData($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\TrainingData.csv");
            });

            // High number of hidden neurons in first layer brings network to constant result for everything.
            // var hiddenLayerNeurons = new int[] { size*size, 3 };

            var hiddenLayerNeurons = new int[] {6000, 9 ,3 };

            api.UseMLPerceptron(0.01, 6, 1, 1, hiddenLayerNeurons);

            Stopwatch sw = new Stopwatch();

            sw.Start();
            IScore score = api.Run() as IScore;
            sw.Stop();
            Trace.WriteLine($"Duration:{(double)(sw.ElapsedMilliseconds / 1000 /60)} min");

            var testImageData = getImageData(size, $"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestingImages");
            //var testImageData= getSomOtherData($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\TestData.csv");

            MLPerceptronResult result = api.Algorithm.Predict(testImageData, api.Context) as MLPerceptronResult;

           // float accuracy = MLPHelpers.GetAccuracy(testImageData, result.results, numberOfOutputs);
        }


        private double[][] getSomOtherData(string file)
        {
            using (var readerTrainData = new StreamReader(file))
            {
               
                List<double[]> listTrainData = new List<double[]>();

                readerTrainData.ReadLine();

                while (!readerTrainData.EndOfStream)
                {
                    var singleRow = new List<double>();

                    var line = readerTrainData.ReadLine();

                    var values = line.Split(',');
                    
                    foreach (var value in values)
                    {
                        singleRow.Add(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                    }

                    listTrainData.Add(singleRow.ToArray());
                }

                return listTrainData.ToArray();
            }
   
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
                        {
                            imgTrainingRow.Add(1);
                        }
                        else if (rowInImg[k] == '0')
                        {
                            imgTrainingRow.Add(0);
                        }
                        else if (rowInImg[k] == '\r' || rowInImg[k] == '\n')
                            continue;
                        else
                            throw new InvalidDataException($"'1' or '0' is supported only. We detected {rowInImg[k]}");
                    }
                }

                if (file.Contains("positive"))
                {
                    Debug.Write("POSITIVE");
                    imgTrainingRow.Add(1);  //Label 1
                    //imgTrainingRow.Add(0);
                }
                else
                {
                    Debug.Write("NEGATIVE");
                    imgTrainingRow.Add(0); ; //Label 0
                    //imgTrainingRow.Add(1);
                }

                data[indx] = imgTrainingRow.ToArray();
                Debug.WriteLine($" Zeros: { data[indx].Count(k => k == 0.0)} - Onces: { data[indx].Count(k => k == 1.0)}");

                indx++;
            }

         

            return data;
        }
    }

}




