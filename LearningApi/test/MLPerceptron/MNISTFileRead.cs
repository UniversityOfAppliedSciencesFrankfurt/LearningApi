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

        public static double[][] testData { get; set; }

        public static string[] inputFeaturesAndLabelValues { get; set; }

        /// <summary>
        /// This method reads the content of the training data and stores the normalized data into a jagged array which is fed as input to the neural network training algorithm
        /// </summary>
        public static void ReadMNISTTrainingData()
        {
            // Read content from the MNIST training data file
            using (var readerTrainData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\mnist_train.csv"))
            {
                int lineCountTrainData = 0;

                int indexTrainData = 0;

                // Calculate the number of training data examples
                while (readerTrainData.ReadLine() != null)
                {
                    lineCountTrainData++;
                }

                int numberOfTrainingExamples = lineCountTrainData - 1;

                trainingData = new double[numberOfTrainingExamples][];

                List<double>[] listTrainData = new List<double>[numberOfTrainingExamples];

                readerTrainData.DiscardBufferedData();

                readerTrainData.BaseStream.Seek(0, SeekOrigin.Begin);

                var firstLine = readerTrainData.ReadLine();

                inputFeaturesAndLabelValues = firstLine.Split(',');

                int numberOfInputFeatures = inputFeaturesAndLabelValues.Length - 1;

                double[] maxValueOfInputFeature = new double[numberOfInputFeatures];

                double[] minValueOfInputFeature = new double[numberOfInputFeatures];

                double[] meanValueOfInputFeature = new double[numberOfInputFeatures];

                List<double>[] listFeatureData = new List<double>[numberOfInputFeatures];

                for (int i = 0; i < numberOfInputFeatures; i++)
                {
                    listFeatureData[i] = new List<double>();
                }

                //Read every example from the training data set and update it in a local jagged array
                while (!readerTrainData.EndOfStream)
                {
                    var line = readerTrainData.ReadLine();

                    var featureAndLabelValues = line.Split(',');

                    listTrainData[indexTrainData] = new List<double>();

                    for (int i = 0; i < numberOfInputFeatures; i++)
                    {
                        listFeatureData[i].Add(Convert.ToDouble(featureAndLabelValues[i], CultureInfo.InvariantCulture));
                    }

                    encodeMNISTValues(indexTrainData, listTrainData, featureAndLabelValues);

                    trainingData[indexTrainData] = listTrainData[indexTrainData].ToArray();

                    indexTrainData++;
                }

                // Perform normalization of the data stored in the jagged array by using the following formula for every input feature:
                // normalizedFeatureValue = (originalFeautreValue - meanValueOfFeautreOverAllExamples)/(maxValueOfFeautreOverAllExamples - minValueOfFeautreOverAllExamples)
                for (int i = 0; i < numberOfInputFeatures; i++)
                {
                    maxValueOfInputFeature[i] = listFeatureData[i].Max();

                    minValueOfInputFeature[i] = listFeatureData[i].Min();

                    meanValueOfInputFeature[i] = (listFeatureData[i].Sum()) / (numberOfTrainingExamples);
                }

                for (int i = 0; i < numberOfTrainingExamples; i++)
                {
                    for (int j = 0; j < numberOfInputFeatures; j++)
                    {
                        if ((maxValueOfInputFeature[j] - minValueOfInputFeature[j]) == 0)
                        {
                            trainingData[i][j] = 0;
                        }
                        else
                        {
                            trainingData[i][j] = (trainingData[i][j] - meanValueOfInputFeature[j]) / (maxValueOfInputFeature[j] - minValueOfInputFeature[j]);
                        }
                    }

                }

            }
        }

        /// <summary>
        /// This method encodes every feature and label value for a training/test example and stores it to a list
        /// </summary>
        /// <param name="indexTrainData"></param>
        /// <param name="listMnistExampleData"></param>
        /// <param name="featureAndLabelValues"></param>
        private static void encodeMNISTValues(int indexTrainData, List<double>[] listMnistExampleData, string[] featureAndLabelValues)
        {
            int index = 0;

            //Iterate over every feature and label of the training data example
            foreach (var value in featureAndLabelValues)
            {
                
                //If the loop is iterating over the input features, add every input feature of the example to the list listTrainData
                if (index != (featureAndLabelValues.Length - 1))
                {
                    listMnistExampleData[indexTrainData].Add(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                    index++;
                }
                //Check if the loop iteration has reached the output label index
                else
                {
                    //If the label is "0", create 10 output neurons with the 1st neuron set to 1 and rest to 0
                    if (value == "0")
                    {
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "1", create 10 output neurons with the 2nd neuron set to 1 and rest to 0
                    else if (value == "1")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "2", create 10 output neurons with the 3rd neuron set to 1 and rest to 0
                    else if (value == "2")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "3", create 10 output neurons with the 4th neuron set to 1 and rest to 0
                    else if (value == "3")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "4", create 10 output neurons with the 5th neuron set to 1 and rest to 0
                    else if (value == "4")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "5", create 10 output neurons with the 6th neuron set to 1 and rest to 0
                    else if (value == "5")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "6", create 10 output neurons with the 7th neuron set to 1 and rest to 0
                    else if (value == "6")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "7", create 10 output neurons with the 8th neuron set to 1 and rest to 0
                    else if (value == "7")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "8", create 10 output neurons with the 9th neuron set to 1 and rest to 0
                    else if (value == "8")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                        listMnistExampleData[indexTrainData].Add(0);
                    }
                    //If the label is "9", create 10 output neurons with the 10th neuron set to 1 and rest to 0
                    else if (value == "9")
                    {
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(0);
                        listMnistExampleData[indexTrainData].Add(1);
                    }
                }
            }
        }


        /// <summary>
        /// This method reads the content of the test data and stores the normalized data into a jagged array which is fed as input to the neural network prediction algorithm
        /// </summary>
        public static void ReadMNISTTestData()
        {
            // Read content from the MNIST test data file
            using (var readerTestData = new StreamReader($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\mnist_test.csv"))
            {
                int lineCountTestData = 0;

                int indexTestData = 0;

                // Calculate the number of test data examples
                while (readerTestData.ReadLine() != null)
                {
                    lineCountTestData++;
                }

                int numberOfTestDataExamples = lineCountTestData - 1;

                testData = new double[numberOfTestDataExamples][];

                List<double>[] listTestData = new List<double>[numberOfTestDataExamples];

                readerTestData.DiscardBufferedData();

                readerTestData.BaseStream.Seek(0, SeekOrigin.Begin);

                var firstLine = readerTestData.ReadLine();

                var inputFeaturesAndLabelValues = firstLine.Split(',');

                int numberOfInputFeatures = inputFeaturesAndLabelValues.Length - 1;

                double[] maxValueOfInputFeature = new double[numberOfInputFeatures];

                double[] minValueOfInputFeature = new double[numberOfInputFeatures];

                double[] meanValueOfInputFeature = new double[numberOfInputFeatures];

                List<double>[] listFeatureData = new List<double>[numberOfInputFeatures];

                for (int i = 0; i < numberOfInputFeatures; i++)
                {
                    listFeatureData[i] = new List<double>();
                }

                //Read every example from the test data set and update it in a local jagged array
                while (!readerTestData.EndOfStream)
                {
                    var line = readerTestData.ReadLine();

                    var featureAndLabelValues = line.Split(',');

                    listTestData[indexTestData] = new List<double>();

                    for (int i = 0; i < numberOfInputFeatures; i++)
                    {
                        listFeatureData[i].Add(Convert.ToDouble(featureAndLabelValues[i], CultureInfo.InvariantCulture));
                    }
                    encodeMNISTValues(indexTestData, listTestData, featureAndLabelValues);

                    testData[indexTestData] = listTestData[indexTestData].ToArray();

                    indexTestData++;
                }

                // Perform normalization of the data stored in the jagged array by using the following formula for every input feature:
                // normalizedFeatureValue = (originalFeautreValue - meanValueOfFeautreOverAllExamples)/(maxValueOfFeautreOverAllExamples - minValueOfFeautreOverAllExamples)
                for (int i = 0; i < numberOfInputFeatures; i++)
                {
                    maxValueOfInputFeature[i] = listFeatureData[i].Max();

                    minValueOfInputFeature[i] = listFeatureData[i].Min();

                    meanValueOfInputFeature[i] = (listFeatureData[i].Sum()) / (numberOfTestDataExamples);
                }

                for (int i = 0; i < numberOfTestDataExamples; i++)
                {
                    for (int j = 0; j < numberOfInputFeatures; j++)
                    {
                        if ((maxValueOfInputFeature[j] - minValueOfInputFeature[j]) == 0)
                        {
                            testData[i][j] = 0;
                        }
                        else
                        {
                            testData[i][j] = (testData[i][j] - meanValueOfInputFeature[j]) / (maxValueOfInputFeature[j] - minValueOfInputFeature[j]);
                        }
                    }

                }
            }
        }
        /// <summary>
        /// This method is invoked from the test runner module to test the MNIST dataset on a multilayer perceptron network
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="learningrate"></param>
        /// <param name="batchSize"></param>
        /// <param name="hiddenLayerNeurons"></param>
        /// <param name="testCaseNumber"></param>
        public void UnitTestMNISTTestRunner(int iterations, double learningrate, int batchSize, int[] hiddenLayerNeurons, int testCaseNumber)
        {
            int numberOfOutputNeurons = 10;

            int numberOfInputFeatures = inputFeaturesAndLabelValues.Length - 1;

            LearningApi api = new LearningApi();

            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                ctx.DataDescriptor = new DataDescriptor();

                ctx.DataDescriptor.Features = new LearningFoundation.DataMappers.Column[numberOfInputFeatures];

                for (int i = 0; i < (numberOfInputFeatures); i++)
                {
                    ctx.DataDescriptor.Features[i] = new LearningFoundation.DataMappers.Column
                    {
                        Id = i,
                        Name = inputFeaturesAndLabelValues[i],
                        Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                        Index = i,
                    };
                }

                ctx.DataDescriptor.LabelIndex = numberOfInputFeatures;

                return trainingData;
            });

            // Invoke the MLPerecptronAlgorithm with a specific learning rate, number of iterations, batch size, test case number, number of hidden layer neurons
            api.UseMLPerceptron(learningrate, iterations, batchSize, testCaseNumber, hiddenLayerNeurons);

            IScore score = api.Run() as IScore;

            // Invoke the Predict method to predict the results on the test data
            var predictedResult = ((MLPerceptronResult)api.Algorithm.Predict(testData, api.Context)).results;

            //Create file to store the test data results
            StreamWriter resultFile = new StreamWriter($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\mnist_result_" + testCaseNumber.ToString() + ".csv");

            double[] tempResultArray = new double[numberOfOutputNeurons];

            int index = 0;

            // Update the predictedResult file with the predicted results on the test dataset
            while (index < predictedResult.Length)
            {
                for (int i = index; i < index + numberOfOutputNeurons; i++)
                {
                    tempResultArray[i - index] = predictedResult[i];
                }

                double max = tempResultArray.Max();

                resultFile.WriteLine(Array.IndexOf(tempResultArray, max));

                index = index + numberOfOutputNeurons;
            }

            int numberOfCorrectClassifications = 0;

            // Calculate the number of test data elements that have been correctly classified
            for (int i = 0; i < testData.Length; i++)
            {
                numberOfCorrectClassifications++;

                for (int j = 0; j < numberOfOutputNeurons; j++)
                {
                    if (testData[i][(testData[i].Length - numberOfOutputNeurons) + j] != (predictedResult[i * numberOfOutputNeurons + j] >= 0.5 ? 1 : 0))
                    {
                        numberOfCorrectClassifications--;
                        break;
                    }
                }
            }

            //Calculate accuracy by using the following formula:
            // accuracy = numberOfCorrectClassifications/numberOfTestDataElements
            double accuracy = ((double)numberOfCorrectClassifications * numberOfOutputNeurons) / predictedResult.Length;
            resultFile.WriteLine("Accuracy = {0}", accuracy.ToString());
      
            resultFile.Close();

            // Save the trained model if the accuracy is greater than a threshold value
            if (accuracy > 0.9)
            {
                // This is where trained model is saved.
                api.Save("mnist_savedmodel_" + testCaseNumber.ToString());
            }
        }
    }
}
