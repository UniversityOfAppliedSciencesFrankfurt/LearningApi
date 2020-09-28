using System;
using System.Collections.Generic;
using LearningFoundation;

namespace AnomalyDetectionK
{
    /// <summary>
    /// Creates Cluster With K-mean and Predict Anomaly 
    /// </summary>
    public class Project : IAlgorithm
    {   
        int numClusters = 3;
        private double m_LearningRate;
        public static Comper use = new Comper();
        public Dictionary<int, float> AverageSpeed { get; set; } = new Dictionary<int, float>();

        /// <summary>
        /// Constractor of TimeSeriesRegression class
        /// </summary>
        /// <param name="learningRate">The learning rate of algorithm. Typically 0.2 - 1.7.</param>
        /// <param name="anotherArg"></param>
        public Project(double learningRate, double anotherArg = 2.5)
        {
            this.m_LearningRate = learningRate;
        }

        /// <summary>
        /// Detect Anomaly based on given input
        /// </summary>
        /// <param name="data">TestData</param>
        /// <param name="ctx">data description</param>
        public IResult Predict(double[][] data, IContext ctx)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            AnomalyDetectionAlgorithmScore clu = new AnomalyDetectionAlgorithmScore();
            double[] DT = new double[numClusters];
            double[] DT2 = new double[numClusters];
            double[] com = new double[numClusters];
            int[] DataResult = new int[data.Length];

            for (int a = 0; a < 3; a++)
            {
                DT[a] = use.avg[a];
            }
            int[] clustering = Cluster(data, numClusters);

