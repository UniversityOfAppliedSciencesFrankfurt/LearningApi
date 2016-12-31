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

    public class LogisticRegressionTests
    {

        public LogisticRegressionTests()
        {

        }

        /// <summary>
        /// Performs the LogisticRegression on specified dataset with 10 iteration and 0.15 learning rate.
        /// </summary>
        [Fact]
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

            IScore score = api.GetScore();

            //Errors during each iteration. IF the learningRate is suitable erros is descrising for every next iteration 
            Assert.Equal(Math.Round(score.Errors[0], 5), 0.24278);
            Assert.Equal(Math.Round(score.Errors[1], 5), 0.23749);
            Assert.Equal(Math.Round(score.Errors[2], 5), 0.23359);
            Assert.Equal(Math.Round(score.Errors[3], 5), 0.23010);
            Assert.Equal(Math.Round(score.Errors[4], 5), 0.22740);
            Assert.Equal(Math.Round(score.Errors[5], 5), 0.22476);
            Assert.Equal(Math.Round(score.Errors[6], 5), 0.22271);
            Assert.Equal(Math.Round(score.Errors[7], 5), 0.22065);
            Assert.Equal(Math.Round(score.Errors[8], 5), 0.21902);
            Assert.Equal(Math.Round(score.Errors[9], 5), 0.21739);

            //LG Model Best Found model in 10 iteration
            Assert.Equal(Math.Round(score.Weights[0], 5), 0.06494);
            Assert.Equal(Math.Round(score.Weights[1], 5), 0.21584);
            Assert.Equal(Math.Round(score.Weights[2], 5), 0.89901);
            Assert.Equal(Math.Round(score.Weights[3], 5), 0.51497);
            Assert.Equal(Math.Round(score.Weights[4], 5), -0.30213);
            Assert.Equal(Math.Round(score.Weights[5], 5), -0.30213);
            Assert.Equal(Math.Round(score.Weights[6], 5), -0.85624);
        }

        /// <summary>
        /// Performs the LogisticRegression algortim on dataset with 10 iteration and 0.15 learning rate.
        /// </summary>
        [Fact]
        public void LogisticRegression_Test_iterations_10_learningrate_015()
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

            //run logistic regression for 10 iteration with learningRate=0.15
            api.UseLogisticRegression(0.15, 10);


            var task = api.Run();

            IScore score = api.GetScore();

            //Errors during each iteration
            Assert.Equal(Math.Round(score.Errors[0], 5), 0.24236);
            Assert.Equal(Math.Round(score.Errors[1], 5), 0.23707);
            Assert.Equal(Math.Round(score.Errors[2], 5), 0.23358);
            Assert.Equal(Math.Round(score.Errors[3], 5), 0.23001);
            Assert.Equal(Math.Round(score.Errors[4], 5), 0.22806);
            Assert.Equal(Math.Round(score.Errors[5], 5), 0.22506);
            Assert.Equal(Math.Round(score.Errors[6], 5), 0.22409);
            Assert.Equal(Math.Round(score.Errors[7], 5), 0.22134);
            Assert.Equal(Math.Round(score.Errors[8], 5), 0.22105);
            Assert.Equal(Math.Round(score.Errors[9], 5), 0.21840);

            //LG Model Best Found model in 10 iteration
            Assert.Equal(Math.Round(score.Weights[0], 5), 0.17820);
            Assert.Equal(Math.Round(score.Weights[1], 5), 0.28781);
            Assert.Equal(Math.Round(score.Weights[2], 5), 1.01732);
            Assert.Equal(Math.Round(score.Weights[3], 5), 0.63396);
            Assert.Equal(Math.Round(score.Weights[4], 5), -0.26733);
            Assert.Equal(Math.Round(score.Weights[5], 5), -0.26733);
            Assert.Equal(Math.Round(score.Weights[6], 5), -0.91266);
        }

        /// <summary>
        /// Performs the LogisticRegression algortim on dataset with 20 iteration and 0.15 learning rate.
        /// </summary>
        [Fact]
        public void LogisticRegression_Test_iterations_20_learningrate_015()
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

            //run logistic regression for 10 iteration with learningRate=0.15
            api.UseLogisticRegression(0.15, 20);

            api.Run();

            IScore score = api.GetScore();

            //Errors during each iteration
            Assert.Equal(Math.Round(score.Errors[0], 5), 0.24236);
            Assert.Equal(Math.Round(score.Errors[1], 5), 0.23707);
            Assert.Equal(Math.Round(score.Errors[2], 5), 0.23358);
            Assert.Equal(Math.Round(score.Errors[3], 5), 0.23001);
            Assert.Equal(Math.Round(score.Errors[4], 5), 0.22806);
            Assert.Equal(Math.Round(score.Errors[5], 5), 0.22506);
            Assert.Equal(Math.Round(score.Errors[6], 5), 0.22409);
            Assert.Equal(Math.Round(score.Errors[7], 5), 0.22134);
            Assert.Equal(Math.Round(score.Errors[8], 5), 0.22105);
            Assert.Equal(Math.Round(score.Errors[9], 5), 0.21840);
            Assert.Equal(Math.Round(score.Errors[10], 5), 0.21857);
            Assert.Equal(Math.Round(score.Errors[11], 5), 0.21595);
            Assert.Equal(Math.Round(score.Errors[12], 5), 0.21640);
            Assert.Equal(Math.Round(score.Errors[13], 5), 0.21381);
            Assert.Equal(Math.Round(score.Errors[14], 5), 0.21439);
            Assert.Equal(Math.Round(score.Errors[15], 5), 0.21189);
            Assert.Equal(Math.Round(score.Errors[16], 5), 0.21251);
            Assert.Equal(Math.Round(score.Errors[17], 5), 0.21015);
            Assert.Equal(Math.Round(score.Errors[18], 5), 0.21076);
            Assert.Equal(Math.Round(score.Errors[19], 5), 0.20860);


            //LG Model Best Found model in 20 iteration
            Assert.Equal(Math.Round(score.Weights[0], 5), 0.28363);
            Assert.Equal(Math.Round(score.Weights[1], 5), 0.37424);
            Assert.Equal(Math.Round(score.Weights[2], 5), 1.41890);
            Assert.Equal(Math.Round(score.Weights[3], 5), 1.01207);
            Assert.Equal(Math.Round(score.Weights[4], 5), -0.33841);
            Assert.Equal(Math.Round(score.Weights[5], 5), -0.33841);
            Assert.Equal(Math.Round(score.Weights[6], 5), -1.62489);
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
            //
            return data;
        }

        #endregion
    }
}
