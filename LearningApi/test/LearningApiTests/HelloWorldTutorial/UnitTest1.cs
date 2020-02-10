using HelloWorldTutorial;
using LearningFoundation;
using LearningFoundation.DataMappers;
using LearningFoundation.DataProviders;
using LearningFoundation.Normalizers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace LearningApiTest
{
    /// <summary>
    /// Testing the Functionality of learning-API Algorithm 
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// UnitestMethod1 which describes how to use algorithm
        /// Here you  can use CSV provider with same sample data.
        /// Use here some normalizer module, which normalize sample data.        
        /// </summary>
        [TestMethod]
        public void UnitestMethod1()
        {
            // giving complete path of the file 
            var file_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"SampleData\Tutorial\temp_rain.csv");

            // loading column names to the desc variable
            var desc = LoadMetaData();

            // Create instance of learning api. 
            LearningApi api = new LearningApi(desc);

            // Using of CSV Data Provider Module to load the csv data 
            api.UseCsvDataProvider(file_path, ',', false, 1);

            // Use mapper for data, which will extract (map) required columns 
            // gets only the required columns
            api.UseDefaultDataMapper();

            // Using some Normalizer Module
            // Normalize the data nothing but removing the errors or noise from the csv data
            api.UseMinMaxNormalizer();

            //Using LearningApi Algorithm
            api.UseLearningApiAlgorithm();

            //Training
            LearningApiAlgorithm score = api.Run() as LearningApiAlgorithm;

            //Prediction            
            var predictingData = new double[2][] {
                    new double[]{ 25, 18 },
                    new double[]{ 32, 23 }
                    };

            LearningApiAlgorithmResult result = api.Algorithm.Predict(predictingData, api.Context) as LearningApiAlgorithmResult;

            Assert.AreEqual(Math.Round(result.PredictedValue[0]), 18);
            Assert.AreEqual(Math.Round(result.PredictedValue[1]), 23);
        }

        /// <summary>
        /// UnitestMethod2 which describes how to use algorithm
        /// In this case use ActionMethod to read data instead of CSV provider
        /// Also use normalizer
        /// </summary>
        [TestMethod]
        public void UnitestMethod2()
        {
            // column names assign
            var desc = LoadMetaData();

            // setting to api
            LearningApi api = new LearningApi(desc);

            //Using Action Module
            api.UseActionModule<double[][], double[][]>((input, ctx) =>
            {
                // real data loading here
                return LoadRealDataSample();
            });

            //Using some Normalizer Module
            api.UseMinMaxNormalizer();

            //Using LearningApi Algorithm
            api.UseLearningApiAlgorithm();

            //Training
            LearningApiAlgorithm score = api.Run() as LearningApiAlgorithm;

            //Prediction            
            var predictingData = new double[2][] {
                    new double[]{ 25, 18 },
                    new double[]{ 32, 23 }
                    };

            LearningApiAlgorithmResult result = api.Algorithm.Predict(predictingData, api.Context) as LearningApiAlgorithmResult;

            Assert.AreEqual(Math.Round(result.PredictedValue[0]), 18);
            Assert.AreEqual(Math.Round(result.PredictedValue[1]), 23);
        }

        /// <summary>
        /// UnitestMethod3 which describes how to use module. Put sample algorithm in pipeline.
        /// Here you  can use CSV provider with same sample data.
        /// Use here some normalizer module, which normalize sample data.
        /// </summary>
        [TestMethod]
        public void UnitestMethod3()
        {
            var file_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"SampleData\Tutorial\temp_rain.csv");

            var desc = LoadMetaData();
            LearningApi api = new LearningApi(desc);

            //Using CSV Data Provider Module
            api.UseCsvDataProvider(file_path, ',', false, 1);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            //Using some Normalizer Module
            api.UseMinMaxNormalizer();

            //Using LearningApi Module
            api.UseLearningApiModule();

            //Using LearningApi Algorithm
            api.UseLearningApiAlgorithm();

            //Training
            LearningApiAlgorithm score = api.Run() as LearningApiAlgorithm;

            //Prediction            
            var predictingData = new double[2][] {
                    new double[]{ 25, 18 },
                    new double[]{ 32, 23 }
                    };

            LearningApiAlgorithmResult result = api.Algorithm.Predict(predictingData, api.Context) as LearningApiAlgorithmResult;

            Assert.AreEqual(Math.Round(result.PredictedValue[0]), 18);
            Assert.AreEqual(Math.Round(result.PredictedValue[1]), 23);
        }

        /// <summary>
        /// UnitestMethod4 which describes how to use module. Put sample algorithm in pipeline.
        /// In this case use ActionMethod to read data instead of CSV provider
        /// Also use normalizer
        /// </summary>
        [TestMethod]  
        public void UnitestMethod4()
        {
            var desc = LoadMetaData();
            LearningApi api = new LearningApi(desc);  

            //Using Action Module
            api.UseActionModule<double[][], double[][]>((input, ctx) =>
            {
                return LoadRealDataSample();
            });

            //Using some Normalizer Module
            api.UseMinMaxNormalizer();

            //Using LearningApi Module
            api.UseLearningApiModule();

            //Using LearningApi Algorithm
            api.UseLearningApiAlgorithm();

            //Training
            LearningApiAlgorithm score = api.Run() as LearningApiAlgorithm;

            //Prediction            
            var predictingData = new double[2][] {
                    new double[]{ 25, 18 },
                    new double[]{ 32, 23 }
                    };

            LearningApiAlgorithmResult result = api.Algorithm.Predict(predictingData, api.Context) as LearningApiAlgorithmResult;

            Assert.AreEqual(Math.Round(result.PredictedValue[0]), 18);
            Assert.AreEqual(Math.Round(result.PredictedValue[1]), 23);
        } 

        /// <summary>
        /// UniteTestMethod5 which describes how to use Save and Load
        /// Use code from method UniTestMethod3 to load a CSV file with portion of data.
        /// Get algorithm value by calling Predict method and ensure that it corresponds for loaded portion of data. For example average value.
        /// Then call learningAPi.Save(filename.json);
        /// As next in the same test method load saved file with api2 = LearningAPI.Load(fileName.json)
        /// Then load second portion of data and call api2.Predict(). Ensure that calculated algorithm value(for example average) corresponds to portion of data 1 and 2.
        /// </summary>
        [TestMethod]
        public void UnitestMethod5()
        {
            var file_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"SampleData\Tutorial\temp_rain.csv");

            var desc = LoadMetaData();
            LearningApi api = new LearningApi(desc);

            //Using CSV Data Provider Module
            api.UseCsvDataProvider(file_path, ',', false, 1);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            //Using some Normalizer Module
            api.UseMinMaxNormalizer();

            //Using LearningApi Algorithm
            api.UseLearningApiAlgorithm();

            //Training
            LearningApiAlgorithm score = api.Run() as LearningApiAlgorithm;

            //Prediction            
            var predictingData = new double[2][] {
                    new double[]{ 25, 18 },
                    new double[]{ 32, 23 }
                    };

            LearningApiAlgorithmResult result = api.Algorithm.Predict(predictingData, api.Context) as LearningApiAlgorithmResult;

            Assert.AreEqual(Math.Round(result.PredictedValue[0]), 18);
            Assert.AreEqual(Math.Round(result.PredictedValue[1]), 23);

            api.Save("LearningApiAlgorithmSave");

            var api2 = LearningApi.Load("LearningApiAlgorithmSave");

            LearningApiAlgorithmResult result2 = api2.Algorithm.Predict(predictingData, api.Context) as LearningApiAlgorithmResult;

            Assert.AreEqual(Math.Round(result2.PredictedValue[0]), 18);
            Assert.AreEqual(Math.Round(result2.PredictedValue[1]), 23);
        } 

        /// <summary>
        /// Load Data about the data nothing but the columns name and number of rows defined etc
        /// </summary>
        /// <returns></returns>
        private DataDescriptor LoadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[2];
            des.Features[0] = new Column { Id = 1, Name = "Temperature", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "Chances of Precipitation", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };           
            des.LabelIndex = -1;
            return des;
        }

        /// <summary>
        /// Loading the Data
        /// </summary>
        /// <returns></returns> 
        private double[][] LoadRealDataSample()
        {
            // 8 rows and 2 columns
            var data = new double[8][] {
                    new double[]{ 36, 60 },
                    new double[]{ 40, 0  },
                    new double[]{ 10, 30  },
                    new double[]{ 15, 20  },
                    new double[]{ 2, 0 },
                    new double[]{ 12, 16 },
                    new double[]{ 4, 2 },
                    new double[]{ 26, 28 }
                    };
            return data;
        }
    }
}
