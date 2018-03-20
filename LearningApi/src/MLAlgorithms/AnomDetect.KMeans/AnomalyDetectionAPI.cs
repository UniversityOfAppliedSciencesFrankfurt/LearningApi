using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AnomalyDetection.Interfaces;

namespace AnomalyDetectionApi
{
    /// <summary>
    /// AnomalyDetectionAPI is a class containing basic information about a clustering instance.
    /// </summary>
    public class AnomalyDetectionAPI : IAnomalyDetectionApi
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        private Instance m_instance;
        private Cluster[] m_cluster;
        public ClusteringSettings clusterSettings { get; internal set; }

        public Cluster[] Clusters { get => m_cluster;  }

        #endregion

        #region Public Function

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[][] GetCentroid()
        {
            return m_instance.Centroids;
        }

        public double[] GetInClusterMaxDistance()
        {
            return m_instance.InClusterMaxDistance;
        }

        /// <summary>
        /// Ctreate Instance of AnomalyDetectionAPI
        /// </summary>
        /// <param name="settings">It should not be null when you call Training function</param>
        public AnomalyDetectionAPI(ClusteringSettings settings = null)
        {
            this.clusterSettings = settings;
        }

        /// <summary>
        /// ImportNewDataForClustering is a function that start a new clustering instance or add to an existing one. It saves the results automatically.
        /// </summary>
        /// <param name="rawData">Data for training</param>
        /// <param name="centroids">Centroid</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "Clustering Complete. K-means stopped at the maximum allowed iteration: " + Maximum_Allowed_Iteration </li>
        /// <li> or </li>
        /// <li> - Code: 0, "Clustering Complete. K-means converged at iteration: " + Iteration_Reached </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse Training(double[][] rawData)
        {
            try
            {

                // does some checks on the passed parameters by the user
                var adResponse = validateParams(rawData, clusterSettings.KmeansAlgorithm, clusterSettings.KmeansMaxIterations, clusterSettings.NumberOfClusters, clusterSettings.NumberOfAttributes);

                if (adResponse.Code != 0)
                {
                    return adResponse;
                }

                Instance instance = new Instance(rawData, clusterSettings.NumberOfClusters);

                double[][] calculatedCentroids;

                int IterationReached = -1;

                Tuple<int[], AnomalyDetectionResponse> kMeansResponse;

                //initiate the clustering process
                kMeansResponse = runKMeansClusteringAlg(rawData, instance.NumberOfClusters, clusterSettings.NumberOfAttributes, clusterSettings.KmeansMaxIterations, clusterSettings.KmeansAlgorithm, clusterSettings.InitialGuess, this.clusterSettings.InitialCentroids, out calculatedCentroids, out IterationReached);

                if (kMeansResponse.Item2.Code != 0)
                {
                    return kMeansResponse.Item2;
                }

                instance.DataToClusterMapping = kMeansResponse.Item1;

                instance.Centroids = calculatedCentroids;

                Tuple<Cluster[], AnomalyDetectionResponse> ccrResponse;

                //create the clusters' result & statistics
                ccrResponse = ClusteringResults.CreateClusteringResult(rawData, instance.DataToClusterMapping, calculatedCentroids, instance.NumberOfClusters);

                if (ccrResponse.Item2.Code != 0)
                {
                    return ccrResponse.Item2;
                }

                m_cluster = ccrResponse.Item1;

                instance.InClusterMaxDistance = new double[instance.NumberOfClusters];

                for (int i = 0; i < instance.NumberOfClusters; i++)
                {
                    instance.InClusterMaxDistance[i] = m_cluster[i].InClusterMaxDistance;
                }

                this.m_instance = instance;

                if (clusterSettings.KmeansMaxIterations <= IterationReached)
                {
                    return new AnomalyDetectionResponse(0, "Clustering Complete. K-means stopped at the maximum allowed iteration: " + clusterSettings.KmeansMaxIterations);
                }
                else
                {
                    return new AnomalyDetectionResponse(0, "Clustering Complete. K-means converged at iteration: " + IterationReached);
                }
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <ImportNewDataForClustering>: Unhnadled exception:\t" + Ex.ToString());
            }

        }

        /// <summary>
        /// To save Instance and Cluster results in json file. Instance will be saved in Instance Result folder and Cluster will be saved in Cluster Result folder.
        /// </summary>
        /// <param name="path">Where it should be saved</param>
        /// <returns></returns>
        public AnomalyDetectionResponse Save(string path)
        {

            JsonSerializer json = new JsonSerializer();

            var adResponse = json.Save(path, this.m_instance);

            if (adResponse.Code != 0)
                return adResponse;

            adResponse = json.Save(path, this.m_cluster);

            if (adResponse.Code != 0)
                return adResponse;

            return adResponse;

        }

