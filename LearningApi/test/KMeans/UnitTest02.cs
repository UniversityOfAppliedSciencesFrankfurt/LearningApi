using LearningFoundation;
using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LearningFoundation.Clustering.KMeans;

namespace Test
{
    static class Globals
    {
        // a global needed for creating permutations
        public static int PermCount;
    }
    /// <summary>
    /// UnitTest02 is a test/application for KMeans algorithm (function recognition).
    /// </summary>
    public class UnitTest02
    {
        #region Tests

        /// <summary>
        /// Test_TrainingSimilarFunctions is a test that trains the similar functions and saves the resulting centroids 
        /// </summary>
        [Fact]
        public void Test_TrainingSimilarFunctions()
        {
            // Settings to import the functions (NRP should match the desired loading file)
            string FunctionName = "F1";
            string directory = @"C:\Users\skiwa\Desktop\Thesis\Functions\" + FunctionName + "\\";
            int NRP = 10;
            
            // Settings for the K-Means Alg
            int maxCount = 500;
            int numClusters = 6;
            int numAttributes = 2;
            int KAlg = 2;
            int Runs = 3;

            // prepare the functions for clustering
            double[][] mFun = Helpers.cSVtoDoubleJaggedArray(directory + "\\NRP" + NRP + "\\" + FunctionName + " SimilarFunctions Normalized NRP" + NRP + ".csv");

            int NumFun = mFun.Length/numAttributes;

            // Creates learning api object
            LearningApi api;

            ClusteringSettings clusterSettings;

            double[][] Centroids;
            // original Centroids
            double[][] oCentroids;
            // matched Centroids
            double[][] mCentroids;

            for (int k = 2; k < numClusters + 1; k++)
            {
                oCentroids = new double[k][];
                clusterSettings = new ClusteringSettings(maxCount, k, numAttributes, KmeansAlgorithm: KAlg, Replace: true);
                for (int j = 0; j < Runs; j++)
                {
                    // save directory
                    string SavePath = directory + "NRP" + NRP + "\\" + FunctionName + " SimilarFunctions Centroids NRP" + NRP + " KA" + KAlg + " C" + k + " I" + maxCount + " R" + (j + 1) + ".csv";

                    for (int i = 0; i < NumFun; i++)
                    {
                        // cluster each function
                        double[][] rawData = getSimilarFunctionsData(mFun, numAttributes, i + 1);
                        api = new LearningApi();
                        api.UseActionModule<object, double[][]>((data, ctx) =>
                        {
                            return rawData;
                        });
                        api.UseKMeans(clusterSettings);

                        // train
                        var resp = api.Run() as KMeansScore;
                     
                        // get resulting centroids
                        Centroids = resp.instance.Centroids;
                        // match the centroids centroids
                        if (i == 0)
                        {
                            oCentroids = Centroids;
                            mCentroids = Centroids;
                        }
                        else
                        {
                            mCentroids = matchCentroids(Centroids, oCentroids);
                        }

                        // save centroids
                        if (i == 0)
                        {
                            // save or overwrite
                            Helpers.Write2CSVFile(mCentroids, SavePath);
                        }
                        else
                        {
                            // append
                            Helpers.Write2CSVFile(mCentroids, SavePath, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Test_FunctionRecognition is an application for KMeans where the algorithm is used to detect a function
        /// </summary>
        [Fact]
        public void Test_FunctionRecognition()
        {
            int numCluster = 2;
            // directory to load
            string myDirectory = @"C:\Users\skiwa\Desktop\Thesis\Functions\";
            // directory to save
            string saveDirectory = @"C:\Users\skiwa\Desktop\Thesis\Pattern Recognition\";
            // functions' paths
            string[] FunctionPaths = new string[]
            {
                myDirectory + "F2\\NRP10\\F2 SimilarFunctions Centroids NRP10 KA2 C" + numCluster + " I500 R1.csv",
                myDirectory + "F2\\NRP10\\F2 SimilarFunctions Centroids NRP10 KA2 C" + numCluster + " I500 R1.csv"
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
            int tolerance = 0;

            // combine testing data
            double[][] testingCentroids = new double[FunctionPaths.Length * numTestFun * numCluster][];
            double[][] loadedCentroids;
            int testingCentroidsOffset = numTrainFun * numCluster;
            for (int i = 0; i < FunctionPaths.Length; i++)
            {
                loadedCentroids = Helpers.cSVtoDoubleJaggedArray(FunctionPaths[i]);
                for(int j = 0; j < numTestFun * numCluster; j++)
                {
                    testingCentroids[i * numTestFun * numCluster + j] = loadedCentroids[j + testingCentroidsOffset];
                }
                // only needed for to avoid training centroids
                testingCentroidsOffset = 0;
            }
            // save the testing centroids
            Helpers.Write2CSVFile(testingCentroids, saveDirectory + "Testing Centroids.csv");

            // check functions
            KMeans kMeans = new KMeans();
            kMeans.setTrivialClusters(numCluster, trainedClusters.Item1, trainedClusters.Item2);
            double[] funResults = patternTesting(testingCentroids, numCluster, kMeans, tolerance);

            // save results
            double[][] tempFunResults = new double[1][];
            tempFunResults[0] = funResults;
            Helpers.Write2CSVFile(tempFunResults, saveDirectory + "Results.csv");

        }

        #endregion

        #region Private Functions
        
        /// <summary>
        /// getSimilarFunctionsData is a function that loads data of the specified similar function and converts its data to the required format for clustering
        /// </summary>
        /// <param name="mFun">the similar functions</param>
        /// <param name="numAttributes">number of attributes of these functions</param>
        /// <param name="functionNumber">the number of the desired similar function</param>
        /// <returns>the data of the desired similar function</returns>
        private static double[][] getSimilarFunctionsData(double[][] mFun, int numAttributes, int functionNumber)
        {
            double[][] Data = new double[numAttributes][];
            // load coordinates of the function
            for(int a=0; a < numAttributes; a++)
            {
                Data[a] = mFun[(functionNumber - 1) * numAttributes + a];
            }
            // change format for clustering purposes (transpose)
            double[][] rawData = new double[Data[0].Length][];
            for (int i = 0; i < Data[0].Length; i++)
            {
                rawData[i] = new double[numAttributes];
                for(int j = 0; j < numAttributes; j++)
                {
                    rawData[i][j] = Data[j][i];
                }
            }
            return rawData;
        }

        /// <summary>
        /// matchCentroids is a function that sorts the clustered centroids based on minimum total distance to the original centroids 
        /// </summary>
        /// <param name="newCentroids">centroids of a similar function</param>
        /// <param name="originalCentroids">centroids of the main function</param>
        /// <returns>the centroids in the order of the original centroids</returns>
        private static double[][] matchCentroids(double[][] newCentroids, double[][] originalCentroids)
        {
            int n = originalCentroids.Length;
            // get all possible permutations
            int[][] permutations = FindPermutations(n);
            // get the squared distances
            double[][] squaredDistances = squaredDistanceMatrix(originalCentroids, newCentroids);
            // get best permutaion
            int bestPerm = bestPermutaion(squaredDistances, permutations);
            // initialize and sort the centroids according to best permutation
            double[][] matchedCentroids = new double[n][];
            for (int i = 0; i < n; i++)
            {
                matchedCentroids[i] = new double[originalCentroids[0].Length];
                matchedCentroids[i] = newCentroids[permutations[bestPerm][i]];
            }
            return matchedCentroids;
        }

        /// <summary>
        /// squaredDistanceMatrix is a function that calculates all squared distances of any possible pair between original and new centroids
        /// </summary>
        /// <param name="originalCentroids">centroids of the main function</param>
        /// <param name="newCentroids">centroids of a similar function</param>
        /// <returns>the matrix of the distances between every possible centroid pair</returns>
        private static double[][] squaredDistanceMatrix(double[][] originalCentroids, double[][] newCentroids)
        {
            int n = originalCentroids.Length;
            // initialize the squared distance matrix
            double[][] distanceMatrix = new double[n][];
            for (int i = 0; i < n; i++)
            {
                distanceMatrix[i] = new double[n];
            }
            // calculate the squared distances
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    distanceMatrix[i][j] = squaredDistance(originalCentroids[i], newCentroids[j]);
                }
            }
            return distanceMatrix;
        }

        /// <summary>
        /// squaredDistance is a function that calculates the squared distance between 2 double arrays 
        /// </summary>
        /// <param name="A">first double array</param>
        /// <param name="B">second double array</param>
        /// <returns>the distance between the two points</returns>
        private static double squaredDistance(double[] A, double[] B)
        {
            double SquaredDistance = 0;
            for (int i = 0; i < A.Length; i++)
            {
                // for each coordinate
                SquaredDistance += Math.Pow(A[i] - B[i], 2);
            }
            return SquaredDistance;
        }

        /// <summary>
        /// bestPermutaion is a function that returns index of the permutaion that result in smallest total distance
        /// </summary>
        /// <param name="squaredDistances">the matrix of the distances between every possible centroid pair</param>
        /// <param name="permutations">all possible permutaions</param>
        /// <returns>the index of the permutaion that result in smallest total distance</returns>
        private static int bestPermutaion(double[][] squaredDistances, int[][] permutations)
        {
            // index resulting in minimum total distance
            int bestPerm = 0;
            // calculate total distances of all possible pairs
            double[] Sum = new double[permutations.Length];
            for (int i = 0; i < permutations.Length; i++)
            {
                Sum[i] = 0;
                for (int j = 0; j < squaredDistances.Length; j++)
                {
                    Sum[i] += squaredDistances[j][permutations[i][j]];
                }
                // update index
                if (Sum[i] < Sum[bestPerm])
                {
                    bestPerm = i;
                }
            }
            return bestPerm;
        }

        #region Copied

        /* 
         * Permutation functions by Ziezi with minor changes
         * https://stackoverflow.com/questions/11208446/generating-permutations-of-a-set-most-efficiently
         */
        /* Method: FindPermutations(n) */
        private static int[][] FindPermutations(int n)
        {
            int[][] PermArray = new int[factorial(n)][];
            for (int i = 0; i < PermArray.Length; i++)
            {
                PermArray[i] = new int[n];
            }
            int[] arr = new int[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = i;
            }
            int iEnd = arr.Length - 1;
            Globals.PermCount = 0;
            Permute(arr, iEnd, PermArray);
            return PermArray;
        }
        /* Method: Permute(arr) */
        private static void Permute(int[] arr, int iEnd, int[][] PermArray)
        {
            if (iEnd == 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    PermArray[Globals.PermCount][i] = arr[i];
                }
                Globals.PermCount++;
                /*
                //PrintArray(arr);
                double[][] temp = new double[1][];
                temp[0] = new double[arr.Length];
                for(int i = 0; i < arr.Length; i++)
                {
                    temp[0][i] = arr[i];
                }
                Helpers.Write2CSVFile(temp, @"C:\Users\KywAnn\Desktop\temp.csv", true);
                */
                return;
            }
            Permute(arr, iEnd - 1, PermArray);
            for (int i = 0; i < iEnd; i++)
            {
                swap(ref arr[i], ref arr[iEnd]);
                Permute(arr, iEnd - 1, PermArray);
                swap(ref arr[i], ref arr[iEnd]);
            }
        }
        /* Method: PrintArray() */
        private static void PrintArray(int[] arr, string label = "")
        {
            Console.WriteLine(label);
            Console.Write("{");
            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write(arr[i]);
                if (i < arr.Length - 1)
                {
                    Console.Write(", ");
                }
            }
            Console.WriteLine("}");
        }
        /* Method: swap(ref int a, ref int b) */
        private static void swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
        // returns factorial
        static int factorial(int n)
        {
            if (n >= 2)
            {
                return n * factorial(n - 1);
            }
            return 1;
        }

        #endregion

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
            double[][] trainedCentroids = Helpers.cSVtoDoubleJaggedArray(path);
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
                for (int j=0; j < numClusters; j++)
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
                    calDist = Math.Sqrt(squaredDistance(clusterCentroids[j], trainedCentroids[i * numClusters + j]));
                    if (calDist > maxDistance[j])
                    {
                        maxDistance[j] = calDist;
                    }
                }
            }

            return Tuple.Create(clusterCentroids, maxDistance);
        }
        
        /// <summary>
        /// patternTesting is a function that checks and returns result of pattern testing (1 for matching, 0 otherwise)
        /// </summary>
        /// <param name="testCentroids">the testing centroids of the testing functions</param>
        /// <param name="numClusters">number of cluster</param>
        /// <param name="kmeanApi">a KMeans object</param>
        /// <param name="tolerance">tolerance for prediction</param>
        /// <returns>a result of the pattern testing for each function</returns>
        private static double[] patternTesting(double[][] testCentroids, int numClusters, KMeans kmeanApi, int tolerance)
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
                    SampleSettings = new CheckingSampleSettings(null, testCentroids[i + j], tolerance: tolerance);
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

        #endregion


    }
}
