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
    /// <summary>
    /// Class MLPerceptronUnitTests contains the unit test cases to test the ML Perceptron algorithm
    /// </summary>
    public class MLPUnitTests
    {
        /// <summary>
        /// MLPerceptronUnitTests Default constructor
        /// </summary>
        static MLPUnitTests()
        {

        }

        /// <summary>
        /// UnitTestOne - The first unit test consists of non linear set of data pairs between 0 and 1,
        /// that are classified into two groups, 0 and 1.s
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

        /// <summary>
        /// UnitTestIris - The second unit test consists of the “Iris” dataset which classifies iris plants into three species. It includes 100 samples distributed between three species, with some properties about each flower
        /// </summary>
        [Theory]
        //[InlineData(new int[] { 5, 2 })]
        [InlineData(1000, 0.1, 25, new int[] { 6 }, 1)]
        //[InlineData(new int[] { 7, 2 })]
        //[InlineData(new int[] { 8, 2 })]
        //[InlineData(new int[] { 9, 2 })]
        //[InlineData(new int[] { 10, 2 })]
        //[InlineData(new int[] { 20, 2 })]
        //[InlineData(new int[] { 30, 2 })]
        //[InlineData(new int[] { 30, 20, 10, 5 })]
        //[InlineData(new int[] { 15, 10, 5, 2 })]
        //[InlineData(new int[] { 50, 30, 20, 10 })]
        //[InlineData(new int[] { 10, 7, 5, 3 })]
        //[InlineData(new int[] { 125, 77, 45, 34 , 19, 12, 9, 3})]
        //[InlineData(new int[] { 50, 30, 20 })]

        public void UnitTestIris(int iterations, double learningrate, int batchSize, int[] hiddenLayerNeurons, int iterationnumber)
        {

            // TODO
            // test by using same test cases in dependence on number of iterations.
            // test by dependence on number of layers
            // test by dependence on number of neurons in layers.
            // [2 layers]: [x,x] [1,2] [1,3] [1,4]

            // Read the csv file which contains the training data
            using (var readerTrainData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\TrainingDataIris.csv"))
            {
                int lineCountTrainData = 0;

                int indexTrainData = 0;

                int numberOfOutputs = 2;

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

                    double[] maxValue = new double[firstLineValues.Length - 1];

                    double[] minValue = new double[firstLineValues.Length - 1];

                    double[] meanValue = new double[firstLineValues.Length - 1];

                    List<double>[] listDataSpecifics = new List<double>[firstLineValues.Length - 1];

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

                        encodeIrisValues(indexTrainData, listTrainData, values);

                        data[indexTrainData] = listTrainData[indexTrainData].ToArray();

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
                            data[i][j] = (data[i][j] - meanValue[j]) / (maxValue[j] - minValue[j]);
                        }

                    }

                    return data;
                });

                //int[] hiddenLayerNeurons = { 6 };
                // Invoke the MLPerecptronAlgorithm with a specific learning rate, number of iterations
                api.UseMLPerceptron(learningrate, iterations, batchSize, iterationnumber, hiddenLayerNeurons);

                IScore score = api.Run() as IScore;

                // Read the csv file which contains the test data
                using (var readerTestData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\TestDataIris.csv"))
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
                        encodeIrisValues(indexTestData, listTestData, values);

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
                            testData[i][j] = (testData[i][j] - meanValue[j]) / (maxValue[j] - minValue[j]);
                        }

                    }

                    // Invoke the Predict method to predict the results on the test data
                    var result = ((MLPerceptronResult)api.Algorithm.Predict(testData, api.Context)).results;

                    float expectedResults = 0;

                    // Check if the test data has been correctly classified by the neural network
                    for (int i = 0; i < lineCountTestData - 1; i++)
                    {
                        for (int j = 0; j < numberOfOutputs; j++)
                        {
                            //Assert.True(testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0));
                            if (testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0))
                                expectedResults++;
                        }
                    }

                    float loss = expectedResults / (numberOfOutputs) / testData.Length;
                    Debug.WriteLine($"{hiddleLayerToString(hiddenLayerNeurons)} - Loss: {loss}");
                }
            }
        }

        private static string hiddleLayerToString(int[] hiddenLayer)
        {
            return $"[{String.Join(',', hiddenLayer)}]";
        }

        private static void encodeIrisValues(int indexTrainData, List<double>[] listTrainData, string[] values)
        {
            foreach (var value in values)
            {
                if (value == "setosa")
                {
                    listTrainData[indexTrainData].Add(0);
                    listTrainData[indexTrainData].Add(0);
                }
                else if (value == "versicolor")
                {
                    listTrainData[indexTrainData].Add(0);
                    listTrainData[indexTrainData].Add(1);
                }
                else if (value == "virginica")
                {
                    listTrainData[indexTrainData].Add(1);
                    listTrainData[indexTrainData].Add(0);
                }
                else
                {
                    listTrainData[indexTrainData].Add(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                }
            }
        }

        [Theory]
        //[InlineData(new int[] { 5, 2 })]
        [InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32, 32, 32, 32, 32 }, 10)]
        [InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32, 32, 32, 32 }, 9)]
        [InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32, 32, 32 }, 8)]
        [InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32, 32 }, 7)]
        [InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32 }, 6)]
        [InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32 }, 5)]
        [InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32 }, 4)]
        [InlineData(25, 0.01, 128, new int[] { 32, 32, 32 }, 3)]
        [InlineData(25, 0.01, 128, new int[] { 32, 32 }, 2)]
        [InlineData(2, 0.01, 128, new int[] { 32 }, 1)]

        public void UnitTestMNIST(int iterations, double learningrate, int batchSize, int[] hiddenLayerNeurons, int testCaseNumber)
        {
            MNISTFileRead mnistObj = new MNISTFileRead();

            MNISTFileRead.ReadMNISTTrainingData();

            MNISTFileRead.ReadMNISTTestData();

            // TODO
            // test by using same test cases in dependence on number of iterations.
            // test by dependence on number of layers
            // test by dependence on number of neurons in layers.
            // [2 layers]: [x,x] [1,2] [1,3] [1,4]

            // Read the csv file which contains the training data

            int numberOfOutputs = 10;

            LearningApi api = new LearningApi();

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                ctx.DataDescriptor = new DataDescriptor();

                ctx.DataDescriptor.Features = new LearningFoundation.DataMappers.Column[MNISTFileRead.firstLineValues.Length - 1];

                for (int i = 0; i < (MNISTFileRead.firstLineValues.Length - 1); i++)
                {
                    ctx.DataDescriptor.Features[i] = new LearningFoundation.DataMappers.Column
                    {
                        Id = i,
                        Name = MNISTFileRead.firstLineValues[i],
                        Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                        Index = i,
                    };
                }

                ctx.DataDescriptor.LabelIndex = MNISTFileRead.firstLineValues.Length - 1;

                return MNISTFileRead.trainingData;
            });

            //int[] hiddenLayerNeurons = { 6 };
            // Invoke the MLPerecptronAlgorithm with a specific learning rate, number of iterations
            api.UseMLPerceptron(learningrate, iterations, batchSize, testCaseNumber, hiddenLayerNeurons);

            IScore score = api.Run() as IScore;

            // Invoke the Predict method to predict the results on the test data
            var result = ((MLPerceptronResult)api.Algorithm.Predict(MNISTFileRead.testData, api.Context)).results;

            int accurateResults = 0;

            // Check if the test data has been correctly classified by the neural network
            for (int i = 0; i < MNISTFileRead.testData.Length; i++)
            {
                accurateResults++;

                for (int j = 0; j < numberOfOutputs; j++)
                {
                    //Assert.True(testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0));
                    if (MNISTFileRead.testData[i][(MNISTFileRead.testData[i].Length - numberOfOutputs) + j] != (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0))
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
            Debug.WriteLine($"Accuracy: {accuracy}");

        }

        /// <summary>
        /// This test case uses the German Dataset as the training dataset for credit prediction.
        /// It classifies people described by a set of attributes as good or bad for bank regarding the credit risk.
        /// </summary>
        [Fact]
        public void UnitTestCreditApproval()
        {
            // Read the csv file which contains the training data
            using (var readerTrainData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\TrainingDataCreditApproval.csv"))
            {
                int lineCountTrainData = 0;

                int indexTrainData = 0;

                int numberOfOutputs = 1;

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

                    double[] maxValue = new double[15];

                    double[] minValue = new double[15];

                    double[] meanValue = new double[15];

                    List<double>[] listDataSpecifics = new List<double>[15];

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

                    for (int i = 0; i < 15; i++)
                    {
                        listDataSpecifics[i] = new List<double>();
                    }

                    while (!readerTrainData.EndOfStream)
                    {
                        var line = readerTrainData.ReadLine();

                        var values = line.Split(',');

                        listTrainData[indexTrainData] = new List<double>();

                        for (int i = 0; i < 15; i++)
                        {
                            listDataSpecifics[i].Add(Convert.ToDouble(values[i], CultureInfo.InvariantCulture));
                        }

                        for (int i = 0; i < values.Length; i++)
                        {
                            listTrainData[indexTrainData].Add(Convert.ToDouble(values[i], CultureInfo.InvariantCulture));
                        }

                        data[indexTrainData] = listTrainData[indexTrainData].ToArray();

                        indexTrainData++;
                    }

                    for (int i = 0; i < 15; i++)
                    {
                        maxValue[i] = listDataSpecifics[i].Max();

                        minValue[i] = listDataSpecifics[i].Min();

                        meanValue[i] = (listDataSpecifics[i].Sum()) / (lineCountTrainData - 1);
                    }

                    for (int i = 0; i < lineCountTrainData - 1; i++)
                    {
                        for (int j = 0; j < 15; j++)
                        {
                            data[i][j] = (data[i][j] - meanValue[j]) / (maxValue[j] - minValue[j]);
                        }

                    }

                    return data;
                });

                int[] hiddenLayerNeurons = { 9 };

                // Invoke the MLPerecptronAlgorithm with a specific learning rate, number of iterations, number of hidden layers
                api.UseMLPerceptron(0.1, 20, 1, 1, hiddenLayerNeurons);

                IScore score = api.Run() as IScore;

                // Read the csv file which contains the training data
                using (var readerTestData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\TestDataCreditApproval.csv"))
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

                    var firstLine = readerTestData.ReadLine();

                    var firstLineValues = firstLine.Split(',');

                    double[] maxValue = new double[15];

                    double[] minValue = new double[15];

                    double[] meanValue = new double[15];

                    List<double>[] listDataSpecifics = new List<double>[15];

                    for (int i = 0; i < 15; i++)
                    {
                        listDataSpecifics[i] = new List<double>();
                    }

                    while (!readerTestData.EndOfStream)
                    {
                        var line = readerTestData.ReadLine();

                        var values = line.Split(',');

                        listTestData[indexTestData] = new List<double>();

                        for (int i = 0; i < 15; i++)
                        {
                            listDataSpecifics[i].Add(Convert.ToDouble(values[i], CultureInfo.InvariantCulture));
                        }

                        for (int i = 0; i < values.Length; i++)
                        {
                            listTestData[indexTestData].Add(Convert.ToDouble(values[i], CultureInfo.InvariantCulture));
                        }

                        testData[indexTestData] = listTestData[indexTestData].ToArray();

                        indexTestData++;
                    }

                    for (int i = 0; i < 15; i++)
                    {
                        maxValue[i] = listDataSpecifics[i].Max();

                        minValue[i] = listDataSpecifics[i].Min();

                        meanValue[i] = (listDataSpecifics[i].Sum()) / (lineCountTestData - 1);
                    }

                    for (int i = 0; i < lineCountTestData - 1; i++)
                    {
                        for (int j = 0; j < 15; j++)
                        {
                            testData[i][j] = (testData[i][j] - meanValue[j]) / (maxValue[j] - minValue[j]);
                        }

                    }

                    // Invoke the Predict method to predict the results on the test data
                    var result = ((MLPerceptronResult)api.Algorithm.Predict(testData, api.Context)).results;

                    int creditApprovedCorrectClassification = 0, creditDeclinedCorrectClassification = 0;

                    // Check how many samples of the test data have been correctly classified by the neural network
                    for (int i = 0; i < lineCountTestData - 1; i++)
                    {
                        for (int j = 0; j < numberOfOutputs; j++)
                        {
                            if (testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0))
                            {
                                if (testData[i][(testData[i].Length - numberOfOutputs) + j] == 0)
                                {
                                    creditDeclinedCorrectClassification++;
                                }
                                else
                                {
                                    creditApprovedCorrectClassification++;
                                }
                            }
                        }
                    }
                }
            }


        }


        /// <summary>
        /// Loads a single JPG and create a binarized version with zeros and ones.
        /// Look after executing of test for file binary.txt.
        /// </summary>
        [Fact]
        public void BinarizerTest()
        {

            string trainingImagesPath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "MLPerceptron"), "TrainingImages");
            Binarizer bizer = new Binarizer();
            bizer.CreateBinary(Path.Combine(trainingImagesPath, "1 (168).jpeg"), "binary.txt");

        }
    }

}






