using System;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralRegression;

namespace TestNeuralRegression
{
    [TestClass]
    public class UnitTestNeural
    {
        /// <summary>
        /// Perform a Test by taking random data as input 
        /// </summary>
        [TestMethod]
        public void TestByData()
        {
            DataDescriptor desc = getDescriptor(); 
            LearningApi api = new LearningApi(desc);
            api.UseActionModule<double[], double[][]>
                ((data, ctx) =>
            {
                return getData();
            });

            // calling neural regression using learning rate 0.005 and maxepochs 10000
            api.UseNeuralRegression(0.005,10000);

            var score = api.Run() as NeuralRegressionScore;

            //Checking Weights value
            Assert.AreEqual(-0.15044, Math.Round(score.Weights[0], 5));
            Assert.AreEqual(-0.25534, Math.Round(score.Weights[1], 5));
            Assert.AreEqual(-0.14371, Math.Round(score.Weights[2], 5));
            Assert.AreEqual(0.14475, Math.Round(score.Weights[3], 5));

            //Errors after every 2000 iteration.
            Assert.AreEqual(0.01126, Math.Round(score.Errors[0], 5));
            Assert.AreEqual(0.00461, Math.Round(score.Errors[1], 5));
            Assert.AreEqual(0.00341, Math.Round(score.Errors[2], 5));
            Assert.AreEqual(0.00439, Math.Round(score.Errors[3], 5));

            //Sample input for Predict function
            double[][] predictors = new double[4][];
            predictors[0] = new double[] { Math.PI };
            predictors[1] = new double[] { Math.PI / 2 };
            predictors[2] = new double[] { 3 * Math.PI / 2.0 };
            predictors[3] = new double[] { 6 * Math.PI };

            var result = api.Algorithm.Predict(predictors, api.Context) as NeuralRegressionResult;

            //Checking the predicted values of Sine function
            Assert.AreEqual(-0.04453, Math.Round(result.PredictedValues[0][0],5));
            Assert.AreEqual(0.97319, Math.Round(result.PredictedValues[1][0],5));
            Assert.AreEqual(-1.01134, Math.Round(result.PredictedValues[2][0],5));
            Assert.AreEqual(5.0182, Math.Round(result.PredictedValues[3][0],5));
        }
        /// <summary>
        /// Performs the Neural Regression on random dataset with maximum iteration
        /// </summary>
        [TestMethod]
        public void TestWithMaxIteration()
        {
            DataDescriptor desc = getDescriptor();
            LearningApi api = new LearningApi(desc);
            api.UseActionModule<double[], double[][]>
                ((data, ctx) =>
                {
                    return getData();
                });

            // calling neural regression using learning rate 0.005 and maxepochs 80000
            api.UseNeuralRegression(0.005, 80000);

            var score = api.Run() as NeuralRegressionScore;

            //Checking Weights value
            Assert.AreEqual(-0.19048, Math.Round(score.Weights[0], 5));
            Assert.AreEqual(-0.62763, Math.Round(score.Weights[1], 5));
            Assert.AreEqual(-0.14277, Math.Round(score.Weights[2], 5));
            Assert.AreEqual(0.15139, Math.Round(score.Weights[3], 5));

            //Errors after every 2000 iteration.
            Assert.AreEqual(0.00136, Math.Round(score.Errors[0], 5));
            Assert.AreEqual(0.00054, Math.Round(score.Errors[1], 5));
            Assert.AreEqual(0.00072, Math.Round(score.Errors[2], 5));
            Assert.AreEqual(0.00033, Math.Round(score.Errors[3], 5));

            //Sample input for Predict function
            double[][] predictors = new double[4][];
            predictors[0] = new double[] { Math.PI };
            predictors[1] = new double[] { Math.PI / 2 };
            predictors[2] = new double[] { 3 * Math.PI / 2.0 };
            predictors[3] = new double[] { 6 * Math.PI };

            var result = api.Algorithm.Predict(predictors, api.Context) as NeuralRegressionResult;

            //Checking the predicted values of Sine function
            Assert.AreEqual(0.0005, Math.Round(result.PredictedValues[0][0], 5));
            Assert.AreEqual(0.99374, Math.Round(result.PredictedValues[1][0], 5));
            Assert.AreEqual(-0.99345, Math.Round(result.PredictedValues[2][0], 5));
            Assert.AreEqual(5.945, Math.Round(result.PredictedValues[3][0], 5));
        }
        /// <summary>
        /// Takes sample data from CSV file and perform Neural Regression on that dataset.
        /// </summary>
        [TestMethod]
        public void TestWithCSVData()
        {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\SampleData.csv");

            DataDescriptor desc = getDescriptor(); ;

            LearningApi api = new LearningApi(desc);

            api.UseCsvDataProvider(path, ',', false,1);

            // Use mapper for data, which will extract (map) required columns
            api.UseDefaultDataMapper();
           
            api.UseNeuralRegression(0.005, 10000);

            var score = api.Run() as NeuralRegressionScore;


            //Checking Weights value
            Assert.AreEqual(0.00658, Math.Round(score.Weights[0], 5));
            Assert.AreEqual(-1.14974, Math.Round(score.Weights[1], 5));
            Assert.AreEqual(0.10792, Math.Round(score.Weights[2], 5));
            Assert.AreEqual(0.02510, Math.Round(score.Weights[3], 5));

            //Errors after every 2000 iteration.
            Assert.AreEqual(0.32654, Math.Round(score.Errors[0], 5));
            Assert.AreEqual(0.32539, Math.Round(score.Errors[1], 5));
            Assert.AreEqual(0.32095, Math.Round(score.Errors[2], 5));
            Assert.AreEqual(0.32113, Math.Round(score.Errors[3], 5));

            //Sample input for Predict function
            double[][] predictors = new double[4][];
            predictors[0] = new double[] { Math.PI };
            predictors[1] = new double[] { Math.PI / 2 };
            predictors[2] = new double[] { 3 * Math.PI / 2.0 };
            predictors[3] = new double[] { 6 * Math.PI };

            var result = api.Algorithm.Predict(predictors, api.Context) as NeuralRegressionResult;

            //Checking the predicted values of Sine function
            Assert.AreEqual(1.05398, Math.Round(result.PredictedValues[0][0], 5));
            Assert.AreEqual(0.64755, Math.Round(result.PredictedValues[1][0], 5));
            Assert.AreEqual(2.32493, Math.Round(result.PredictedValues[2][0], 5));
            Assert.AreEqual(4.32282, Math.Round(result.PredictedValues[3][0], 5));

        }

