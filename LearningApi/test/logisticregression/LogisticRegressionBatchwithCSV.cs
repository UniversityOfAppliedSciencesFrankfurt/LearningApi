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
    public class LogisticRegressionBatchwithCSV
    {
        /// <summary>
        /// Unit Test Batch with a CSV file
        /// Performs the LogisticRegression algortim on dataset with 10 iteration and 0.15 learning rate.
        /// </summary>
        [Fact]
        public void LogisticRegression_Batch_Real_Example()
        {
            string m_binary_data_path = @"SampleData\binary\admit_binary.csv";

            var binary_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_binary_data_path);

            LearningApi api = new LearningApi(loadMetaData1());
            api.UseBatchCsvDataProvider(binary_path, ',', false, 1, 50);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseMinMaxNormalizer();

            //run logistic regression for 10 iteration with learningRate=0.15
            api.UseLogisticRegression(0.00012, 200);


            var score = api.RunBatch();

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
            Assert.Equal(Math.Round(result.PredictedValues[0], 0), 0);
            Assert.Equal(Math.Round(result.PredictedValues[1], 0), 0);
            Assert.Equal(Math.Round(result.PredictedValues[2], 0), 0);
            Assert.Equal(Math.Round(result.PredictedValues[3], 0), 0);
            Assert.Equal(Math.Round(result.PredictedValues[3], 0), 0);


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

            des.Features = new Column[4];
            des.Features[0] = new Column { Id = 1, Name = "gre", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "gpa", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[2] = new Column { Id = 3, Name = "rank", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[3] = new Column { Id = 4, Name = "admit", Index = 3, Type = ColumnType.BINARY, DefaultMissingValue = 0, Values = new string[2] { "0", "1" } };

            des.LabelIndex = 3;
            return des;
        }
    }
}
