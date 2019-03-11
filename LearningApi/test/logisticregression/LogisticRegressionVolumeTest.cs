using test;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using LearningFoundation;
using LogisticRegression;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using LearningFoundation.Normalizers;
using LearningFoundation.Statistics;

namespace test.logisticregression
{
    public class LogisticRegressionVolumeTest
    {
        /// <summary>
        /// Performs the LogisticRegression algortim on dataset with 400 iteration and 0.029 learning rate.
        /// </summary>
        [Fact]
        public void LogisticRegression_Batch_Real_BankExample()
        {
            string m_binary_data_path = @"SampleData\bank\bank-data.csv";

            var binary_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_binary_data_path);

            LearningApi api = new LearningApi(loadMetaData1());
            api.UseBatchCsvDataProvider(binary_path, ';', false, 1, 1000);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseMinMaxNormalizer();

            //run logistic regression for 400 iteration with learningRate=0.029
            //api.UseLogisticRegression(0.00012, 200);
            //api.UseLogisticRegression(0.029, 200);
            api.UseLogisticRegression(0.029, 400);

            var score = api.RunBatch();

            //LogisticRegressionScore score = api.GetScore() as LogisticRegressionScore;

            ///**************PREDICTION AFTER MODEL IS CREATED*********////
            /////define data for testing (prediction)
            LearningApi apiPrediction = new LearningApi(loadMetaData1());
            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            apiPrediction.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                var data = new object[25][] {
                    new object[] { 30, "blue-collar", "married", "no", "yes", "no", 487, 0, "failure", 1.8, "no" },
                    new object[] { 39, "services", "single", "no", "no", "no", 346, 0, "failure", 1.1, "no" } ,
                    new object[] { 25, "services", "married", "no", "yes", "no", 227, 0, "nonexistent", 1.4, "no" },
                    new object[] { 38, "services", "married", "no", "yes", "unknown", 17, 0, "failure", 1.4, "no" },
                    new object[] { 47, "admin.", "married", "no", "yes", "no", 58, 0, "nonexistent", -0.1, "no" },
                    new object[] { 32, "services", "single", "no", "no", "no", 128, 2, "failure", 1.1, "no" },
                    new object[] { 32, "admin.", "single", "no", "yes", "no", 290, 0, "nonexistent", -1.1, "no" },
                    new object[] { 41, "entrepreneur", "married", "unknown", "yes", "no", 44, 0, "failure", 0.1, "no" },
                    new object[] { 31, "services", "divorced", "no", "no", "no", 68, 1, "failure", -0.1, "no" },
                    new object[] { 35, "blue-collar", "married", "unknown", "no", "no", 170, 0, "failure", 1.1, "no" },
                    new object[] { 25, "services", "single", "unknown", "yes", "no", 301, 0, "nonexistent", 1.4, "no" },
                    new object[] { 36, "self-employed", "single", "no", "no", "no", 148, 0, "failure", 1.4, "no" },
                    new object[] { 36, "admin.", "married", "no", "no", "no", 97, 0, "nonexistent", 1.1, "no" },
                    new object[] { 47, "blue-collar", "married", "no", "yes", "no", 211, 0, "nonexistent", 1.4, "no" },
                    new object[] { 29, "admin.", "single", "no", "no", "no", 553, 0, "failure", 1.8, "no" },
                    new object[] { 27, "services", "single", "no", "no", "no", 698, 0, "failure", 1.4, "no" },
                    new object[] { 44, "admin.", "divorced", "no", "no", "no", 191, 0, "nonexistent", 1.4, "no" },
                    new object[] { 46, "admin.", "divorced", "no", "yes", "no", 59, 0, "nonexistent", 1.4, "no" },
                    new object[] { 45, "entrepreneur", "married", "unknown", "yes", "yes", 38, 0, "nonexistent", 1.4, "no" },
                    new object[] { 50, "blue-collar", "married", "no", "no", "no", 849, 0, "success", -1.4, "yes" },
                    new object[] { 55, "services", "married", "unknown", "yes", "no", 326, 0, "nonexistent", 1.4, "no" },
                    new object[] { 39, "technician", "divorced", "no", "no", "no", 222, 2, "success", -1.8, "yes" },
                    new object[] { 29, "technician", "single", "no", "yes", "yes", 626, 0, "nonexistent", 1.4, "no" },
                    new object[] { 40, "management", "married", "no", "no", "yes", 119, 0, "nonexistent", 1.4, "no" },
                    new object[] { 40, "management", "married", "no", "no", "yes", 119, 0, "nonexistent", 1.4, "no" },
                    };
                //
                return data;
            });

            // Use mapper for data, which will extract (map) required columns 
            apiPrediction.UseDefaultDataMapper();
            apiPrediction.UseMinMaxNormalizer();
            var testData = apiPrediction.Run();

            //use previous trained model
            var result = api.Algorithm.Predict(testData as double[][], api.Context) as LogisticRegressionResult;

            ////
            Assert.Equal(Math.Round(result.PredictedValues[0], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[1], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[2], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[3], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[4], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[5], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[6], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[7], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[8], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[9], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[10], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[11], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[12], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[13], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[14], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[15], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[16], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[17], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[18], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[19], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[20], 0), 1);
            Assert.Equal(result.PredictedValues[21], 0.99999999999999734);
            Assert.Equal(Math.Round(result.PredictedValues[22], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[23], 0), 1);
            Assert.Equal(Math.Round(result.PredictedValues[24], 0), 1);

        }



        #region Data Sample

        #endregion

        /// <summary>
        /// Meta data for binary__admin data sample
        /// </summary>
        /// <returns></returns>
        private DataDescriptor loadMetaData1()
        {
            var des = new DataDescriptor();
            des.Features = new Column[11];
            des.Features[0] = new Column { Id = 1, Name = "age", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "job", Index = 1, Type = ColumnType.CLASS, DefaultMissingValue = 0, Values = new string[12] { "admin.", "blue-collar", "entrepreneur", "housemaid", "management", "retired", "self-employed", "services", "student", "technician", "unemployed", "unknown" } };
            des.Features[2] = new Column { Id = 3, Name = "marital", Index = 2, Type = ColumnType.CLASS, DefaultMissingValue = 0, Values = new string[4] { "divorced", "married", "single", "unknown" } };
            des.Features[3] = new Column { Id = 4, Name = "default", Index = 3, Type = ColumnType.CLASS, DefaultMissingValue = 1, Values = new string[3] { "no", "yes", "unknown" } };
            des.Features[4] = new Column { Id = 5, Name = "housing", Index = 4, Type = ColumnType.CLASS, DefaultMissingValue = 1, Values = new string[3] { "no", "yes", "unknown" } };
            des.Features[5] = new Column { Id = 6, Name = "loan", Index = 5, Type = ColumnType.CLASS, DefaultMissingValue = 1, Values = new string[3] { "no", "yes", "unknown" } };
            des.Features[6] = new Column { Id = 7, Name = "duration", Index = 6, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[7] = new Column { Id = 8, Name = "previous", Index = 7, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[8] = new Column { Id = 9, Name = "poutcome", Index = 8, Type = ColumnType.CLASS, DefaultMissingValue = 0, Values = new string[4] { "failure", "success", "other", "nonexistent" } };
            des.Features[9] = new Column { Id = 10, Name = "emp.var.rate", Index = 9, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[10] = new Column { Id = 11, Name = "y", Index = 10, Type = ColumnType.BINARY, DefaultMissingValue = 1, Values = new string[2] { "no", "yes" } };
            des.LabelIndex = 10;
            return des;
        }
    }
}
