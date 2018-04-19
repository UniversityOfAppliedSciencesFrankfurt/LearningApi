using LearningFoundation;
using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LearningFoundation.Clustering.KMeans;
using LearningFoundation.Helpers;
using AnomDetect.KMeans.FunctionRecognition;

namespace Test
{
    public class TestFunctionRecognition
    {
        private static string rootFolder = System.IO.Path.GetFullPath(@"..\..\..\") + "KMeans\\TestFiles\\";

        #region Tests

        /// <summary>
        /// Test_FunctionRecognition is an application for KMeans where the algorithm is used to detect a function
        /// </summary>
        [Fact]
        public void Test_FunctionRecognition()
        {
            int numCluster = 2;
            // a value in % representing the tolerance to possible outliers
            double tolerance = 0;
            // directory to load
            string loadDirectory = rootFolder + "Functions\\";
            // directory to save
            string saveDirectory = rootFolder + "Function Recognition\\";

            //Assert.True(File.Exists("KMeans\\TestFiles\\TestFile01.csv"), "Expected file was not deployed to unit test foilder.");

            // functions' paths
            string[] FunctionPaths = new string[]
            {
                loadDirectory + "TestFile01\\NRP10\\TestFile01 SimilarFunctions Normalized Centroids NRP10 KA2 C" + numCluster + " I500 R1.csv",
                loadDirectory + "TestFile02\\NRP10\\TestFile02 SimilarFunctions Normalized Centroids NRP10 KA2 C" + numCluster + " I500 R1.csv"
            };

            int numTrainFun = 800;
            int numTestFun = 200;

            Tuple<double[][], double[]> trainedClusters = formClusters(FunctionPaths[0], numCluster, numTrainFun);

            // save the formed clusters
            Helpers.Write2CSVFile(trainedClusters.Item1, saveDirectory + "Calculated Centroids.csv");
            double[][] tempMaxDistance = new double[1][];
            tempMaxDistance[0] = trainedClusters.Item2;
            Helpers.Write2CSVFile(tempMaxDistance, saveDirectory + "Calculated Max Distance.csv");

            // start testing for function recognition

            // combine testing data
            double[][] testingCentroids = new double[FunctionPaths.Length * numTestFun * numCluster][];
            double[][] loadedCentroids;
            int testingCentroidsOffset = numTrainFun * numCluster;
            for (int i = 0; i < FunctionPaths.Length; i++)
            {
                loadedCentroids = Helpers.LoadFunctionData(FunctionPaths[i]);
                for (int j = 0; j < numTestFun * numCluster; j++)
                {
                    testingCentroids[i * numTestFun * numCluster + j] = loadedCentroids[j + testingCentroidsOffset];
                }
                // only needed for to avoid training centroids
                testingCentroidsOffset = 0;
            }
            // save the testing centroids
            Helpers.Write2CSVFile(testingCentroids, saveDirectory + "Testing Centroids.csv");

            // check functions


            //KMeans kMeans = new KMeans();
            //kMeans.setTrivialClusters(numCluster, trainedClusters.Item1, trainedClusters.Item2);

            // Creates learning api object
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((data, ctx) =>
            {
                return null;
            });

            // basic settings for prediction
            ClusteringSettings settings = new ClusteringSettings(0, numCluster, 0, tolerance: tolerance, initialCentroids: trainedClusters.Item1);

            // construct trivial clusters
            api.UseKMeans(settings, trainedClusters.Item2);

            double[] funResults = patternTesting(api, settings.NumberOfClusters, testingCentroids);

            // save results
            double[][] tempFunResults = new double[1][];
            tempFunResults[0] = funResults;
            Helpers.Write2CSVFile(tempFunResults, saveDirectory + "Results.csv");

        }

        #endregion

        #region Private Functions


        /// <summary>
        /// formClusters is a function that calculates the centroids and maximum distance of the clusters based on the centroids of the trained similar functions
        /// </summary>
        /// <param name="path">path to the centroids of trained similar functions</param>
        /// <param name="numClusters">number of clusters</param>
        /// <param name="numTrainFun">number of training functions</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: the centroids of the clusters<br />
        /// - Item 2: maximum distance in each cluster
        /// </returns>
        private static Tuple<double[][], double[]> formClusters(string path, int numClusters, int numTrainFun)
        {
            // get centroids of training functions
            double[][] trainedCentroids = Helpers.LoadFunctionData(path);
            // number of attributes
            int dimenions = trainedCentroids[0].Length;
            // initialize cluster centroids
            double[][] clusterCentroids = new double[numClusters][];
            for (int i = 0; i < numClusters; i++)
            {
                clusterCentroids[i] = new double[dimenions];
            }
            // calculate the clusters
            for (int i = 0; i < numTrainFun; i++)
            {
                for (int j = 0; j < numClusters; j++)
                {
                    for (int a = 0; a < dimenions; a++)
                    {
                        clusterCentroids[j][a] += trainedCentroids[i * numClusters + j][a] / numTrainFun;
                    }
                }
            }

            // initialize distances
            double[] maxDistance = new double[numClusters];
            // get max distance in each cluster
            double calDist;
            for (int i = 0; i < numTrainFun; i++)
            {
                for (int j = 0; j < numClusters; j++)
                {
                    calDist = Math.Sqrt(TestTrainingSimilarFunctions.squaredDistance(clusterCentroids[j], trainedCentroids[i * numClusters + j]));
                    if (calDist > maxDistance[j])
                    {
                        maxDistance[j] = calDist;
                    }
                }
            }

            return Tuple.Create(clusterCentroids, maxDistance);
        }

        /*
        /// <summary>
        /// patternTesting is a function that checks and returns result of pattern testing (1 for matching, 0 otherwise)
        /// </summary>
        /// <param name="testCentroids">the testing centroids of the testing functions</param>
        /// <param name="numClusters">number of cluster</param>
        /// <param name="kmeanApi">a KMeans object</param>
        /// <param name="tolerance">tolerance for prediction</param>
        /// <returns>a result of the pattern testing for each function</returns>
        private static double[] patternTesting2(double[][] testCentroids, int numClusters, KMeans kmeanApi, int tolerance)
        {
            CheckingSampleSettings SampleSettings;
            int clusterIndex;
            bool fitsPattern;
            double[] result = new double[testCentroids.Length / numClusters];
            for (int i = 0; i < testCentroids.Length; i = i + numClusters)
            {
                fitsPattern = true;
                // check each centroid of each function
                for (int j = 0; j < numClusters; j++)
                {
                    // check centroids
                    SampleSettings = new CheckingSampleSettings(testCentroids[i + j], tolerance: tolerance);
                    clusterIndex = kmeanApi.PredictSample(SampleSettings);
                    // if a centroid doesn't belong to any cluster
                    if (clusterIndex == -1)
                    {
                        fitsPattern = false;
                        break;
                    }
                }
                if (fitsPattern)
                {
                    result[i / numClusters] = 1;
                }
                else
                {
                    result[i / numClusters] = 0;
                }
            }
            // contains results of pattern testing (1 for matching, 0 otherwise)
            return result;
        }
        */

        /// <summary>
        /// patternTesting is a function that checks and returns result of pattern testing (1 for matching, 0 otherwise)
        /// </summary>
        /// <param name="testCentroids">the testing centroids of the testing functions</param>
        /// <param name="numClusters">number of cluster</param>
        /// <param name="kmeanApi">a KMeans object</param>
        /// <param name="tolerance">tolerance for prediction</param>
        /// <returns>a result of the pattern testing for each function</returns>
        private static double[] patternTesting(LearningApi api, int numClusters, double[][] testCentroids)
        {

            double[][] oneFunctionData = new double[numClusters][];
            //api kmeanApi.Instance;

            //double[] oneFunctionResult = new double[numClusters];
            double[] result = new double[testCentroids.Length / numClusters];
            for (int i = 0; i < testCentroids.Length; i = i + numClusters)
            {

                // check each centroid of each function
                for (int j = 0; j < numClusters; j++)
                {
                    // fill function centroids
                    oneFunctionData[j] = testCentroids[i + j];
                }

                //get prediction
                var res = api.Algorithm.Predict(oneFunctionData, api.Context) as KMeansResult;

                result[i / numClusters] = checkPredictions(res.PredictedClusters);
            }
            // contains results of pattern testing (1 for matching, 0 otherwise)
            return result;
        }

        /// <summary>
        /// checkPredictions is a function that checks if prediction of the centroids of 1 function fits in the clusters (one centroid for each cluster) 
        /// </summary>
        /// <param name="results">prediction of each centroid of one function</param>
        /// <returns></returns>
        private static int checkPredictions(int[] results)
        {
            string clusters = "";
            for (int i = 0; i < results.Length; i++)
            {
                clusters += ";" + i + ";";
            }
            for (int i = 0; i < results.Length; i++)
            {
                if (clusters.Contains(";" + results[i] + ";"))
                {
                    clusters.Replace(";" + results[i] + ";", "");
                }
                else
                {
                    return 0;
                }
            }
            return 1;
        }


        /// <summary>
        /// Creates 100 (batch=100) similar SIN functions, which differ in 10% of theit Y values.
        /// Functions are 2D=>{X,Y}. As next it creates predicting functions, which differ in value specified by 
        /// noiceForPrediction.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="noiceForPrediction">Noice for SIN reference function. Algorithm is trained with noic 10%.
        /// That means all noices less than 10% (plus/minus some delta) should be recognized as true positives.
        /// All values higher than 25% should be recognized as true negatives.
        /// All other values between 10 and 24 are most likely recognized as true negatives. This is not mathematicall 100% safe, so we excluded these values 
        /// from tests.</param>
        [Theory]
        [InlineData(200, 10)]
        [InlineData(200, 9)]
        [InlineData(200, 5)]
        [InlineData(200, 1)]
        [InlineData(400, 28)]
        [InlineData(200, 25)]
        [InlineData(400, 30)]
        [InlineData(1000, 30)]
        [InlineData(1000, 25)]
        [InlineData(1000, 35)]
        public void Test_FunctionRecognitionModule(int points, int noiceForPrediction)
        {
            #region Training
            var batch = 100;

            var funcData = FunctionGenerator.CreateFunction(points, 2, 2 * Math.PI / 100);

            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                var similarFuncData = FunctionGenerator.CreateSimilarFromReferenceFunc(funcData.ToArray(), 10);

                double[][] formattedData = formatData(similarFuncData);

                return formattedData;
            });

