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

namespace test.MLPerceptron
{
    public class MNISTFileRead
    {
        public static double[][] trainingData { get; set; }

        public static string[] firstLineValues { get; set; }

        public static double[][] testData { get; set; }

        public static void ReadMNISTTrainingData()
        {
            using (var readerTrainData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\mnist_train.csv"))
            {
                int lineCountTrainData = 0;

                int indexTrainData = 0;

                while (readerTrainData.ReadLine() != null)
                {
                    lineCountTrainData++;
                }

                trainingData = new double[lineCountTrainData - 1][];

                List<double>[] listTrainData = new List<double>[lineCountTrainData - 1];

                readerTrainData.DiscardBufferedData();

                readerTrainData.BaseStream.Seek(0, SeekOrigin.Begin);

                var firstLine = readerTrainData.ReadLine();

                firstLineValues = firstLine.Split(',');

                double[] maxValue = new double[firstLineValues.Length - 1];

                double[] minValue = new double[firstLineValues.Length - 1];

                double[] meanValue = new double[firstLineValues.Length - 1];

                List<double>[] listDataSpecifics = new List<double>[firstLineValues.Length - 1];

                for (int i = 0; i < firstLineValues.Length - 1; i++)
                {
                    listDataSpecifics[i] = new List<double>();
                }

                while (!readerTrainData.EndOfStream)
                {
                    var line = readerTrainData.ReadLine();

                    var values = line.Split(',');

                    listTrainData[indexTrainData] = new List<double>();

                    for (int i = 0; i < firstLineValues.Length - 1; i++)
                    {
                        listDataSpecifics[i].Add(Convert.ToDouble(values[i], CultureInfo.InvariantCulture));
                    }

                    encodeMNISTValues(indexTrainData, listTrainData, values);

                    trainingData[indexTrainData] = listTrainData[indexTrainData].ToArray();

                    indexTrainData++;
                }

                for (int i = 0; i < firstLineValues.Length - 1; i++)
                {
                    maxValue[i] = listDataSpecifics[i].Max();

                    minValue[i] = listDataSpecifics[i].Min();

                    meanValue[i] = (listDataSpecifics[i].Sum()) / (lineCountTrainData - 1);
                }

                for (int i = 0; i < lineCountTrainData - 1; i++)
                {
                    for (int j = 0; j < firstLineValues.Length - 1; j++)
                    {
                        if ((maxValue[j] - minValue[j]) == 0)
                        {
                            trainingData[i][j] = 0;
                        }
                        else
                        {
                            trainingData[i][j] = (trainingData[i][j] - meanValue[j]) / (maxValue[j] - minValue[j]);
                        }
                    }

                }

            }
        }

        private static void encodeMNISTValues(int indexTrainData, List<double>[] listTrainData, string[] values)
        {
            int index = 0;

            foreach (var value in values)
            {
                if (index == (values.Length - 1))
                {
                    if (value == "0")
                    {
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "1")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "2")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "3")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "4")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "5")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "6")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "7")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "8")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                        listTrainData[indexTrainData].Add(0);
                    }
                    else if (value == "9")
                    {
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(0);
                        listTrainData[indexTrainData].Add(1);
                    }
                }
                else
                {
                    listTrainData[indexTrainData].Add(Convert.ToDouble(value, CultureInfo.InvariantCulture));

                    index++;
                }
            }
        }

        public static void ReadMNISTTestData()
        {
            using (var readerTestData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\mnist_test.csv"))
            {
                int lineCountTestData = 0;

                int indexTestData = 0;

                while (readerTestData.ReadLine() != null)
                {
                    lineCountTestData++;
                }

                testData = new double[lineCountTestData - 1][];

                List<double>[] listTestData = new List<double>[lineCountTestData - 1];

                readerTestData.DiscardBufferedData();

                readerTestData.BaseStream.Seek(0, SeekOrigin.Begin);

                var firstLine = readerTestData.ReadLine();

                var firstLineValues = firstLine.Split(',');

                double[] maxValue = new double[firstLineValues.Length - 1];

                double[] minValue = new double[firstLineValues.Length - 1];

                double[] meanValue = new double[firstLineValues.Length - 1];

                List<double>[] listDataSpecifics = new List<double>[firstLineValues.Length - 1];

                for (int i = 0; i < firstLineValues.Length - 1; i++)
                {
                    listDataSpecifics[i] = new List<double>();
                }

                while (!readerTestData.EndOfStream)
                {
                    var line = readerTestData.ReadLine();

                    var values = line.Split(',');

                    listTestData[indexTestData] = new List<double>();

                    for (int i = 0; i < firstLineValues.Length - 1; i++)
                    {
                        listDataSpecifics[i].Add(Convert.ToDouble(values[i], CultureInfo.InvariantCulture));
                    }
                    encodeMNISTValues(indexTestData, listTestData, values);

                    testData[indexTestData] = listTestData[indexTestData].ToArray();

                    indexTestData++;
                }

                for (int i = 0; i < firstLineValues.Length - 1; i++)
                {
                    maxValue[i] = listDataSpecifics[i].Max();

                    minValue[i] = listDataSpecifics[i].Min();

                    meanValue[i] = (listDataSpecifics[i].Sum()) / (lineCountTestData - 1);
                }

                for (int i = 0; i < lineCountTestData - 1; i++)
                {
                    for (int j = 0; j < firstLineValues.Length - 1; j++)
                    {
                        if ((maxValue[j] - minValue[j]) == 0)
                        {
                            testData[i][j] = 0;
                        }
                        else
                        {
                            testData[i][j] = (testData[i][j] - meanValue[j]) / (maxValue[j] - minValue[j]);
                        }
                    }

                }
            }
        }

        public void UnitTestMNISTTestRunner(int iterations, double learningrate, int batchSize, int[] hiddenLayerNeurons, int testCaseNumber)
        {
            int numberOfOutputs = 10;

            LearningApi api = new LearningApi();

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                ctx.DataDescriptor = new DataDescriptor();

                ctx.DataDescriptor.Features = new LearningFoundation.DataMappers.Column[firstLineValues.Length - 1];

                for (int i = 0; i < (firstLineValues.Length - 1); i++)
                {
                    ctx.DataDescriptor.Features[i] = new LearningFoundation.DataMappers.Column
                    {
                        Id = i,
                        Name = firstLineValues[i],
                        Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                        Index = i,
                    };
                }

                ctx.DataDescriptor.LabelIndex = firstLineValues.Length - 1;

                return trainingData;
            });

            //int[] hiddenLayerNeurons = { 6 };
            // Invoke the MLPerecptronAlgorithm with a specific learning rate, number of iterations
            api.UseMLPerceptron(learningrate, iterations, batchSize, testCaseNumber, hiddenLayerNeurons);

            IScore score = api.Run() as IScore;

            // Invoke the Predict method to predict the results on the test data
            var result = ((MLPerceptronResult)api.Algorithm.Predict(testData, api.Context)).results;

            int accurateResults = 0;

            // Check if the test data has been correctly classified by the neural network
            for (int i = 0; i < testData.Length; i++)
            {
                accurateResults++;

                for (int j = 0; j < numberOfOutputs; j++)
                {
                    //Assert.True(testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0));
                    if (testData[i][(testData[i].Length - numberOfOutputs) + j] != (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0))
                    {
                        accurateResults--;
                        break;
                    }
                }
            }

            /*
            float loss = expectedResults / (numberOfOutputs) / testData.Length;
            Debug.WriteLine($"{hiddleLayerToString(hiddenLayerNeurons)} - Loss: {loss}");
            */
            double accuracy = ((double)accurateResults * numberOfOutputs) / result.Length;
        }
    }
}
