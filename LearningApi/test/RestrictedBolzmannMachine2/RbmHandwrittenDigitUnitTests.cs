using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeuralNet.RestrictedBolzmannMachine2;
using ImageBinarizer;
using NeuralNet.Perceptron;
using LearningFoundation.DataProviders;
using LearningFoundation;
using Xunit;
using LearningFoundation.DataMappers;
using System.Globalization;

namespace test.RestrictedBolzmannMachine2
{
    public class RbmHandwrittenDigitUnitTests
    {
        static RbmHandwrittenDigitUnitTests()
        {

        }

        private DataDescriptor getDescriptorForDigits()
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[4096];
            des.LabelIndex = -1;

            des.Features = new Column[4096];
            int k = 1;
            for (int i = 0; i < 4096; i++)
            {

                des.Features[i] = new Column { Id = k, Name = "col" + k, Index = i, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
                k = k + 1;
            }
            return des;
        }

        /// <summary>
        /// TODO...
        /// </summary>
        [Theory]
        [InlineData(1, 4096, 10)]
        [InlineData(2, 4096, 10)]
        //[InlineData(20, 4096, 10)]
        public void DigitRecognitionTest(int iterations,int visNodes, int hidNodes)
        {
            LearningApi api = new LearningApi(this.getDescriptorForDigits());

            // Initialize data provider
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\DigitDataset.csv"), ',', false, 0);
            api.UseDefaultDataMapper();
            //api1.UseRbm(0.2, 100, 784, 392);
            //api.UseRbm(0.2, 1, 4096, 10);
            api.UseRbm(0.2, iterations, visNodes, hidNodes);

            RbmResult score = api.Run() as RbmResult;

            var testData = readData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\DigitTest.csv"));

            var result = api.Algorithm.Predict(testData, api.Context);

            var predictedData = ((RbmResult)result).VisibleNodesPredictions;

            writeOutputMatrix(iterations, visNodes, hidNodes, predictedData, testData);
        }

        private static void writeOutputMatrix(int iterations, int visNodes, int hidNodes, double[][] predictedData, double[][] testData, int lineLength = 65)
        {
            TextWriter tw = new StreamWriter($"PredictedDigit_I{iterations}_V{visNodes}_H{hidNodes}.txt");

            //you need two loops - 1. To loop through alle out predicted nodes. 2. To loop through every single node
            int k = 1;
            for (int i = 0; i < predictedData.Length; i++)
            {
                tw.WriteLine();
                tw.WriteLine($"Image {i} Prediction");

                for (int j = 0; j < testData[i].Length; j++)
                {
                    if ((k % lineLength) == 0)
                    { 
                        k = 1;

                        tw.Write("\t\t\t");

                        for (int n = 0; n < predictedData[i].Length; j++)
                        {
                            if ((k % lineLength) == 0)
                            {
                                tw.WriteLine();
                                k = 1;
                            }

                            tw.Write($"{predictedData[i][n]}");
                            k++;
                        }
                    }

                    tw.Write($"{testData[i][j]}");
                    k++;
                }



                if ((k % lineLength) == 0)
                {
                    tw.WriteLine();
                    k = 1;
                }


                /*
                foreach (var item in predictedData[i])
                {
                    if ((k % lineLength) == 0)
                    {
                        tw.WriteLine();
                        k = 1;
                    }
                    tw.Write(item);
                    k++;
                }*/

                tw.WriteLine();
            }
            tw.Close();
        }

        private static double[][] readData(string path)
        {
            List<double[]> data = new List<double[]>();

            var reader = new StreamReader(File.OpenRead(path));

            StreamReader sr = new StreamReader(path);
            String line;

            while ((line = sr.ReadLine()) != null)
            {
                List<double> row = new List<double>();
                var tokens = line.Split(',');
                foreach (var item in tokens)
                {
                    if (item != "")
                        row.Add(double.Parse(item, CultureInfo.InvariantCulture));
                }

                data.Add(row.ToArray());
            }

            return data.ToArray();
        }
    }
}
