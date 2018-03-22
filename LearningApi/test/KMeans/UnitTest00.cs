using LearningFoundation;
using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LearningFoundation.Clustering.KMeans;
using System.Reflection;

namespace Test
{
    /// <summary>
    /// UnitTest00 is a class that contains tests for the main functions of "KMeans.cs" 
    /// </summary>
    public class UnitTest00
    {
        private static string rootFolder = System.IO.Path.GetFullPath(@"..\..\..\") + "KMeans\\TestFiles\\JsonSaves\\";

        static UnitTest00()
        {
            if (Directory.Exists(rootFolder) == false)
            {
                Directory.CreateDirectory(rootFolder);
            }
        }

        #region Tests

        /// <summary>
        /// Test_TrainAndPredict is a test for the KMeans.Run or KMeans.Train and KMeans.Predict functions through LearningApi
        /// </summary>
        [Fact]
        public void Test_TrainAndPredict()
        {
            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1, Replace: true);
          
            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);
                return rawData ;
            });

            api.UseKMeans(settings);
            
            // train
            var resp = api.Run() as KMeansScore;
            
            Assert.True(resp.Model.Clusters != null);
            Assert.True(resp.Model.Clusters.Length == clusterCentars.Length);

            // Predict
            var result = api.Algorithm.Predict(clusterCentars, api.Context) as KMeansResult;
            Assert.True(result.PredictedClusters[0] == 0);
            Assert.True(result.PredictedClusters[1] == 1);
            Assert.True(result.PredictedClusters[2] == 2);
        }

        /// <summary>
        /// Test_Save is a test for KMeans.Save function
        /// </summary>
        [Fact]
        public void Test_Save()
        {

            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1,  Replace: true);

            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());

            // creates data
            var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);
            KMeans kMeans = new KMeans(settings);
            // train
            var response = kMeans.Run(rawData, api.Context);

            string fileName = "Test01.json";

            kMeans.Save(rootFolder + fileName);
        }

        /// <summary>
        /// Test_Load is a test for KMeans.Load function
        /// </summary>
        [Fact]
        public void Test_Load()
        {
            string fileName = "Test01.json";

            KMeans kMeans = new KMeans();
            kMeans.Load(rootFolder + fileName);

            Assert.True(kMeans.Instance != null);
            Assert.True(kMeans.Instance.Clusters != null);
        }

        /// <summary>
        /// Test_LoadSave is a test for the api.Save method
        /// </summary>
        [Fact]
        public void Test_LoadSave()
        {
            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1,  Replace: true);

            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);
                return rawData;
            });

            api.UseKMeans(settings);

            var resp = api.Run() as KMeansScore;

            Assert.True(resp.Model.Clusters != null);
            Assert.True(resp.Model.Clusters.Length == clusterCentars.Length);

            var result = api.Algorithm.Predict(clusterCentars, api.Context) as KMeansResult;
            Assert.True(result.PredictedClusters[0] == 0);
            Assert.True(result.PredictedClusters[1] == 1);
            Assert.True(result.PredictedClusters[2] == 2);

            api.Save("unittestmodel.json");
        }

        #endregion

        #region Private Functions

        private static DataDescriptor loadDescriptor()
        {
            var des = new DataDescriptor();

            des.Features = new Column[2];
            des.Features[0] = new Column { Id = 1, Name = "Height", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.0 };
            des.Features[1] = new Column { Id = 2, Name = "Weight", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.0 };

            return des;
        }

        #endregion
    }
}
