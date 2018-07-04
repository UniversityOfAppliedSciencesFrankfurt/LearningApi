using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeuralNet.RestrictedBolzmannMachine2;
using ImageBinarizer;
using NeuralNet.Perceptron;
using LearningFoundation.DataProviders;
using LearningFoundation;
using Xunit;
using LearningFoundation.DataMappers;


namespace test.RestrictedBolzmannMachine2
{
    public class RBM_002HandwrittenDigits
    {
        static RBM_002HandwrittenDigits()
        {

        }

        private DataDescriptor getDescriptorForDigits()
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[4096];
            des.LabelIndex = -1;

            des.Features = new Column[4096];
            int k = 1;
            for (int i = 0; i < 4096; i++)
            {

                des.Features[i] = new Column { Id = k, Name = "col" + k, Index = i, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
                k = k + 1;
            }
            return des;
        }

        [Fact]
        public void DigitRecognitionTest()
        {
            var DigitDatasetCSVPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\DigitDataset.csv");

            LearningApi api = new LearningApi(this.getDescriptorForDigits());

            // Initialize data provider
            api.UseCsvDataProvider(DigitDatasetCSVPath, ',', false, 0);
            api.UseDefaultDataMapper();
            //api1.UseRbm(0.2, 100, 784, 392);
            api.UseRbm(0.2, 1, 4096, 10);

            RbmResult score = api.Run() as RbmResult;

            // test data
            var DigitTestCSVPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\DigitTest.csv");
            LearningApi api1 = new LearningApi(this.getDescriptorForDigits());

            double[][] testData = new double[10][];
            var reader = new StreamReader(File.OpenRead(DigitTestCSVPath));

            StreamReader sr = new StreamReader(DigitTestCSVPath);
            String line;

            while ((line = sr.ReadLine()) != null)
            {
                for (int i = 0; i < testData.Length; i++)
                {
                    double[] doubles = Array.ConvertAll(line.Split(','), Double.Parse);
                    testData[i] = doubles;
                }

            }

            var result = api.Algorithm.Predict(testData, api.Context); 
            double[][] predictedData = new double[10][];
            predictedData = ((NeuralNet.RestrictedBolzmannMachine2.RbmResult)result).VisibleNodesPredictions;

            TextWriter tw = new StreamWriter("PredictedDigit1.txt");

            //you need two loops - 1. To loop through alle out predicted nodes. 2. To loop through every single node
            int k = 1;
            for (int i = 0; i < predictedData.Length; i++)
            {
                tw.WriteLine();
                tw.WriteLine("Image" + i + "Prediction");
                foreach (var item in predictedData[i])
                {
                    if ((k % 65) == 0)
                    {
                        tw.WriteLine();
                        k = 1;
                    }
                    tw.Write(item);
                    k++;
                }
                tw.WriteLine();
            }
            tw.Close();

        }
    }
}
