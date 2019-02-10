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

    /// <summary>
    /// 
    /// </summary>
    public class LogisticRegressionSaveLoad
    {

        public LogisticRegressionSaveLoad()
        {

        }

        /// <summary>
        /// Performs the LogisticRegression on specified dataset with 10 iteration and 0.15 learning rate.
        /// </summary>
        [Fact]
        public void LogisticsRegression_Test_iterations_10_learningrate_batch1()
        {
            var desc = loadMetaData();

            LearningApi api = new LearningApi(desc);

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample1();
            });

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseMinMaxNormalizer();

            //run logistic regression for 10 iterations with learningRate=0.13
            api.UseLogisticRegression(0.13, 10);

            api.Run();

            api.Save("LogisticRegressionBatch1.json");

            LogisticRegressionScore score1 = api.GetScore() as LogisticRegressionScore;

            LearningApi api2 = new LearningApi(desc);

            var batch2 = LearningApi.Load("LogisticRegressionBatch1.json");

            api2.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample2();
            });

            // Use mapper for data, which will extract (map) required columns 
            api2.UseDefaultDataMapper();

            api2.UseMinMaxNormalizer();

            //run logistic regression for 10 iterations with learningRate=0.13
            api2.UseLogisticRegression(0.13, 10);

            api2.Run();

            api2.Save("LogisticRegressionBatch2");

            LogisticRegressionScore score2 = api2.GetScore() as LogisticRegressionScore;

            ////Load batch 1 to batch2

            //LearningApi api2 = new LearningApi(desc);

            //var batch2 = LearningApi.Load("LogisticRegressionBatch1.json");

            //api.UseActionModule<object[][], object[][]>((input, ctx) =>
            //{
            //    return loadRealDataSample2();
            //});

            //// Use mapper for data, which will extract (map) required columns 
            //api.UseDefaultDataMapper();

            //api.UseMinMaxNormalizer();

            ////run logistic regression for 10 iterations with learningRate=0.13
            //api.UseLogisticRegression(0.13, 10);

            //api.Run();

            //api.Save("LogisticRegressionBatch2.json");

            //LogisticRegressionScore score2 = api.GetScore() as LogisticRegressionScore;

            ////Load batch 2 to batch3

            //LearningApi api3 = new LearningApi(desc);

            //var batch3 = LearningApi.Load("LogisticRegressionBatch2.json");

            //api.UseActionModule<object[][], object[][]>((input, ctx) =>
            //{
            //    return loadRealDataSample3();
            //});

            //// Use mapper for data, which will extract (map) required columns 
            //api.UseDefaultDataMapper();

            //api.UseMinMaxNormalizer();

            ////run logistic regression for 10 iterations with learningRate=0.13
            //api.UseLogisticRegression(0.13, 10);

            //api.Run();

            //api.Save("LogisticRegressionBatch3.json");

            //LogisticRegressionScore score3 = api.GetScore() as LogisticRegressionScore;

            ////Load batch 3 to batch 4

            //LearningApi api4 = new LearningApi(desc);

            //var batch4 = LearningApi.Load("LogisticRegressionBatch3.json");

            //api.UseActionModule<object[][], object[][]>((input, ctx) =>
            //{
            //    return loadRealDataSample4();
            //});

            //// Use mapper for data, which will extract (map) required columns 
            //api.UseDefaultDataMapper();

            //api.UseMinMaxNormalizer();

            ////run logistic regression for 10 iterations with learningRate=0.13
            //api.UseLogisticRegression(0.13, 10);

            //api.Run();

            //api.Save("LogisticRegressionBatch4.json");

            //LogisticRegressionScore score4 = api.GetScore() as LogisticRegressionScore;

            ////Load batch 4 to batch 5

            //LearningApi api5 = new LearningApi(desc);

            //var batch5 = LearningApi.Load("LogisticRegressionBatch4.json");

            //api.UseActionModule<object[][], object[][]>((input, ctx) =>
            //{
            //    return loadRealDataSample5();
            //});

            //// Use mapper for data, which will extract (map) required columns 
            //api.UseDefaultDataMapper();

            //api.UseMinMaxNormalizer();

            ////run logistic regression for 10 iterations with learningRate=0.13
            //api.UseLogisticRegression(0.13, 10);

            //api.Run();

            //api.Save("LogisticRegressionBatch5.json");

            //LogisticRegressionScore score5 = api.GetScore() as LogisticRegressionScore;





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

            //Assert.Equal(Math.Round(result.PredictedValues[0], 5), 1E-05);
            //Assert.Equal(Math.Round(result.PredictedValues[1], 5), 0);
            //Assert.Equal(Math.Round(result.PredictedValues[2], 5), 0);
            //Assert.Equal(Math.Round(result.PredictedValues[3], 5), 0);
        }


        // Batch 2
        [Fact]
        public void LogisticsRegression_Test_iterations_10_learningrate_batch2()
        {
            var desc = loadMetaData();

            LearningApi api2 = new LearningApi(desc);

            var batch2 = LearningApi.Load("LogisticRegressionBatch1.json");

            api2.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample2();
            });

            // Use mapper for data, which will extract (map) required columns 
            api2.UseDefaultDataMapper();

            api2.UseMinMaxNormalizer();

            //run logistic regression for 10 iterations with learningRate=0.13
            api2.UseLogisticRegression(0.13, 10);

            api2.Run();

            api2.Save("LogisticRegressionBatch2.json");

            LogisticRegressionScore score2 = api2.GetScore() as LogisticRegressionScore;

            
        }

        //Batch 2
        //batch 3
        [Fact]
        public void LogisticsRegression_Test_iterations_10_learningrate_batch3()
        {
            var desc = loadMetaData();

            LearningApi api3 = new LearningApi(desc);

            var batch3 = LearningApi.Load("LogisticRegressionBatch2.json");

            api3.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample3();
            });

            // Use mapper for data, which will extract (map) required columns 
            api3.UseDefaultDataMapper();

            api3.UseMinMaxNormalizer();

            //run logistic regression for 10 iterations with learningRate=0.13
            api3.UseLogisticRegression(0.13, 10);

            api3.Run();

            api3.Save("LogisticRegressionBatch3.json");

            LogisticRegressionScore score3 = api3.GetScore() as LogisticRegressionScore;


        }

        //batch 3
        //batch 4
        [Fact]
        public void LogisticsRegression_Test_iterations_10_learningrate_batch4()
        {
            var desc = loadMetaData();

            LearningApi api4 = new LearningApi(desc);

            var batch4 = LearningApi.Load("LogisticRegressionBatch3.json");

            api4.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample4();
            });

            // Use mapper for data, which will extract (map) required columns 
            api4.UseDefaultDataMapper();

            api4.UseMinMaxNormalizer();

            //run logistic regression for 10 iterations with learningRate=0.13
            api4.UseLogisticRegression(0.13, 10);

            api4.Run();

            api4.Save("LogisticRegressionBatch4.json");

            LogisticRegressionScore score4 = api4.GetScore() as LogisticRegressionScore;


        }

        //batch 4
        //batch 5
        [Fact]
        public void LogisticsRegression_Test_iterations_10_learningrate_batch5()
        {
            var desc = loadMetaData();

            LearningApi api5 = new LearningApi(desc);

            var batch5 = LearningApi.Load("LogisticRegressionBatch4.json");

            api5.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample5();
            });

            // Use mapper for data, which will extract (map) required columns 
            api5.UseDefaultDataMapper();

            api5.UseMinMaxNormalizer();

            //run logistic regression for 10 iterations with learningRate=0.13
            api5.UseLogisticRegression(0.13, 10);

            api5.Run();

            api5.Save("LogisticRegressionBatch5.json");

            LogisticRegressionScore score5 = api5.GetScore() as LogisticRegressionScore;


        }
        //batch 5
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

        //private object[][] loadRealDataSample()
        //{
        //    var data = new object[25][] {
        //            new object[]{0.202,"blue", "male",13,"yes" },
        //            new object[]{0.447,"green","female",37,"no" },
        //            new object[]{0.120,"red","male", "21", "yes" },
        //            new object[]{0.313,"green","male",22,"yes" },
        //            new object[]{0.369,"blue","male",47,"no" },
        //            new object[]{0.452,"blue", "male", 20,"no" },
        //            new object[]{0.923,"green","female",35,"no" },
        //            new object[]{0.354,"red","female",37,"yes" },
        //            new object[]{0.814,"blue", "male", 14,  "yes" },
        //            new object[]{0.443,"green", "male", 25,  "no" },
        //            new object[]{0.761,"red", "female",  19,  "yes" },
        //            new object[]{0.759,"red", "male", 44 , "no" },
        //            new object[]{0.118,"green", "female", 10,  "yes" },
        //            new object[]{0.343,"blue", "male", 30,  "no" },
        //            new object[]{0.179,"red", "female",  26,  "no" },
        //            new object[]{0.688,"green","female",  10,  "no" },
        //            new object[]{0.915,"green","male", 33,  "yes" },
        //            new object[]{0.582,"green","male", 49,  "yes" },
        //            new object[]{0.111,"red", "male", 17,  "yes" },
        //            new object[]{0.065,"blue","female",  32,  "no" },
        //            new object[]{0.233,"blue","female",  24,  "no" },
        //            new object[]{0.810,"red","male", 37,"no" },
        //            new object[]{0.966,"red","male", 39,"yes" },
        //            new object[]{0.644,"green","female",36,"yes" },
        //            new object[]{0.169,"green","male", 43,"no" }
        //            };
        //    //
        //    return data;
        //}

        #endregion
        ///Batch 1
        private object[][] loadRealDataSample1()
        {
            var data1 = new object[5][] {
                    new object[]{0.202,"blue", "male",13,"yes" },
                    new object[]{0.447,"green","female",37,"no" },
                    new object[]{0.120,"red","male", "21", "yes" },
                    new object[]{0.313,"green","male",22,"yes" },
                    new object[]{0.369,"blue","male",47,"no" }
                    //new object[]{0.452,"blue", "male", 20,"no" },
                    //new object[]{0.923,"green","female",35,"no" },
                    //new object[]{0.354,"red","female",37,"yes" },
                    //new object[]{0.814,"blue", "male", 14,  "yes" },
                    //new object[]{0.443,"green", "male", 25,  "no" },
                    //new object[]{0.761,"red", "female",  19,  "yes" },
                    //new object[]{0.759,"red", "male", 44 , "no" },
                    //new object[]{0.118,"green", "female", 10,  "yes" },
                    //new object[]{0.343,"blue", "male", 30,  "no" },
                    //new object[]{0.179,"red", "female",  26,  "no" },
                    //new object[]{0.688,"green","female",  10,  "no" },
                    //new object[]{0.915,"green","male", 33,  "yes" },
                    //new object[]{0.582,"green","male", 49,  "yes" },
                    //new object[]{0.111,"red", "male", 17,  "yes" },
                    //new object[]{0.065,"blue","female",  32,  "no" },
                    //new object[]{0.233,"blue","female",  24,  "no" },
                    //new object[]{0.810,"red","male", 37,"no" },
                    //new object[]{0.966,"red","male", 39,"yes" },
                    //new object[]{0.644,"green","female",36,"yes" },
                    //new object[]{0.169,"green","male", 43,"no" }
                    };
            //
            return data1;
        }
        ///Batch 2
        private object[][] loadRealDataSample2()
        {
            var data2 = new object[5][] {
                    //new object[]{0.202,"blue", "male",13,"yes" },
                    //new object[]{0.447,"green","female",37,"no" },
                    //new object[]{0.120,"red","male", "21", "yes" },
                    //new object[]{0.313,"green","male",22,"yes" },
                    //new object[]{0.369,"blue","male",47,"no" },
                    new object[]{0.452,"blue", "male", 20,"no" },
                    new object[]{0.923,"green","female",35,"no" },
                    new object[]{0.354,"red","female",37,"yes" },
                    new object[]{0.814,"blue", "male", 14,  "yes" },
                    new object[]{0.443,"green", "male", 25,  "no" }
                    //new object[]{0.761,"red", "female",  19,  "yes" },
                    //new object[]{0.759,"red", "male", 44 , "no" },
                    //new object[]{0.118,"green", "female", 10,  "yes" },
                    //new object[]{0.343,"blue", "male", 30,  "no" },
                    //new object[]{0.179,"red", "female",  26,  "no" },
                    //new object[]{0.688,"green","female",  10,  "no" },
                    //new object[]{0.915,"green","male", 33,  "yes" },
                    //new object[]{0.582,"green","male", 49,  "yes" },
                    //new object[]{0.111,"red", "male", 17,  "yes" },
                    //new object[]{0.065,"blue","female",  32,  "no" },
                    //new object[]{0.233,"blue","female",  24,  "no" },
                    //new object[]{0.810,"red","male", 37,"no" },
                    //new object[]{0.966,"red","male", 39,"yes" },
                    //new object[]{0.644,"green","female",36,"yes" },
                    //new object[]{0.169,"green","male", 43,"no" }
                    };
            //
            return data2;
        }
        ///Batch 3
        private object[][] loadRealDataSample3()
        {
            var data3 = new object[5][] {
                    //new object[]{0.202,"blue", "male",13,"yes" },
                    //new object[]{0.447,"green","female",37,"no" },
                    //new object[]{0.120,"red","male", "21", "yes" },
                    //new object[]{0.313,"green","male",22,"yes" },
                    //new object[]{0.369,"blue","male",47,"no" },
                    //new object[]{0.452,"blue", "male", 20,"no" },
                    //new object[]{0.923,"green","female",35,"no" },
                    //new object[]{0.354,"red","female",37,"yes" },
                    //new object[]{0.814,"blue", "male", 14,  "yes" },
                    //new object[]{0.443,"green", "male", 25,  "no" },
                    new object[]{0.761,"red", "female",  19,  "yes" },
                    new object[]{0.759,"red", "male", 44 , "no" },
                    new object[]{0.118,"green", "female", 10,  "yes" },
                    new object[]{0.343,"blue", "male", 30,  "no" },
                    new object[]{0.179,"red", "female",  26,  "no" }
                    //new object[]{0.688,"green","female",  10,  "no" },
                    //new object[]{0.915,"green","male", 33,  "yes" },
                    //new object[]{0.582,"green","male", 49,  "yes" },
                    //new object[]{0.111,"red", "male", 17,  "yes" },
                    //new object[]{0.065,"blue","female",  32,  "no" },
                    //new object[]{0.233,"blue","female",  24,  "no" },
                    //new object[]{0.810,"red","male", 37,"no" },
                    //new object[]{0.966,"red","male", 39,"yes" },
                    //new object[]{0.644,"green","female",36,"yes" },
                    //new object[]{0.169,"green","male", 43,"no" }
                    };
            //
            return data3;
        }
        ///Batch 4
        private object[][] loadRealDataSample4()
    {
        var data4 = new object[5][] {
                    //new object[]{0.202,"blue", "male",13,"yes" },
                    //new object[]{0.447,"green","female",37,"no" },
                    //new object[]{0.120,"red","male", "21", "yes" },
                    //new object[]{0.313,"green","male",22,"yes" },
                    //new object[]{0.369,"blue","male",47,"no" },
                    //new object[]{0.452,"blue", "male", 20,"no" },
                    //new object[]{0.923,"green","female",35,"no" },
                    //new object[]{0.354,"red","female",37,"yes" },
                    //new object[]{0.814,"blue", "male", 14,  "yes" },
                    //new object[]{0.443,"green", "male", 25,  "no" },
                    //new object[]{0.761,"red", "female",  19,  "yes" },
                    //new object[]{0.759,"red", "male", 44 , "no" },
                    //new object[]{0.118,"green", "female", 10,  "yes" },
                    //new object[]{0.343,"blue", "male", 30,  "no" },
                    //new object[]{0.179,"red", "female",  26,  "no" },
                    new object[]{0.688,"green","female",  10,  "no" },
                    new object[]{0.915,"green","male", 33,  "yes" },
                    new object[]{0.582,"green","male", 49,  "yes" },
                    new object[]{0.111,"red", "male", 17,  "yes" },
                    new object[]{0.065,"blue","female",  32,  "no" }
                    //new object[]{0.233,"blue","female",  24,  "no" },
                    //new object[]{0.810,"red","male", 37,"no" },
                    //new object[]{0.966,"red","male", 39,"yes" },
                    //new object[]{0.644,"green","female",36,"yes" },
                    //new object[]{0.169,"green","male", 43,"no" }
                    };
        //
        return data4;
    }
    ///Batch 5
    private object[][] loadRealDataSample5()
    {
        var data5 = new object[5][] {
                    //new object[]{0.202,"blue", "male",13,"yes" },
                    //new object[]{0.447,"green","female",37,"no" },
                    //new object[]{0.120,"red","male", "21", "yes" },
                    //new object[]{0.313,"green","male",22,"yes" },
                    //new object[]{0.369,"blue","male",47,"no" },
                    //new object[]{0.452,"blue", "male", 20,"no" },
                    //new object[]{0.923,"green","female",35,"no" },
                    //new object[]{0.354,"red","female",37,"yes" },
                    //new object[]{0.814,"blue", "male", 14,  "yes" },
                    //new object[]{0.443,"green", "male", 25,  "no" },
                    //new object[]{0.761,"red", "female",  19,  "yes" },
                    //new object[]{0.759,"red", "male", 44 , "no" },
                    //new object[]{0.118,"green", "female", 10,  "yes" },
                    //new object[]{0.343,"blue", "male", 30,  "no" },
                    //new object[]{0.179,"red", "female",  26,  "no" },
                    //new object[]{0.688,"green","female",  10,  "no" },
                    //new object[]{0.915,"green","male", 33,  "yes" },
                    //new object[]{0.582,"green","male", 49,  "yes" },
                    //new object[]{0.111,"red", "male", 17,  "yes" },
                    //new object[]{0.065,"blue","female",  32,  "no" },
                    new object[]{0.233,"blue","female",  24,  "no" },
                    new object[]{0.810,"red","male", 37,"no" },
                    new object[]{0.966,"red","male", 39,"yes" },
                    new object[]{0.644,"green","female",36,"yes" },
                    new object[]{0.169,"green","male", 43,"no" }
                    };
        //
        return data5;
    }
    /// <summary>
    /// Meta data for binary__admin data sample
    /// </summary>
    /// <returns></returns>
    }
}
