using LearningFoundation;
using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearningFoundation.Clustering.KMeans;
using LearningFoundation.Clustering.KMeans.FunctionRecognition;
using System.Reflection;

namespace Test
{
    public class TestOptimalNumberOfClusters
    {
        private static string rootFolder = System.IO.Path.GetFullPath(@"..\..\..\") + "KMeans\\TestFiles\\";

        [TestMethod]
        public void Test_OptimalNumberOfCLustersBasic()
        {
            double[][] clusterCenters = new double[3][];
            clusterCenters[0] = new double[] { 5.0, 5.0 };
            clusterCenters[1] = new double[] { 15.0, 15.0 };
            clusterCenters[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 0;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 2);

            // Creates learning api object
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                var rawData = Helpers.CreateSampleData(clusterCenters, 2, 10000, 0.5);
                return rawData;
            });

            api.UseKMeans(settings);

            // train
            var resp = api.Run() as KMeansScore;

            Assert.IsTrue(resp.Model.NumberOfClusters > 1);

            int points = 150;
            var delta = 2 * Math.PI / 100;
            List<double[]> rows = LearningFoundation.Helpers.FunctionGenerator.CreateFunction(points, 2, delta);
            double[][] rawData2 = new double[points][];
            for (int i = 0; i < points; i++)
            {
                rawData2[i] = new double[2];
                for (int j = 0; j < 2; j++)
                {
                    rawData2[i][j] = rows[j][i];
                }
            }


            // Creates learning api object
            LearningApi api2 = new LearningApi();
            api2.UseActionModule<object, double[][]>((data, ctx) =>
            {
                return rawData2;
            });

            api2.UseKMeans(settings);

            // train
            var resp2 = api2.Run() as KMeansScore;

            Assert.IsTrue(resp2.Model.NumberOfClusters > 1);

        }

        [TestMethod]
        public void Test_OptimalNumberOfCLusters()
        {
            // directory to load
            string loadDirectory = rootFolder + "Functions\\";           
            string FunctionName = "SIN X"; //without extension
            string savePath = rootFolder + "Optimal Clusters\\" + FunctionName +" Results.csv";

            double[][] function = Helpers.LoadFunctionData(loadDirectory + FunctionName + "\\" + FunctionName + ".csv");
            function = TestFunctionGenerators.normalizeData(function);

            int numAttributes = 2;  // 2 in this demo (height,weight)
            int numClusters = 0;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 2);

            // Creates learning api object
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                return KMeansAlgorithm.transposeFunction(function);
            });

            api.UseKMeans(settings);

            // train
            var resp = api.Run() as KMeansScore;

            Assert.IsTrue(resp.Model.NumberOfClusters > 1);

            double[][] OptimalClustersResults = new double[4][];
            OptimalClustersResults[0] = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            OptimalClustersResults[1] = resp.Model.D;
            OptimalClustersResults[2] = resp.Model.DPrime;
            OptimalClustersResults[3] = resp.Model.Fmin;

            Helpers.Write2CSVFile(OptimalClustersResults, savePath);

            // implement
        }

        [TestMethod]
        public void Test_OptimalNumberOfClusters_TwoFunctions()
        {
            int numAttributes = 3;
            int MinNumClusters = 2;
            int MaxNumClusters = 10;
            int maxCount = 300;

            double[] Fmins;

            // directory to load
            string loadDirectory = rootFolder + "Functions\\";

            string functionA = "SIN_SIN X";
            string functionB = "SIN_COS X";
            // load Data
            double[][] loadedSimFunctions1 = Helpers.LoadFunctionData(loadDirectory + functionA + "\\NRP5-10\\" + functionA + " SimilarFunctions Normalized NRP5-10.csv");
            double[][] loadedSimFunctions2 = Helpers.LoadFunctionData(loadDirectory + functionB + "\\NRP5-10\\" + functionB + " SimilarFunctions Normalized NRP5-10.csv");

            ClusteringSettings settings = new ClusteringSettings(maxCount, MinNumClusters, numAttributes, KmeansAlgorithm: 2);

            int recNumClusters = KMeansFunctionRecognitionAlgorithm.OptimalNumberOfClusters_TwoFunctions(loadedSimFunctions1, loadedSimFunctions2, settings, MinNumClusters, MaxNumClusters, out Fmins);
           
        }
    }
}
