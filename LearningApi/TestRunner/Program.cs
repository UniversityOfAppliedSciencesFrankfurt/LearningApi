using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using test.RestrictedBolzmannMachine2;
using test.MLPerceptron;

namespace TestRunner
{
    class Program
    {
        private static int iterationNum = 0;

        static void Main(string[] args)
        {
            int testNum = 0;
            int concurrentThreads = 4;

            Console.ForegroundColor = ConsoleColor.Green;
            string file;

            Console.WriteLine("Runner started!");
            if (args.Length == 0)
                file = "Tests.txt";
            else if (args.Length == 1)
                file = args[0];
            else if (args.Length == 2)
            {
                file = args[0];
                concurrentThreads = int.Parse(args[1]);
            }
            else
                throw new ArgumentException("Please provide the name of the test file as argument.");

            if (file.Contains("MNIST") == true)
            {
			    //Load the MNIST training and test data from the csv files into jagged arrays
                MNISTFileRead.ReadMNISTTrainingData();
                MNISTFileRead.ReadMNISTTestData();

                //Read the MNIST parameter test file content
                using (StreamReader sr = new StreamReader(file))
                {
                    int threadCounter = 0;
                    List<Task> tasks = new List<Task>();

                    sr.ReadLine(); //read header line.
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null && tasks.Count == 0)
                            break;
                        else if (line == null && tasks.Count > 0)
                        {
                            startBatch(tasks);
                            break;
                        }

                        var tokens = line.Split(";");

                        int value, iterations = 0;
                        if (int.TryParse(tokens[0], out value))
                        {
                            iterations = value;
                        }
                        double rate = double.Parse(tokens[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                        int batchsize = int.Parse(tokens[2]);
                        var tokens3 = tokens[3].Split("-");
                        List<int> nodes = new List<int>();
                        foreach (var item in tokens3)
                        {
                            nodes.Add(int.Parse(item));
                        }

                        if (threadCounter < concurrentThreads)
                        {
                            testNum++;
                            tasks.Add(runMLPTest(iterations, rate, batchsize, nodes.ToArray()));
                        }

                        if (++threadCounter == concurrentThreads)
                        {
                            startBatch(tasks);
                            threadCounter = 0;
                        }
                    }
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    int threadCounter = 0;
                    List<Task> tasks = new List<Task>();

                    sr.ReadLine(); //read header line.
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null && tasks.Count == 0)
                            break;
                        else if (line == null && tasks.Count > 0)
                        {
                            startBatch(tasks);
                            break;
                        }

                        var tokens = line.Split(";");
                        int iterations = int.Parse(tokens[0]);
                        double rate = double.Parse(tokens[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                        var tokens2 = tokens[2].Split("-");
                        List<int> nodes = new List<int>();
                        foreach (var item in tokens2)
                        {
                            nodes.Add(int.Parse(item));
                        }

                        if (threadCounter < concurrentThreads)
                        {
                            testNum++;
                            tasks.Add(runTest(iterations, rate, nodes.ToArray()));
                        }

                        if (++threadCounter == concurrentThreads)
                        {
                            startBatch(tasks);
                            threadCounter = 0;
                        }
                    }
                }
            }

            Console.WriteLine($"Test execution completed. Executed {testNum} tests.");

            Console.WriteLine("Enter the Saved Model to be loaded: ");

            string jsonFile = Console.ReadLine();

            MNISTLoadModel loadModel = new MNISTLoadModel();

            loadModel.LoadMNISTModel(jsonFile);
        }

        private static void startBatch(List<Task> tasks)
        {
            Console.WriteLine($"Starting batch of {tasks.Count} tests.");
            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
            Console.WriteLine("Batch ended.");
        }


        private static Task runTestold(int iterations, double learningRate, int[] layers)
        {
            return Task.Run(() =>
            {
                movieRecommendation test = new movieRecommendation();

                Stopwatch watch = new Stopwatch();
                Console.WriteLine($"{DateTime.Now} - Started test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)}");
                watch.Start();
                test.movieRecommendationTestDeepRbm(iterations, learningRate, layers);
                watch.Stop();
                Console.WriteLine($"{DateTime.Now} - End test test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)} in {watch.ElapsedMilliseconds * 1000} sec.");
            }
            );
        }


        /// <summary>
        /// Please use this method to execute CRBm Test in runner.
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="learningRate"></param>
        /// <param name="layers"></param>
        /// <returns></returns>
        private static Task runTest(int iterations, double learningRate, int[] layers)
        {
            return Task.Run(() =>
            {
                //RbmHandwrittenDigitUnitTests test = new RbmHandwrittenDigitUnitTests();
                movieRecommendation test = new movieRecommendation();
                Stopwatch watch = new Stopwatch();
                Console.WriteLine($"{DateTime.Now} - Started test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)}");
                watch.Start();
                test.movieRecommendationTestRbm(iterations, learningRate, layers[0] /*V*/, layers[1] /*H*/);
                watch.Stop();
                Console.WriteLine($"{DateTime.Now} - End test test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)} in {watch.ElapsedMilliseconds * 1000} sec.");
            }
            );
        }

        private static Task runMLPTest(int iterations, double learningRate, int batchSize, int[] layers)
        {
            return Task.Run(() =>
            {
                MNISTFileRead test = new MNISTFileRead();
                //RbmHandwrittenDigitUnitTests test = new RbmHandwrittenDigitUnitTests();
                Stopwatch watch = new Stopwatch();
                Console.WriteLine($"{DateTime.Now} - Started test Epochs:{iterations} learning rate:{learningRate} BatchSize:{batchSize} HiddenLayerNeurons:{String.Join("-", layers)}");
                watch.Start();
                test.UnitTestMNISTTestRunner(iterations, learningRate, batchSize, layers, Program.iterationNum++);
                watch.Stop();
                Console.WriteLine($"{DateTime.Now} - End test test Epochs:{iterations} learning rate:{learningRate} BatchSize:{batchSize} HiddenLayerNeurons:{String.Join("-", layers)} in {watch.ElapsedMilliseconds / 1000} sec.");
            }
            );
        }


        private static Task runTestold1(int iterations, double learningRate, int[] layers)
        {
            return Task.Run(() =>
            {
                movieRecommendation test = new movieRecommendation();

                Stopwatch watch = new Stopwatch();
                Console.WriteLine($"{DateTime.Now} - Started test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)}");
                watch.Start();
                test.movieRecommendationTestCRbm(iterations, learningRate, layers[0] /*V*/, layers[1] /*H*/);
                watch.Stop();
                Console.WriteLine($"{DateTime.Now} - End test test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)} in {watch.ElapsedMilliseconds * 1000} sec.");
            }
            );
        }

    }
}
