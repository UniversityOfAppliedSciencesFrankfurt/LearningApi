using ImageBinarizer;
using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingProjectionAlgorithm;
using System;
using System.Drawing.Imaging;
using System.IO;
using LearningFoundation.Statistics;
using System.Collections.Generic;

namespace UnitTestRingProjectionAlgorithm
{
    [TestClass]
    public class RingProjectionTest
    {
        /// <summary>
        /// Iteration Path test
        /// </summary>
        [TestMethod]
        public void RingProjectionLoopPath()
        {
            int size = 21;
            double[][] data = new double[size][];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new double[size];
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = 0;
                }
            }
            new RingProjectionPipelineModule().RingProjection(data, out double[][] loopPath);
            string savePath = Path.Combine(AppContext.BaseDirectory, "LoopPath.csv");
            if (!File.Exists(savePath))
            {
                File.Create(savePath).Dispose();
            }
            using (StreamWriter streamWriter = new StreamWriter(savePath))
            {
                // Header for CSV file compatible with Excel
                streamWriter.WriteLine("sep=;");
                for (int y = 0; y < loopPath.Length; y++)
                {
                    for (int x = 0; x < loopPath[y].Length; x++)
                    {
                        streamWriter.Write(loopPath[y][x]);
                        if (x < loopPath[y].Length - 1)
                        {
                            streamWriter.Write(";");
                        }
                        else
                        {
                            streamWriter.WriteLine();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert High-endian to Low-endian and vice versa
        /// </summary>
        [TestMethod]
        public void ByteRevearsalTest()
        {
            Assert.AreEqual(new LoadMNISTPipelineModule().ReverseBytes(76), 1275068416);
            Assert.AreEqual(new LoadMNISTPipelineModule().ReverseBytes(60000), 1625948160);
        }

        /// <summary>
        /// Letter A samples with cross correlation and visualized function
        /// </summary>
        [TestMethod]
        public void RingProjection2D()
        {
            string[] testImages = { "LetterA.jpg", "LetterA45.jpg", "LetterA-45.jpg", "LetterA180.jpg" };
            string trainingImagesPath = Path.Combine(AppContext.BaseDirectory, "TestImages");

            LearningApi api;
            for (int i = 0; i < testImages.Length; i++)
            {
                api = new LearningApi();
                api.UseActionModule<object, double[][]>((input, ctx) =>
                {
                    Binarizer biImg = new Binarizer();
                    double[][] data = biImg.GetBinaryArray(
                        Path.Combine(trainingImagesPath, testImages[i]), 50);
                    return data;
                });
                api.UseRingProjectionPipelineComponent();
                api.AddModule(new RingProjectionFunctionToCSVPipelineModule("LetterA", i, ";", trainingImagesPath));
                api.Run();
            }

            // Loading the CSV files created before back to the test
            int count = 0;
            List<double[]> functions = new List<double[]>();
            while (File.Exists(Path.Combine(trainingImagesPath, $"LetterA.{count++}.csv"))) ;

            for (int i = 0; i < count - 1; i++)
            {
                using (var reader = new StreamReader(@Path.Combine(trainingImagesPath, $"LetterA.{i}.csv")))
                {
                    List<int> tempList = new List<int>();
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        int temp;
                        int.TryParse(values[1], out temp);
                        tempList.Add(temp);
                    }
                    double[] csvData = new double[tempList.Count];
                    int k = 0;
                    foreach (var data in tempList)
                    {
                        csvData[k++] = data;
                    }
                    functions.Add(csvData);
                }
            }

            // Calculate cross correlation coefficients
            List<double> corrCoefficient = new List<double>();

            foreach (var currentFunction in functions)
            {
                foreach (var function in functions)
                {
                    if (currentFunction != function)
                    {
                        double result = currentFunction.CorrCoeffOf(function);
                        corrCoefficient.Add(result);
                    }
                }
            }

            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, trainingImagesPath, "Boxplot"));
            string savePath = Path.Combine(trainingImagesPath, "Boxplot", "LetterA.txt");
            if (!File.Exists(savePath))
            {
                File.Create(savePath).Dispose();
            }
            using (StreamWriter streamWriter = new StreamWriter(savePath))
            {
                streamWriter.Write("LetterA ");
                foreach (var item in corrCoefficient)
                {
                    streamWriter.Write($" | {item.ToString("0.0000")}");
                }
            }
        }

        /// <summary>
        /// Image dimension reduction of MNIST database of handwritten digits (traning data)
        /// </summary>
        [TestMethod]
        public void RingProjectionMNISTImage()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<string[], BinaryReader[]>((input, ctx) =>
            {
                string[] trainDatabase = { "train-images.idx3-ubyte", "train-labels.idx1-ubyte" };
                string trainImages = Path.Combine(AppContext.BaseDirectory, "MNISTImages", trainDatabase[0]);
                string trainLabels = Path.Combine(AppContext.BaseDirectory, "MNISTImages", trainDatabase[1]);

                BinaryReader[] mnistData = new BinaryReader[2];
                mnistData[0] = new BinaryReader(new FileStream(trainImages, FileMode.Open));
                mnistData[1] = new BinaryReader(new FileStream(trainLabels, FileMode.Open));
                return mnistData;
            });
            api.AddModule(new LoadMNISTPipelineModule());
            MNISTImage[] mnistImages = api.Run() as MNISTImage[];

            int[] count = new int[10];
            for (int i = 0; i < count.Length; i++)
            {
                count[i] = 0;
            }

            for (int i = 0; i < mnistImages.Length; i++)
            {
                string basePath = Path.Combine(AppContext.BaseDirectory, $"Digit {mnistImages[i].Label}");
                Directory.CreateDirectory(basePath);
                int label = mnistImages[i].Label;
                
                // Create image file from MNIST dataset
                api = new LearningApi();
                api.UseActionModule<object, double[][]>((input, ctx) =>
                {
                    return ByteArrayToDoubleArray(mnistImages[i].Pixels);
                });
                api.AddModule(new DoubleArrrayToBitmapPipelineModule(mnistImages[i].Width, mnistImages[i].Height));
                api.AddModule(new BitmapToImageFilePipelineModule($"{label}.{++count[label]}", basePath, ImageFormat.Png));
                api.Run();

                // Apply Ring-Projected algorithm
                api = new LearningApi();
                api.UseActionModule<object, double[][]>((input, ctx) =>
                {
                    return ByteArrayToDoubleArray(mnistImages[i].Pixels);
                });
                api.UseActionModule<double[][], double[][]>((input, ctx) =>
                {
                    Binarizer biImg = new Binarizer();
                    double[][] data = biImg.ConvertToBinary(input, 30);
                    return data;
                });
                api.UseRingProjectionPipelineComponent();
                api.AddModule(new RingProjectionFunctionToCSVPipelineModule(label.ToString(), count[label], ";", basePath));
                api.Run();
            }

            // Calculate cross-correlation coefficients
            for (int i = 0; i < 10; i++)
            {
                CrossCorrCoeff(i);
            }
        }