            double[][] initCentroids = new double[4][];
            initCentroids[0] = new double[] { 1.53, 0.63 };
            initCentroids[1] = new double[] { 4.68, -0.63 };
            initCentroids[2] = new double[] { 7.85, 0.62 };
            initCentroids[3] = new double[] { 10.99, -0.64 };

            ClusteringSettings settings = new ClusteringSettings(0, numClusters: 4, numDims: 2, KmeansAlgorithm: 2, initialCentroids: initCentroids, tolerance: 0) { KmeansMaxIterations = 1000 };

            api.UseKMeansFunctionRecognitionModule(settings);

            KMeansFunctionRecognitonScore res;

            while (batch-- > 0)
            {
                res = api.RunBatch() as KMeansFunctionRecognitonScore;
            }
            #endregion

            #region Prediction
            var noicedFunc = FunctionGenerator.CreateSimilarFromReferenceFunc(funcData.ToArray(), noiceForPrediction);

            double[][] data = formatData(noicedFunc);

            var predictionResult = api.Algorithm.Predict(data, null) as KMeansFuncionRecognitionResult;

            // TRUE positives
            if (noiceForPrediction <= 10)
            {
                Assert.True(predictionResult.Loss == 1.0);
            }
            // TRUE negatives
            else if (noiceForPrediction >= 25)
            {
                Assert.False(predictionResult.Loss == 1.0);
            }
            #endregion
        }

