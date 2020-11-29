using LearningFoundation;
using LearningFoundation.DataMappers;
using LearningFoundation.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SumAlgorithm;
using System.Diagnostics;
using System.IO;

namespace SumAlgorithmTest
{
    [TestClass]
    public class SumAlgorithmTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ///**************TRAINING MODEL IS CREATED - Data is read by TRAIN CSV file*********///
            ///
            string trainDataPathString = @"SampleData\power_consumption_train.csv";

            var trainDataPath = Path.Combine(Directory.GetCurrentDirectory(), trainDataPathString);

            LearningApi api = new LearningApi(LoadMetaData());

            // Using CSV data provider for reading CSV data
            api.UseCsvDataProvider(trainDataPath, ',', false);

            // Using mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            // Using sum algorithm for training
            api.UseSum();

            api.Run();

            SumScore score = api.GetScore() as SumScore;

            Debug.WriteLine("Loss : " + score.Loss);

            ///**************PREDICTION AFTER MODEL IS CREATED - Data is read by TEST CSV file*********/// 

            string testDataPathString = @"SampleData\power_consumption_test.csv";

            var testDataPath = Path.Combine(Directory.GetCurrentDirectory(), testDataPathString);

            LearningApi apiPrediction = new LearningApi(LoadMetaData());

            // Using CSV data provider for reading CSV data
            apiPrediction.UseCsvDataProvider(testDataPath, ',', false);

            // Using mapper for data, which will extract (map) required columns 
            apiPrediction.UseDefaultDataMapper();

            double[][] testData = apiPrediction.Run() as double[][];

            // Using previously trained sum model for prediction
            var result = api.Algorithm.Predict(testData, api.Context) as SumResult;

            Assert.AreEqual(result.PredictedValues[0], testData[0][2]);
            Assert.AreEqual(result.PredictedValues[1], testData[1][2]);
        }

        /// <summary>
        /// Meta data for power_consumption_prediction sample
        /// </summary>
        /// <returns></returns>
        private DataDescriptor LoadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[3];
            des.Features[0] = new Column { Id = 1, Name = "solar", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "wind", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[2] = new Column { Id = 3, Name = "power", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };

            des.LabelIndex = 2;
            return des;
        }
    }
}
