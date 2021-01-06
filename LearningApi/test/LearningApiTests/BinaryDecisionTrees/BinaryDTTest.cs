using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using LearningFoundation.DataProviders;
using System;
using DecisionTreeForLearningAPI.BinaryDecisionTree;

namespace DecisionTreeForLearningAPI.BinaryDecisionTreeTests
{
    [TestClass]
    public class BinaryDTTest
    {
        [TestMethod]
        public void TestBinaryDTImplBasic()
        {
            string scoreData = @"Samples\aptitudeScoreReport.csv";
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), scoreData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor("samples/dataMapperForAptitude.json")); 
            api.UseCsvDataProvider(path, ',', true, 0);

            //convert data in csv file from string to double.
            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
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
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });

            api.UseBinaryDTAlgorithm();

            //Train
            BranchedScore score = api.Run() as BranchedScore;

            //Predict
            double[][] testData = new double[2][];

            testData[0] = new double[] { 5, 6, 0 };
            testData[1] = new double[] { 1, 15, 1 };

            BinaryDTResult binaryDTResult = api.Algorithm.Predict(testData, api.Context) as BinaryDTResult;
            Assert.IsNotNull(binaryDTResult);

            List<BinaryProbability> binaryProbabilities = binaryDTResult.getBinaryDTResult();
            Assert.IsNotNull(binaryProbabilities);

            if(binaryProbabilities.Count == 0)
            {
                Assert.Fail("Nothing to assert, prediction results are empty");
            }

            BinaryProbability row1 = binaryProbabilities[0];
            Assert.AreEqual(1, row1.GetprobabilityOfOccuranceOfZero());
            Assert.AreEqual(0, row1.GetprobabilityOfOccuranceOfOne());

            BinaryProbability row2 = binaryProbabilities[1];
            Assert.AreEqual(0.5, row2.GetprobabilityOfOccuranceOfZero());
            Assert.AreEqual(0.5, row2.GetprobabilityOfOccuranceOfOne());
        }

        [TestMethod]
        public void TestBinaryDTImplUsingCsvDataForPrediction()
        {
            string scoreData = @"Samples\aptitudeScoreReport.csv";
            string predictionData = @"Samples\testAptitudeScoreReport.csv";
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), scoreData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor("samples/dataMapperForAptitude.json"));
            api.UseCsvDataProvider(path, ',', true, 0);

            //convert data in csv file from string to double.
            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
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
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });

            api.UseBinaryDTAlgorithm();

            //Train
            BranchedScore score = api.Run() as BranchedScore;

            //Predict
            string[] lines = File.ReadAllLines(predictionData);
            double[][] predictedArray = new double[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] strArray = lines[i].Split(',');
                double[] doubleArray = Array.ConvertAll(strArray, double.Parse);
                predictedArray[i] = doubleArray;
            }

            LearningApi predictApi = new LearningApi(Helpers.GetDescriptor("samples/dataMapperForAptitudePrediction.json"));
            BinaryDTResult binaryDTResult = api.Algorithm.Predict(predictedArray, predictApi.Context) as BinaryDTResult;
            Assert.IsNotNull(binaryDTResult);

            List<BinaryProbability> binaryProbabilities = binaryDTResult.getBinaryDTResult();
            Assert.IsNotNull(binaryProbabilities);

            if (binaryProbabilities.Count == 0)
            {
                Assert.Fail("Nothing to assert, prediction results are empty");
            }

            BinaryProbability row1 = binaryProbabilities[0];
            Assert.AreEqual(0.5, row1.GetprobabilityOfOccuranceOfZero());
            Assert.AreEqual(0.5, row1.GetprobabilityOfOccuranceOfOne());

            BinaryProbability row2 = binaryProbabilities[2];
            Assert.AreEqual(1, row2.GetprobabilityOfOccuranceOfZero());
            Assert.AreEqual(0, row2.GetprobabilityOfOccuranceOfOne());
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void TestBinaryDTImplForNonBinaryTargetFeatureValue()
        {
            string scoreData = @"Samples\invalidScoreReport.csv";
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), scoreData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor("samples/dataMapperForAptitude.json"));
            api.UseCsvDataProvider(path, ',', true, 0);

            //convert data in csv file from string to double.
            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
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
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });

            api.UseBinaryDTAlgorithm();

            //Train : should throw an exception.
            BranchedScore score = api.Run() as BranchedScore;
        }

        [TestMethod]
        public void TestBinaryDTImplForBanking()
        {
            string scoreData = @"Samples\bankingReport.csv";
            string predictionData = @"Samples\testBankingReport.csv";
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), scoreData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor("samples/dataMapperForBanking.json"));
            api.UseCsvDataProvider(path, ',', true, 0);

            //convert data in csv file from string to double.
            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
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
                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });

            api.UseBinaryDTAlgorithm();

            //Train
            BranchedScore score = api.Run() as BranchedScore;

            //Predict
            string[] lines = File.ReadAllLines(predictionData);
            double[][] predictedArray = new double[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] strArray = lines[i].Split(',');
                double[] doubleArray = Array.ConvertAll(strArray, double.Parse);
                predictedArray[i] = doubleArray;
            }

            LearningApi predictApi = new LearningApi(Helpers.GetDescriptor("samples/dataMapperForBankingPrediction.json"));
            BinaryDTResult binaryDTResult = api.Algorithm.Predict(predictedArray, predictApi.Context) as BinaryDTResult;
            Assert.IsNotNull(binaryDTResult);

            List<BinaryProbability> binaryProbabilities = binaryDTResult.getBinaryDTResult();
            Assert.IsNotNull(binaryProbabilities);
            Assert.AreEqual(1800, binaryProbabilities.Count);

            if (binaryProbabilities.Count == 0)
            {
                Assert.Fail("Nothing to assert, prediction results are empty");
            }

            BinaryProbability row1 = binaryProbabilities[0];
            Assert.AreEqual(0.8950588235294118, row1.GetprobabilityOfOccuranceOfZero());
            Assert.AreEqual(0.10494117647058823, row1.GetprobabilityOfOccuranceOfOne());

            BinaryProbability row2 = binaryProbabilities[1799];
            Assert.AreEqual(0.76470588235294124, row2.GetprobabilityOfOccuranceOfZero());
            Assert.AreEqual(0.23529411764705882, row2.GetprobabilityOfOccuranceOfOne());
        }
    }
}
