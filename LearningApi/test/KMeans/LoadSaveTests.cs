using LearningFoundation;
using LearningFoundation.Clustering.KMeans;
using LearningFoundation.DataMappers;
using Xunit;
using System.Linq;

namespace Test
{

    public class LoadSaveTests
    {

        /// <summary>
        /// Demonstrates how to use Save and Load of KMeans.
        /// Same principal can be used for every other algorithm, as long algoritm does support model persistence.
        /// </summary>
        [Fact]
        public void Test_LoadSave()
        {
            string moduleName = "test-action";

            double[][] clusterCentars = new double[3][];
            clusterCentars[0] = new double[] { 5.0, 5.0 };
            clusterCentars[1] = new double[] { 15.0, 15.0 };
            clusterCentars[2] = new double[] { 30.0, 30.0 };

            string[] attributes = new string[] { "Height", "Weight" };

            int numAttributes = attributes.Length;  // 2 in this demo (height,weight)
            int numClusters = 3;  // vary this to experiment (must be between 2 and number data tuples)
            int maxCount = 300;  // trial and error

            ClusteringSettings settings = new ClusteringSettings(maxCount, numClusters, numAttributes, KmeansAlgorithm: 1);

            // Creates learning api object
            LearningApi api = new LearningApi(loadDescriptor());

            //
            // Defines action method, which will generate training data.
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);
                return rawData;
            }, moduleName);

            api.UseKMeans(settings);

            var resp = api.Run() as KMeansScore;

            Assert.True(resp.Model.Clusters != null);
            Assert.True(resp.Model.Clusters.Length == clusterCentars.Length);

            var result = api.Algorithm.Predict(clusterCentars, api.Context) as KMeansResult;
            Assert.True(result.PredictedClusters[0] == 0);
            Assert.True(result.PredictedClusters[1] == 1);
            Assert.True(result.PredictedClusters[2] == 2);

            // This is where trained model is saved.
            api.Save(nameof(LoadSaveTests));

            // Loads the saved model.
            var loadedApi = LearningApi.Load(nameof(LoadSaveTests));

            //
            // Because we have used action method in the LearningApi, we will have to setup it again.
            // This is not required because API design limitation. It is restriction of .NET framework. It cannot persist code.
            loadedApi.ReplaceActionModule<object, double[][]>(moduleName, (data, ctx) =>
            {
                var rawData = Helpers.CreateSampleData(clusterCentars, 2, 10000, 0.5);
                return rawData;
            });

            loadedApi.Run();
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
