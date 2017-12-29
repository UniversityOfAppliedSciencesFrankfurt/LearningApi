using AnomalyDetection.Interfaces;
using AnomalyDetectionApi;
using LearningFoundation;
using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AnomDetect.KMeans;


namespace Test
{

    public class UnitTest
    {
      
        [Fact]
        public void Test_GetClusters()
        {
            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            //double[][] testData = new double[3][];
            //testData[0] = new double[] { 5.0, 5.0 };
            //testData[1] = new double[] { 15.0, 15.0 };
            //clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1, InitialGuess: true, Replace: true);
          
            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);
                return rawData ;
            });

            api.UseKMeans(settings);

            var resp = api.Run() as KMeansResult;
            
            Assert.True(resp.Clusters != null);
            Assert.True(resp.Clusters.Length == clusterCentars.Length);

            var result = api.Algorithm.Predict(clusterCentars, api.Context);
            Assert.True(result[0] == 0);
            Assert.True(result[1] == 1);
            Assert.True(result[2] == 2);
        }

      
        private static DataDescriptor loadDescriptor()
        {
            var des = new DataDescriptor();

            des.Features = new Column[2];
            des.Features[0] = new Column { Id = 1, Name = "Height", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.0 };
            des.Features[1] = new Column { Id = 2, Name = "Weight", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.0 };

            return des;
        }
    }
}
