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
using NeuralNet.MLPerceptron;
using ImageBinarizer;
using System.Globalization;
using AkkaMLPerceptron;
using System.Threading;

namespace test.MLPerceptron
{
    /// <summary>
    /// 
    /// </summary>
    public class AkkaMLPUnitTests
    {
        /// <summary>
        /// 
        /// </summary>
        static AkkaMLPUnitTests()
        {

        }

        private static void runNodes()
        {
            /*
            
                Runs seed node on 8081.
                dotnet akkahost.dll --port  8081 --seedhosts "localhost:8081, localhost:8082" --sysname ClusterSystem
 
                Runs another seed node on 8081
                dotnet akkahost.dll --port  8082 --seedhosts "localhost:8081, localhost:8082" --sysname ClusterSystem 

             */
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void UnitTestZero()
        {
            string clusterSystemName = "ClusterSystem";

            AkaMLPerceptronAlgorithm alg = new AkaMLPerceptronAlgorithm(clusterSystemName, 
                new string[] { $"akka.tcp://{clusterSystemName}@localhost:8081", $"akka.tcp://{clusterSystemName}@localhost:8082" });


            List<double[]> data = new List<double[]>();

            for (int i = 0; i < 10; i++)
            {
                int numFeatures = 1024;
                double[] features = new double[numFeatures];

                for (int j = 0; i < numFeatures; i++)
                {
                    features[j] = i;
                }

                data.Add(features);
            }                       

            alg.Run(data.ToArray(), null);

           
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void UnitTestOne()
        {
            // Read the csv file which contains the training data
            using (var readerTrainData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\TrainingData.csv"))
            {
                int lineCountTrainData = 0;

                int indexTrainData = 0;

                int numberOfOutputs = 0;

                while (readerTrainData.ReadLine() != null)
                {
                    lineCountTrainData++;
                }

                double[][] data = new double[lineCountTrainData - 1][];

                List<double>[] listTrainData = new List<double>[lineCountTrainData - 1];

                LearningApi api = new LearningApi();

                api.UseActionModule<object, double[][]>((notUsed, ctx) =>
                {
                    ctx.DataDescriptor = new DataDescriptor();

                    readerTrainData.DiscardBufferedData();

                    readerTrainData.BaseStream.Seek(0, SeekOrigin.Begin);

                    var firstLine = readerTrainData.ReadLine();

                    var firstLineValues = firstLine.Split(',');

                    foreach (var value in firstLineValues)
                    {
                        if (value.Contains("Output"))
                        {
                            numberOfOutputs++;
                        }
                    }

                    ctx.DataDescriptor.Features = new LearningFoundation.DataMappers.Column[firstLineValues.Length - numberOfOutputs];

                    for (int i = 0; i < (firstLineValues.Length - numberOfOutputs); i++)
                    {
                        ctx.DataDescriptor.Features[i] = new LearningFoundation.DataMappers.Column
                        {
                            Id = i,
                            Name = firstLineValues[i],
                            Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                            Index = i,
                        };
                    }

                    ctx.DataDescriptor.LabelIndex = firstLineValues.Length - numberOfOutputs;


                    while (!readerTrainData.EndOfStream)
                    {
                        var line = readerTrainData.ReadLine();

                        var values = line.Split(',');

                        listTrainData[indexTrainData] = new List<double>();

                        foreach (var value in values)
                        {
                            listTrainData[indexTrainData].Add(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        }

                        data[indexTrainData] = listTrainData[indexTrainData].ToArray();

                        indexTrainData++;
                    }

                    return data;
                });

                int[] hiddenLayerNeurons = { 6 };

                // Invoke the MLPerecptronAlgorithm with a specific learning rate, number of iterations, and number of hidden layer neurons
                api.UseMLPerceptron(0.1, 10000, 1, 1, hiddenLayerNeurons);

                IScore score = api.Run() as IScore;

                // Read the csv file which contains the test data
                using (var readerTestData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\testData.csv"))
                {
                    int lineCountTestData = 0;

                    int indexTestData = 0;

                    while (readerTestData.ReadLine() != null)
                    {
                        lineCountTestData++;
                    }

                    double[][] testData = new double[lineCountTestData - 1][];

                    List<double>[] listTestData = new List<double>[lineCountTestData - 1];

                    readerTestData.DiscardBufferedData();

                    readerTestData.BaseStream.Seek(0, SeekOrigin.Begin);

                    readerTestData.ReadLine();

                    while (!readerTestData.EndOfStream)
                    {
                        var line = readerTestData.ReadLine();

                        var values = line.Split(',');

                        listTestData[indexTestData] = new List<double>();

                        foreach (var value in values)
                        {
                            listTestData[indexTestData].Add(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        }

                        testData[indexTestData] = listTestData[indexTestData].ToArray();

                        indexTestData++;
                    }

                    // Invoke the Predict method to predict the results on the test data
                    var result = ((MLPerceptronResult)api.Algorithm.Predict(testData, api.Context)).results;

                    int expectedResults = 0;

                    Debug.WriteLine("\r\nTraining completed.");
                    Debug.WriteLine("-------------------------------------------");
                    Debug.WriteLine($"Testing {lineCountTestData - 1} samples.");

                    // Check if the test data has been correctly classified by the neural network
                    for (int i = 0; i < lineCountTestData - 1; i++)
                    {
                        Debug.WriteLine("");

                        for (int j = 0; j < numberOfOutputs; j++)
                        {
                            //Assert.True(testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0));
                            if (testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0))
                                expectedResults++;

                            Debug.Write($"{i.ToString("D2")} - ");

                            for (int k = 0; k < api.Context.DataDescriptor.Features.Length; k++)
                            {
                                Debug.Write($"[{testData[i][k]}] ");
                            }

                            Debug.Write("\t\t");
                            Debug.Write($"Expected: {testData[i][(testData[i].Length - numberOfOutputs) + j]} - Predicted: {testData[i][(testData[i].Length - numberOfOutputs) + j]} - Result: {testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0)} --\t\t");
                        }
                    }

                    float accuracy = (float)expectedResults / (float)numberOfOutputs / (float)testData.Length;

                    Debug.WriteLine("-------------------------------------------");
                    Debug.WriteLine($"Accuracy: {accuracy * 100}%");
                }
            }
        }

    }

}






