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
                    double rate = double.Parse(tokens[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                    int vNodes = int.Parse(tokens[2]);
                    int hNodes = int.Parse(tokens[3]);

                    if (threadCounter < concurrentThreads)
                    {
                        testNum++;
                        tasks.Add(runTest(iterations, rate, vNodes, hNodes));
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

        private static Task runTest(int iterations, double learningRate, int visNodes, int hidNodes)
        {
            return Task.Run(() =>
            {
                RbmHandwrittenDigitUnitTests test = new RbmHandwrittenDigitUnitTests();

                Stopwatch watch = new Stopwatch();
                Console.WriteLine($"{DateTime.Now} - Started test I:{iterations} learning rate:{learningRate} VisibleNodes:{visNodes} HiddenNodes:{hidNodes}");
                watch.Start();
                test.DigitRecognitionTest(iterations, learningRate, visNodes, hidNodes);
                watch.Stop();
                Console.WriteLine($"{DateTime.Now} - End test test I:{iterations} learning rate:{learningRate} VisibleNodes:{visNodes} HiddenNodes:{hidNodes} in {watch.ElapsedMilliseconds * 1000} sec.");
            }
            );
        }

    }
}
