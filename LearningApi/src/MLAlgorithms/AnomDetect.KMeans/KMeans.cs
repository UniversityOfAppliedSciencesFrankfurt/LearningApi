using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// KMeans is the class responsible for clustering data based on K-means algorithm
    /// </summary>
    public class KMeans : IAlgorithm
    {
        #region Properties

        /// <summary>
        /// Instance object
        /// </summary>
        private Instance m_instance;

        /// <summary>
        /// array of Cluster objects
        /// </summary>
        private Cluster[] m_cluster;

        /// <summary>
        /// ClusteringSettings object
        /// </summary>
        public ClusteringSettings clusterSettings { get; internal set; }

        #endregion

        #region Public Function

        #region Basic Functions

        /// <summary>
        /// method to get the Cluster object
        /// </summary>
        public Cluster[] clusters { get => m_cluster; }


        /// <summary>
        /// method to get the Instance object
        /// </summary>
        /// <returns></returns>
        public Instance instance { get => m_instance; }

        /// <summary>
        /// setTrivialClusters creates a very basic instance for clusters
        /// </summary>
        /// <param name="numCLusters">number of clusters</param>
        /// <param name="centroids">centroids of the clusters</param>
        /// <param name="maxDistance">maximum distance per cluster</param>
        public void setTrivialClusters(int numCLusters, double[][] centroids, double[] maxDistance)
        {
            this.m_instance = new Instance(null, numCLusters, centroids);
            this.m_instance.InClusterMaxDistance = maxDistance;
        }

        /// <summary>
        /// Constructor for Kmeans
        /// </summary>
        /// <param name="settings">contains settings for clustering (should not be null when training)</param>
        public KMeans(ClusteringSettings settings = null)
        {
            this.clusterSettings = settings;
        }

        #endregion

        #region Main Functions

        /// <summary>
        /// Run is function that will call Train to starts a new clustering instance.
        /// </summary>
        /// <param name="data">data for training</param>
        /// <param name="ctx"></param>
        /// <returns>the training results</returns>
        public IScore Run(double[][] data, IContext ctx)
        {
            return Train(data, ctx);
        }

        /// <summary>
        /// Train is a function that starts a new clustering instance.
        /// </summary>
        /// <param name="rawData">data for training</param>
        /// <param name="ctx"></param>
        /// <returns>the training results</returns>
        public IScore Train(double[][] rawData, IContext ctx)
        {
            KMeansScore res = new KMeansScore();
            int Code;
            string Message = "Function <Train>: ";
            try
            {

                // does some checks on the passed parameters by the user
                validateParams(rawData, clusterSettings.KmeansAlgorithm, clusterSettings.KmeansMaxIterations, clusterSettings.NumberOfClusters, clusterSettings.NumberOfAttributes);

                Instance instance = new Instance(rawData, clusterSettings.NumberOfClusters);

                double[][] calculatedCentroids;

                int IterationReached = -1;

                //initiate the clustering process
                instance.DataToClusterMapping = kMeansClusteringAlg(rawData, instance.NumberOfClusters, clusterSettings.NumberOfAttributes, clusterSettings.KmeansMaxIterations, clusterSettings.KmeansAlgorithm, clusterSettings.InitialGuess, this.clusterSettings.InitialCentroids, out calculatedCentroids, out IterationReached);

                instance.Centroids = calculatedCentroids;


                //create the clusters' result & statistics
                m_cluster = ClusteringResults.CreateClusteringResult(rawData, instance.DataToClusterMapping, calculatedCentroids, instance.NumberOfClusters);

                instance.InClusterMaxDistance = new double[instance.NumberOfClusters];

                for (int i = 0; i < instance.NumberOfClusters; i++)
                {
                    instance.InClusterMaxDistance[i] = m_cluster[i].InClusterMaxDistance;
                }

                this.m_instance = instance;

                if (clusterSettings.KmeansMaxIterations <= IterationReached)
                {
                    res.Message = "Clustering Complete. K-means stopped at the maximum allowed iteration: " + clusterSettings.KmeansMaxIterations;
                }
                else
                {
                    res.Message = "Clustering Complete. K-means converged at iteration: " + IterationReached;
                }
                res.Clusters = this.m_cluster;
                res.instance = this.m_instance;

                return res;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }

        }

        /// <summary>
        /// Predict detects to which cluster the given sample belongs to.
        /// </summary>
        /// <param name="settings">the desired settings for detecting to which, if any, cluster the sample belongs.(Path can be null if you use Run() or Train() before)</param>
        /// <returns>the cluster number to which the sample belongs (-1 if the sample doesn't belong to any cluster).</returns>
        public int PredictSample(CheckingSampleSettings settings)
        {
            int clusterIndex;
            int Code;
            string Message = "Function <PredictSample>: ";
            try
            {
                Instance instance = null;
                JsonSerializer json = new JsonSerializer();

                //some checks on the passed parameters by the user
                if (settings.Tolerance < 0)
                {
                    Code = 110;
                    Message += "Unacceptable tolerance value";
                    throw new KMeansException(Code, Message);
                }
                if (settings.Path != null)
                {
                    //load the clustering project containing the clusters to one of which, if any, the sample will be assigned to
                    instance = json.LoadInstance(settings.Path);

                }
                else
                {
                    instance = this.m_instance;
                }

                //returns error if the new sample has different number of attributes compared to the samples in the desired project
                if (instance.Centroids[0].Length != settings.Sample.Length)
                {
                    Code = 114;
                    Message += "Mismatch in number of attributes";
                    throw new KMeansException(Code, Message);
                }

                double calculatedDistance;
                double minDistance = double.MaxValue;
                int closestCentroid = -1;

                //determines to which centroid the sample is closest and the distance
                for (int j = 0; j < instance.NumberOfClusters; j++)
                {
                    calculatedDistance = calculateDistance(settings.Sample, instance.Centroids[j]);

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
                }
                else
                {
                    clusterIndex = -1;
                }

                return clusterIndex;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// Predict is a function that detects to which clusters a set of samples belongs.
        /// </summary>
        /// <param name="data">the set of samples</param>
        /// <param name="ctx"></param>
        /// <returns>the prediction results</returns>
        public IResult Predict(double[][] data, IContext ctx)
        {
            int Code;
            string Message = "Function <Predict>: ";
            try
            {
                KMeansResult res = new KMeansResult()
                {
                    PredictedClusters = new int[data.Length],
                };

                int clusterIndex = -1;
                List<double> list = new List<double>();

                int n = 0;
                foreach (var item in data)
                {
                    clusterIndex = PredictSample(new CheckingSampleSettings(null, item, 0));
                    res.PredictedClusters[n++] = clusterIndex;
                }

                return res;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// Save is a function that saves Instance and Cluster results in json file. Instance will be saved in 'Instance Result' folder and Cluster will be saved in 'Cluster Result' folder.
        /// </summary>
        /// <param name="path">Where it should be saved to</param>
        public void Save(string path)
        {
            int Code;
            string Message = "Function <Save>: ";
            try
            {
                JsonSerializer json = new JsonSerializer();

                json.Save(path, this.m_instance);
                json.Save(path, this.m_cluster);
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }

        }

        /// <summary>
        /// Load is a function that loads Instance and Cluster results from json file to the calling KMeans object.
        /// </summary>
        /// <param name="path">Where it should be loaded from</param>
        public void Load(string path)
        {
            int Code;
            string Message = "Function <Load>: ";
            try
            {
                JsonSerializer json = new JsonSerializer();

                this.m_instance = json.LoadInstance(path);
                this.m_cluster = json.LoadClusters(path);
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
        /// validateParams is a function that does some checks on the passed parameters by the user.
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="kmeansAlgorithm">the desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul></param>
        /// <param name="kmeansMaxIterations">maximum allowed number of Kmeans iteration for clustering</param>
        /// <param name="numberOfClusters">desired number of clusters</param>
        /// <param name="numberOfAttributes">number of attributes for each sample</param>
        private static void validateParams(double[][] rawData, int kmeansAlgorithm, int kmeansMaxIterations, int numberOfClusters, int numberOfAttributes)
        {
            int Code;
            string Message = "Function <validateParams>: ";
            try
            {
                if (kmeansMaxIterations < 1)
                {
                    Code = 108;
                    Message += "Unacceptable number of maximum iterations";
                    throw new KMeansException(Code, Message);
                }

                if (numberOfClusters < 2)
                {
                    Code = 106;
                    Message += "Unacceptable number of clusters. Must be at least 2";
                    throw new KMeansException(Code, Message);
                }

                if (rawData != null)
                {
                    if (numberOfClusters > rawData.Length)
                    {
                        Code = 105;
                        Message += "Unacceptable number of clusters. Clusters more than samples";
                        throw new KMeansException(Code, Message);
                    }
                }
                else
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

                if (kmeansAlgorithm < 1 || kmeansAlgorithm > 2)
                {
                    Code = 124;
                    Message += "Unacceptable input for K-means Algorithm";
                    throw new KMeansException(Code, Message);
                }

                verifyRawDataConsistency(rawData, numberOfAttributes);

            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// verifyRawDataConsistency is a function that checks that all the samples have same given number of attributes.
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="numberOfAttributes">number of attributes for each sample</param>
        internal static void verifyRawDataConsistency(double[][] rawData, int numberOfAttributes)
        {
            int Code;
            string Message = "Function <verifyRawDataConsistency>: ";
            try
            {
                if (rawData == null)
                {
                    Code = 100;
                    Message += "RawData is null";
                    throw new KMeansException(Code, Message);
                }

                if (rawData.Length < 1)
                {
                    Code = 102;
                    Message += "RawData is empty";
                    throw new KMeansException(Code, Message);
                }

                int DataLength = rawData.Length;

                for (int i = 0; i < DataLength; i++)
                {
                    if (rawData[i] == null || rawData[i].Length != numberOfAttributes)
                    {
                        Code = 111;
                        Message += "Data sample and number of attributes are inconsistent. First encountered inconsistency in data sample: " + i + ".";
                        throw new KMeansException(Code, Message);
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
        /// KMeansClusteringAlg is a function that clusters the given samples based on the K-means algorithm.
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
        /// <param name="initialCentroids">the initial centroids</param>
        /// <param name="centroids">the variable through which the resulting centroids are returned</param>
        /// <param name="IterationReached">the variable through which the iteration reached is returned</param>
        /// <returns>the assigned cluster number for each sample of the RawData</returns>
        internal static int[] kMeansClusteringAlg(double[][] rawData, int numClusters, int numAttributes, int maxCount, int kMeanAlgorithm, bool initialGuess, double[][] initialCentroids, out double[][] centroids, out int IterationReached)
        {
            int Code;
            string Message = "Function <KMeansClusteringAlg>: ";
            int[] clustering;
            try
            {
                bool changed = true;
                int cnt = 0;
                int numTuples = rawData.Length;

                clustering = new int[rawData.Length];

                // just makes things a bit cleaner
                double[][] allocated = allocate(numClusters, numAttributes, initialCentroids);

                double[][] means = allocated;

                centroids = allocated;

                if (initialGuess)
                {
                    calculateInitialMeans(rawData, numClusters, out means);

                    if (kMeanAlgorithm == 1)
                    {
                        double[] currDist = new double[numClusters];
                        double[] minDist = new double[numClusters];

                        for (int i = 0; i < numClusters; i++)
                        {
                            minDist[i] = double.MaxValue;
                        }


                        for (int i = 0; i < rawData.Length; ++i)
                        {
                            for (int j = 0; j < numClusters; j++)
                            {
                                currDist[j] = calculateDistance(rawData[i], means[j]);

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
                    clustering = initClustering(numTuples, numClusters, 0);

                    // could call this instead inside UpdateCentroids
                    updateMeans(rawData, clustering, means);

                    if (kMeanAlgorithm == 1)
                    {
                        updateCentroids(rawData, clustering, means, centroids);
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


                while (changed == true && cnt < maxCount)
                {
                    ++cnt;

                    // use centroids to update cluster assignment
                    changed = assign(rawData, clustering, centroids);

                    // use new clustering to update cluster means
                    updateMeans(rawData, clustering, means);

                    // use new means to update centroids
                    if (kMeanAlgorithm == 1)
                    {
                        updateCentroids(rawData, clustering, means, centroids);
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

                return clustering;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// allocate is a function that creates a double[][] with the specified size (number of clusters x number of attributes) or returns the provided item if it was not null.
        /// </summary>
        /// <param name="numClusters">number of clusters</param>
        /// <param name="numAttributes">number of attributes</param>
        /// <returns>the allocated double[][]</returns>
        private static double[][] allocate(int numClusters, int numAttributes, double[][] Item)
        {
            double[][] newItem;
            int Code;
            string Message = "Function <allocate>: ";
            try
            {
                // helper allocater for means[][] and centroids[][]
                if (numClusters < 2)
                {
                    Code = 106;
                    Message += "Unacceptable number of clusters. Must be at least 2";
                    throw new KMeansException(Code, Message);
                }
                if (numAttributes < 1)
                {
                    Code = 107;
                    Message += "Unacceptable number of attributes. Must be at least 1";
                    throw new KMeansException(Code, Message);
                }

                if (Item == null)
                {
                    newItem = new double[numClusters][];

                    for (int k = 0; k < numClusters; ++k)
                        newItem[k] = new double[numAttributes];
                }
                else
                    newItem = Item;

                return newItem;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// calculateInitialMeans returns initial guess for the means.
        /// </summary>
        /// <param name="rawData">The samples to be clustered</param>
        /// <param name="numberOfClusters">Number of clusters</param>
        /// <param name="initialMeans">The initial guess for the means</param></returns>
        private static void calculateInitialMeans(double[][] rawData, int numberOfClusters, out double[][] initialMeans)
        {
            int Code;
            string Message = "Function <calculateInitialMeans>: ";
            try
            {
                double[] MinValues = new double[rawData[0].Length];
                double[] MaxValues = new double[rawData[0].Length];

                initialMeans = new double[numberOfClusters][];

                int NumberOfAttributes = rawData[0].Length;

                for (int i = 0; i < numberOfClusters; i++)
                {
                    initialMeans[i] = new double[NumberOfAttributes];
                }

                for (int j = 0; j < NumberOfAttributes; j++)
                {
                    MinValues[j] = rawData[0][j];
                    MaxValues[j] = rawData[0][j];
                }

                for (int i = 1; i < rawData.Length; i++)
                {
                    for (int j = 0; j < NumberOfAttributes; j++)
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

                for (int i = 0; i < numberOfClusters; i++)
                {
                    for (int j = 0; j < NumberOfAttributes; j++)
                    {
                        initialMeans[i][j] = MinValues[j] + ((MaxValues[j] - MinValues[j]) * (i * 2 + 1)) / (numberOfClusters * 2);
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
        /// calculateDistance is a function that claculates the distance between two elements of same size.
        /// </summary>
        /// <param name="firstElement">First element</param>
        /// <param name="secondElement">Second element</param>
        /// <returns>Distance between the two elements</returns>
        internal static double calculateDistance(double[] firstElement, double[] secondElement)
        {
            int Code;
            string Message = "Function <calculateDistance>: ";

            try
            {
                if (firstElement == null || secondElement == null)
                {
                    Code = 101;
                    Message += "At least one input is null";
                    throw new KMeansException(Code, Message);
                }

                if (firstElement.Length != secondElement.Length)
                {
                    Code = 115;
                    Message += "Inputs have different dimensions";
                    throw new KMeansException(Code, Message);
                }

                double SquaredDistance = 0;

                for (int i = 0; i < firstElement.Length; i++)
                {
                    SquaredDistance += Math.Pow(firstElement[i] - secondElement[i], 2);
                }

                return Math.Sqrt(SquaredDistance);
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary> 
        /// initClustering is a function that assigns a sample to each cluster and then randomly assigns the remaining samples on all clusters.
        /// </summary>
        /// <param name="numTuples">number of samples</param>
        /// <param name="numClusters">number of clusters</param>
        /// <param name="randomSeed">random seed for randomly assigning the samples to the clusters</param>
        /// <returns>the assigned cluster number for each sample of the RawData</returns>
        private static int[] initClustering(int numTuples, int numClusters, int randomSeed)
        {
            int[] clustering;
            int Code;
            string Message = "Function <initClustering>: ";
            try
            {
                if (numClusters < 2)
                {
                    Code = 106;
                    Message += "Unacceptable number of clusters. Must be at least 2";
                    throw new KMeansException(Code, Message);
                }
                if (numTuples < numClusters)
                {
                    Code = 105;
                    Message += "Unacceptable number of clusters. Clusters more than samples";
                    throw new KMeansException(Code, Message);
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

                return clustering;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// UpdateMeans is a function that calculates the new mean of each cluster.
        /// </summary>
        /// <param name="previousMeanValue">The mean value of the previous minibatch.</param>
        /// <param name="previousSampleCount">The number of samples in previous minibatch.</param>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="clustering">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="means">mean of each cluster (Updated in the function)</param>
      
        internal static void updateMeans(double[][] rawData, int[] clustering, double[][] means, long previousSampleCount = 0, double[] previousMeanValues = null)
        {
            int Code;
            string Message = "Function <updateMeans>: ";
            try
            {
                if (rawData == null || rawData.Length < 1)
                {
                    Code = 102;
                    Message += "RawData is empty";
                    throw new KMeansException(Code, Message);
                }

                if (means == null || means.Length < 1)
                {
                    Code = 103;
                    Message += "Means is empty";
                    throw new KMeansException(Code, Message);
                }

                // assumes means[][] exists. consider making means[][] a ref parameter
                int numClusters = means.Length;

                // zero-out means[][]
                for (int k = 0; k < means.Length; ++k)
                {
                    for (int j = 0; j < means[0].Length; ++j)
                        means[k][j] = 0.0;
                }


                // make an array to hold cluster counts
                int[] clusterCounts = new int[numClusters];

                // walk through each tuple, accumulate sum for each attribute, update cluster count
                for (int i = 0; i < rawData.Length; ++i)
                {
                    int cluster = clustering[i];

                    ++clusterCounts[cluster];

                    for (int j = 0; j < rawData[i].Length; ++j)
                        means[cluster][j] += rawData[i][j];
                }

                // divide each attribute sum by cluster count to get average (mean)
                for (int k = 0; k < means.Length; ++k)
                {
                    if (clusterCounts[k] == 0)
                    {
                        continue;
                    }

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
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// updateCentroids is a function that assigns the nearest sample to each mean as the centroid of a cluster.
        /// </summary>
        /// <param name="rawData">The samples to be clustered</param>
        /// <param name="clustering">Contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="means">Mean of each cluster</param>
        /// <param name="centroids">Centroid of each cluster (Updated in the function)</param>
        private static void updateCentroids(double[][] rawData, int[] clustering, double[][] means, double[][] centroids)
        {
            int Code;
            string Message = "Function <updateCentroids>: ";
            try
            {

                // updates all centroids by calling helper that updates one centroid
                for (int k = 0; k < centroids.Length; ++k)
                {
                    double[] centroid = computeCentroid(rawData, clustering, k, means);

                    centroids[k] = centroid;
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
        /// computeCentroid is a function that assigns the nearest sample to the mean as the centroid of a cluster.
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="clustering">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="cluster">number of the cluster</param>
        /// <param name="means">mean of each cluster</param>
        /// <returns>the centroid</returns>
        private static double[] computeCentroid(double[][] rawData, int[] clustering, int cluster, double[][] means)
        {
            double[] centroid;
            int Code;
            string Message = "Function <computeCentroid>: ";
            try
            {
                // the centroid is the actual tuple values that are closest to the cluster mean
                int numAttributes = means[0].Length;

                centroid = new double[numAttributes];

                double minDist = double.MaxValue;

                for (int i = 0; i < rawData.Length; ++i) // walk thru each data tuple
                {
                    int c = clustering[i];  // if current tuple isn't in the cluster we're computing for, continue on

                    if (c != cluster)
                        continue;

                    double currDist = calculateDistance(rawData[i], means[cluster]);  // call helper

                    if (currDist < minDist)
                    {
                        minDist = currDist;

                        for (int j = 0; j < centroid.Length; ++j)
                            centroid[j] = rawData[i][j];
                    }
                }

                return centroid;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }

        }

        /// <summary>
        /// assign is a function that assigns each sample to the nearest clusters' centroids. If the new assignment is the same as the older one, it returns true else it will return false.
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="clustering">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="centroids">centroid of each cluster</param>
        /// <returns>true if new assignment is the same as the old one, else false</returns>
        private static bool assign(double[][] rawData, int[] clustering, double[][] centroids)
        {
            int Code;
            string Message = "Function <assign>: ";
            try
            {
                // assign each tuple to best cluster (closest to cluster centroid)
                // return true if any new cluster assignment is different from old/curr cluster
                // does not prevent a state where a cluster has no tuples assigned. see article for details
                int numClusters = centroids.Length;
                bool changed = false;
                int changedClusters = 0;

                double[] distances = new double[numClusters]; // distance from current tuple to each cluster mean

                for (int i = 0; i < rawData.Length; ++i)      // walk thru each tuple
                {
                    for (int k = 0; k < numClusters; ++k)       // compute distances to all centroids
                    {
                        distances[k] = calculateDistance(rawData[i], centroids[k]);
                    }
                    // distances[k] = Distance(rawData[i], centroids[k]);

                    int newCluster = minIndex(distances);  // find the index == custerID of closest 

                    if (newCluster != clustering[i]) // different cluster assignment?
                    {
                        changed = true;

                        clustering[i] = newCluster;

                        changedClusters++;
                    } // else no change              
                }

                return changed; // was there any change in clustering?
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// minIndex is a function that returns the index of the smallest distance between a set of distances.
        /// </summary>
        /// <param name="distances">Distance between each sample and the centroid</param>
        /// <returns>the index of the smallest distance</returns>
        private static int minIndex(double[] distances)
        {
            int Code;
            string Message = "Function <minIndex>: ";
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
                return indexOfMin;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        #endregion

        #endregion
    }
}