        [InlineData(10)]
        [InlineData(9)]
        [InlineData(5)]
        [InlineData(1)]
        [InlineData(28)]
        [InlineData(25)]
        [InlineData(30)]
        [InlineData(35)]
        [Theory]
        public void Test_FunctionRecognitionModuleSave(int noiceForPrediction)
        {
            #region Train and Save
            var batch = 100;

            var funcData = FunctionGenerator.CreateFunction(500, 2, 2 * Math.PI / 100);

            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                var similarFuncData = FunctionGenerator.CreateSimilarFromReferenceFunc(funcData.ToArray(), 10);

                double[][] formattedData = formatData(similarFuncData);

                return formattedData;
            });

            double[][] initCentroids = new double[4][];
            initCentroids[0] = new double[] { 1.53, 0.63 };
            initCentroids[1] = new double[] { 4.68, -0.63 };
            initCentroids[2] = new double[] { 7.85, 0.62 };
            initCentroids[3] = new double[] { 10.99, -0.64 };

            ClusteringSettings settings = new ClusteringSettings(0, numClusters: 4, numDims: 2, KmeansAlgorithm: 2, initialCentroids: initCentroids, tolerance: 0) { KmeansMaxIterations = 1000 };

            api.UseKMeansFunctionRecognitionModule(settings);

            KMeansFunctionRecognitonScore res;

            while (batch-- > 0)
            {
                res = api.RunBatch() as KMeansFunctionRecognitonScore;
            }

            api.Save("sinusmodel");
            #endregion

            #region Load And Predict
            var api2 = LearningApi.Load("sinusmodel");
            var noicedFunc = FunctionGenerator.CreateSimilarFromReferenceFunc(funcData.ToArray(), noiceForPrediction);

            double[][] data = formatData(noicedFunc);

            var predictionResult = api2.Algorithm.Predict(data, null) as KMeansFuncionRecognitionResult;

            // TRUE positives
            if (noiceForPrediction <= 10)
            {
                Assert.True(predictionResult.Loss == 1.0);
            }
            // TRUE negatives
            else if (noiceForPrediction >= 25)
            {
                Assert.False(predictionResult.Loss == 1.0);
            }
            #endregion
        }
        private static double[][] formatData(double[][] similarFuncData)
        {
            double[][] data = new double[similarFuncData[0].Length][];
            for (int i = 0; i < similarFuncData[0].Length; i++)
            {
                data[i] = new double[similarFuncData.Length];
                for (int j = 0; j < similarFuncData.Length; j++)
                {
                    data[i][j] = similarFuncData[j][i];
                }
            }

            return data;
        }
        #endregion
    }
}
