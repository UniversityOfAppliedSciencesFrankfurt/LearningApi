using DigitRecognizer;
using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
namespace test.DigitRecognition
{
    [TestClass]
    public class DigitRecognizerUnitTests
    {


        /// <summary>
        /// MNIST_Test - unit test that train network with mnist and test the network with a noisy data of digit 7.
        /// </summary>
        [TestMethod]
        public void Seven_Test()
        {

            Debug.WriteLine("----------Seven_Test----------");
                       
            LearningApi api = TrainNetworkWithMNIST_TrainingData();


            var testData = new double[1][];

            testData[0] = new double[] { 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 84, 185, 159, 151, 60, 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 222, 254, 254, 254, 254, 241, 198, 198, 198, 198, 198, 198, 198, 198, 170, 52, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 67, 114, 72, 114, 163, 227, 254, 225, 254, 254, 254, 250, 229, 254, 254, 140, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 17, 66, 14, 67, 67, 67, 59, 21, 236, 254, 106, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 83, 253, 209, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 22, 233, 255, 83, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 129, 254, 238, 44, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59, 249, 254, 62, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 133, 254, 187, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 205, 248, 58, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 126, 254, 182, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 75, 251, 240, 57, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 19, 221, 254, 166, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 203, 254, 219, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 38, 254, 254, 77, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 31, 224, 254, 115, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 133, 254, 254, 52, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 61, 242, 254, 254, 52, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 121, 254, 254, 219, 40, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 121, 254, 207, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


            Debug.WriteLine("Starting Testing.");




            for (int i = 0; i < testData.Length; i++)
            {
                var expectedOutput = testData[i][0];
                var input = new double[1][];
                input[0] = testData[i];
                // Invoke the Predict method to predict the results on the test data
                var result = ((NeuroOCRResult)api.Algorithm.Predict(input, api.Context)).Result;


                // check if output is correct
                if (result == expectedOutput)
                {
                    Debug.WriteLine("Test Passed");
                }
                else
                {
                    Debug.WriteLine("Test Failed");
                }
            }
                                                  
            Debug.WriteLine("-------------------------------------------");

            // Now we have a trainded network and we want to use it for another test
            Zero_Test(api);

        }


        private void Zero_Test(LearningApi api)
        {

            Debug.WriteLine("----------Zero_Test----------");

            


            var testData = new double[1][];

            // define test data of zero here 
            testData[0] = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 150, 253, 202, 31, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 37, 251, 251, 253, 107, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 21, 197, 251, 251, 253, 107, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 110, 190, 251, 251, 251, 253, 169, 109, 62, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 253, 251, 251, 251, 251, 253, 251, 251, 220, 51, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 182, 255, 253, 253, 253, 253, 234, 222, 253, 253, 253, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 63, 221, 253, 251, 251, 251, 147, 77, 62, 128, 251, 251, 105, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 32, 231, 251, 253, 251, 220, 137, 10, 0, 0, 31, 230, 251, 243, 113, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 37, 251, 251, 253, 188, 20, 0, 0, 0, 0, 0, 109, 251, 253, 251, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 37, 251, 251, 201, 30, 0, 0, 0, 0, 0, 0, 31, 200, 253, 251, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 37, 253, 253, 0, 0, 0, 0, 0, 0, 0, 0, 32, 202, 255, 253, 164, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 140, 251, 251, 0, 0, 0, 0, 0, 0, 0, 0, 109, 251, 253, 251, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 217, 251, 251, 0, 0, 0, 0, 0, 0, 21, 63, 231, 251, 253, 230, 30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 217, 251, 251, 0, 0, 0, 0, 0, 0, 144, 251, 251, 251, 221, 61, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 217, 251, 251, 0, 0, 0, 0, 0, 182, 221, 251, 251, 251, 180, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 218, 253, 253, 73, 73, 228, 253, 253, 255, 253, 253, 253, 253, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 113, 251, 251, 253, 251, 251, 251, 251, 253, 251, 251, 251, 147, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 31, 230, 251, 253, 251, 251, 251, 251, 253, 230, 189, 35, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 62, 142, 253, 251, 251, 251, 251, 253, 107, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 72, 174, 251, 173, 71, 72, 30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


            Debug.WriteLine("Starting Testing.");




            for (int i = 0; i < testData.Length; i++)
            {
                var expectedOutput = testData[i][0];
                var input = new double[1][];
                input[0] = testData[i];
                // Invoke the Predict method to predict the results on the test data
                var result = ((NeuroOCRResult)api.Algorithm.Predict(input, api.Context)).Result;


                // check if output is correct
                if (result == expectedOutput)
                {
                    Debug.WriteLine("Test Passed");
                }
                else
                {
                    Debug.WriteLine("Test Failed");
                }
            }

            Debug.WriteLine("-------------------------------------------");

            


        }



        /// <summary>
        /// MNIST_Test - unit test that train network with mnist data set and test the network with a noisy data of digit 7.
        /// </summary>
        [TestMethod]
        public void Seven_Test_With_Image()
        {

            Debug.WriteLine("----------Seven_Test_With_Image----------");


            string trainingDataFile = $"{Directory.GetCurrentDirectory()}\\DigitRecognition\\TestFiles\\mnist_train_images.csv";
            if (File.Exists(trainingDataFile) == false)
            {
                trainingDataFile = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\DigitRecognition\\TestFiles\\mnist_train_images.csv";
            }

            var testData = new double[1][];

            var imgFilePath = Path.Combine(Directory.GetParent(trainingDataFile).FullName, "Test Images\\74.jpeg");


            testData[0] = new double[785];
            testData[0][0] = 7;
            GetImageData(new System.Drawing.Bitmap(System.Drawing.Bitmap.FromFile(imgFilePath))).CopyTo(testData[0], 1);

            LearningApi api = Train_Network_With_MNISTIMagesTrainingData();
                                          
            Debug.WriteLine("Starting Testing.");
                                 
            for (int i = 0; i < testData.Length; i++)
            {
                var expectedOutput = testData[i][0];
                var input = new double[1][];
                input[0] = testData[i];
                // Invoke the Predict method to predict the results on the test data
                var result = ((NeuroOCRResult)api.Algorithm.Predict(input, api.Context)).Result;


                // check if output is correct
                if (result == expectedOutput)
                {
                    Debug.WriteLine("Test Passed");
                }
                else
                {
                    Debug.WriteLine("Test Failed");
                }
            }
                                                      
            Debug.WriteLine("-------------------------------------------");

            
        }

        ///// <summary>
        ///// MNIST_Test - unit test consists of mnist data set
        /// </summary>
        [TestMethod]
        public void MNIST_Test()
        {

            Debug.WriteLine("----------MNIST_Test----------");
          var api = TrainNetworkWithMNIST_TrainingData();
            Stopwatch stopwatch = new Stopwatch();
            Debug.WriteLine("Reading Test Data");
            stopwatch.Start();

            var testDataFile = $"{Directory.GetCurrentDirectory()}\\DigitRecognition\\TestFiles\\mnist_test.csv";
            if (File.Exists(testDataFile) == false)
            {
                testDataFile = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\DigitRecognition\\TestFiles\\mnist_test.csv";
            }
            // Read the csv file which contains the test data
            using (var readerTestData = new StreamReader(testDataFile))
            {
                int lineCountTestData = 0;

                int indexTestData = 0;

                while (readerTestData.ReadLine() != null)
                {
                    lineCountTestData++;
                }
                Debug.WriteLine($"Testing {lineCountTestData} samples.");

                double[][] testData = new double[lineCountTestData][];

                List<double>[] listTestData = new List<double>[lineCountTestData];

                readerTestData.DiscardBufferedData();

                readerTestData.BaseStream.Seek(0, SeekOrigin.Begin);



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

                stopwatch.Stop();

                Debug.WriteLine("Reading Test data completed.");

                Debug.WriteLine("Elapsed Time: " + stopwatch.Elapsed);

                stopwatch.Reset();

                Debug.WriteLine("Starting Testing.");
                stopwatch.Start();

                int correctResults = 0;

                for (int i = 0; i < testData.Length; i++)
                {
                    var expectedOutput = testData[i][0];
                    var input = new double[1][];
                    input[0] = testData[i];
                    // Invoke the Predict method to predict the results on the test data
                    var result = ((NeuroOCRResult)api.Algorithm.Predict(input, api.Context)).Result;

                    // check if output is correct
                    if (result == expectedOutput)
                    {
                        correctResults++;
                    }
                }


                stopwatch.Stop();
                Debug.WriteLine("Testing Completed.");
                Debug.WriteLine("Elapsed Time: " + stopwatch.Elapsed);




                Debug.WriteLine("-------------------------------------------");
                Debug.WriteLine($"Performed {lineCountTestData} queries. Correct answers were {correctResults}.");
                Debug.WriteLine($"Network has a performance of {correctResults / Convert.ToDouble(lineCountTestData) * 100}");

            }
        }

        private LearningApi TrainNetworkWithMNIST_TrainingData()
        {
            //Read the csv file which contains the training data
            Stopwatch stopwatch = new Stopwatch();
            Debug.WriteLine("Reading Training Data");
            stopwatch.Start();
            string trainingDataFile = $"{Directory.GetCurrentDirectory()}\\DigitRecognition\\TestFiles\\mnist_train.csv";
            if (File.Exists(trainingDataFile) == false)
            {
                trainingDataFile = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\DigitRecognition\\TestFiles\\mnist_train.csv";
            }
            using (var readerTrainData = new StreamReader(trainingDataFile))
            {
                int lineCountTrainData = 0;

                int indexTrainData = 0;


                // Get data length
                while (readerTrainData.ReadLine() != null)
                {
                    lineCountTrainData++;
                }

                double[][] data = new double[lineCountTrainData][];

                List<double>[] listTrainData = new List<double>[lineCountTrainData];

                LearningApi api = new LearningApi();


                api.UseActionModule<object, double[][]>((notUsed, ctx) =>
                {

                                                         
                    readerTrainData.DiscardBufferedData();

                    readerTrainData.BaseStream.Seek(0, SeekOrigin.Begin);
                                                                                                         
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

                    stopwatch.Stop();

                    Debug.WriteLine("Reading Training data completed");

                    Debug.WriteLine("Elapsed Time: " + stopwatch.Elapsed.ToString());

                    return data;
                });

                // Invoke the NeuroOCR with a specific function, learning rate, number of input count, and number of hidden layer neurons
                api.AddModule(new NeuroOCR(new AForge.Neuro.BipolarSigmoidFunction(2), 784, 10, 1));

                IScore score = api.Run() as IScore;


                Debug.WriteLine("\r\nTraining completed.");
                Debug.WriteLine("-------------------------------------------");
                return api;
            }
        }



        /// <summary>
        /// MNIST_Test_WithImages - unit test consists of mnist data set in forms of images of 28 * 28 size
        /// </summary>
        [TestMethod]
        public void MNIST_Test_WithImages()
        {

            Debug.WriteLine("----------MNIST_Test_WithImages----------");
            var api = Train_Network_With_MNISTIMagesTrainingData();
            //var api = Train_Network_With_MNISTIMagesTrainingData(); // To Train with images
            Stopwatch stopwatch = new Stopwatch();
            Debug.WriteLine("Reading Test Data");
            stopwatch.Start();

            var testDataFile = $"{Directory.GetCurrentDirectory()}\\DigitRecognition\\TestFiles\\mnist_test_images.csv";
            if (File.Exists(testDataFile) == false)
            {
                testDataFile = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\DigitRecognition\\TestFiles\\mnist_test_images.csv";
            }
            // Read the csv file which contains the test data
            using (var readerTestData = new StreamReader(testDataFile))
            {
                int lineCountTestData = 0;

                int indexTestData = 0;

                while (readerTestData.ReadLine() != null)
                {
                    lineCountTestData++;
                }
                Debug.WriteLine($"Testing {lineCountTestData} samples.");

                double[][] testData = new double[lineCountTestData][];

                List<double>[] listTestData = new List<double>[lineCountTestData];

                readerTestData.DiscardBufferedData();

                readerTestData.BaseStream.Seek(0, SeekOrigin.Begin);



                while (!readerTestData.EndOfStream)
                {
                    var line = readerTestData.ReadLine();

                    var values = line.Split(',');

                    listTestData[indexTestData] = new List<double>();

                    listTestData[indexTestData].Add(Convert.ToDouble(values[0], CultureInfo.InvariantCulture));

                    var imgFilePath = Path.Combine(Directory.GetParent(testDataFile).FullName, values[1]);
                    listTestData[indexTestData].AddRange(GetImageData(new System.Drawing.Bitmap(System.Drawing.Bitmap.FromFile(imgFilePath))));

                    testData[indexTestData] = listTestData[indexTestData].ToArray();

                    indexTestData++;
                }
                Debug.WriteLine("Reading Test data completed.");

                Debug.WriteLine("Elapsed Time: " + stopwatch.Elapsed);

                stopwatch.Reset();

                Debug.WriteLine("Starting Testing.");
                stopwatch.Start();

                int correctResults = 0;

                for (int i = 0; i < testData.Length; i++)
                {

                    var expectedOutput = testData[i][0];
                    var input = new double[1][];
                    input[0] = testData[i];
                    // Invoke the Predict method to predict the results on the test data
                    var result = ((NeuroOCRResult)api.Algorithm.Predict(input, api.Context)).Result;

                    // check if output is correct
                    if (result == expectedOutput)
                    {
                        correctResults++;
                    }
                }
                                                                      
                stopwatch.Stop();
                Debug.WriteLine("Testing Completed.");
                Debug.WriteLine("Elapsed Time: " + stopwatch.Elapsed);
                               

                Debug.WriteLine("-------------------------------------------");
                Debug.WriteLine($"Performed {lineCountTestData} queries. Correct answers were {correctResults}.");
                Debug.WriteLine($"Network has a performance of {correctResults / Convert.ToDouble(lineCountTestData) * 100}");
            }

        }


        private LearningApi Train_Network_With_MNISTIMagesTrainingData()
        {
            Stopwatch stopwatch = new Stopwatch();
            Debug.WriteLine("Reading Training Data");
            stopwatch.Start();
            //Read the csv file which contains the training data

            string trainingDataFile = $"{Directory.GetCurrentDirectory()}\\DigitRecognition\\TestFiles\\mnist_train_images.csv";
            if (File.Exists(trainingDataFile) == false)
            {
                trainingDataFile = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\DigitRecognition\\TestFiles\\mnist_train_images.csv";
            }
            using (var readerTrainData = new StreamReader(trainingDataFile))
            {
                int lineCountTrainData = 0;

                int indexTrainData = 0;



                while (readerTrainData.ReadLine() != null)
                {
                    lineCountTrainData++;
                }

                double[][] data = new double[lineCountTrainData][];

                List<double>[] listTrainData = new List<double>[lineCountTrainData];

                LearningApi api = new LearningApi();

                api.UseActionModule<object, double[][]>((notUsed, ctx) =>
                {


                    readerTrainData.DiscardBufferedData();

                    readerTrainData.BaseStream.Seek(0, SeekOrigin.Begin);

                                                                                          

                    while (!readerTrainData.EndOfStream)
                    {
                        var line = readerTrainData.ReadLine();

                        var values = line.Split(',');

                        listTrainData[indexTrainData] = new List<double>();


                        listTrainData[indexTrainData].Add(Convert.ToDouble(values[0], CultureInfo.InvariantCulture));

                        var imgFilePath = Path.Combine(Directory.GetParent(trainingDataFile).FullName, values[1]);
                        listTrainData[indexTrainData].AddRange(GetImageData(new System.Drawing.Bitmap(System.Drawing.Bitmap.FromFile(imgFilePath))));

                        data[indexTrainData] = listTrainData[indexTrainData].ToArray();

                        indexTrainData++;
                    }

                    stopwatch.Stop();

                    Debug.WriteLine("Reading Training data completed");

                    Debug.WriteLine("Elapsed Time: " + stopwatch.Elapsed.ToString());

                    return data;
                });
                // Invoke the NeuroOCR with a function, specific learning rate, number of input count, and number of hidden layer neurons
                api.AddModule(new NeuroOCR(new AForge.Neuro.BipolarSigmoidFunction(2), 784, 10, 1));

                IScore score = api.Run() as IScore;


                Debug.WriteLine("\r\nTraining completed.");
                Debug.WriteLine("-------------------------------------------");
                return api;
            }
        }

        // Convert images to double array of pixels
         
        // 
        // <param name = "bmp"Image</param>
        // <returns>returns the double[]</returns>
        // 
        private double[] GetImageData(System.Drawing.Bitmap bmp)
        {
            double[] imageData = null;



            int height = bmp.Height;
            int width = bmp.Width;
            imageData = new double[height * width];
            int imagePointer = 0;

            // Read image pixel by [pixel
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    var pixel = bmp.GetPixel(i, j);
                    Color[] ColorArray =
        {
            Color.White,
            Color.Black
        };
                    pixel = GetClosestColor(ColorArray, pixel);


                    //Identify the black points of the image
                    if (pixel == System.Drawing.Color.Black)
                    {
                        imageData[imagePointer] = 1;
                    }
                    else
                    {
                        imageData[imagePointer] = -1;
                    }
                    imagePointer++;
                }

            }

            return imageData;
        }

        private Color GetClosestColor(Color[] colorArray, Color baseColor)
        {
            var colors = colorArray.Select(x => new { Value = x, Diff = GetDiff(x, baseColor) }).ToList();
            var min = colors.Min(x => x.Diff);
            return colors.Find(x => x.Diff == min).Value;
        }

        private int GetDiff(Color color, Color baseColor)
        {
            int a = color.A - baseColor.A,
                r = color.R - baseColor.R,
                g = color.G - baseColor.G,
                b = color.B - baseColor.B;
            return a * a + r * r + g * g + b * b;
        }


    }
    }
