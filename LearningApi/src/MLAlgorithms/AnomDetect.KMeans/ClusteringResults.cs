using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// ClusteringResults is a class that provides the results per cluster of a clustering instance with additional statistics.
    /// </summary>
    public class ClusteringResults
    {
        #region Public Function

        /// <summary>
        /// CreateClusteringResult is a function that generates the clustering results using several subfunctions.
        /// </summary>
        /// <param name="RawData">Data to be clustered</param>
        /// <param name="DataToClusterMapping">Contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="Centroids">The centroids of the clusters</param>
        /// <param name="NumberOfClusters">The number of clusters</param>
        /// <returns>The results for each cluster</returns>
        public static Cluster[] CreateClusteringResult(double[][] RawData, int[] DataToClusterMapping, double[][] Centroids, int NumberOfClusters)
        {
            Cluster[] cluster;
            int Code;
            string Message = "Function <CreateClusteringResult>: ";
            try
            {
                //Get how many samples are there in each cluster
                int[] clusterSamplesCounter = samplesInClusters(RawData, DataToClusterMapping, NumberOfClusters);

                cluster = new Cluster[NumberOfClusters];

                for (int i = 0; i < NumberOfClusters; i++)
                {
                    cluster[i] = new Cluster();
                    cluster[i].Centroid = Centroids[i];
                    cluster[i].NumberOfSamples = clusterSamplesCounter[i];
                    cluster[i].ClusterData = new double[clusterSamplesCounter[i]][];
                    cluster[i].ClusterDataOriginalIndex = new int[clusterSamplesCounter[i]];

                    
                    //group the samples of the cluster
                    assignSamplesToClusters(RawData, DataToClusterMapping, i, cluster[i]);

                    calculateStatistics(cluster[i]);

                }

                //Use functions to calculate the properties and statistics of each clusters.
                Tuple<int[], double[]> nearestCluster = calculateNearestCluster(Centroids, clusterSamplesCounter);

                for (int i = 0; i < NumberOfClusters; i++)
                {
                    cluster[i].NearestCluster = nearestCluster.Item1[i];
                    cluster[i].DistanceToNearestClusterCentroid = nearestCluster.Item2[i];
                }

                double[][] NearestForeignSampleInNearestClusterArray;
                double[] DistanceToNearestForeignSampleInNearestClusterArray;
                double[][] NearestForeignSampleArray;
                double[] DistanceToNearestForeignSampleArray;
                int[] ClusterOfNearestForeignSampleArray;

                calculateMoreStatistics(RawData, DataToClusterMapping, Centroids, nearestCluster.Item1, out NearestForeignSampleInNearestClusterArray, out DistanceToNearestForeignSampleInNearestClusterArray, out NearestForeignSampleArray, out DistanceToNearestForeignSampleArray, out ClusterOfNearestForeignSampleArray);

                for (int i = 0; i < NumberOfClusters; i++)
                {
                    cluster[i].NearestForeignSampleInNearestCluster = NearestForeignSampleInNearestClusterArray[i];
                    cluster[i].DistanceToNearestForeignSampleInNearestCluster = DistanceToNearestForeignSampleInNearestClusterArray[i];
                    cluster[i].NearestForeignSample = NearestForeignSampleArray[i];
                    cluster[i].DistanceToNearestForeignSample = DistanceToNearestForeignSampleArray[i];
                    cluster[i].ClusterOfNearestForeignSample = ClusterOfNearestForeignSampleArray[i];
                }

                return cluster;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        #endregion

        #region Private Functoins

        /// <summary>
        /// samplesInCLusterNumber is a function that counts the number of samples of each cluster.
        /// </summary>
        /// <param name="RawData">data to be clustered</param>
        /// <param name="DataToClusterMapping">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="NumberOfClusters">the number of clusters</param>
        /// <returns>The number of samples for each cluster</returns>
        private static int[] samplesInClusters(double[][] RawData, int[] DataToClusterMapping, int NumberOfClusters)
        {
            int[] ClusterSamplesCounter;
            int Code;
            string Message = "Function <samplesInClusters>: ";

            try
            {
                ClusterSamplesCounter = new int[NumberOfClusters];
                for (int i = 0; i < DataToClusterMapping.Length; i++)
                {
                    ClusterSamplesCounter[DataToClusterMapping[i]]++;
                }

                return ClusterSamplesCounter;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// assignSamplesToClusters is a function that groups the samples of a cluster.
        /// </summary>
        /// <param name="RawData">data to be clustered</param>
        /// <param name="DataToClusterMapping">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="ClusterIndex">number of the cluster</param>
        /// <param name="Cluster">a cluster object</param>
        private static void assignSamplesToClusters(double[][] RawData, int[] DataToClusterMapping, int ClusterIndex, Cluster cls)
        {
            int Code;
            string Message = "Function <assignSamplesToClusters>: ";
            try
            {
                for (int i = 0, j = 0; i < DataToClusterMapping.Length; i++)
                {
                    if (DataToClusterMapping[i] == ClusterIndex)
                    {
                        cls.ClusterData[j] = RawData[i];
                        cls.ClusterDataOriginalIndex[j] = i;
                        j++;
                    }
                }
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// calculateStatistics is a function that claculates statistics and properties of a cluster. These statistics are independent on other clusters.
        /// </summary>
        /// <param name="Cluster">a cluster object</param>
        private static void calculateStatistics(Cluster cls)
        {
            int Code;
            string Message = "Function <calculateStatistics>: ";
            try
            {
                int NumberOfSamples = cls.ClusterData.Length;
                int NumberOfAttributes = cls.Centroid.Length;
                cls.ClusterDataDistanceToCentroid = new double[NumberOfSamples];
                cls.Mean = new double[NumberOfAttributes];
                cls.StandardDeviation = new double[NumberOfAttributes];
                cls.InClusterMaxDistance = -1;

                //in case of empty cluster
                if (NumberOfSamples == 0)
                {
                    cls.InClusterFarthestSampleIndex = 0;
                    cls.InClusterMaxDistance = 0;
                    cls.InClusterFarthestSample = new double[NumberOfAttributes];

                    for (int j = 0; j < NumberOfAttributes; j++)
                    {
                        cls.Mean[j] = 0;
                        cls.Centroid[j] = 0;
                        cls.InClusterFarthestSample[j] = 0;
                    }
                    cls.NearestCluster = -1;
                }
                else
                {
                    for (int i = 0; i < NumberOfSamples; i++)
                    {
                        //calculate distance for each sample
                        cls.ClusterDataDistanceToCentroid[i] = KMeansAlgorithm.CalculateDistance(cls.ClusterData[i], cls.Centroid);
                        if (cls.ClusterDataDistanceToCentroid[i] > cls.InClusterMaxDistance)
                        {
                            //farthest sample
                            cls.InClusterFarthestSampleIndex = i;
                            cls.InClusterFarthestSample = cls.ClusterData[i];
                            cls.InClusterMaxDistance = cls.ClusterDataDistanceToCentroid[i];
                        }

                        for (int j = 0; j < NumberOfAttributes; j++)
                        {
                            cls.Mean[j] += cls.ClusterData[i][j] / NumberOfSamples;
                        }
                    }

                    double[] ClusterVariance = new double[NumberOfAttributes];

                    for (int i = 0; i < NumberOfSamples; i++)
                    {
                        for (int j = 0; j < NumberOfAttributes; j++)
                        {
                            ClusterVariance[j] += Math.Pow((cls.ClusterData[i][j] - cls.Mean[j]), 2) / NumberOfSamples;
                        }
                    }

                    for (int i = 0; i < NumberOfAttributes; i++)
                    {
                        cls.StandardDeviation[i] = Math.Sqrt(ClusterVariance[i]);
                    }
                }
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// calculateNearestCluster is a function that determines the nearest cluster and calculates the distance between those two clusters.
        /// </summary>
        /// <param name="Centroids">the centroids of the clusters</param>
        /// <param name="SamplesInClusters">number of samples in each cluster</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: contains the number of nearest cluster <br />
        /// - Item 2: contains the distance to the nearest cluster
        /// </returns>
        private static Tuple<int[], double[]> calculateNearestCluster(double[][] Centroids, int[] SamplesInClusters)
        {
            int[] NearestClustersArray = new int[Centroids.Length];
            double[] DistanceToNearestClusterArray = new double[Centroids.Length];
            int Code;
            string Message = "Function <calculateNearestCluster>: ";

            try
            {
                double curDistance;
                for (int i = 0; i < Centroids.Length; i++)
                {
                    //in case of empty cluster
                    if (SamplesInClusters[i] == 0)
                    {
                        NearestClustersArray[i] = -1;
                        DistanceToNearestClusterArray[i] = -1;
                        continue;
                    }

                    DistanceToNearestClusterArray[i] = double.MaxValue;

                    for (int j = 0; j < Centroids.Length; j++)
                    {
                        if (i == j || SamplesInClusters[j] == 0)
                        {
                            continue;
                        }

                        curDistance = KMeansAlgorithm.CalculateDistance(Centroids[i], Centroids[j]);

                        if (curDistance < DistanceToNearestClusterArray[i])
                        {
                            DistanceToNearestClusterArray[i] = curDistance;
                            NearestClustersArray[i] = j;
                        }
                    }
                }

                return Tuple.Create(NearestClustersArray, DistanceToNearestClusterArray);
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }

        }

        /// <summary>
        /// calculateNoreStatistics is a function that claculates statistics of a cluster. These statistics are dependent on other clusters.
        /// </summary>
        /// <param name="RawData">data to be clustered</param>
        /// <param name="DataToClusterMapping">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="Centroids">the centroids of the clusters</param>
        /// <param name="NearestCluster">nearest cluster number</param>
        /// <param name="NearestForeignSampleInNearestCluster">nearest sample belonging of the nearest cluster to this cluster's centroid</param>
        /// <param name="DistanceToNearestForeignSampleInNearestCluster">distance between the nearest sample of the nearest cluster and this cluster's centroid</param>
        /// <param name="NearestForeignSample">nearest sample not belonging to this cluster and this cluster's centroid</param>
        /// <param name="DistanceToNearestForeignSample">distance between the nearest foreign sample and this cluster's centroid</param>
        /// <param name="ClusterOfNearestForeignSample">the cluster to which the nearest foreign sample belongs</param>
        private static void calculateMoreStatistics(double[][] RawData, int[] DataToClusterMapping, double[][] Centroids, int[] NearestCluster, out double[][] NearestForeignSampleInNearestCluster, out double[] DistanceToNearestForeignSampleInNearestCluster, out double[][] NearestForeignSample, out double[] DistanceToNearestForeignSample, out int[] ClusterOfNearestForeignSample)
        {
            int Code;
            string Message = "Function <calculateMoreStatistics>: ";
            try
            {
                NearestForeignSampleInNearestCluster = new double[Centroids.Length][];
                DistanceToNearestForeignSampleInNearestCluster = new double[Centroids.Length];
                NearestForeignSample = new double[Centroids.Length][];
                DistanceToNearestForeignSample = new double[Centroids.Length];
                ClusterOfNearestForeignSample = new int[Centroids.Length];

                for (int i = 0; i < Centroids.Length; i++)
                {
                    //in case of empty cluster
                    if (NearestCluster[i] == -1)
                    {
                        NearestForeignSampleInNearestCluster[i] = null;
                        NearestForeignSample[i] = null;
                        DistanceToNearestForeignSampleInNearestCluster[i] = -1;
                        DistanceToNearestForeignSample[i] = -1;
                        ClusterOfNearestForeignSample[i] = -1;
                    }
                    else
                    {
                        DistanceToNearestForeignSampleInNearestCluster[i] = double.MaxValue;
                        DistanceToNearestForeignSample[i] = double.MaxValue;
                    }
                }

                double curDistance;

                for (int i = 0; i < RawData.Length; i++)
                {
                    for (int j = 0; j < Centroids.Length; j++)
                    {
                        //skip if sample belong to the cluster itself or the cluster is empty
                        if (DataToClusterMapping[i] == j || NearestCluster[j] == -1)
                        {
                            continue;
                        }

                        curDistance = KMeansAlgorithm.CalculateDistance(RawData[i], Centroids[j]);
                        
                        if (curDistance < DistanceToNearestForeignSample[j])
                        {
                            DistanceToNearestForeignSample[j] = curDistance;
                            NearestForeignSample[j] = RawData[i];
                            ClusterOfNearestForeignSample[j] = DataToClusterMapping[i];
                        }

                        if (DataToClusterMapping[i] == NearestCluster[j])
                        {
                            if (curDistance < DistanceToNearestForeignSampleInNearestCluster[j])
                            {
                                DistanceToNearestForeignSampleInNearestCluster[j] = curDistance;
                                NearestForeignSampleInNearestCluster[j] = RawData[i];
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        #endregion
    }
}
