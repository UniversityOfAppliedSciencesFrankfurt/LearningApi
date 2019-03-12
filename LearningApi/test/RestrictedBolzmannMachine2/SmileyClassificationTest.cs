using LearningFoundation.Arrays;
using LearningFoundation.DataMappers;
using LearningFoundation.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralNet.RestrictedBolzmannMachine2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LearningFoundation.Test
{
    [TestClass]
    public class SmileyClassificationTest
    {

        private DataDescriptor getDescriptorForRbm(int dims)
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[dims];

            // Label not used.
            des.LabelIndex = -1;
            des.Features = new Column[dims];
            int k = 1;
            for (int i = 0; i < dims; i++)
            {
                des.Features[i] = new Column { Id = k, Name = $"col{k}", Index = i, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
                k = k + 1;
            }

            return des;
        }

        private double calcDelta(double[][] positiveGradient, double[][] negGrad)
        {
            double sum = 0;
            for (int i = 0; i < positiveGradient.Length; i++)
            {
                for (int j = 0; j < positiveGradient[i].Length; j++)
                {
                    sum += Math.Abs(positiveGradient[i][j] - negGrad[i][j]);
                }
            }

            return sum;
        }

        /// <summary>
        /// TODO...
        /// </summary>
        [DataTestMethod]
        [DataRow(1000, 0.01, 1600, 500)]

        public void smileyTestRbm(int iterations,double learningRate, int visNodes, int hidNodes)
        {

            LearningApi api = new LearningApi(getDescriptorForRbm(1600));

            // Initialize data provider
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\Smiley.csv"), ',', false, 0);
            api.UseDefaultDataMapper();
            api.UseRbm(learningRate, iterations, visNodes, hidNodes);

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

            var testData = ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\SmileyTest.csv"));

            var result = api.Algorithm.Predict(testData, api.Context);

            var predictedData = ((RbmResult)result).VisibleNodesPredictions;

            var predictedHiddenNodes = ((RbmResult)result).HiddenNodesPredictions;

            var acc = testData.GetHammingDistance(predictedData);

            var ValTest = calcDelta(predictedData, testData);
            var lossTest = ValTest / (visNodes);

            Debug.WriteLine($"lossTest: {lossTest}");

            WriteDeepResult(iterations, new int[] { visNodes, hidNodes }, acc, watch.ElapsedMilliseconds*1000, predictedHiddenNodes);

            WriteOutputMatrix(iterations, new int[] { visNodes, hidNodes }, predictedData, testData);
        }

        [DataTestMethod]
        [DataRow(1000, 0.01, 1600, 500)]

        public void smileyTestCRbm(int iterations, double learningRate, int visNodes, int hidNodes)
        {

            LearningApi api = new LearningApi(getDescriptorForRbm(1600));

            // Initialize data provider
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\Smiley.csv"), ',', false, 0);
            api.UseDefaultDataMapper();
            double[] featureVector = new double[] { 0.1, 0.2 };
            api.UseCRbm(featureVector, learningRate, iterations, visNodes, hidNodes);

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

            var testData = ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\SmileyTest.csv"));

            var result = api.Algorithm.Predict(testData, api.Context);

            var predictedData = ((RbmResult)result).VisibleNodesPredictions;

            var predictedHiddenNodes = ((RbmResult)result).HiddenNodesPredictions;

            var acc = testData.GetHammingDistance(predictedData);

            var ValTest = calcDelta(predictedData, testData);
            var lossTest = ValTest / (visNodes);

            Debug.WriteLine($"lossTest: {lossTest}");

            WriteDeepResult(iterations, new int[] { visNodes, hidNodes }, acc, watch.ElapsedMilliseconds * 1000, predictedHiddenNodes);

            WriteOutputMatrix(iterations, new int[] { visNodes, hidNodes }, predictedData, testData);
        }

        [DataTestMethod]
        [DataRow(1000, 0.01, new int[] {1600, 500, 300 })]

        public void smileyTestDeepRbm(int iterations, double learningRate, int[] layers)
        {

            LearningApi api = new LearningApi(getDescriptorForRbm(1600));

            // Initialize data provider
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\Smiley.csv"), ',', false, 0);
            api.UseDefaultDataMapper();
            api.UseDeepRbm(learningRate, iterations, layers);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            RbmDeepScore score = api.Run() as RbmDeepScore;
            watch.Stop();

            var testData = ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\SmileyTest.csv"));

            var result = api.Algorithm.Predict(testData, api.Context) as RbmDeepResult;
            var accList = new double[result.Results.Count];
            var predictions = new double[result.Results.Count][];
            var predictedHiddenNodes = new double[result.Results.Count][];
            var Time = watch.ElapsedMilliseconds / 1000;

            int i = 0;
            foreach (var item in result.Results)
            {
                predictions[i] = item.First().VisibleNodesPredictions;
                predictedHiddenNodes[i] = item.Last().HiddenNodesPredictions;
                accList[i] = testData[i].GetHammingDistance(predictions[i]);
                i++;
            }
            var ValTest = calcDelta(predictions, testData);
            var lossTest = ValTest / (layers.First());
            Debug.WriteLine($"lossTest: {lossTest}");

            WriteDeepResult(iterations, layers, accList, Time/60.0 , predictedHiddenNodes);
            /// write predicted hidden nodes.......
            WriteOutputMatrix(iterations, layers, predictions, testData);

        }

        internal static void WriteDeepResult(int iterations, int[] layers, double[] accuracy, double executionTime, double[][] predictedNodes)
        {
            double sum = 0;

            using (StreamWriter tw = new StreamWriter($"SmileyData_Result_I{iterations}_V{String.Join("-", layers)}_ACC.txt"))
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
        internal static void WriteOutputMatrix(int iterations, int[] layers, double[][] predictedData, double[][] testData, int lineLength = 40)
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
                tw.Write("\t\t\t\t Predicted Image \t\t\t\t\t\t\t\t\t\t\t\t\t\t Original Image");
                tw.WriteLine();
                int k = 1;

                for (var i = 0; i < finalRowCount; i++)
                {
                    if (k == 41)
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