        /// <summary>
        /// Convert 2D byte array to 2D double array
        /// </summary>
        /// <param name="byteArray">Input byte array</param>
        /// <returns></returns>
        private double[][] ByteArrayToDoubleArray(byte[][] byteArray)
        {
            double[][] doubleArray = new double[byteArray.Length][];

            for (int i = 0; i < byteArray.Length; i++)
            {
                doubleArray[i] = new double[byteArray[0].Length];
                for (int j = 0; j < byteArray[0].Length; j++)
                {
                    doubleArray[i][j] = byteArray[i][j];
                }
            }
            return doubleArray;
        }

        /// <summary>
        /// Calculate the cross-correlation coefficients among the Functions from the same digit
        /// </summary>
        /// <param name="digit">digit representation</param>
        private void CrossCorrCoeff(int digit)
        {
            string folder = Path.Combine(AppContext.BaseDirectory, $"Digit {digit}");
            int count = 0;

            List<double[]> newList = new List<double[]>();

            while (File.Exists(Path.Combine(folder, $"{digit}.{++count}.csv"))) ;
            for (int i = 1; i < count; i++)
            {
                using (var reader = new StreamReader(@Path.Combine(folder, $"{digit}.{i}.csv")))
                {
                    List<int> tempList = new List<int>();
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        int temp;
                        int.TryParse(values[1], out temp);
                        tempList.Add(temp);
                    }
                    double[] csvData = new double[tempList.Count];
                    int k = 0;
                    foreach (var data in tempList)
                    {
                        csvData[k++] = data;
                    }
                    newList.Add(csvData);
                }
            }

            List<double> corrCoefficient = new List<double>();
            bool init = false;
            double min = 0;
            double max = 0;
            double deviation = 0;
            double average = 0;
            foreach (var currentFunction in newList)
            {
                foreach (var function in newList)
                {
                    if (currentFunction != function)
                    {
                        double result = currentFunction.CorrCoeffOf(function);
                        if (!init)
                        {
                            min = result;
                            max = result;
                            init = true;
                        }

                        if (min > result)
                        {
                            min = result;
                        }
                        if (max < result)
                        {
                            max = result;
                        }
                        corrCoefficient.Add(result);
                        average += result;
                    }
                }
            }
            average /= corrCoefficient.Count;

            foreach (var item in corrCoefficient)
            {
                deviation += (item - average) * (item - average);
            }
            deviation /= corrCoefficient.Count;
            deviation = Math.Sqrt(deviation);

            // Export the statistical result of cross-correlation coefficients
            WriteToCSVStat(AppContext.BaseDirectory, digit, count - 1, average, min, max, deviation);
        }

        /// <summary>
        /// Provide the statistical result of cross-correlation coefficients among samples for each digit as CSV file
        /// </summary>
        /// <param name="baseFolder">save path of csv file</param>
        /// <param name="digit">digit representation</param>
        /// <param name="noSample">number of sample</param>
        /// <param name="average">average of cross correlation coefficients </param>
        /// <param name="min">minimum cross correlation coefficient</param>
        /// <param name="max">maximum cross correlation coefficient</param>
        /// <param name="deviation">standard deviation of cross correlation coefficients</param>
        private void WriteToCSVStat(string baseFolder, int digit, int noSample, double average, double min, double max, double deviation)
        {
            string csvPath = Path.Combine(baseFolder, "stat.csv");
            if (digit < 1)
            {
                File.Create(csvPath).Dispose();
                using (StreamWriter sw = new StreamWriter(csvPath))
                {
                    sw.WriteLine($"digit;number of samples;avgcorrcoeff;mincorrcoeff;maxcorrcoeff;deviation");
                    sw.Write($"{digit};");
                    sw.Write($"{noSample};");
                    sw.Write($"{average.ToString("0.0000")};");
                    sw.Write($"{min.ToString("0.0000")};");
                    sw.Write($"{max.ToString("0.0000")};");
                    sw.WriteLine($"{deviation.ToString("0.0000")}");
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(csvPath))
                {
                    sw.Write($"{digit};");
                    sw.Write($"{noSample};");
                    sw.Write($"{average.ToString("0.0000")};");
                    sw.Write($"{min.ToString("0.0000")};");
                    sw.Write($"{max.ToString("0.0000")};");
                    sw.WriteLine($"{deviation.ToString("0.0000")}");
                }
            }
        }
    }
}
