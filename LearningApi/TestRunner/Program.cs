using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using test.RestrictedBolzmannMachine2;

namespace TestRunner
{
    class Program
    {
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
                    double rate = double.Parse(tokens[1], NumberStyles.AllowDecimalPoint ,CultureInfo.InvariantCulture);

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

            Console.WriteLine($"Test execution completed. Executed {testNum} tests.");
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
                DeepRbmUnitTests test = new DeepRbmUnitTests();
                
                Stopwatch watch = new Stopwatch();
                Console.WriteLine($"{DateTime.Now} - Started test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)}");
                watch.Start();
                test.DigitRecognitionDeepTest(iterations, learningRate, layers);
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
                RbmMovieRecommendation test = new RbmMovieRecommendation();
                Stopwatch watch = new Stopwatch();
                Console.WriteLine($"{DateTime.Now} - Started test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)}");
                watch.Start();
                test.movieRecommendationTest(iterations, learningRate, layers[0] /*V*/, layers[1] /*H*/);
                watch.Stop();
                Console.WriteLine($"{DateTime.Now} - End test test I:{iterations} learning rate:{learningRate} Nodes:{String.Join("-", layers)} in {watch.ElapsedMilliseconds * 1000} sec.");
            }
            );
        }

        private static Task runTestold1(int iterations, double learningRate, int[] layers)
        {
            return Task.Run(() =>
            {
                RbmMovieRecommendation test = new RbmMovieRecommendation();

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