        /// <summary>
        /// Detects to which cluster the given sample belongs to. You have to give Instance file path, which is saved in Instance Reulst folder
        /// </summary>
        /// <param name="settings">Contains the desired settings for detecting to which, if any, cluster the sample belongs. Path can be null if you run Training()</param>
        /// <param name="clusterIndex">the cluster number to which the sample belongs (-1 if the sample doesn't belong to any cluster or if an error was encountered).</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "This sample belongs to cluster: " + Cluster_Number </li>
        /// <li> or </li>
        /// <li> - Code: 1, "This sample doesn't belong to any cluster.This is an outlier!" </li>
        /// </ul>       
        /// </returns>
        public AnomalyDetectionResponse CheckSample(CheckingSampleSettings settings, out int clusterIndex)
        {
            try
            {
                Instance instance = null;
                JsonSerializer json = new JsonSerializer();

                //some checks on the passed parameters by the user
                if (settings.Tolerance < 0)
                {
                    clusterIndex = -1;

                    return new AnomalyDetectionResponse(110, "Function <CheckSample>: Unacceptable tolerance value");
                }
                if (settings.Path != null)
                {

                    Tuple<Instance, AnomalyDetectionResponse> jsonResult;

                    //
                    //load the clustering project containing the clusters to one of which, if any, the sample will be assigned to
                    jsonResult = json.ReadJsonObject(settings.Path);

                    if (jsonResult.Item2.Code != 0)
                    {
                        clusterIndex = -1;

                        return jsonResult.Item2;
                    }

                    instance = jsonResult.Item1;
                }
                else
                {
                    instance = this.m_instance;
                }

                //returns error if the new sample has different number of attributes compared to the samples in the desired project
                if (instance.Centroids[0].Length != settings.Sample.Length)
                {
                    clusterIndex = -1;

                    return new AnomalyDetectionResponse(114, "Function <CheckSample>: Mismatch in number of attributes");
                }

                double calculatedDistance;
                double minDistance = double.MaxValue;
                int closestCentroid = -1;

                Tuple<double, AnomalyDetectionResponse> cdResponse;

                //determines to which centroid the sample is closest and the distance
                for (int j = 0; j < instance.NumberOfClusters; j++)
                {
                    cdResponse = calculateDistance(settings.Sample, instance.Centroids[j]);

                    if (cdResponse.Item2.Code != 0)
                    {
                        clusterIndex = -1;

                        return cdResponse.Item2;
                    }

                    calculatedDistance = cdResponse.Item1;

                    if (calculatedDistance < minDistance)
                    {
                        minDistance = calculatedDistance;

                        closestCentroid = j;
                    }
                }

                //decides based on the maximum distance in the cluster & the tolerance whether the sample really belongs to the cluster or not 
                if (minDistance < instance.InClusterMaxDistance[closestCentroid] * (1.0 + settings.Tolerance / 100.0))
                {
                    clusterIndex = closestCentroid;

                    return new AnomalyDetectionResponse(0, "This sample belongs to cluster: " + closestCentroid.ToString());
                }
                else
                {
                    clusterIndex = -1;

                    return new AnomalyDetectionResponse(1, "This sample doesn't belong to any cluster.This is an outlier! ");
                }
            }
            catch (Exception Ex)
            {
                clusterIndex = -1;

                return new AnomalyDetectionResponse(400, "Function <CheckSample>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// Returns the results of an existing Cluster instance 
        /// </summary>
        /// <param name="path">Json formated Cluster instance path, if it is null Training() should be run</param>
        /// <param name="clusters">The variable through which the clustering result are returned</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse GetClusters(string path, out Cluster[] clusters)
        {
            try
            {
                JsonSerializer json = new JsonSerializer();

                //gets the path of the results instead of the instance
                Tuple<Cluster[], AnomalyDetectionResponse> clusterResponse;

                if (path != null)
                {
                    //load the results
                    clusterResponse = json.GetClusters(path);

                    if (clusterResponse.Item2.Code != 0)
                    {
                        clusters = null;

                        return clusterResponse.Item2;
                    }

                    clusters = clusterResponse.Item1;
                }
                else
                {
                    clusters = this.m_cluster;
                }

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                clusters = null;

                return new AnomalyDetectionResponse(400, "Function <GetCluster>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        ///// <summary>
        ///// Loads samples from a previous clustering instance
        ///// </summary>
        ///// <param name="path">Json formated Instance path, which is generally saved in Instance Result folder</param>
        ///// <param name="oldSample">The variable through which the samples are returned</param>
        ///// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        ///// <ul style="list-style-type:none">
        ///// <li> - Code: 0, "OK" </li>
        ///// </ul>
        ///// </returns>
        //public AnomalyDetectionResponse GetPreviousSamples(string path, out double[][] oldSample)
        //{
        //    try
        //    {
        //        JsonSerializer json = new JsonSerializer();
        //        Tuple<Instance, AnomalyDetectionResponse> oldSampleResponse;

        //        //load the clustering instance
        //        oldSampleResponse = json.ReadJsonObject(path);

        //        if (oldSampleResponse.Item2.Code != 0)
        //        {
        //            oldSample = null;

        //            return oldSampleResponse.Item2;
        //        }

        //        oldSample = oldSampleResponse.Item1.RawData;

        //        return new AnomalyDetectionResponse(0, "OK");
        //    }
        //    catch (Exception Ex)
        //    {
        //        oldSample = null;

        //        return new AnomalyDetectionResponse(400, "Function <GetPreviousSamples>: Unhandled exception:\t" + Ex.ToString());
        //    }
        //}

        /// <summary>
        /// RecommendedNumberOfClusters is a function that returns a recommended number of clusters for the given samples.
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
        /// <param name="method">Integer 0,1 or 2 representing the method to be used. 
        /// <ul style = "list-style-type:none" >
        /// <li> - Method 0: Radial method in which the farthest sample of each cluster must be closer to the cluster centoid than the nearest foreign sample of the other clusters </li>
        /// <li> - Method 1: Standard Deviation method in which the standard deviation in each cluster must be less than the desired standard deviation </li>
        /// <li> - Method 2: Both. uses radial and standard deviation methods at the same time </li>
        /// <li> - Method 3: Balanced clusters method in which the clusters contain the closest number of samples</li>
        /// </ul>
        /// </param>
        /// <param name="standardDeviation">The desired standard deviation upper limit in each cluster</param>
        /// <param name="recommendedNumbersOfCluster">The variable through which the recommended number of clusters is returned</param>
        /// <param name="centroids">Centroid</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style = "list-style-type:none" >
        /// <li> - Code: 0, "OK" </li>
        /// <li> or </li>
        /// <li> - Code: 1, "Could not find a recommended number of clusters based on the desired constraints" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse RecommendedNumberOfClusters(double[][] rawData, int kmeansMaxIterations, int numberOfAttributes, int maxNumberOfClusters, int minNumberOfClusters, int method, double[] standardDeviation, out int recommendedNumbersOfCluster, int kmeansAlgorithm = 1, double[][] centroids = null)
        {
            try
            {
                //some checks
                if (maxNumberOfClusters < 2)
                {
                    recommendedNumbersOfCluster = 0;

                    return new AnomalyDetectionResponse(104, "Function <RecommendedNumberOfClusters>: Maximum number of clusters must be at least 2");
                }

                int MaxClusters = Math.Min(rawData.Length, maxNumberOfClusters);

                if (minNumberOfClusters < 2)
                {
                    minNumberOfClusters = 2;
                }

                if (method > 3 || method < 0)
                {
                    recommendedNumbersOfCluster = 0;

                    return new AnomalyDetectionResponse(122, "Function <RecommendedNumberOfClusters>: Method must be either 0,1,2 or 3");
                }

                if ((method == 1 || method == 2) && standardDeviation == null)
                {
                    recommendedNumbersOfCluster = 0;

                    return new AnomalyDetectionResponse(123, "Function <RecommendedNumberOfClusters>: Parameter StdDev is needed");
                }

                if (kmeansMaxIterations < 1)
                {
                    recommendedNumbersOfCluster = 0;

                    return new AnomalyDetectionResponse(108, "Function <RecommendedNumberOfClusters>: Unacceptable number of maximum iterations");
                }

                if (rawData == null)
                {
                    recommendedNumbersOfCluster = 0;

                    return new AnomalyDetectionResponse(100, "Function <RecommendedNumberOfClusters>: RawData is null");
                }

                if (numberOfAttributes < 1)
                {
                    recommendedNumbersOfCluster = 0;

                    return new AnomalyDetectionResponse(107, "Function <RecommendedNumberOfClusters>: Unacceptable number of attributes. Must be at least 1");
                }

                if (kmeansAlgorithm != 2)
                {
                    kmeansAlgorithm = 1;
                }

                //checks that all the samples have same number of attributes
                AnomalyDetectionResponse ADResponse = verifyRawDataConsistency(rawData, numberOfAttributes);

                if (ADResponse.Code != 0)
                {
                    recommendedNumbersOfCluster = 0;

                    return ADResponse;
                }

                double[][] Centroids;
                int IterationReached = -1;
                Tuple<int[], AnomalyDetectionResponse> kMeansResponse;
                Tuple<Cluster[], AnomalyDetectionResponse> clusterResponse;
                Cluster[] cluster;
                bool isRadial, isStandardDeviation;
                Tuple<bool, AnomalyDetectionResponse> boolChecks;
                double[] balancedError = new double[MaxClusters - minNumberOfClusters + 1];

                for (int i = minNumberOfClusters; i <= MaxClusters; i++)
                {
                    //cluster the data with number of clusters equals to i
                    kMeansResponse = runKMeansClusteringAlg(rawData, i, numberOfAttributes, kmeansMaxIterations, kmeansAlgorithm, true, centroids, out Centroids, out IterationReached);

                    if (kMeansResponse.Item2.Code != 0)
                    {
                        recommendedNumbersOfCluster = 0;

                        return kMeansResponse.Item2;
                    }

                    clusterResponse = ClusteringResults.CreateClusteringResult(rawData, kMeansResponse.Item1, Centroids, i);

                    if (clusterResponse.Item2.Code != 0)
                    {
                        recommendedNumbersOfCluster = 0;

                        return clusterResponse.Item2;
                    }

                    cluster = clusterResponse.Item1;

                    isRadial = true;

                    isStandardDeviation = true;

                    if (method == 0 || method == 2) 
                    {
                        //radial method check
                        boolChecks = radialClustersCheck(cluster);

                        if (boolChecks.Item2.Code != 0)
                        {
                            recommendedNumbersOfCluster = 0;

                            return boolChecks.Item2;
                        }

                        isRadial = boolChecks.Item1;
                    }

                    if (method == 1 || method == 2)
                    {
                        //standard deviation check
                        boolChecks = stdDeviationClustersCheck(cluster, standardDeviation);

                        if (boolChecks.Item2.Code != 0)
                        {
                            recommendedNumbersOfCluster = 0;

                            return boolChecks.Item2;
                        }

                        isStandardDeviation = boolChecks.Item1;
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

                        return new AnomalyDetectionResponse(0, "OK");
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

                    return new AnomalyDetectionResponse(0, "OK");
                }

                recommendedNumbersOfCluster = 0;

                return new AnomalyDetectionResponse(1, "Could not find a recommended number of clusters based on the desired constraints");
            }
            catch (Exception Ex)
            {
                recommendedNumbersOfCluster = 0;

                return new AnomalyDetectionResponse(400, "Function <RecommendedNumberOfClusters>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        #endregion

        #region Private Function

        /// <summary>
        /// KMeansClusteringAlg is a function that clusters the given samples based on the K-means concept.
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="numClusters">desired number of clusters</param>
        /// <param name="numAttributes">number of attributes for each sample</param>
        /// <param name="maxCount">maximum allowed number of Kmeans iteration for clustering</param>
        /// <param name="kMeanAlgorithm">the desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul></param>
        /// <param name="initialGuess">a bool, if true Kmeans clustering start with an initial guess for the centroids else it will start with a random assignment</param>
        /// <param name="centroids">the variable through which the resulting centroids are returned</param>
        /// <param name="IterationReached">the variable through which the iteration reached is returned</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: contains the assigned cluster number for each sample of the RawData <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return: 
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<int[], AnomalyDetectionResponse> runKMeansClusteringAlg(double[][] rawData, int numClusters, int numAttributes, int maxCount, int kMeanAlgorithm, bool initialGuess, double[][] initialCentroids, out double[][] centroids, out int IterationReached)
        {
            int[] clustering;
            try
            {
                bool changed = true;
                int cnt = 0;
                Tuple<int[], AnomalyDetectionResponse> initClusterResponse;
                Tuple<double[][], AnomalyDetectionResponse> allocatedTupple;
                Tuple<bool, AnomalyDetectionResponse> assignResponse;
                AnomalyDetectionResponse adResponse;
                int numTuples = rawData.Length;

                clustering = new int[rawData.Length];

                // just makes things a bit cleaner
                allocatedTupple = allocateCentroidTupple(numClusters, numAttributes, initialCentroids);

                if (allocatedTupple.Item2.Code != 0)
                {
                    centroids = null;

                    IterationReached = -1;

                    clustering = null;

                    return Tuple.Create(clustering, allocatedTupple.Item2);
                }

                double[][] means = allocatedTupple.Item1;

                centroids = allocatedTupple.Item1;

                if (initialGuess)
                {
                    adResponse = calculateInitialMeans(rawData, numClusters, out means);

                    if (adResponse.Code != 0)
                    {
                        centroids = null;

                        IterationReached = -1;

                        clustering = null;

                        return Tuple.Create(clustering, adResponse);
                    }

                    if (kMeanAlgorithm == 1)
                    {
                        double[] currDist = new double[numClusters];
                        double[] minDist = new double[numClusters];

                        for (int i = 0; i < numClusters; i++)
                        {
                            minDist[i] = double.MaxValue;
                        }

                        Tuple<double, AnomalyDetectionResponse> CDResponse;

                        for (int i = 0; i < rawData.Length; ++i)
                        {
                            for (int j = 0; j < numClusters; j++)
                            {
                                CDResponse = calculateDistance(rawData[i], means[j]);

                                if (CDResponse.Item2.Code != 0)
                                {
                                    centroids = null;

                                    IterationReached = -1;

                                    clustering = null;

                                    return Tuple.Create(clustering, CDResponse.Item2);
                                }

                                currDist[j] = CDResponse.Item1;

                                if (currDist[j] < minDist[j])
                                {
                                    minDist[j] = currDist[j];

                                    centroids[j] = rawData[i];
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < means.Length; i++)
                        {
                            for (int j = 0; j < means[0].Length; j++)
                            {
                                if (Double.IsNaN(means[i][j]))
                                {
                                    centroids[i][j] = 0;
                                }
                                else
                                {
                                    centroids[i][j] = means[i][j];
                                }
                            }
                        }
                    }

                }
                else
                {
                    // 0 is a seed for random
                    initClusterResponse = InitClustering(numTuples, numClusters, 0);

                    if (initClusterResponse.Item2.Code != 0)
                    {
                        centroids = null;

                        IterationReached = -1;

                        clustering = null;

                        return Tuple.Create(clustering, initClusterResponse.Item2);
                    }
                    clustering = initClusterResponse.Item1;

                    // could call this instead inside UpdateCentroids
                    adResponse = updateMeans(rawData, clustering, means);

                    if (adResponse.Code != 0)
                    {
                        centroids = null;

                        IterationReached = -1;

                        clustering = null;

                        return Tuple.Create(clustering, adResponse);
                    }

                    if (kMeanAlgorithm == 1)
                    {
                        adResponse = updateCentroids(rawData, clustering, means, centroids);

                        if (adResponse.Code != 0)
                        {
                            centroids = null;

                            IterationReached = -1;

                            clustering = null;

                            return Tuple.Create(clustering, adResponse);
                        }
                    }
                    else
                    {
                        //
                        // Sets centroids to mean value.
                        for (int i = 0; i < means.Length; i++)
                        {
                            for (int j = 0; j < means[0].Length; j++)
                            {
                                if (Double.IsNaN(means[i][j]))
                                {
                                    centroids[i][j] = 0;
                                }
                                else
                                {
                                    centroids[i][j] = means[i][j];
                                }
                            }
                        }
                    }
                }


                while (changed == true && cnt < maxCount)
                {
                    ++cnt;

                    // use centroids to update cluster assignment
                    assignResponse = assign(rawData, clustering, centroids);

                    if (assignResponse.Item2.Code != 0)
                    {
                        centroids = null;

                        IterationReached = -1;

                        clustering = null;

                        return Tuple.Create(clustering, assignResponse.Item2);
                    }

                    changed = assignResponse.Item1;

                    // use new clustering to update cluster means
                    adResponse = updateMeans(rawData, clustering, means);

                    if (adResponse.Code != 0)
                    {
                        centroids = null;

                        IterationReached = -1;

                        clustering = null;

                        return Tuple.Create(clustering, adResponse);
                    }

                    // use new means to update centroids
                    if (kMeanAlgorithm == 1)
                    {
                        adResponse = updateCentroids(rawData, clustering, means, centroids);

                        if (adResponse.Code != 0)
                        {
                            centroids = null;

                            IterationReached = -1;

                            clustering = null;

                            return Tuple.Create(clustering, adResponse);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < means.Length; i++)
                        {
                            for (int j = 0; j < means[0].Length; j++)
                            {
                                if (Double.IsNaN(means[i][j]))
                                {
                                    centroids[i][j] = 0;
                                }
                                else
                                {
                                    centroids[i][j] = means[i][j];
                                }
                            }
                        }
                    }
                }

                IterationReached = cnt;

                return Tuple.Create(clustering, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                centroids = null;

                IterationReached = -1;

                clustering = null;

                return Tuple.Create(clustering, new AnomalyDetectionResponse(400, "Fuction <KMeansClusteringAlg>: Unhandled exception:\t" + Ex.ToString()));
            }
        }

        /// <summary> 
        /// InitClustering is a function that assigns a sample to each cluster and then randomly assigns the remaining samples on all clusters.
        /// </summary>
        /// <param name="numTuples">number of samples</param>
        /// <param name="numClusters">number of clusters</param>
        /// <param name="randomSeed">random seed for randomly assigning the samples to the clusters</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: contains the assigned cluster number for each sample of the RawData <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<int[], AnomalyDetectionResponse> InitClustering(int numTuples, int numClusters, int randomSeed)
        {
            int[] clustering;
            AnomalyDetectionResponse ADResponse;
            try
            {
                if (numClusters < 2)
                {
                    clustering = null;
                    ADResponse = new AnomalyDetectionResponse(106, "Function <InitClustering>: Unacceptable number of clusters. Must be at least 2");

                    return Tuple.Create(clustering, ADResponse);
                }
                if (numTuples < numClusters)
                {
                    clustering = null;
                    ADResponse = new AnomalyDetectionResponse(105, "Function <InitClustering>: Unacceptable number of clusters. Clusters more than samples");

                    return Tuple.Create(clustering, ADResponse);
                }

                // assign each tuple to a random cluster, making sure that there's at least
                // one tuple assigned to every cluster
                Random random = new Random(randomSeed);
                clustering = new int[numTuples];

                // assign first numClusters tuples to clusters 0..k-1
                for (int i = 0; i < numClusters; ++i)
                    clustering[i] = i;

                // assign rest randomly
                for (int i = numClusters; i < clustering.Length; ++i)
                    clustering[i] = random.Next(0, numClusters);

                ADResponse = new AnomalyDetectionResponse(0, "OK");

                return Tuple.Create(clustering, ADResponse);
            }
            catch (Exception Ex)
            {
                clustering = null;
                ADResponse = new AnomalyDetectionResponse(400, "Function <InitClustering>: Unhandled exception:\t" + Ex.ToString());

                return Tuple.Create(clustering, ADResponse);
            }
        }

        /// <summary>
        /// Allocate is a function that creates a double[][] with the specified size (number of clusters x number of attributes).
        /// </summary>
        /// <param name="numClusters">number of clusters</param>
        /// <param name="numAttributes">number of attributes</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: the allocated double[][] <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<double[][], AnomalyDetectionResponse> allocateCentroidTupple(int numClusters, int numAttributes, double[][] initialCentroids)
        {
            double[][] centroids;
            AnomalyDetectionResponse res;

            try
            {
                // helper allocater for means[][] and centroids[][]
                if (numClusters < 2)
                {
                    centroids = null;

                    res = new AnomalyDetectionResponse(106, "Function <Allocate>: Unacceptable number of clusters. Must be at least 2");

                    return Tuple.Create(centroids, res);
                }
                if (numAttributes < 1)
                {
                    centroids = null;

                    res = new AnomalyDetectionResponse(107, "Function <Allocate>: Unacceptable number of attributes. Must be at least 1");

                    return Tuple.Create(centroids, res);
                }

                if (initialCentroids == null)
                {
                    centroids = new double[numClusters][];

                    for (int k = 0; k < numClusters; ++k)
                        centroids[k] = new double[numAttributes];
                }
                else
                    centroids = initialCentroids;

                res = new AnomalyDetectionResponse(0, "OK");

                return Tuple.Create(centroids, res);
            }
            catch (Exception Ex)
            {
                centroids = null;

                res = new AnomalyDetectionResponse(400, "Function <Allocate>: Unhandled excepttion:\t" + Ex.ToString());

                return Tuple.Create(centroids, res);
            }
        }

        /// <summary>
        /// Number of processed data samples in model across all minibatches.
        /// </summary>
        public double m_ProcessedDataSamples { get; set; }

        /// <summary>
        /// UpdateMeans is a function that calculates the new mean of each cluster.
        /// </summary>
        /// <param name="previousMeanValue">The mean value of the previous minibatch.</param>
        /// <param name="previousSampleCount">The number of samples in previous minibatch.</param>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="clustering">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="means">mean of each cluster (Updated in the function)</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        internal static AnomalyDetectionResponse updateMeans(double[][] rawData, int[] clustering, double[][] means, long previousSampleCount = 0, double[] previousMeanValues = null)
        {
            try
            {
                if (rawData == null || rawData.Length < 1)
                {
                    return new AnomalyDetectionResponse(102, "Function <UpdateMeans>: RawData is empty");
                }

                if (means == null || means.Length < 1)
                {
                    return new AnomalyDetectionResponse(103, "Function <UpdateMeans>: Means is empty");
                }

                // assumes means[][] exists. consider making means[][] a ref parameter
                int numClusters = means.Length;

                //
                // Zero-out means[][]
                for (int k = 0; k < means.Length; ++k)
                {
                    for (int j = 0; j < means[0].Length; ++j)
                        means[k][j] = 0.0;
                }


                // Make an array to hold cluster counts
                int[] clusterCounts = new int[numClusters];

                //
                // walk through each tuple, accumulate sum for each attribute, update cluster count
                for (int i = 0; i < rawData.Length; ++i)
                {
                    int cluster = clustering[i];

                    // Increment number of samples inside of this cluster.
                    ++clusterCounts[cluster];

                    // Here we build a sum for minibatch.
                    for (int j = 0; j < rawData[i].Length; ++j)
                        means[cluster][j] += rawData[i][j];
                }

                //
                // Divide each attribute sum by cluster count to get average (mean)
                for (int k = 0; k < means.Length; ++k)
                {
                    if (clusterCounts[k] != 0)
                    {
                        for (int j = 0; j < means[k].Length; ++j)
                        {
                            means[k][j] /= clusterCounts[k];
                            //
                            // This code recalculate sum by adding a mean from previous minibatch.
                            if (previousSampleCount != 0 && previousMeanValues != null)
                            {
                                double f = (double)1 / (rawData.Length + previousSampleCount);
                                means[k][j] = f * (previousSampleCount * previousMeanValues[k] + rawData.Length * means[k][j]);
                            }
                        }
                    }
                }

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <UpdateMeans>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// UpdateCentroids is a function that assigns the nearest sample to each mean as the centroid of a cluster.
        /// </summary>
        /// <param name="rawData">The samples to be clustered</param>
        /// <param name="clustering">Contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="means">Mean of each cluster</param>
        /// <param name="centroids">Centroid of each cluster (Updated in the function)</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static AnomalyDetectionResponse updateCentroids(double[][] rawData, int[] clustering, double[][] means, double[][] centroids)
        {
            try
            {
                Tuple<double[], AnomalyDetectionResponse> ccResponse;

                // updates all centroids by calling helper that updates one centroid
                for (int k = 0; k < centroids.Length; ++k)
                {
                    ccResponse = computeCentroid(rawData, clustering, k, means);

                    if (ccResponse.Item2.Code != 0)
                    {
                        return ccResponse.Item2;
                    }

                    double[] centroid = ccResponse.Item1;

                    centroids[k] = centroid;
                }

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception EX)
            {
                return new AnomalyDetectionResponse(400, "Function<UpdateCentroids>: Unhandled exception:\t" + EX.ToString());
            }
        }

        /// <summary>
        /// ComputeCentroid is a function that assigns the nearest sample to the mean as the centroid of a cluster.
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="clustering">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="cluster">number of the cluster</param>
        /// <param name="means">mean of each cluster</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: the centroid <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns> <br />
        private static Tuple<double[], AnomalyDetectionResponse> computeCentroid(double[][] rawData, int[] clustering, int cluster, double[][] means)
        {
            double[] centroid;
            try
            {
                // the centroid is the actual tuple values that are closest to the cluster mean
                int numAttributes = means[0].Length;

                centroid = new double[numAttributes];

                double minDist = double.MaxValue;
                Tuple<double, AnomalyDetectionResponse> cdResponse;

                for (int i = 0; i < rawData.Length; ++i) // walk thru each data tuple
                {
                    int c = clustering[i];  // if current tuple isn't in the cluster we're computing for, continue on

                    if (c != cluster)
                        continue;

                    cdResponse = calculateDistance(rawData[i], means[cluster]);  // call helper

                    if (cdResponse.Item2.Code != 0)
                    {
                        centroid = null;

                        return Tuple.Create(centroid, cdResponse.Item2);
                    }

                    double currDist = cdResponse.Item1;

                    if (currDist < minDist)
                    {
                        minDist = currDist;

                        for (int j = 0; j < centroid.Length; ++j)
                            centroid[j] = rawData[i][j];
                    }
                }

                return Tuple.Create(centroid, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                centroid = null;

                return Tuple.Create(centroid, new AnomalyDetectionResponse(400, "Function<ComputeCentroid>: Unhandled exception:\t" + Ex.ToString()));
            }

        }

        /// <summary>
        /// Assign is a function that assigns each sample to the nearest clusters' centroids. If the new assignment is the same as the older one, it returns true else it will return false.
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="clustering">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="centroids">centroid of each cluster</param>
        /// <returns>Tuple of two Items: <br />
        /// - Item 1: true if new assignment is the same as the old one, else false. <br />
        /// - Item 2: a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<bool, AnomalyDetectionResponse> assign(double[][] rawData, int[] clustering, double[][] centroids)
        {
            try
            {
                // assign each tuple to best cluster (closest to cluster centroid)
                // return true if any new cluster assignment is different from old/curr cluster
                // does not prevent a state where a cluster has no tuples assigned. see article for details
                int numClusters = centroids.Length;
                bool changed = false;
                int changedClusters = 0;
                Tuple<double, AnomalyDetectionResponse> cdResponse;
                Tuple<int, AnomalyDetectionResponse> miResponse;

                double[] distances = new double[numClusters]; // distance from current tuple to each cluster mean

                for (int i = 0; i < rawData.Length; ++i)      // walk thru each tuple
                {
                    for (int k = 0; k < numClusters; ++k)       // compute distances to all centroids
                    {
                        cdResponse = calculateDistance(rawData[i], centroids[k]);

                        if (cdResponse.Item2.Code != 0)
                        {
                            return Tuple.Create(false, cdResponse.Item2);
                        }

                        distances[k] = cdResponse.Item1;
                    }
                    // distances[k] = Distance(rawData[i], centroids[k]);

                    miResponse = minIndex(distances);  // find the index == custerID of closest 

                    if (miResponse.Item2.Code != 0)
                    {
                        return Tuple.Create(false, miResponse.Item2);
                    }

                    int newCluster = miResponse.Item1;

                    if (newCluster != clustering[i]) // different cluster assignment?
                    {
                        changed = true;

                        clustering[i] = newCluster;

                        changedClusters++;
                    } // else no change              
                }

                //Console.WriteLine("Changed clusters {0}", changedClusters);
                return Tuple.Create(changed, new AnomalyDetectionResponse(0, "OK")); // was there any change in clustering?
            }
            catch (Exception Ex)
            {
                return Tuple.Create(false, new AnomalyDetectionResponse(400, "Function<Assign>: Unhandled exception:\t" + Ex.ToString()));
            }
        } 


        /// <summary>
        /// MinIndex is a function that returns the index of the smallest distance between a set of distances.
        /// </summary>
        /// <param name="distances">Distance between each sample and the centroid</param>
        /// <returns>
        /// - Item 1: Index of the smallest distance <br />
        /// - Item 2: A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<int, AnomalyDetectionResponse> minIndex(double[] distances)
        {
            try
            {
                // index of smallest value in distances[]
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
                return Tuple.Create(indexOfMin, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                return Tuple.Create(0, new AnomalyDetectionResponse(400, "Function <MinIndex>: Unhandled exception:\t" + Ex.ToString()));
            }
        }

        /// <summary>
        /// GetInitialGuess returns initial guess for the means.
        /// </summary>
        /// <param name="rawData">The samples to be clustered</param>
        /// <param name="numberOfClusters">Number of clusters</param>
        /// <param name="initialMeans">The initial guess for the means</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul></returns>
        private static AnomalyDetectionResponse calculateInitialMeans(double[][] rawData, int numberOfClusters, out double[][] initialMeans)
        {
            try
            {
                double[] MinValues = new double[rawData[0].Length];
                double[] MaxValues = new double[rawData[0].Length];

                initialMeans = new double[numberOfClusters][];

                // We peek here same sample vector.
                int numOfAttrs = rawData[0].Length;

                //
                // Allocate space for initial means.
                for (int i = 0; i < numberOfClusters; i++)
                {
                    initialMeans[i] = new double[numOfAttrs];
                }

                for (int j = 0; j < numOfAttrs; j++)
                {
                    MinValues[j] = rawData[0][j];
                    MaxValues[j] = rawData[0][j];
                }

                // 
                // Calculate min and max of all samples.
                for (int i = 1; i < rawData.Length; i++)
                {
                    for (int j = 0; j < numOfAttrs; j++)
                    {
                        if (rawData[i][j] > MaxValues[j])
                        {
                            MaxValues[j] = rawData[i][j];
                        }
                        if (rawData[i][j] < MinValues[j])
                        {
                            MinValues[j] = rawData[i][j];
                        }
                    }
                }

                //
                // Take some reasonable values as start values from min+(min-max)/numofclusters to min+(min-max)*[2*numdclusters+1/numofclusters*2)
                for (int i = 0; i < numberOfClusters; i++)
                {
                    for (int j = 0; j < numOfAttrs; j++)
                    {
                        initialMeans[i][j] = MinValues[j] + ((MaxValues[j] - MinValues[j]) * (i * 2 + 1)) / (numberOfClusters * 2);
                    }
                }

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                initialMeans = null;

                return new AnomalyDetectionResponse(400, "Function<CentroidsInitialGuess>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// PreproccessingOfParameters is a function that does some checks on the passed parameters by the user. Some errors in the paths can be corrected.
        /// </summary>
        /// <param name="rawData">The samples to be clustered</param>
        /// <param name="kmeansAlgorithm">The desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul></param>
        /// <param name="kmeansMaxIterations">Maximum allowed number of Kmeans iteration for clustering</param>
        /// <param name="numberOfClusters">Desired number of clusters</param>
        /// <param name="numberOfAttributes">Number of attributes for each sample</param>
        /// <param name="saveSettings">Settings to save the clustering instance</param>
        /// <param name="loadSettings">Settings to load the clustering instance. Should be null in case of not loading</param>
        /// <param name="checkedSaveSettings">The object through which the error-less save settings are returned</param>
        /// <param name="checkedLoadSettings">The object through which the error-less load settings are returned</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static AnomalyDetectionResponse validateParams(double[][] rawData, int kmeansAlgorithm, int kmeansMaxIterations, int numberOfClusters, int numberOfAttributes)
        {
            try
            {
                if (kmeansMaxIterations < 1)
                {

                    return new AnomalyDetectionResponse(108, "Function <PreproccessingOfParameters>: Unacceptable number of maximum iterations");
                }

                if (numberOfClusters < 2)
                {

                    return new AnomalyDetectionResponse(106, "Function <PreproccessingOfParameters>: Unacceptable number of clusters. Must be at least 2");
                }

                if (rawData != null)
                {
                    if (numberOfClusters > rawData.Length)
                    {

                        return new AnomalyDetectionResponse(105, "Function <PreproccessingOfParameters>: Unacceptable number of clusters. Clusters more than samples");
                    }
                }
                else
                {

                    return new AnomalyDetectionResponse(100, "Function <PreproccessingOfParameters>: RawData is null");
                }

                if (numberOfAttributes < 1)
                {

                    return new AnomalyDetectionResponse(107, "Function <PreproccessingOfParameters>: Unacceptable number of attributes. Must be at least 1");
                }

                if (kmeansAlgorithm < 1 || kmeansAlgorithm > 2)
                {
                    return new AnomalyDetectionResponse(124, "Function <PreproccessingOfParameters>: Unacceptable input for K-means Algorithm");
                }

                var adResponse = verifyRawDataConsistency(rawData, numberOfAttributes);

                if (adResponse.Code != 0)
                {
                    return adResponse;
                }
                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <PreproccessingOfParameters>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// VerifyRawDataConsistency is a function that checks that all the samples have same given number of attributes.
        /// </summary>
        /// <param name="rawData">The samples to be clustered</param>
        /// <param name="numberOfAttributes">Number of attributes for each sample</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static AnomalyDetectionResponse verifyRawDataConsistency(double[][] rawData, int numberOfAttributes)
        {
            try
            {
                if (rawData == null)
                {
                    return new AnomalyDetectionResponse(100, "Function <VerifyRawDataConsistency>: RawData is null");
                }

                if (rawData.Length < 1)
                {
                    return new AnomalyDetectionResponse(102, "Function <VerifyRawDataConsistency>: RawData is empty");
                }

                int DataLength = rawData.Length;

                for (int i = 0; i < DataLength; i++)
                {
                    if (rawData[i] == null || rawData[i].Length != numberOfAttributes)
                    {
                        return new AnomalyDetectionResponse(111, "Function <VerifyRawDataConsistency>: Data sample and number of attributes are inconsistent. First encountered inconsistency in data sample: " + i + ".");
                    }
                }

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <VerifyRawDataConsistency>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// PrivateCheckSamples is a function that remove the outliers from the given samples and returns the accepted samples only.
        /// </summary>
        /// <param name="rawData">The samples to be clustered</param>
        /// <param name="centroids">Centroid of each cluster</param>
        /// <param name="inClusterMaxDistance">Distance between the centroid and the farthest sample of each cluster</param>
        /// <returns>
        /// - Item 1: The accepted samples (outliers removed) <br />
        /// - Item 2: A  code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<double[][], AnomalyDetectionResponse> privateCheckSamples(double[][] rawData, double[][] centroids, double[] inClusterMaxDistance)
        {
            double[][] acceptedSamples;
            AnomalyDetectionResponse adResponse;

            try
            {
                int acceptedSamplesCount = 0;
                int numberOfSamples = rawData.Length;
                int numberOfClusters = centroids.Length;
                double minDistance, calculatedDistance;
                int closestCentroid;
                Tuple<double, AnomalyDetectionResponse> cdResponse;

                //allocate space for storing accepted samples
                double[][] Temp = new double[numberOfSamples][];

                for (int i = 0; i < numberOfSamples; i++)
                {
                    minDistance = double.MaxValue;

                    closestCentroid = -1;

                    //check which centroid is closer to the sample
                    for (int j = 0; j < numberOfClusters; j++)
                    {
                        cdResponse = calculateDistance(rawData[i], centroids[j]);

                        if (cdResponse.Item2.Code != 0)
                        {
                            acceptedSamples = null;

                            return Tuple.Create(acceptedSamples, cdResponse.Item2);
                        }

                        calculatedDistance = cdResponse.Item1;

                        if (calculatedDistance < minDistance)
                        {
                            minDistance = calculatedDistance;

                            closestCentroid = j;
                        }
                    }

                    //accept sample if it is closer than  the farthest sample to the centroid else ignore it
                    if (minDistance < inClusterMaxDistance[closestCentroid])
                    {
                        Temp[acceptedSamplesCount] = rawData[i];

                        acceptedSamplesCount++;
                    }
                }

                adResponse = new AnomalyDetectionResponse(0, "OK");

                if (acceptedSamplesCount < numberOfSamples)
                {
                    //remove empty rows from Temp
                    acceptedSamples = new double[acceptedSamplesCount][];

                    for (int i = 0; i < acceptedSamplesCount; i++)
                    {
                        acceptedSamples[i] = Temp[i];
                    }

                    return Tuple.Create(acceptedSamples, adResponse);
                }

                return Tuple.Create(Temp, adResponse);
            }
            catch (Exception Ex)
            {
                acceptedSamples = null;

                adResponse = new AnomalyDetectionResponse(400, "Function <PrivateCheckSamples>: Unhandled exception:\t" + Ex.ToString());

                return Tuple.Create(acceptedSamples, adResponse);
            }
        }

        /// <summary>
        /// CalculateDistance is a function that claculates the distance between two elements of same size.
        /// </summary>
        /// <param name="firstElement">First element</param>
        /// <param name="secondElement">Second element</param>
        /// <returns>
        /// - Item 1: Distance between the two elements <br />
        /// - Item 2: A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        internal static Tuple<double, AnomalyDetectionResponse> calculateDistance(double[] firstElement, double[] secondElement)
        {
            AnomalyDetectionResponse adResponse;

            try
            {
                if (firstElement == null || secondElement == null)
                {
                    adResponse = new AnomalyDetectionResponse(101, "Function <CalculateDistance>: At least one input is null");

                    return Tuple.Create(-1.0, adResponse);
                }

                if (firstElement.Length != secondElement.Length)
                {
                    adResponse = new AnomalyDetectionResponse(115, "Function <CalculateDistance>: Inputs have different dimensions");

                    return Tuple.Create(-1.0, adResponse);
                }

                double SquaredDistance = 0;

                for (int i = 0; i < firstElement.Length; i++)
                {
                    SquaredDistance += Math.Pow(firstElement[i] - secondElement[i], 2);
                }

                adResponse = new AnomalyDetectionResponse(0, "OK");

                return Tuple.Create(Math.Sqrt(SquaredDistance), adResponse);
            }
            catch (Exception Ex)
            {
                adResponse = new AnomalyDetectionResponse(400, "Function <CalculateDistance>: Unhandled exception:\t" + Ex.ToString());

                return Tuple.Create(-1.0, adResponse);
            }
        }

        /// <summary>
        /// RadialClustersCheck is a function that returns true if the farthest sample of each cluster is closer to the cluster centoid than the nearest foreign sample of the other clusters.
        /// </summary>
        /// <param name="clusters">Results of a clustering instance</param>
        /// <returns>
        /// - Item 1: True if the farthest sample of each cluster is closer to the cluster centoid than the nearest foreign sample of the other clusters, else false <br />
        /// - Item 2: A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<bool, AnomalyDetectionResponse> radialClustersCheck(Cluster[] clusters)
        {
            try
            {
                for (int j = 0; j < clusters.Length; j++)
                {
                    if (clusters[j].DistanceToNearestForeignSample < clusters[j].InClusterMaxDistance)
                    {
                        return Tuple.Create(false, new AnomalyDetectionResponse(0, "OK"));
                    }
                }
                return Tuple.Create(true, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                return Tuple.Create(false, new AnomalyDetectionResponse(400, "Function <RadialClustersCheck>: Unhandled exception:\t" + Ex.ToString()));
            }
        }

        /// <summary>
        /// StdDeviationClustersCheck is a function that returns true if the standard deviation in each cluster is less than the desired standard deviation
        /// </summary>
        /// <param name="clusters">Results of a clustering instance</param>
        /// <param name="stdDeviation">The desired standard deviation upper limit in each cluster</param> 
        /// <returns>
        /// - Item 1: True if the standard deviation in each cluster is less than the desired standard deviation, else false <br />
        /// - Item 2: A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        private static Tuple<bool, AnomalyDetectionResponse> stdDeviationClustersCheck(Cluster[] clusters, double[] stdDeviation)
        {
            try
            {
                for (int j = 0; j < clusters.Length; j++)
                {
                    for (int k = 0; k < clusters[j].StandardDeviation.Length; k++)
                    {
                        if (clusters[j].StandardDeviation[k] > stdDeviation[k])
                        {
                            return Tuple.Create(false, new AnomalyDetectionResponse(0, "OK"));
                        }
                    }
                }
                return Tuple.Create(true, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                return Tuple.Create(false, new AnomalyDetectionResponse(400, "Function <StdDeviationClustersCheck>: Unhandled exception:\t" + Ex.ToString()));
            }
        }

        /// <summary>
        /// SelectInterfaceType is a function that creates the needed save/load interface desired by the user to save or load.
        /// </summary>
        /// <param name="saveSettings">Settings to save or load</param>
        /// <param name="interface">The created interface to save or load</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style = "list-style-type:none" >
        /// <li> - Code: 0, "OK" </li>
        /// <li> or </li>
        /// <li> - Code: 1, "Warning: Null SaveLoadSettingss Object" </li>
        /// </ul></returns>
        private static AnomalyDetectionResponse selectInterfaceType(SaveLoadSettings saveSettings, out IJsonSerializer @interface)
        {
            try
            {
                if (saveSettings == null || saveSettings.Method == null)
                {
                    @interface = null;

                    return new AnomalyDetectionResponse(1, "Warning: Null SaveLoadSettingss Object");
                }
                else if (saveSettings.Method.Equals("JSON"))
                {
                    @interface = new JsonSerializer();

                    return new AnomalyDetectionResponse(0, "OK");
                }
                else
                {
                    @interface = null;

                    return new AnomalyDetectionResponse(127, "Function<SelectInterfaceType>: Undefined Method to save or load");
                }
            }
            catch (Exception Ex)
            {
                @interface = null;

                return new AnomalyDetectionResponse(400, "Function<SelectInterfaceType>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        #endregion

        /*
        public static void GenerateSimilarFunctions(string path, int NumFunctions, double RandomNoiseLimit)
        {
            double[][] mFun = ReadCSV(path);
            double[][] mFun2 = new double[mFun.Length - 1][];
            for (int i = 0; i < mFun2.Length; i++)
            {
                mFun2[i] = new double[mFun[0].Length];
            }
            string fName = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + " SimilarFunctions.csv";
            WriteCSV(fName, mFun);
            int seed = 0;
            for (int i = 0; i < NumFunctions; i++)
            {
                //only for 2 dimensions
                for (int j = 0; j < mFun[0].Length; j++)
                {
                    //RandomClass rClass = new RandomClass();                 
                    mFun2[0][j] = mFun[1][j] + GetRandomNumber(RandomNoiseLimit * -1, RandomNoiseLimit, seed);
                    seed++;
                }
                WriteCSV(fName, mFun2, true);
            }
        }

        private static double[][] ReadCSV(string path)
        {
            System.IO.MemoryStream mStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(path));
            StreamReader sr = new StreamReader(mStream);
            var lines = new List<double[]>();
            while (!sr.EndOfStream)
            {
                string[] Line = sr.ReadLine().Split(',');
                double[] dLine = new double[Line.Length];

                for (int i = 0; i < Line.Length; i++)
                {
                    dLine[i] = Convert.ToDouble(Line[i]); // Convert to Double
                }

                lines.Add(dLine);
            }

            double[][] data = lines.ToArray();
            sr.Close();
            return data;
        }

        private static void WriteCSV(string path, double[][] mFunctions, bool Append = false)
        {
            StreamWriter file = new StreamWriter(path, Append);
            string Text = "";
            for (int i = 0; i < mFunctions.Length; i++)
            {
                for (int j = 0; j < mFunctions[0].Length; j++)
                {
                    Text = Text + mFunctions[i][j].ToString() + ",";
                }
                Text = Text.TrimEnd(',') + "\n"; // go to next line
            }
            //file.WriteLine(Text);
            file.Write(Text);
            file.Close();
        }

        private static double GetRandomNumber(double minimum, double maximum, int seed)
        {
            Random rnd = new Random(seed * DateTime.Now.Millisecond);
            return rnd.NextDouble() * (maximum - minimum) + minimum;
        }*/
        
    }
}
