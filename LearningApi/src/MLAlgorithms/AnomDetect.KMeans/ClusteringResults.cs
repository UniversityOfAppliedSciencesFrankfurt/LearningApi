using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AnomalyDetection.Interfaces
{
    /// <summary>
    /// ClusteringResults is a class that contains the results per cluster of a clustering instance with additional statistics.
    /// </summary>
    public class ClusteringResults
    {
        /// <summary>
        /// CreateClusteringResult is a function that generates the clustering results using several subfunctions.
        /// </summary>
        /// <param name="RawData">Data to be clustered</param>
        /// <param name="DataToClusterMapping">Contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="Centroids">The centroids of the clusters</param>
        /// <param name="NumberOfClusters">The number of clusters</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: contains the results for each cluster <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public static Tuple<Cluster[], AnomalyDetectionResponse> CreateClusteringResult(double[][] RawData, int[] DataToClusterMapping, double[][] Centroids, int NumberOfClusters)
        {
            Cluster[] cluster;
            AnomalyDetectionResponse adResponse;
            try
            {
                //Get how many samples are there in each cluster
                Tuple<int[], AnomalyDetectionResponse> sampleInCluster = samplesInClusters(RawData, DataToClusterMapping, NumberOfClusters);

                if (sampleInCluster.Item2.Code != 0)
                {
                    cluster = null;
                    return Tuple.Create(cluster, sampleInCluster.Item2);
                }

                int[] clusterSamplesCounter = sampleInCluster.Item1;

                cluster = new Cluster[NumberOfClusters];

                for (int i = 0; i < NumberOfClusters; i++)
                {
                    cluster[i] = new Cluster();
                    cluster[i].Centroid = Centroids[i];
                    cluster[i].ClusterData = new double[clusterSamplesCounter[i]][];
                    cluster[i].ClusterDataOriginalIndex = new int[clusterSamplesCounter[i]];

                    //
                    //group the samples of the cluster
                    adResponse = AssignSamplesToClusters(RawData, DataToClusterMapping, i, cluster[i]);

                    if (adResponse.Code != 0)
                    {
                        cluster = null;
                        return Tuple.Create(cluster, adResponse);
                    }

                    adResponse = CalculateStatistics(cluster[i]);

                    if (adResponse.Code != 0)
                    {
                        cluster = null;
                        return Tuple.Create(cluster, adResponse);
                    }
                }

                //Use functions to calculate the properties and statistics of each clusters.
                Tuple<int[], double[], AnomalyDetectionResponse> nearestCluster = calculateNearestCluster(Centroids, clusterSamplesCounter);

                if (nearestCluster.Item3.Code != 0)
                {
                    cluster = null;
                    return Tuple.Create(cluster, nearestCluster.Item3);
                }

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

                adResponse = calculateMoreStatistics(RawData, DataToClusterMapping, Centroids, nearestCluster.Item1, out NearestForeignSampleInNearestClusterArray, out DistanceToNearestForeignSampleInNearestClusterArray, out NearestForeignSampleArray, out DistanceToNearestForeignSampleArray, out ClusterOfNearestForeignSampleArray);

                if (adResponse.Code != 0)
                {
                    cluster = null;
                    return Tuple.Create(cluster, adResponse);
                }

                for (int i = 0; i < NumberOfClusters; i++)
                {
                    cluster[i].NearestForeignSampleInNearestCluster = NearestForeignSampleInNearestClusterArray[i];
                    cluster[i].DistanceToNearestForeignSampleInNearestCluster = DistanceToNearestForeignSampleInNearestClusterArray[i];
                    cluster[i].NearestForeignSample = NearestForeignSampleArray[i];
                    cluster[i].DistanceToNearestForeignSample = DistanceToNearestForeignSampleArray[i];
                    cluster[i].ClusterOfNearestForeignSample = ClusterOfNearestForeignSampleArray[i];
                }

                adResponse = new AnomalyDetectionResponse(0, "OK");

                return Tuple.Create(cluster, adResponse);
            }
            catch (Exception Ex)
            {
                cluster = null;
                adResponse = new AnomalyDetectionResponse(400, "Function <CreateClusteringResult>: Unhandled exception:\t" + Ex.ToString());

                return Tuple.Create(cluster, adResponse);
            }
        }

        /// <summary>
        /// SamplesInCLusterNumber is a function that counts the number of samples of each cluster.
        /// </summary>
        /// <param name="RawData">data to be clustered</param>
        /// <param name="DataToClusterMapping">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="NumberOfClusters">the number of clusters</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: contains the number of samples for each cluster <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<int[], AnomalyDetectionResponse> samplesInClusters(double[][] RawData, int[] DataToClusterMapping, int NumberOfClusters)
        {
            AnomalyDetectionResponse ADResponse;
            int[] ClusterSamplesCounter;

            try
            {
                ClusterSamplesCounter = new int[NumberOfClusters];
                for (int i = 0; i < DataToClusterMapping.Length; i++)
                {
                    ClusterSamplesCounter[DataToClusterMapping[i]]++;

                }

                ADResponse = new AnomalyDetectionResponse(0, "OK");

                return Tuple.Create(ClusterSamplesCounter, ADResponse);
            }
            catch (Exception Ex)
            {
                ClusterSamplesCounter = null;
                ADResponse = new AnomalyDetectionResponse(400, "Function <SamplesInClusterNumber>: Unhandled exception:\t" + Ex.ToString());
                return Tuple.Create(ClusterSamplesCounter, ADResponse);
            }
        }

        /// <summary>
        /// AssignSamplesToClusters is a function that groups the samples of a cluster.
        /// </summary>
        /// <param name="RawData">data to be clustered</param>
        /// <param name="DataToClusterMapping">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="ClusterIndex">number of the cluster</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static AnomalyDetectionResponse AssignSamplesToClusters(double[][] RawData, int[] DataToClusterMapping, int ClusterIndex, Cluster cls)
        {
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
                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <AssignSamplesToClusters>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// CalculateStatistics is a function that claculates statistics and properties of a cluster. These statistics are independent on other clusters.
        /// </summary>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static AnomalyDetectionResponse CalculateStatistics(Cluster cls)
        {
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

                    return new AnomalyDetectionResponse(0, "OK");
                }

                Tuple<double, AnomalyDetectionResponse> CDResponse;

                for (int i = 0; i < NumberOfSamples; i++)
                {
                    CDResponse = CalculateDistance(cls.ClusterData[i], cls.Centroid);

                    if (CDResponse.Item2.Code != 0)
                    {
                        return CDResponse.Item2;
                    }

                    //calculate distance for each sample
                    cls.ClusterDataDistanceToCentroid[i] = CDResponse.Item1;
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

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <CalculateStatistics>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// CalculateNearestCluster is a function that determines the nearest cluster and calculates the distance between those two clusters.
        /// </summary>
        /// <param name="Centroids">the centroids of the clusters</param>
        /// <param name="SamplesInClusters">number of samples in each cluster</param>
        /// <returns>Tuple of three Items: <br />
        /// - Item 1: contains the number of nearest cluster <br />
        /// - Item 2: contains the distance to the nearest cluster <br />
        /// - Item 3: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<int[], double[], AnomalyDetectionResponse> calculateNearestCluster(double[][] Centroids, int[] SamplesInClusters)
        {
            AnomalyDetectionResponse ADResponse;
            int[] NearestClustersArray = new int[Centroids.Length];
            double[] DistanceToNearestClusterArray = new double[Centroids.Length];

            try
            {
                Tuple<double, AnomalyDetectionResponse> CDResponse;

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

                        CDResponse = CalculateDistance(Centroids[i], Centroids[j]);

                        if (CDResponse.Item2.Code != 0)
                        {
                            NearestClustersArray = null;
                            DistanceToNearestClusterArray = null;
                            return Tuple.Create(NearestClustersArray, DistanceToNearestClusterArray, CDResponse.Item2);
                        }

                        if (CDResponse.Item1 < DistanceToNearestClusterArray[i])
                        {
                            DistanceToNearestClusterArray[i] = CDResponse.Item1;
                            NearestClustersArray[i] = j;
                        }
                    }
                }

                ADResponse = new AnomalyDetectionResponse(0, "OK");

                return Tuple.Create(NearestClustersArray, DistanceToNearestClusterArray, ADResponse);
            }
            catch (Exception Ex)
            {
                NearestClustersArray = null;
                DistanceToNearestClusterArray = null;
                ADResponse = new AnomalyDetectionResponse(400, "Function <CalculateNearestCluster>: Unhandled exception:\t" + Ex.ToString());
                return Tuple.Create(NearestClustersArray, DistanceToNearestClusterArray, ADResponse);
            }

        }

        /// <summary>
        /// CalculateNoreStatistics is a function that claculates statistics of a cluster. These statistics are dependent on other clusters.
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
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static AnomalyDetectionResponse calculateMoreStatistics(double[][] RawData, int[] DataToClusterMapping, double[][] Centroids, int[] NearestCluster, out double[][] NearestForeignSampleInNearestCluster, out double[] DistanceToNearestForeignSampleInNearestCluster, out double[][] NearestForeignSample, out double[] DistanceToNearestForeignSample, out int[] ClusterOfNearestForeignSample)
        {
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

                Tuple<double, AnomalyDetectionResponse> CDResponse;

                for (int i = 0; i < RawData.Length; i++)
                {
                    for (int j = 0; j < Centroids.Length; j++)
                    {
                        //skip if sample belong to the cluster itself or the cluster is empty
                        if (DataToClusterMapping[i] == j || NearestCluster[j] == -1)
                        {
                            continue;
                        }

                        CDResponse = CalculateDistance(RawData[i], Centroids[j]);
                        if (CDResponse.Item2.Code != 0)
                        {
                            NearestForeignSampleInNearestCluster = null;
                            DistanceToNearestForeignSampleInNearestCluster = null;
                            NearestForeignSample = null;
                            DistanceToNearestForeignSample = null;
                            ClusterOfNearestForeignSample = null;
                            return CDResponse.Item2;
                        }

                        if (CDResponse.Item1 < DistanceToNearestForeignSample[j])
                        {
                            DistanceToNearestForeignSample[j] = CDResponse.Item1;
                            NearestForeignSample[j] = RawData[i];
                            ClusterOfNearestForeignSample[j] = DataToClusterMapping[i];
                        }

                        if (DataToClusterMapping[i] == NearestCluster[j])
                        {
                            if (CDResponse.Item1 < DistanceToNearestForeignSampleInNearestCluster[j])
                            {
                                DistanceToNearestForeignSampleInNearestCluster[j] = CDResponse.Item1;
                                NearestForeignSampleInNearestCluster[j] = RawData[i];
                            }
                        }
                    }
                }
                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                NearestForeignSampleInNearestCluster = null;
                DistanceToNearestForeignSampleInNearestCluster = null;
                NearestForeignSample = null;
                DistanceToNearestForeignSample = null;
                ClusterOfNearestForeignSample = null;
                return new AnomalyDetectionResponse(400, "Function <CalculateMoreStatistics>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// CalculateDistance is a function that claculates the distance between two elements of same size.
        /// </summary>
        /// <param name="FirstElement">first element</param>
        /// <param name="SecondElement">second element</param>
        /// <returns> Tuple of two Items: <br />
        /// - Item 1: distance between the two elements <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        internal static Tuple<double, AnomalyDetectionResponse> CalculateDistance(double[] FirstElement, double[] SecondElement)
        {
            AnomalyDetectionResponse ADResponse;
            try
            {
                if (FirstElement == null || SecondElement == null)
                {
                    ADResponse = new AnomalyDetectionResponse(101, "Function <CalculateDistance>: At least one input is null");
                    return Tuple.Create(-1.0, ADResponse);
                }

                if (FirstElement.Length != SecondElement.Length)
                {
                    ADResponse = new AnomalyDetectionResponse(115, "Function <CalculateDistance>: Inputs have different dimensions");
                    return Tuple.Create(-1.0, ADResponse);
                }
                double SquaredDistance = 0;
                for (int i = 0; i < FirstElement.Length; i++)
                {
                    SquaredDistance += Math.Pow(FirstElement[i] - SecondElement[i], 2);
                }
                ADResponse = new AnomalyDetectionResponse(0, "OK");
                return Tuple.Create(Math.Sqrt(SquaredDistance), ADResponse);
            }
            catch (Exception Ex)
            {
                ADResponse = new AnomalyDetectionResponse(400, "Function <CalculateDistance>: Unhandled exception:\t" + Ex.ToString());
                return Tuple.Create(-1.0, ADResponse);
            }
        }

    }
}
