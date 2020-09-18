using System;
using LearningFoundation;
using LearningFoundation.DataProviders;
using AnomalyDetectionK;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
namespace AnomalyDetectionTest
{
    /// <summary>
    /// Testing class of Anomaly Detection with k-mean Algorithom 
    /// </summary>
    public class AnomalyDetectionTestClass
    {
        private const string m_IrisData = @"Samples\iris.csv";
        public LearningApi api1 = new LearningApi();
        private LearningApi api2 = new LearningApi();
        public double[][] Data;
        int count;
        int Dlength;


        /// <summary>
        /// Test case that Test the outcome result with some known inputs and outputs
        /// </summary>
        [Fact]
        public void Verified_Expected_and_Outcomes_Result()
        {
            // The path of CSV file
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_IrisData);
            //learningApi Initialization
            api1 = new LearningApi(Iprojecthelp.Helpers.GetDescriptor());
            //retrieve Data
            api1.UseCsvDataProvider(path, ',', true);
            // Deserializing and parsing of the csv file.
            api1.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {
                        double converted;
                        if (double.TryParse((string)item[i], out converted))
                            row.Add(converted);
                        else
                            throw new System.Exception("Column is not convertable to double.");
                    }
                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "0":
                            row.Add(0);
                            break;
                        case "1":
                            row.Add(1);
                            break;
                    }
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });
            // Getting the training data. first 80% data is trainig data.
            api1.UseActionModule<double[][], double[][]>((double[][] data, IContext ctx) =>
            {
                Data = data;
                Dlength = data.Length;
                int trainingDataLength = (int)Math.Ceiling(data.Length * 0.8);
                var trainingData = new double[trainingDataLength][];
                for (count = 0; count < trainingDataLength; count++)
                {
                    trainingData[count] = data[count];  // 80% of data from iris.csv is assigning as training data
                }
                return trainingData;
            });
            // Integrating algorithm with LearningApi.
            api1.UseADAlgorithm(0.1);
            // Geting Training data
            var score = api1.Run() as AnomalyDetectionAlgorithmScore;
            for (int i = count; i < Dlength; i++)
            {
                double[][] testData = new double[i][];
                for (int r = 0; r < i; r++)
                {
                    testData[r] = Data[r];
                }
                var result = api1.Algorithm.Predict(testData, api1.Context) as AnomalyDetectionAlgorithmResult;
                //Using last 3 test Data with Anomaly value 
                // 11	1.9	    0 anomaly
                // 5.1  15      1 no anomaly
                // 5.7  10      1 no anomaly


                if (i - 1 < Dlength-3)
                {
                    Assert.Equal(1, result.Results[i - 1]);  // return 1 if there has no anomaly
                }
                else
                    Assert.Equal(0, result.Results[i - 1]);  // return 0  if there has anomaly
            }
        }


        /// <summary>
        /// Test case that tests if the calculated predictions are correct.
        /// </summary>
        [Fact]
        public void PredictNumber_Equals_TestNumber()
        {
            // The path of CSV file
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_IrisData);
            //learningApi Initialization
            api1 = new LearningApi(Iprojecthelp.Helpers.GetDescriptor());
            //retrieve Data
            api1.UseCsvDataProvider(path, ',', true);
            // Deserializing and parsing of the csv file.
            api1.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {
                        double converted;
                        if (double.TryParse((string)item[i], out converted))
                            row.Add(converted);
                        else
                            throw new System.Exception("Column is not convertable to double.");
                    }
                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "0":
                            row.Add(0);
                            break;
                        case "1":
                            row.Add(1);
                            break;
                    }
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });
            // Getting the training data. first 80% data is trainig data.
            api1.UseActionModule<double[][], double[][]>((double[][] data, IContext ctx) =>
            {
                Data = data;
                Dlength = data.Length;
                int trainingDataLength = (int)Math.Ceiling(data.Length * 0.8);
                var trainingData = new double[trainingDataLength][];
                for (count = 0; count < trainingDataLength; count++)
                {
                    trainingData[count] = data[count];
                }
                return trainingData;
            });
            // Integrating algorithm with LearningApi.
            api1.UseADAlgorithm(0.1);
            // Geting Training data
            var score = api1.Run() as AnomalyDetectionAlgorithmScore;
            for (int i = count; i < Dlength; i++)
            {
                double[][] testData = new double[i][];
                for (int r = 0; r < i; r++)
                {
                    testData[r] = Data[r];
                }
                var result = api1.Algorithm.Predict(testData, api1.Context) as AnomalyDetectionAlgorithmResult;
                if (result != null)
                    Assert.Equal(testData.Length, result.Results.Length); // number of inputs equal number of outputs
            }
        }


        ///<summary>
        /// Test case that checks if ArgumentNullException is thrown in case of null argument for test data in prediction method.
        /// </summary>
        [Fact]
        public void Predict_WithTestDataNull_ThrowsArgumentNullException()
        {           // The path of CSV file
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_IrisData);
            //learningApi Initialization
            api1 = new LearningApi(Iprojecthelp.Helpers.GetDescriptor());
            //retrieve Data
            api1.UseCsvDataProvider(path, ',', true);
            // Deserializing and parsing of the csv file.
            api1.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {
                        double converted;
                        if (double.TryParse((string)item[i], out converted))
                            row.Add(converted);
                        else
                            throw new System.Exception("Column is not convertable to double.");
                    }
                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "0":
                            row.Add(0);
                            break;
                        case "1":
                            row.Add(1);
                            break;
                    }
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });
            // Getting the training data. first 80% data is trainig data.
            api1.UseActionModule<double[][], double[][]>((double[][] data, IContext ctx) =>
            {
                Data = data;
                Dlength = data.Length;
                int trainingDataLength = (int)Math.Ceiling(data.Length * 0.8);
                var trainingData = new double[trainingDataLength][];
                for (count = 0; count < trainingDataLength; count++)
                {
                    trainingData[count] = data[count];
                }
                return trainingData;
            });
            // Integrating algorithm with LearningApi.
            api1.UseADAlgorithm(0.1);

            // Geting Training data
            var score = api1.Run() as AnomalyDetectionAlgorithmScore;
            Assert.Throws<ArgumentNullException>(() => api1.Algorithm.Predict(null, api1.Context)); // check the ArgumentNullException
        }


        ///<summary>
        ///  Test case that tests if number of Cluster value is calculated for each training data
        /// </summary>
        [Fact]
        public void Run_NumberOfAlphaValues_EqualsTo_NumberOfTrainingData()
        {          
            // The path of CSV file
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_IrisData);
            //learningApi Initialization
            api1 = new LearningApi(Iprojecthelp.Helpers.GetDescriptor());
            //retrieve Data
            api1.UseCsvDataProvider(path, ',', true);
            // Deserializing and parsing of the csv file.
            api1.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {
                        double converted;
                        if (double.TryParse((string)item[i], out converted))
                            row.Add(converted);
                        else
                            throw new System.Exception("Column is not convertable to double.");
                    }
                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "0":
                            row.Add(0);
                            break;
                        case "1":
                            row.Add(1);
                            break;
                    }
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });
            // Getting the training data. first 80% data is trainig data.
            api1.UseActionModule<double[][], double[][]>((double[][] data, IContext ctx) =>
            {
                Data = data;
                Dlength = data.Length;
                int trainingDataLength = (int)Math.Ceiling(data.Length * 0.8);
                var trainingData = new double[trainingDataLength][];
                for (count = 0; count < trainingDataLength; count++)
                {
                    trainingData[count] = data[count];
                }          
                return trainingData;
            });
            // Integrating algorithm with LearningApi.
            api1.UseADAlgorithm(0.1);
            // Geting Training data
            var score = api1.Run() as AnomalyDetectionAlgorithmScore;
            var trainingData1 = new double[10][];
            trainingData1[0] = new[] { 7.5, 8.5 };
            trainingData1[1] = new[] { 9.5, 0.5 };
            trainingData1[2] = new[] { 1.0, 2.0 };
            trainingData1[3] = new[] { 1.5, 2.0 };
            trainingData1[4] = new[] { 0.5, 2.0 };
            trainingData1[5] = new[] { 2.5, 5.5 };
            trainingData1[6] = new[] { 2.5, 1.0 };
            trainingData1[7] = new[] { 4.0, 0.0 };
            trainingData1[8] = new[] { 9.5, 1.5 };
            trainingData1[9] = new[] { 1.0, -3.5 };
            var score1 = api1.Algorithm.Run(trainingData1, api1.Context) as AnomalyDetectionAlgorithmScore;
            if (score1 != null)
                Assert.Equal(trainingData1.Length, score1.cl.Length); //compare the length of training data array and length of the claster Array.
        }


        ///<summary>
        /// Test case that checks if ArgumentNullException is thrown in case of null argument for train in Run method.
        /// </summary>
        [Fact]
        public void Run_WithNull_ThrowsArgumentNuillException()
        {
            Verified_Expected_and_Outcomes_Result();
            Assert.Throws<ArgumentNullException>(() => api1.Algorithm.Run(null, api1.Context));
        }


        ///<summary>
        /// Test case that tests if data value is calculated for each training data
        /// </summary>
        [Fact]
        public void Traain_NumberOfAlphaValues_EqualsTo_NumberOfTrainingData()
        {           // The path of CSV file
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_IrisData);
            //learningApi Initialization
            api1 = new LearningApi(Iprojecthelp.Helpers.GetDescriptor());
            //retrieve Data
            api1.UseCsvDataProvider(path, ',', true);
            // Deserializing and parsing of the csv file.
            api1.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {
                        double converted;
                        if (double.TryParse((string)item[i], out converted))
                            row.Add(converted);
                        else
                            throw new System.Exception("Column is not convertable to double.");
                    }
                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "0":
                            row.Add(0);
                            break;
                        case "1":
                            row.Add(1);
                            break;
                    }
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });
            // Getting the training data. first 80% data is trainig data.
            api1.UseActionModule<double[][], double[][]>((double[][] data, IContext ctx) =>
            {
                Data = data;
                Dlength = data.Length;
                int trainingDataLength = (int)Math.Ceiling(data.Length * 0.8);
                var trainingData1 = new double[trainingDataLength][];
                for (count = 0; count < trainingDataLength; count++)
                {
                    trainingData1[count] = data[count];
                }
                return trainingData1;
            });
            // Integrating algorithm with LearningApi.
            api1.UseADAlgorithm(0.1);
            // Geting Training data
            var score = api1.Run() as AnomalyDetectionAlgorithmScore;
            var trainingData = new double[10][];
            trainingData[0] = new[] { 5.5, 8.5 };
            trainingData[1] = new[] { 9.5, 0.5 };
            trainingData[2] = new[] { 1.0, 2.0 };
            trainingData[3] = new[] { 4.5, 2.0 };
            trainingData[4] = new[] { 2.5, 2.0 };
            trainingData[5] = new[] { 7.5, 3.5 };
            trainingData[6] = new[] { 2.5, 1.5 };
            trainingData[7] = new[] { 6.0, 4.0 };
            trainingData[8] = new[] { 10.5, 2.5 };
            trainingData[9] = new[] { 12.0, -2.5 };
            var score2 = api1.Algorithm.Train(trainingData, api1.Context) as AnomalyDetectionAlgorithmScore;
            if (score2 != null)
                Assert.Equal(trainingData.Length, score2.OldData.Length); // compare the input and output data array langth
        }
    }
}