        /// <summary>
        /// Performs the Neural Regression with minimum learning rate
        /// </summary>
        [TestMethod]
        public void TestWithMaxLearningRate()
        {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\SampleData.csv");

            DataDescriptor desc = getDescriptor(); ;

            LearningApi api = new LearningApi(desc);

            api.UseCsvDataProvider(path, ',', false, 1);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseNeuralRegression(0.01, 10000);

            var score = api.Run() as NeuralRegressionScore;

            //Checking Weights value
            Assert.AreEqual(0.00612, Math.Round(score.Weights[0], 5));
            Assert.AreEqual(-1.62082, Math.Round(score.Weights[1], 5));
            Assert.AreEqual(0.51902, Math.Round(score.Weights[2], 5));
            Assert.AreEqual(0.02292, Math.Round(score.Weights[3], 5));

            //Errors after every 2000 iteration.
            Assert.AreEqual(0.32526, Math.Round(score.Errors[0], 5));
            Assert.AreEqual(0.32625, Math.Round(score.Errors[1], 5));
            Assert.AreEqual(0.31942, Math.Round(score.Errors[2], 5));
            Assert.AreEqual(0.32239, Math.Round(score.Errors[3], 5));


            //Sample input for Predict function
            double[][] predictors = new double[4][];
            predictors[0] = new double[] { Math.PI };
            predictors[1] = new double[] { Math.PI / 2 };
            predictors[2] = new double[] { 3 * Math.PI / 2.0 };
            predictors[3] = new double[] { 6 * Math.PI };

            var result = api.Algorithm.Predict(predictors, api.Context) as NeuralRegressionResult;

            //Checking the predicted values of Sine function
            Assert.AreEqual(1.10758, Math.Round(result.PredictedValues[0][0], 5));
            Assert.AreEqual(0.64127, Math.Round(result.PredictedValues[1][0], 5));
            Assert.AreEqual(2.35681, Math.Round(result.PredictedValues[2][0], 5));
            Assert.AreEqual(4.25913, Math.Round(result.PredictedValues[3][0], 5));

        }

