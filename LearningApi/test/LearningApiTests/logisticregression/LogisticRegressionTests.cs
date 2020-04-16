using LearningFoundation;
using LearningFoundation.DataMappers;
using LearningFoundation.DataProviders;
using LearningFoundation.Normalizers;
using LearningFoundation.Statistics;
using LogisticRegression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Test.LogisticRegression
{
    [TestClass]
    public class LogisticRegressionTests
    {
        public LogisticRegressionTests()
        {
            ///
        }

        /// <summary>
        /// Performs the LogisticRegression on specified dataset with 10 iteration and 0.13 learning rate.
        /// </summary>
        [TestMethod]
        public void LogisticsRegression_Test_iterations_10_learningrate_013()
        {
            var desc = loadMetaData();
            LearningApi api = new LearningApi(desc);

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample();
            });

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseMinMaxNormalizer();

            //run logistic regression for 10 iterations with learningRate=0.13
            api.UseLogisticRegression(0.13, 10);

            api.Run();

            LogisticRegressionScore score = api.GetScore() as LogisticRegressionScore;

            //Errors during each iteration. IF the learningRate is suitable errors is describing for every next iteration 
            Assert.AreEqual(Math.Round(score.Errors[0], 5), 0.24278);
            Assert.AreEqual(Math.Round(score.Errors[1], 5), 0.23749);
            Assert.AreEqual(Math.Round(score.Errors[2], 5), 0.23359);
            Assert.AreEqual(Math.Round(score.Errors[3], 5), 0.23010);
            Assert.AreEqual(Math.Round(score.Errors[4], 5), 0.22740);
            Assert.AreEqual(Math.Round(score.Errors[5], 5), 0.22476);
            Assert.AreEqual(Math.Round(score.Errors[6], 5), 0.22271);
            Assert.AreEqual(Math.Round(score.Errors[7], 5), 0.22065);
            Assert.AreEqual(Math.Round(score.Errors[8], 5), 0.21902);
            Assert.AreEqual(Math.Round(score.Errors[9], 5), 0.21739);

            //LG Model Best Found model in 10 iteration
            Assert.AreEqual(Math.Round(score.Weights[0], 5), 0.06494);
            Assert.AreEqual(Math.Round(score.Weights[1], 5), 0.21584);
            Assert.AreEqual(Math.Round(score.Weights[2], 5), 0.89901);
            Assert.AreEqual(Math.Round(score.Weights[3], 5), 0.51497);
            Assert.AreEqual(Math.Round(score.Weights[4], 5), -0.30213);
            Assert.AreEqual(Math.Round(score.Weights[5], 5), -0.30213);
            Assert.AreEqual(Math.Round(score.Weights[6], 5), -0.85624);


            //define data for testing (prediction)
            LearningApi apiPrediction = new LearningApi(desc);
            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            apiPrediction.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                var data = new object[4][]
                {
                    new object[]{0.202,"blue", "male",13,"yes" },
                    new object[]{0.447,"green","female",37,"no" },
                    new object[]{0.120,"red","male", "21", "yes" },
                    new object[]{0.313,"green","male",22,"yes" },
                    };
                return data;
            });

            // Use mapper for data, which will extract (map) required columns 
            apiPrediction.UseDefaultDataMapper();
            var testData = apiPrediction.Run();

            //use previous trained model
            var result = api.Algorithm.Predict(testData as double[][], api.Context) as LogisticRegressionResult;

            Assert.AreEqual(Math.Round(result.PredictedValues[0], 5), 1E-05);
            Assert.AreEqual(Math.Round(result.PredictedValues[1], 5), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[2], 5), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[3], 5), 0);
        }

        /// <summary>
        /// Performs the LogisticRegression algortim on dataset with 10 iteration and 0.15 learning rate.
        /// </summary>
        [TestMethod]
        public void LogisticRegression_Test_Real_Example()
        {
            string m_binary_data_path = @"SampleData\binary\admit_binary.csv";

            var binary_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_binary_data_path);

            LearningApi api = new LearningApi(loadMetaData1());
            api.UseCsvDataProvider(binary_path, ',', false, 1);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseMinMaxNormalizer();

            //run logistic regression for 10 iteration with learningRate=0.15
            api.UseLogisticRegression(0.00012, 200);


            var score = api.Run();

            ///**************PREDICTION AFTER MODEL IS CREATED*********////
            /////define data for testing (prediction)
            LearningApi apiPrediction = new LearningApi(loadMetaData1());
            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            apiPrediction.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                var data = new object[5][]
                {
                    new object[]{660,3.88,2,1},
                    new object[]{580,3.36,2,0},
                    new object[]{640,3.17,2,0},
                    new object[]{640,3.51,2,0},
                    new object[]{800,3.05,2,1},

                    };
                return data;
            });

            // Use mapper for data, which will extract (map) required columns 
            apiPrediction.UseDefaultDataMapper();
            apiPrediction.UseMinMaxNormalizer();
            var testData = apiPrediction.Run();

            //use previous trained model
            var result = api.Algorithm.Predict(testData as double[][], api.Context) as LogisticRegressionResult;

            //
            Assert.AreEqual(Math.Round(result.PredictedValues[0], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[1], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[2], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[3], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[3], 0), 0);


        }


        /// <summary>
        /// Performs the LogisticRegression algorithm on dataset with 1000 iterations and 0.02 learning rate 
        /// on Breast Cancer Diagonis Dataset
        /// </summary>
        [TestMethod]
        public void LogisticRegressionTest_BreastCancerDataset_RealExample()
        {
            string BreastCancerData_path = @"SampleData\binary\breast_cancer_dataset.csv";

            var BreastCancerFile_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), BreastCancerData_path);

            LearningApi api = new LearningApi(LoadBreastCancerMetaData());
            api.UseCsvDataProvider(BreastCancerFile_path, ',', false, 50);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseMinMaxNormalizer();

            //run logistic regression for 1000 iterations with learningRate=0.02
            api.UseLogisticRegression(0.02, 1000);


            var score = api.Run();

            ///**************PREDICTION AFTER MODEL IS CREATED*********////
            /////define data for testing (prediction)
            LearningApi apiPrediction = new LearningApi(LoadBreastCancerMetaData());
            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            apiPrediction.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                var data = new object[16][]
                {
                    new object[] { 4, 1, 1, 3, 2, 1, 3, 1, 1, 0 },
                    new object[] { 8, 10, 10, 8, 7, 10, 9, 7, 1, 1 },
                    new object[] { 1, 1, 1, 1, 2, 10, 3, 1, 1, 0 },
                    new object[] { 2, 1, 2, 1, 2, 1, 3, 1, 1, 0 },
                    new object[] { 2, 1, 1, 1, 2, 1, 1, 1, 5, 0 },
                    new object[] { 4, 2, 1, 1, 2, 1, 2, 1, 1, 0 },
                    new object[] { 1, 1, 1, 1, 1, 1, 3, 1, 1, 0 },
                    new object[] { 2, 1, 1, 1, 2, 1, 2, 1, 1, 0 },
                    new object[] { 5, 3, 3, 3, 2, 3, 4, 4, 1, 1 },
                    new object[] { 1, 1, 1, 1, 2, 3, 3, 1, 1, 0 },
                    new object[] { 8, 7, 5, 10, 7, 9, 5, 5, 4, 1 },
                    new object[] { 7, 4, 6, 4, 6, 1, 4, 3, 1, 1 },
                    new object[] { 4, 1, 1, 1, 2, 1, 2, 1, 1, 0 },
                    new object[] { 4, 1, 1, 1, 2, 1, 3, 1, 1, 0 },
                    new object[] { 10, 7, 7, 6, 4, 10, 4, 1, 2, 1 },
                    new object[] { 6, 1, 1, 1, 2, 1, 3, 1, 1, 0 },
                    };

                return data;
            });

            // Use mapper for data, which will extract (map) required columns 
            apiPrediction.UseDefaultDataMapper();
            apiPrediction.UseMinMaxNormalizer();
            var testData = apiPrediction.Run();

            //use previous trained model
            var result = api.Algorithm.Predict(testData as double[][], api.Context) as LogisticRegressionResult;

            //
            Assert.AreEqual(Math.Round(result.PredictedValues[0], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[1], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[2], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[3], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[4], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[5], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[6], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[7], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[8], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[9], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[10], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[11], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[12], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[13], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[14], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[15], 0), 0);
        }

        /// <summary>
        /// Performs the LogisticRegression algorithm on dataset with 90 iterations and 0.02 learning rate 
        /// on Social Network Ads DataSet
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void LogisticRegressionTest_SocialNetworkAdsDataset_RealExample()
        {
            string NetworkAdsData_path = @"SampleData\binary\social_network_ads_dataset.csv";

            var NetworkAdsFile_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), NetworkAdsData_path);

            LearningApi api = new LearningApi(LoadSocialNetworkAdsMetaData());
            api.UseCsvDataProvider(NetworkAdsFile_path, ',', false, 50);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseMinMaxNormalizer();

            //run logistic regression for 90 iterations with learningRate=0.02
            api.UseLogisticRegression(0.02, 90);


            var score = api.Run();

            ///**************PREDICTION AFTER MODEL IS CREATED*********////
            /////define data for testing (prediction)
            LearningApi apiPrediction = new LearningApi(LoadSocialNetworkAdsMetaData());
            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            apiPrediction.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                var data = new object[20][]
                {
                    new object[]{"Male", 19, 19000, 0 },
                    new object[]{"Male", 35, 20000, 0 },
                    new object[]{"Female", 26, 43000, 0 },
                    new object[]{"Female", 27, 57000, 0 },
                    new object[]{"Male", 19, 76000, 0 },
                    new object[]{"Male", 27, 58000, 0 },
                    new object[]{"Female", 27, 84000, 0 },
                    new object[]{"Female", 32, 150000, 1 },
                    new object[]{"Male", 25, 33000, 0 },
                    new object[]{"Female", 35, 65000, 0 },
                    new object[]{"Female", 26, 80000, 0 },
                    new object[]{"Female", 26, 52000, 0 },
                    new object[]{"Male", 20, 86000, 0 },
                    new object[]{"Male", 32, 18000, 0 },
                    new object[]{"Male", 18, 82000, 0 },
                    new object[]{"Male", 29, 80000, 0 },
                    new object[]{"Male", 47, 25000, 1 },
                    new object[]{"Male", 45, 26000, 1 },
                    new object[]{"Male", 46, 28000, 1 },
                    new object[]{"Female", 48, 29000, 1 },
                    };

                return data;
            });

            // Use mapper for data, which will extract (map) required columns 
            apiPrediction.UseDefaultDataMapper();
            apiPrediction.UseMinMaxNormalizer();
            var testData = apiPrediction.Run();

            //use previous trained model
            var result = api.Algorithm.Predict(testData as double[][], api.Context) as LogisticRegressionResult;

            //
            Assert.AreEqual(Math.Round(result.PredictedValues[0], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[1], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[2], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[3], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[4], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[5], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[6], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[7], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[8], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[9], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[10], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[11], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[12], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[13], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[14], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[15], 0), 0);
            Assert.AreEqual(Math.Round(result.PredictedValues[16], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[17], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[18], 0), 1);
            Assert.AreEqual(Math.Round(result.PredictedValues[19], 0), 1);
        }


        #region Data Sample

        private DataDescriptor loadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[5];
            des.Features[0] = new Column { Id = 1, Name = "precent", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0.5, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "color", Index = 1, Type = ColumnType.CLASS, DefaultMissingValue = 0, Values = new string[3] { "red", "green", "blue" } };
            des.Features[2] = new Column { Id = 3, Name = "gender", Index = 2, Type = ColumnType.BINARY, DefaultMissingValue = 1, Values = new string[2] { "male", "female" } };
            des.Features[3] = new Column { Id = 4, Name = "year", Index = 3, Type = ColumnType.NUMERIC, DefaultMissingValue = 15, Values = null };
            des.Features[4] = new Column { Id = 5, Name = "y", Index = 4, Type = ColumnType.BINARY, DefaultMissingValue = 1, Values = new string[2] { "no", "yes" } };

            des.LabelIndex = 4;
            return des;
        }

        private object[][] loadRealDataSample()
        {
            var data = new object[25][] {
                    new object[]{0.202,"blue", "male",13,"yes" },
                    new object[]{0.447,"green","female",37,"no" },
                    new object[]{0.120,"red","male", "21", "yes" },
                    new object[]{0.313,"green","male",22,"yes" },
                    new object[]{0.369,"blue","male",47,"no" },
                    new object[]{0.452,"blue", "male", 20,"no" },
                    new object[]{0.923,"green","female",35,"no" },
                    new object[]{0.354,"red","female",37,"yes" },
                    new object[]{0.814,"blue", "male", 14,  "yes" },
                    new object[]{0.443,"green", "male", 25,  "no" },
                    new object[]{0.761,"red", "female",  19,  "yes" },
                    new object[]{0.759,"red", "male", 44 , "no" },
                    new object[]{0.118,"green", "female", 10,  "yes" },
                    new object[]{0.343,"blue", "male", 30,  "no" },
                    new object[]{0.179,"red", "female",  26,  "no" },
                    new object[]{0.688,"green","female",  10,  "no" },
                    new object[]{0.915,"green","male", 33,  "yes" },
                    new object[]{0.582,"green","male", 49,  "yes" },
                    new object[]{0.111,"red", "male", 17,  "yes" },
                    new object[]{0.065,"blue","female",  32,  "no" },
                    new object[]{0.233,"blue","female",  24,  "no" },
                    new object[]{0.810,"red","male", 37,"no" },
                    new object[]{0.966,"red","male", 39,"yes" },
                    new object[]{0.644,"green","female",36,"yes" },
                    new object[]{0.169,"green","male", 43,"no" }
                    };
            
            return data;
        }

        #endregion

        #region Metadata for Data samples

        /// <summary>
        /// Meta data for binary__admin data sample
        /// </summary>
        /// <returns></returns>
        private DataDescriptor loadMetaData1()
        {
            var des = new DataDescriptor();

            des.Features = new Column[4];
            des.Features[0] = new Column { Id = 1, Name = "gre", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "gpa", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[2] = new Column { Id = 3, Name = "rank", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[3] = new Column { Id = 4, Name = "admit", Index = 3, Type = ColumnType.BINARY, DefaultMissingValue = 0, Values = new string[2] { "0", "1" } };

            des.LabelIndex = 3;
            return des;
        }


        /// <summary>
        /// Meta Data for Breast Cancer Diagonosis Data Sample
        /// </summary>
        /// <returns></returns>
        private DataDescriptor LoadBreastCancerMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[10];
            des.Features[0] = new Column { Id = 1, Name = "Cell.thickness", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "Cell.size", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[2] = new Column { Id = 3, Name = "Cell.shape", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[3] = new Column { Id = 4, Name = "Marg.adhesion", Index = 3, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[4] = new Column { Id = 5, Name = "Epith.c.size", Index = 4, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[5] = new Column { Id = 6, Name = "Bare.nuclei", Index = 5, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[6] = new Column { Id = 7, Name = "Bl.cromatin", Index = 6, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[7] = new Column { Id = 8, Name = "Normal.nucleoli", Index = 7, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[8] = new Column { Id = 9, Name = "Mitoses", Index = 8, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[9] = new Column { Id = 10, Name = "Class", Index = 9, Type = ColumnType.BINARY, DefaultMissingValue = 0, Values = new string[2] { "0", "1" } };

            des.LabelIndex = 9;
            return des;
        }

        /// <summary>
        /// Meta Data for Social Network Ads/Purchase Data Sample
        /// </summary>
        /// <returns></returns>
        private DataDescriptor LoadSocialNetworkAdsMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[4];
            des.Features[0] = new Column { Id = 1, Name = "Gender", Index = 0, Type = ColumnType.BINARY, DefaultMissingValue = 1, Values = new string[2] { "Male", "Female" } };
            des.Features[1] = new Column { Id = 2, Name = "Age", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 20, Values = null };
            des.Features[2] = new Column { Id = 3, Name = "EstimatedSalary", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[3] = new Column { Id = 4, Name = "Purchased", Index = 3, Type = ColumnType.BINARY, DefaultMissingValue = 0, Values = new string[2] { "0", "1" } };

            des.LabelIndex = 3;
            return des;
        }

        #endregion
    }
}
