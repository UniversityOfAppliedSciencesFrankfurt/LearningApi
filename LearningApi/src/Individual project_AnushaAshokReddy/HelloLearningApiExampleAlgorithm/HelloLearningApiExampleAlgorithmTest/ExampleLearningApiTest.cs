using LearningFoundation;
using LearningFoundation.DataMappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace ExampleLearningApiTest
{
    [TestClass]
    public class LinearRegressionTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            double learningRate = 0.01;

            int epochs = 1000;

            string trainDataPathString = @"SampleData\house_price_train.csv";

            var trainDataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), trainDataPathString);

            LearningApi api = new LearningApi(LoadMetaData());

            // Using CSV data provider for reading CSV data
            api.UseCsvDataProvider(trainDataPath, ',', false);

            // Using mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            // Using linear regression algorithm for training
            api.UseLinearRegression(learningRate, epochs);

            api.Run();

            LinearRegressionScore score = api.GetScore() as LinearRegressionScore;

            Debug.WriteLine("Loss : " + score.Loss[epochs - 1]);

            ///**************PREDICTION AFTER MODEL IS CREATED*********///

            string testDataPathString = @"SampleData\house_price_test.csv";

            var testDataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), testDataPathString);

            LearningApi apiPrediction = new LearningApi(LoadMetaData());

            // Using CSV data provider for reading CSV data
            apiPrediction.UseCsvDataProvider(testDataPath, ',', false);

            // Using mapper for data, which will extract (map) required columns 
            apiPrediction.UseDefaultDataMapper();

            double[][] testData = apiPrediction.Run() as double[][];

            // Using previously trained linear regression model for prediction
            var result = api.Algorithm.Predict(testData, api.Context) as LinearRegressionResult;

            Assert.AreEqual(Math.Round(result.PredictedValues[0], 1), testData[0][2]);
            Assert.AreEqual(Math.Round(result.PredictedValues[1], 1), testData[1][2]);
        }

        /// <summary>
        /// Meta data for house_price_prediction sample
        /// </summary>
        /// <returns></returns>
        private DataDescriptor LoadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[3];
            des.Features[0] = new Column { Id = 1, Name = "size", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "rooms", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[2] = new Column { Id = 3, Name = "price", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };

            des.LabelIndex = 2;
            return des;
        }

    }
}