        /// <summary>
        /// Performs the Neural Regression with minimum iteration rate
        /// </summary>
        [TestMethod]
        public void TestWithMinimumIteration()
        {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\SampleData.csv");

            DataDescriptor desc = getDescriptor(); ;

            LearningApi api = new LearningApi(desc);

            api.UseCsvDataProvider(path, ',', false, 1);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            api.UseNeuralRegression(0.005, 8000);

            var score = api.Run() as NeuralRegressionScore;

            //Checking Weights value
            Assert.AreEqual(0.00699, Math.Round(score.Weights[0], 5));
            Assert.AreEqual(-1.07739, Math.Round(score.Weights[1], 5));
            Assert.AreEqual(0.11352, Math.Round(score.Weights[2], 5));
            Assert.AreEqual(0.02667, Math.Round(score.Weights[3], 5));

            //Errors after every 2000 iteration.
            Assert.AreEqual(0.35317, Math.Round(score.Errors[0], 5));
            Assert.AreEqual(0.32448, Math.Round(score.Errors[1], 5));
            Assert.AreEqual(0.33701, Math.Round(score.Errors[2], 5));
            Assert.AreEqual(0.32060, Math.Round(score.Errors[3], 5));

            //Sample input for Predict function
            double[][] predictors = new double[4][];
            predictors[0] = new double[] { Math.PI };
            predictors[1] = new double[] { Math.PI / 2 };
            predictors[2] = new double[] { 3 * Math.PI / 2.0 };
            predictors[3] = new double[] { 6 * Math.PI };

            var result = api.Algorithm.Predict(predictors, api.Context) as NeuralRegressionResult;

            //Checking the predicted values of Sine function
            Assert.AreEqual(1.06164, Math.Round(result.PredictedValues[0][0], 5));
            Assert.AreEqual(0.64727, Math.Round(result.PredictedValues[1][0], 5));
            Assert.AreEqual(2.34583, Math.Round(result.PredictedValues[2][0], 5));
            Assert.AreEqual(4.33913, Math.Round(result.PredictedValues[3][0], 5));

        }


        /// <summary>
        /// getDescriptor performs DataDescription
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptor()
        {
            DataDescriptor desc = new DataDescriptor();
            desc.Features = new LearningFoundation.DataMappers.Column[2];
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "X1",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };
            desc.Features[1] = new LearningFoundation.DataMappers.Column()
            {
                Id = 1,
                Name = "X2",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 1,
            };

            return desc;
        }

        /// <summary>
        /// Sample data generate randomely
        /// </summary>
        /// <returns></returns>
        private double[][] getData()
        {
            int numItems = 80;

            double[][] trainData = new double[numItems][];
            Random rnd = new Random(1);

            for (int i = 0; i < numItems; ++i)
            {
                double x = 6.4 * rnd.NextDouble(); // [0 to 2PI]
                double sx = Math.Sin(x);
                trainData[i] = new double[] { x, sx };
            }

            return trainData;
        }
    }
}
