using System;
using System.Collections.Generic;
using System.IO;
using NeuralNet.RestrictedBolzmannMachine2;
using LearningFoundation.DataProviders;
using LearningFoundation;
using Xunit;
using LearningFoundation.DataMappers;
using System.Globalization;
using LearningFoundation.Arrays;
using System.Diagnostics;

namespace test.RestrictedBolzmannMachine2
{
    public class RbmHandwrittenDigitUnitTests
    {
       
        internal static DataDescriptor GetDescriptorForDigits()
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
        /// Ensures that all RBM layers are correctly allocated.
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="layers"></param>
        [Theory]
        [InlineData(1, new int[] { 9, 5 })]
        [InlineData(1, new int[] { 19, 15, 14, 7 })]
        [InlineData(1, new int[] { 250, 150, 10 })]
        public void DeepRbmConstructorTest(int iterations, int[] layers)
        {
            DeepRbm rbm = new DeepRbm(layers, iterations, 0.01);
            Assert.True(rbm.Layers.Length == layers.Length - 1);
            foreach (var layer in rbm.Layers)
            {
                Assert.True(layer != null);
            }
        }

        /// <summary>
        /// TODO...
        /// </summary>
        [Theory]
        [InlineData(10, 4096, 900)]
        //[InlineData(10, 4096, 10)]
       
        public void DigitRecognitionTest(int iterations, int visNodes, int hidNodes)
        {
            Debug.WriteLine($"{iterations}-{visNodes}-{hidNodes}");

            LearningApi api = new LearningApi(GetDescriptorForDigits());

            // Initialize data provider
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\DigitDataset.csv"), ',', false, 0);
            api.UseDefaultDataMapper();
            api.UseRbm(0.2, iterations, visNodes, hidNodes);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            RbmScore score = api.Run() as RbmScore;
            watch.Stop();

            var hiddenNodes = score.HiddenValues;
            var hiddenWeight = score.HiddenBisases;


            double[] learnedFeatures = new double[hidNodes];
            double[] hiddenWeights = new double[hidNodes];
            for (int i = 0; i < hidNodes; i++)
            {
                learnedFeatures[i] = hiddenNodes[i];
                hiddenWeights[i] = hiddenWeight[i];
            }

            StreamWriter tw = new StreamWriter($"PredictedDigit_I{iterations}_V{visNodes}_H{hidNodes}_learnedbias.txt");
            foreach (var item in score.HiddenBisases)
            {
                tw.WriteLine(item);
            }
            tw.Close();

            var testData = ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\DigitTest.csv"));

            var result = api.Algorithm.Predict(testData, api.Context);

            var predictedData = ((RbmResult)result).VisibleNodesPredictions;

            var predictedHiddenNodes = ((RbmResult)result).HiddenNodesPredictions;

            var acc = testData.GetHammingDistance(predictedData);

            WriteDeepResult(iterations, new int[] { visNodes, hidNodes }, acc, watch.ElapsedMilliseconds*1000, predictedHiddenNodes);

            WriteOutputMatrix(iterations, new int[] { visNodes, hidNodes }, predictedData, testData);
        }




        internal static void WriteDeepResult(int iterations, int[] layers, double[] accuracy, double executionTime, double[][] predictedNodes)
        {
            double sum = 0;

            using (StreamWriter tw = new StreamWriter($"Result_I{iterations}_V{String.Join("-", layers)}_ACC.txt"))
            {
                tw.WriteLine($"HiddenNodes;\t\tSample;Iterations;Accuracy;ExecutionTime");
                for (int i = 0; i < accuracy.Length; i++)
                {    
                    for (int j = 0; j<predictedNodes[i].Length; j++)
                    {
                        tw.Write(predictedNodes[i][j]);
                        tw.Write(",");
                    }
                    tw.Write("\t");
                    tw.WriteLine($"{i};{iterations};{accuracy[i]};{executionTime}");
                    tw.WriteLine();
                    sum += accuracy[i];
                }

                // Here we write out average accuracy.
                tw.WriteLine($"{accuracy.Length};{iterations};{sum / accuracy.Length}");
                tw.Close();
            }
            
        }

        internal static void WriteOutputMatrix(int iterations, int[] layers, double[][] predictedData, double[][] testData, int lineLength = 64)
        {
            using (TextWriter tw = new StreamWriter($"PredictedDigit_I{iterations}_V{String.Join("_", layers)}.txt"))
            {
                int initialRowLength = predictedData[0].Length;
                int finalRowCount = predictedData.Length * (initialRowLength / lineLength);
                double[,] predictedDataLines = new double[finalRowCount, lineLength];
                double[,] testDataLines = new double[finalRowCount, lineLength];
                for (int i = 0; i < predictedData.Length; i++)
                {
                    int col = 0;
                    for (int j = 0; j < lineLength; j++)
                    {
                        int row = i * lineLength + j;

                        for (int z = 0; z < lineLength; z++)
                        {
                            //int col = row * lineLength + z;                    
                            predictedDataLines[row, z] = predictedData[i][col];
                            testDataLines[row, z] = testData[i][col];
                            col = col + 1;
                        }

                    }
                }

                tw.WriteLine();
                tw.Write("\t\t\t\t\t\t Predicted Image \t\t\t\t\t\t\t\t\t\t\t\t\t\t\t Original Image");
                tw.WriteLine();
                int k = 1;

                for (var i = 0; i < finalRowCount; i++)
                {
                    if (k == 65)
                    {
                        tw.WriteLine();
                        tw.Write("New Image");
                        tw.WriteLine();
                        k = 1;
                    }
                    for (int j = 0; j < lineLength; j++)
                    {
                        tw.Write(testDataLines[i, j]);
                    }
                    tw.Write("\t\t\t\t");
                    for (int j = 0; j < lineLength; j++)
                    {
                        tw.Write(predictedDataLines[i, j]);
                    }
                    tw.WriteLine();
                    k++;
                }

                tw.WriteLine();
                tw.Close();
            }
        }


        internal static double[][] ReadData(string path)
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