            for (int a = 0; a < 3; a++)
            {
                DT2[a] = use.avg[a];
                com[a] = Math.Abs(DT[a] - DT2[a]);
                com[a] = (com[a] / DT[a]) * 100;
                if (com[a] >= 20)
                {
                        DataResult[data.Length-1] = 0;
                        break;
                    
                }
                else
                        DataResult[data.Length-1] = 1;
            }
            AnomalyDetectionAlgorithmResult results = new AnomalyDetectionAlgorithmResult();
            results.Results = DataResult;
            return results;
        }
        /// <summary>
        /// Creat Cluster for Train data
        /// </summary>
        /// <param name="data">TrainData</param>
        /// <param name="ctx">data description</param>
        public IScore Run(double[][] data, IContext ctx)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }   
            // this is calling Cluster Function
            int[] clustering = Cluster(data, numClusters);
            Console.WriteLine(clustering);
            AnomalyDetectionAlgorithmScore alg = new AnomalyDetectionAlgorithmScore();
            alg.cl = clustering;
            alg.OldData = data;
            return alg;
        }

        /// <summary>
        /// Return Run Function
        /// </summary>
        /// <param name="data">TrainData</param>
        /// <param name="ctx">data description</param>
        public IScore Train(double[][] data, IContext ctx)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return Run(data, ctx);
        }
        // ===============================================================================================================================================================================
        /// <summary>
        /// k-means clustering
        /// </summary>
        /// <param name="rawData">Data</param>
        /// <param name="numClusters">Number of Clusters</param>
        public static int[] Cluster(double[][] rawData, int numClusters)
        {
            // so large values don't dominate
            double[][] Data = Normalized(rawData);
            // was there a change in at least one cluster assignment?
            bool changed = true;
            // were all means able to be computed? (no zero-count clusters)
            bool success = true;
            // semi-random initialization
            int[] clustering = InitClustering(Data.Length, numClusters, 0);
            // small convenience
            double[][] means = Allocate(numClusters, Data[0].Length);
            // sanity check
            int maxCount = Data.Length * 10; 
            int ct = 0;
            while (changed == true && success == true && ct < maxCount)
            {
                ++ct;
                // compute new cluster means if possible. no effect if fail
                success = UpdateMeans(Data, clustering, means);
                // (re)assign tuples to clusters. no effect if fail
                changed = UpdateClustering(Data, clustering, means);
            }
            return clustering;
        }
        /// <summary>
        /// normalize raw data by computing (x - mean) / stddev
        /// </summary>
        /// <param name="rawData">Data</param>
        private static double[][] Normalized(double[][] rawData)
        {
            double[][] result = new double[rawData.Length][];
            for (int i = 0; i < rawData.Length; ++i)
            {
                result[i] = new double[rawData[i].Length];
                Array.Copy(rawData[i], result[i], rawData[i].Length);
            }
            // each col
            for (int j = 0; j < result[0].Length; ++j) 
            {
                double colSum = 0.0;
                for (int i = 0; i < result.Length; ++i)
                    colSum += result[i][j];
                double mean = colSum / result.Length;
                double sum = 0.0;
                for (int i = 0; i < result.Length; ++i)
                    sum += (result[i][j] - mean) * (result[i][j] - mean);
                double sd = sum / result.Length;
                for (int i = 0; i < result.Length; ++i)
                    result[i][j] = (result[i][j] - mean) / sd;
            }
            return result;
        }

        /// <summary>
        ///  init clustering semi-randomly (at least one tuple in each cluster)
        /// </summary>
        /// <param name="numTuples">Data length</param>
        /// <param name="numClusters">Number of Clusters</param>
        /// <param name="randomSeed">instead of randomly assigning</param>
        private static int[] InitClustering(int numTuples, int numClusters, int randomSeed)
        {
            Random random = new Random(randomSeed);
            int[] clustering = new int[numTuples];
            // make sure each cluster has at least one tuple
            for (int i = 0; i < numClusters; ++i) 
                clustering[i] = i;
            // other assignments random
            for (int i = numClusters; i < clustering.Length; ++i)
                clustering[i] = random.Next(0, numClusters); 
            return clustering;
        }
        /// <summary>
        ///  convenience matrix allocator for Cluster()
        /// </summary>
        /// <param name="numClusters">Number of Clusters</param>
        /// <param name="numColumns">Number of Columns</param>
        private static double[][] Allocate(int numClusters, int numColumns)
        {
            double[][] result = new double[numClusters][];
            for (int k = 0; k < numClusters; ++k)
                result[k] = new double[numColumns];
            return result;
        }
        /// <summary>
        /// Calculate means of every cluster
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="clustering">Clusters</param>
        /// <param name="means">means value of Clusters</param>
        private static bool UpdateMeans(double[][] data, int[] clustering, double[][] means)
        {
            int numClusters = means.Length;
            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = clustering[i];
                ++clusterCounts[cluster];
            }
            // bad clustering. no change to means[][]
            for (int k = 0; k < numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false; 
            // update, zero-out means so it can be used as scratch matrix 
            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] = 0.0;

            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = clustering[i];
                for (int j = 0; j < data[i].Length; ++j)
                    means[cluster][j] += data[i][j]; // accumulate sum
            }

            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] /= clusterCounts[k]; // danger of div by 0
            return true;
        }
        /// <summary>
        /// (re)assign each tuple to a cluster (closest mean)
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="clustering">Clusters</param>
        /// <param name="means">means value of Clusters</param>
        private static bool UpdateClustering(double[][] data, int[] clustering, double[][] means)
        {
            int numClusters = means.Length;
            bool changed = false;
            
            // proposed result
            int[] newClustering = new int[clustering.Length]; 
            Array.Copy(clustering, newClustering, clustering.Length);

            // distances from curr tuple to each mean
            double[] distances = new double[numClusters]; 
            double[] sumD = new double[numClusters];
            double[] count = new double[numClusters];
            double[] avgD = new double[numClusters];
            for (int k = 0; k < numClusters; ++k)
            {
                sumD[k] = 0;
                count[k] = 0;
                avgD[k] = 0;
            }
            // walk thru each tuple
            for (int i = 0; i < data.Length; ++i) 
            {  
                // compute distances from curr tuple to all k means
                for (int k = 0; k < numClusters; ++k)
                    distances[k] = Distance(data[i], means[k]);
                // find closest mean ID
                int newClusterID = MinIndex(distances); 
                if (newClusterID != newClustering[i])
                {
                    changed = true;
                    newClustering[i] = newClusterID; // update
                }
                sumD[newClusterID] = sumD[newClusterID] + distances[newClustering[i]];
                count[newClusterID] = count[newClusterID]+1;
            }

            for (int i = 0; i < numClusters; ++i)
            {
                avgD[i] = sumD[i] / count[i];
                use.avg[i] = avgD[i];
            }
            // no change so bail and don't update clustering[][]
            if (changed == false)
                return false;
            // check proposed clustering[] cluster counts
            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = newClustering[i];
                ++clusterCounts[cluster];
            }
            // bad clustering. no change to clustering[][]
            for (int k = 0; k < numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false; 
            Array.Copy(newClustering, clustering, newClustering.Length); // update

            // good clustering and at least one change
            return true; 
        }
        /// <summary>
        /// Euclidean distance between two vectors for UpdateClustering()
        /// </summary>
        /// <param name="tuple">Tuple</param>
        /// <param name="means">means value of Clusters</param>
        private static double Distance(double[] tuple, double[] mean)
        {
            double sumSquaredDiffs = 0.0;
            for (int j = 0; j < tuple.Length; ++j)
                sumSquaredDiffs += Math.Pow((tuple[j] - mean[j]), 2);
            return Math.Sqrt(sumSquaredDiffs);
        }
        /// <summary>
        /// index of smallest value in array
        /// </summary>
        /// <param name="distances">Euclidean distance between two vectors</param>
        private static int MinIndex(double[] distances)
        {
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }
    }
    public class Comper
    {
        /// <summary>
        /// Store Average value distan of cluster
        /// </summary>
        /// <param name="api">Object</param>
        public double[] avg = new double[3];
    }
}
// ===============================================================================================================================================================================