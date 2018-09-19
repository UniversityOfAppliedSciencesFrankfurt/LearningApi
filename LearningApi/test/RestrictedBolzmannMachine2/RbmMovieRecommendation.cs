using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RbmMovieRecommendation
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
        [InlineData(2, 0.01, 3898, 500)]
        //[InlineData(10, 4096, 10)]
       
        public void movieRecommendationTest(int iterations,double learningRate, int visNodes, int hidNodes)
        {
            Debug.WriteLine($"{iterations}-{visNodes}-{hidNodes}");

            LearningApi api = new LearningApi(getDescriptorForRbm(3898));

            // Initialize data provider
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\movieDatasetTrain1.csv"), ',', false, 0);
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

            StreamWriter tw = new StreamWriter($"PredictedDigit_I{iterations}_V{visNodes}_H{hidNodes}_learnedbias.txt");
            foreach (var item in score.HiddenBisases)
            {
                tw.WriteLine(item);
            }
            tw.Close();

            var testData = ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\movieDatasetTest1.csv"));

            var result = api.Algorithm.Predict(testData, api.Context);

            var predictedData = ((RbmResult)result).VisibleNodesPredictions;

            var predictedHiddenNodes = ((RbmResult)result).HiddenNodesPredictions;

            var acc = testData.GetHammingDistance(predictedData);

            WriteDeepResult(iterations, new int[] { visNodes, hidNodes }, acc, watch.ElapsedMilliseconds*1000, predictedHiddenNodes);

            WriteOutputMatrix(iterations, new int[] { visNodes, hidNodes }, predictedData, testData);
        }

        [Theory]
        [InlineData(10, 0.01, 3898, 1500)]
        public void movieRecommendationTestCRbm(int iterations, double learningRate, int visNodes, int hidNodes)
        {
            Debug.WriteLine($"{iterations}-{visNodes}-{hidNodes}");

            LearningApi api = new LearningApi(getDescriptorForRbm(3898));

            // Initialize data provider
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\movieDatasetTrain.csv"), ',', false, 0);
            api.UseDefaultDataMapper();
            double[] featureVector = new double[] { 0, 0.05, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5, 0.55, 0.6, 0.65, 0.7, 0.75, 0.8, 0.85};
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

            StreamWriter tw = new StreamWriter($"PredictedDigit_I{iterations}_V{visNodes}_H{hidNodes}_learnedbias.txt");
            foreach (var item in score.HiddenBisases)
            {
                tw.WriteLine(item);
            }
            tw.Close();

            var testData = ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\movieDatasetTest.csv"));

            var result = api.Algorithm.Predict(testData, api.Context);

            var predictedData = ((RbmResult)result).VisibleNodesPredictions;

            var predictedHiddenNodes = ((RbmResult)result).HiddenNodesPredictions;

            var acc = testData.GetHammingDistance(predictedData);

            WriteDeepResult(iterations, new int[] { visNodes, hidNodes }, acc, watch.ElapsedMilliseconds * 1000, predictedHiddenNodes);

            WriteOutputMatrix(iterations, new int[] { visNodes, hidNodes }, predictedData, testData);
        }

        [Theory]
        //[InlineData(1, 4096, new int[] { 4096, 250, 10 })]       
        [InlineData(10, 0.01, new int[] { 3898, 1500, 30 })]
        public void movieRecommendationTestDeepRbm(int iterations, double learningRate, int[] layers)
        {
            Debug.WriteLine($"{iterations}-{String.Join("", layers)}");

            LearningApi api = new LearningApi(getDescriptorForRbm(3898));

            // Initialize data provider
            // TODO: Describe Digit Dataset.
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\movieDatasetTrain.csv"), ',', false, 0);
            api.UseDefaultDataMapper();

            api.UseDeepRbm(learningRate, iterations, layers);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            RbmDeepScore score = api.Run() as RbmDeepScore;
            watch.Stop();

            var testData = RbmHandwrittenDigitUnitTests.ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\movieDatasetTest.csv"));

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

            WriteDeepResult(iterations, layers, accList, Time * 1000, predictedHiddenNodes);
            /// write predicted hidden nodes.......
            WriteOutputMatrix(iterations, layers, predictions, testData);
        }
        internal static void WriteDeepResult(int iterations, int[] layers, double[] accuracy, double executionTime, double[][] predictedNodes)
        {
            double sum = 0;

            using (StreamWriter tw = new StreamWriter($"I{iterations}_V{String.Join("-", layers)}_ACC.txt"))
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

        internal static void WriteOutputMatrix(int iterations, int[] layers, double[][] predictedData, double[][] testData, int lineLength = 1)
        {
            using (TextWriter tw = new StreamWriter($"PredictedDigit_I{iterations}_V{String.Join("_", layers)}.txt"))
            {

                int initialRowLength = predictedData.Length;
                //int finalRowCount = predictedData.Length * (initialRowLength / lineLength);
                //double[,] predictedDataLines = new double[finalRowCount, lineLength];
                //double[,] testDataLines = new double[finalRowCount, lineLength];
                //for (int i = 0; i < predictedData.Length; i++)
                //{
                  //  int col = 0;
                   // for (int j = 0; j < lineLength; j++)
                    //{
                      //  int row = i * lineLength + j;

 //                       for (int z = 0; z < lineLength; z++)
   //                     {
     //                       //int col = row * lineLength + z;                    
                              //predictedDataLines[row, z] = predictedData[i][col];
         //                   testDataLines[row, z] = testData[i][col];
           //                 col = col + 1;
             //           }
             //
               //     }
                //}

                tw.WriteLine();
                tw.Write("\t\t\t\t\t\t Original Image \t\t\t\t\t\t\t\t\t\t\t\t\t Predicted Image");
                tw.WriteLine();
                int k = 1;

                for (var i = 0; i < initialRowLength; i++)
                {
                    if (k == 3899)
                    {
                        tw.WriteLine();
                        tw.Write("New Image");
                        tw.WriteLine();
                        k = 1;
                    }
                    for (int j = 0; j < 3897; j++)
                    {
                        tw.Write(testData[i] [j]);
                        tw.Write("\t\t\t\t");
                        tw.Write(predictedData[i][j]);
                        tw.WriteLine();
                        k++;
                    }
                    
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
