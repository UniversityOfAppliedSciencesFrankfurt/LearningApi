using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// Beta is a temporary class containing functions which are still work in progress.
    /// </summary>
    public class Beta
    {
        #region Main Beta Functions

        /// <summary>
        /// RecommendedNumberOfClusters is a function that gives a recommended number of clusters for the given samples based on some provided methods.
        /// </summary>
        /// <param name="rawData">The samples to be clustered</param>
        /// <param name="kmeansMaxIterations">Maximum allowed number of Kmeans iteration for clustering</param>
        /// <param name="kmeansAlgorithm">The desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul></param>
        /// <param name="numberOfAttributes">Number of attributes for each sample</param>
        /// <param name="maxNumberOfClusters">Maximum desired number of clusters</param>
        /// <param name="minNumberOfClusters">Minimum desired number of clusters</param>
        /// <param name="method">Integer 0,1,2 or 3 representing the method to be used 
        /// <ul style = "list-style-type:none" >
        /// <li> - Method 0: Radial method in which the farthest sample of each cluster must be closer to the cluster centoid than the nearest foreign sample of the other clusters </li>
        /// <li> - Method 1: Standard Deviation method in which the standard deviation in each cluster must be less than the desired standard deviation </li>
        /// <li> - Method 2: Both. uses radial and standard deviation methods at the same time </li>
        /// <li> - Method 3: Balanced clusters method in which the clusters contain the closest number of samples</li>
        /// </ul>
        /// </param>
        /// <param name="standardDeviation">The desired standard deviation upper limit in each cluster</param>
        /// <param name="recommendedNumbersOfCluster">The variable through which the recommended number of clusters is returned</param>
        /// <param name="centroids">Initial Centroids</param>
        /// <returns>The recommended number of clusters for the given samples based on the specified method.</returns>
        public int RecommendedNumberOfClusters(double[][] rawData, int kmeansMaxIterations, int numberOfAttributes, int maxNumberOfClusters, int minNumberOfClusters, int method, double[] standardDeviation, int kmeansAlgorithm = 1, double[][] centroids = null)
        {
            int recommendedNumbersOfCluster;
            int Code;
            string Message = "Function <RecommendedNumberOfClusters>: ";
            try
            {
                //some checks
                if (maxNumberOfClusters < 2)
                {
                    Code = 104;
                    Message += "Maximum number of clusters must be at least 2";
                    throw new KMeansException(Code, Message);
                }

                int MaxClusters = Math.Min(rawData.Length, maxNumberOfClusters);

                if (minNumberOfClusters < 2)
                {
                    minNumberOfClusters = 2;
                }

                if (method > 3 || method < 0)
                {
                    Code = 122;
                    Message += "Method must be either 0,1,2 or 3";
                    throw new KMeansException(Code, Message);
                }

                if ((method == 1 || method == 2) && standardDeviation == null)
                {
                    Code = 123;
                    Message += "Parameter StdDev is needed";
                    throw new KMeansException(Code, Message);
                }

                if (kmeansMaxIterations < 1)
                {
                    Code = 108;
                    Message += "Unacceptable number of maximum iterations";
                    throw new KMeansException(Code, Message);
                }

                if (rawData == null)
                {
                    Code = 100;
                    Message += "RawData is null";
                    throw new KMeansException(Code, Message);
                }

                if (numberOfAttributes < 1)
                {
                    Code = 107;
                    Message += "Unacceptable number of attributes. Must be at least 1";
                    throw new KMeansException(Code, Message);
                }

                if (kmeansAlgorithm != 2)
                {
                    kmeansAlgorithm = 1;
                }

                //checks that all the samples have same number of attributes
                KMeansAlgorithm.verifyRawDataConsistency(rawData, numberOfAttributes);
                

                double[][] Centroids;
                int IterationReached = -1;
                int[] kMeansResponse;
                Cluster[] cluster;
                bool isRadial, isStandardDeviation;
                double[] balancedError = new double[MaxClusters - minNumberOfClusters + 1];

                for (int i = minNumberOfClusters; i <= MaxClusters; i++)
                {
                    //cluster the data with number of clusters equals to i
                    kMeansResponse = KMeansAlgorithm.runKMeansAlgorithm(rawData, i, numberOfAttributes, kmeansMaxIterations, kmeansAlgorithm, centroids, out Centroids, out IterationReached);
                    
                    cluster = ClusteringResults.CreateClusteringResult(rawData, kMeansResponse, Centroids, i);

                    isRadial = true;

                    isStandardDeviation = true;

                    if (method == 0 || method == 2) 
                    {
                        //radial method check
                        isRadial = radialClustersCheck(cluster);
                    }

                    if (method == 1 || method == 2)
                    {
                        //standard deviation check
                        isStandardDeviation = stdDeviationClustersCheck(cluster, standardDeviation);
                    }

                    if(method == 3)
                    {
                        //start balanced check
                        balancedError[i - minNumberOfClusters] = 0;
                        double[] countSamples = new double[i];
                        double average = 0;
                        for (int c = 0; c < i; c++)
                        {
                            countSamples[c] = cluster[c].ClusterData.Length;
                            average = average + countSamples[c]/i;
                        }
                        for (int c = 0; c < i; c++)
                        {
                            //error calculation
                            balancedError[i-minNumberOfClusters] = balancedError[i-minNumberOfClusters] + Math.Pow(countSamples[c]-average,2)/i;
                        }
                    }
                    else if (isRadial && isStandardDeviation)
                    {
                        recommendedNumbersOfCluster = i;

                        //return new AnomalyDetectionResponse(0, "OK");
                        return recommendedNumbersOfCluster;
                    }
                }

                if (method == 3)
                {
                    // get minimum value (most balanced solution)
                    int minIndex = 0;
                    for (int l = 1; l < balancedError.Length; l++)
                    {
                        if (balancedError[l] < balancedError[minIndex])
                        {
                            minIndex = l;
                        }
                    }

                    recommendedNumbersOfCluster = minIndex + minNumberOfClusters;

                    //return new AnomalyDetectionResponse(0, "OK");
                    return recommendedNumbersOfCluster;
                }

                ///// find a way to throw the response
                recommendedNumbersOfCluster = 0;

                //return new AnomalyDetectionResponse(1, "Could not find a recommended number of clusters based on the desired constraints");
                return recommendedNumbersOfCluster;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }
        
        #endregion

        #region Private Function

        /// <summary>
        /// radialClustersCheck is a function that returns true if the farthest sample of each cluster is closer to the cluster's centoid than the nearest foreign sample of the other clusters.
        /// </summary>
        /// <param name="clusters">Results of a clustering instance</param>
        /// <returns>True if the farthest sample of each cluster is closer to the cluster' centoid than the nearest foreign sample of the other clusters, else false.</returns>
        private static bool radialClustersCheck(Cluster[] clusters)
        {
            int Code;
            string Message = "Function <radialClustersCheck>: ";
            try
            {
                for (int j = 0; j < clusters.Length; j++)
                {
                    if (clusters[j].DistanceToNearestForeignSample < clusters[j].InClusterMaxDistance)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// stdDeviationClustersCheck is a function that returns true if the standard deviation in each cluster is less than the desired standard deviation.
        /// </summary>
        /// <param name="clusters">Results of a clustering instance</param>
        /// <param name="stdDeviation">The desired standard deviation upper limit in each cluster</param> 
        /// <returns>True if the standard deviation in each cluster is less than the desired standard deviation, else false.</returns>
        private static bool stdDeviationClustersCheck(Cluster[] clusters, double[] stdDeviation)
        {
            int Code;
            string Message = "Function <stdDeviationClustersCheck>: ";
            try
            {
                for (int j = 0; j < clusters.Length; j++)
                {
                    for (int k = 0; k < clusters[j].StandardDeviation.Length; k++)
                    {
                        if (clusters[j].StandardDeviation[k] > stdDeviation[k])
                        {
                            return false;
                        }
                    }
                }
                return true;
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
