using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// KMeans is the class responsible for clustering data based on K-means algorithm
    /// </summary>
    public class KMeansAlgorithm : IAlgorithm
    {
        #region Properties

        /// <summary>
        /// Instance object
        /// </summary>
        private KMeansModel m_instance;


        /// <summary>
        /// ClusteringSettings object
        /// </summary>
        [DataMember]
        public ClusteringSettings ClusterSettings { get; set; }


        /// <summary>
        /// method to get the Instance object
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public KMeansModel Instance { get => m_instance; set => m_instance = value; }

        #endregion

        #region Public Function

        #region Basic Functions


        /// <summary>
        /// Constructor for Kmeans. If settings was provided, it will create KMeans object with those settings. If all arguments are provided, it will set the desired trivial clusters.
        /// </summary>
        /// <param name="settings">contains settings for clustering (should not be null when training)</param>
        /// <param name="centroids">centroids of the clusters</param>
        /// <param name="maxDistance">maximum distance per cluster</param>
        public KMeansAlgorithm(ClusteringSettings settings, double[] maxDistance = null)
        {
            this.ClusterSettings = settings;
            if (maxDistance != null && settings != null)
            {
                this.m_instance = new KMeansModel(settings.NumberOfClusters);
                this.m_instance.Clusters = new Cluster[settings.NumberOfClusters];
                for (int i = 0; i < settings.NumberOfClusters; i++)
                {
                    this.m_instance.Clusters[i] = new Cluster();
                    this.m_instance.Clusters[i].Centroid = settings.InitialCentroids[i];
                    this.m_instance.Clusters[i].InClusterMaxDistance = maxDistance[i];
                }
            }
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
            KMeansScore score = new KMeansScore();
            double[] Fmin = null, D = null, DPrime = null;
            if (ClusterSettings.NumberOfClusters <= 1)
            {
                ClusterSettings.NumberOfClusters = estimateOptimalNumOfClusters(rawData, out Fmin, out D, out DPrime);
            }

            this.m_instance = null;
            score = runCalculation(rawData);
            if (Fmin != null)
            {
                score.Model.Fmin = Fmin;
                score.Model.D = D;
                score.Model.DPrime = DPrime;
            }

            return score;

        }

        /// <summary>
        /// Calculates the optimal number of clusters.
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private int estimateOptimalNumOfClusters(double[][] rawData, out double[] Fmin, out double[] D, out double[] DPrime)
        {
            int numFunctions = 50;
            int minClusters = 2;
            int maxClusters = 10;
            double[][] funcData = TransposeFunction(rawData);
            double[][] similarFunctions = getSimilarFunctions(funcData, numFunctions);
            double[][] trainedCentroids;

            Fmin = new double[maxClusters - minClusters + 1];
            D = new double[maxClusters - minClusters + 1];
            DPrime = new double[maxClusters - minClusters + 1];

            float fMax = 0f;
            KMeansScore score = new KMeansScore();

            int optimalNumOfClusters = 0;

            for (int i = minClusters; i < maxClusters+1; i++)
            {
                ClusterSettings.NumberOfClusters = i;
                trainedCentroids = new double[numFunctions * i][];

                for (int f = 0; f < numFunctions; f++)
                {
                    rawData = TransposeFunction(selectFunction(similarFunctions, f + 1, funcData.Length));

                    this.m_instance = null;
                    score = runCalculation(rawData);
                    for (int k = 0; k < i; k++)
                    {
                        trainedCentroids[f * i + k] = score.Model.Clusters[k].Centroid;
                    }
                }

                this.m_instance = null;
                //score = runCalculation(trainedCentroids);
                formClusters(score, trainedCentroids, i, numFunctions);
                

                float fMin = calcFMin(score, Fmin, D, DPrime);

                if (fMin > fMax)
                {
                    fMax = fMin;
                    optimalNumOfClusters = i;
                }
            }
            /*
            score.Model.Fmin = Fmin;
            score.Model.FminPrime = FminPrime;
            score.Model.D = D;
            score.Model.DPrime = DPrime;*/

            return optimalNumOfClusters;
        }

        private static float calcFMin(KMeansScore score, double[] Fmin, double[] D, double[] DPrime)
        {
            float FMin = float.MaxValue;
            float Fcur = 0f;

            ///// other calcultion for testing
            double DMin = Double.MaxValue;
            double DMinPrime = Double.MaxValue;
            double distancePrime = 0;
            /////

            double distance = 0;
            for (int k = 0; k < score.Model.NumberOfClusters - 1; k++)
            {
                for (int i = k + 1; i < score.Model.NumberOfClusters; i++)
                {
                    distance = CalculateDistance(score.Model.Clusters[k].Centroid, score.Model.Clusters[i].Centroid);
                    Fcur = (float)(distance / (score.Model.Clusters[k].InClusterMaxDistance + score.Model.Clusters[i].InClusterMaxDistance));
                    if (Fcur < FMin)
                    {
                        FMin = Fcur;
                    }

                    ///// other calcultion for testing
                    if (distance < DMin)
                    {
                        DMin = distance;
                    }
                    distancePrime = distance - (double)(score.Model.Clusters[k].InClusterMaxDistance + score.Model.Clusters[i].InClusterMaxDistance);
                    if (distancePrime < DMinPrime)
                    {
                        DMinPrime = distancePrime;
                    }
                    /////

                }
            }

            ///// other calcultion for testing
            Fmin[score.Model.NumberOfClusters - 2] = (double)FMin;
            D[score.Model.NumberOfClusters - 2] = DMin;
            DPrime[score.Model.NumberOfClusters - 2] = DMinPrime;
            /////

            return FMin;
        }

        private static double[][] getSimilarFunctions(double[][] refFunction, int numFunctions)
        {
            double[][] SimFunctions = new double[refFunction.Length * numFunctions][];
            double[][] curSimFunc = new double[refFunction.Length][];
            for (int n = 0; n < numFunctions; n++)
            {
                if (n == 0)
                {
                    curSimFunc = refFunction;
                }
                else
                {
                    curSimFunc = LearningFoundation.Helpers.FunctionGenerator.CreateSimilarFromReferenceFunc(refFunction, 5, 10);
                }
                for (int d = 0; d < refFunction.Length; d++)
                {
                    SimFunctions[n * refFunction.Length + d] = curSimFunc[d];
                }
            }
            return SimFunctions;
        }

        public static double[][] selectFunction(double[][] Functions, int numFunction, int dimension)
        {
            double[][] curFunc = new double[dimension][];
            for (int d = 0; d < dimension; d++)
            {
                curFunc[d] = Functions[(numFunction - 1) * dimension + d];
            }
            return curFunc;
        }

        private static void formClusters(KMeansScore score, double[][] trainedCentroids, int numClusters, int numTrainFun)
        {
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
                    calDist = CalculateDistance(clusterCentroids[j], trainedCentroids[i * numClusters + j]);
                    if (calDist > maxDistance[j])
                    {
                        maxDistance[j] = calDist;
                    }
                }
            }

            for (int i = 0; i < numClusters; i++)
            {
                score.Model.Clusters[i].Centroid = clusterCentroids[i];
                score.Model.Clusters[i].InClusterMaxDistance = maxDistance[i];
            }
        }


        private KMeansScore runCalculation(double[][] rawData)
        {
            KMeansScore res = new KMeansScore();
            int Code;
            string Message = "Function <Train>: ";
            try
            {
                // does some checks on the passed parameters by the user
                validateParams(rawData, ClusterSettings.KmeansAlgorithm, ClusterSettings.KmeansMaxIterations, ClusterSettings.NumberOfClusters, ClusterSettings.NumOfDimensions);

                // If first run. No load model done.
                if (this.m_instance == null)
                    this.m_instance = new KMeansModel(ClusterSettings.NumberOfClusters, rawData.Length);
                else
                    this.m_instance.NumOfSamples += rawData.Length;

                double[][] calculatedCentroids;

                int IterationReached = -1;

                //initiate the clustering process
                this.m_instance.DataToClusterMapping = runKMeansAlgorithm(rawData, this.m_instance.NumberOfClusters, ClusterSettings.NumOfDimensions, ClusterSettings.KmeansMaxIterations, ClusterSettings.KmeansAlgorithm, this.ClusterSettings.InitialCentroids, out calculatedCentroids, out IterationReached);

                //update centroids
                for (int i = 0; i < this.m_instance.NumberOfClusters; i++)
                {
                    this.m_instance.Clusters[i].Centroid = calculatedCentroids[i];
                }

                //create the clusters' result & statistics
                Cluster[] cluster;
                cluster = ClusteringResults.CreateClusteringResult(rawData, this.m_instance.DataToClusterMapping, calculatedCentroids, this.m_instance.NumberOfClusters);

                //import new results into current clusters
                getNewStats(cluster);

                //adjust due to partials
                recalcPartialClusters();

                if (ClusterSettings.KmeansMaxIterations <= IterationReached)
                {
                    res.Message = "Clustering Complete. K-means stopped at the maximum allowed iteration: " + ClusterSettings.KmeansMaxIterations;
                }
                else
                {
                    res.Message = "Clustering Complete. K-means converged at iteration: " + IterationReached;
                }

                res.Model = this.m_instance;

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

                /*
                if (this.m_instance==null)
                {
                    
                }*/

                int clusterIndex = -1;
                List<double> list = new List<double>();

                int n = 0;
                foreach (var item in data)
                {
                    clusterIndex = PredictSample(item, this.ClusterSettings.Tolerance);
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
                // json.Save(path, this.m_cluster);
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

                this.m_instance = json.Load(path);
                //this.m_cluster = json.LoadClusters(path);
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
        /// PredictSample detects to which cluster the given sample belongs to.
        /// </summary>
        /// <param name="Sample">the sample to be predicted</param>
        /// <param name="Tolerance">a value in % representing the tolerance to possible outliers</param>
        /// <returns>the cluster number to which the sample belongs (-1 if the sample doesn't belong to any cluster).</returns>
        private int PredictSample(double[] Sample, double Tolerance)
        {
            int clusterIndex;
            int Code;
            string Message = "Function <PredictSample>: ";
            try
            {
                //JsonSerializer json = new JsonSerializer();

                //some checks on the passed parameters by the user
                if (Tolerance < 0)
                {
                    Code = 110;
                    Message += "Unacceptable tolerance value";
                    throw new KMeansException(Code, Message);
                }

                //returns error if the new sample has different number of attributes compared to the samples in the desired project
                if (this.m_instance.Clusters[0].Centroid.Length != Sample.Length)
                {
                    Code = 114;
                    Message += "Mismatch in number of attributes";
                    throw new KMeansException(Code, Message);
                }

                double calculatedDistance;
                double minDistance = double.MaxValue;
                int closestCentroid = -1;

                //determines to which centroid the sample is closest and the distance
                for (int j = 0; j < this.m_instance.NumberOfClusters; j++)
                {
                    calculatedDistance = CalculateDistance(Sample, this.m_instance.Clusters[j].Centroid);

                    if (calculatedDistance < minDistance)
                    {
                        minDistance = calculatedDistance;

                        closestCentroid = j;
                    }
                }

                //decides based on the maximum distance in the cluster & the tolerance whether the sample really belongs to the cluster or not 
                if (minDistance < this.m_instance.Clusters[closestCentroid].InClusterMaxDistance * (1.0 + Tolerance / 100.0))
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
        /// runKMeansAlgorithm is a function that clusters the given samples based on the K-means algorithm.
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="numClusters">desired number of clusters</param>
        /// <param name="numDims">Number of dimensions.</param>
        /// <param name="maxCount">maximum allowed number of Kmeans iteration for clustering</param>
        /// <param name="kMeanAlgorithm">the desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul></param>
        /// <param name="initialCentroids">Used as initial centroids if specified.
        /// If not specified then Kmeans clustering starts with an initial guess for the centroids as a random assignment.</param>
        /// <param name="centroids">the variable through which the resulting centroids are returned</param>
        /// <param name="IterationReached">the variable through which the iteration reached is returned</param>
        /// <returns>the assigned cluster number for each sample of the RawData</returns>
        internal static int[] runKMeansAlgorithm(double[][] rawData, int numClusters, int numDims, int maxCount, int kMeanAlgorithm, double[][] initialCentroids, out double[][] centroids, out int IterationReached)
        {
            int Code;
            string Message = "Function <runKMeansAlgorithm>: ";
            int[] clusterAssignments;
            try
            {
                bool changed = true;
                int cnt = 0;
                int numTuples = rawData.Length;

                clusterAssignments = new int[rawData.Length];

                // means must be initialized (declared row number) before calling updateMeans
                double[][] means = new double[numClusters][];

                // claculate initial centroids (educated initial guess)
                if (initialCentroids == null)
                {
                    centroids = new double[numClusters][];

                    for (int k = 0; k < numClusters; ++k)
                        centroids[k] = new double[numDims];

                    means = calculateInitialMeans(rawData, numClusters);

                    if (kMeanAlgorithm == 1)
                    {
                        assignCentroidsToNearestMeanSample(rawData, numClusters, means, centroids);
                    }
                    else
                    {
                        assignCentroidsToMean(means, centroids);
                    }
                }
                // use specified initial centroids
                else
                {
                    centroids = initialCentroids;
                }


                while (changed == true && cnt < maxCount)
                {
                    ++cnt;

                    // use centroids to update cluster assignment
                    changed = assign(rawData, clusterAssignments, centroids);

                    // use new clustering to update cluster means
                    UpdateMeans(rawData, clusterAssignments, means);

                    // use new means to update centroids
                    if (kMeanAlgorithm == 1)
                    {
                        updateCentroids(rawData, clusterAssignments, means, centroids);
                    }
                    else
                    {
                        assignCentroidsToMean(means, centroids);
                    }
                }

                //recalcPartialCentroids(centroids, rawData.Length);

                IterationReached = cnt;

                return clusterAssignments;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// recalcPartialClusters is a function that recaculates clusters' centroids and maximum distance in clusters based on the new minibatch and the old cluster results
        /// </summary>
        private void recalcPartialClusters()
        {
            int Code;
            string Message = "Function <recalcPartialClusters>: ";
            try
            {
                // This code recalculate sum by adding a mean from previous minibatch.
                for (int i = 0; i < this.Instance.NumberOfClusters; i++)
                {

                    //if it's a partial cluster calculation
                    if (this.Instance.Clusters[i].PreviousCentroid != null)
                    {
                        double f = (double)1 / (this.Instance.Clusters[i].NumberOfSamples + this.Instance.Clusters[i].PreviousNumberOfSamples);

                        //for each attribute
                        for (int j = 0; j < this.Instance.Clusters[0].Centroid.Length; j++)
                        {
                            //recalculate cetroid
                            this.Instance.Clusters[i].Centroid[j] = f * (this.Instance.Clusters[i].PreviousNumberOfSamples * this.Instance.Clusters[i].PreviousCentroid[j] + this.Instance.Clusters[i].NumberOfSamples * this.Instance.Clusters[i].Centroid[j]);
                        }

                        //recalculate maximum distance
                        adjustInClusterMaxDistance(i);
                    }

                    //adjust cluster properties
                    this.Instance.Clusters[i].NumberOfSamples += this.Instance.Clusters[i].PreviousNumberOfSamples;
                    this.Instance.Clusters[i].PreviousNumberOfSamples = this.Instance.Clusters[i].NumberOfSamples;
                    this.Instance.Clusters[i].PreviousCentroid = this.Instance.Clusters[i].Centroid;
                    this.Instance.Clusters[i].PreviousInClusterMaxDistance = this.Instance.Clusters[i].InClusterMaxDistance;
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
        /// adjustInClusterMaxDistance is a function that recalculates/approximate the maximum distance in the cluster for partial clustering
        /// </summary>
        /// <param name="cluster">index of the cluster</param>
        private void adjustInClusterMaxDistance(int cluster)
        {
            int Code;
            string Message = "Function <adjustInClusterMaxDistance>: ";
            try
            {
                this.Instance.Clusters[cluster].InClusterMaxDistance = 0;
                // calculate new in cluster max distance
                double curDistance;
                for (int i = 0; i < this.Instance.Clusters[cluster].NumberOfSamples; i++)
                {
                    curDistance = CalculateDistance(this.Instance.Clusters[cluster].Centroid, this.Instance.Clusters[cluster].ClusterData[i]);
                    if (curDistance > this.Instance.Clusters[cluster].InClusterMaxDistance)
                    {
                        this.Instance.Clusters[cluster].InClusterMaxDistance = curDistance;
                    }
                }

                // compare to max possible old in cluster max distance
                double oldDistance = this.Instance.Clusters[cluster].PreviousInClusterMaxDistance + CalculateDistance(this.Instance.Clusters[cluster].Centroid, this.Instance.Clusters[cluster].PreviousCentroid);
                if (oldDistance > this.Instance.Clusters[cluster].InClusterMaxDistance)
                {
                    this.Instance.Clusters[cluster].InClusterMaxDistance = oldDistance;
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
        /// getNewStats is a function that loads the new cluster stats from cluster array while preserving the previously calculated properties
        /// </summary>
        /// <param name="cluster">array containing the new cluster stats</param>
        private void getNewStats(Cluster[] cluster)
        {
            int Code;
            string Message = "Function <getNewStats>: ";
            try
            {
                for (int i = 0; i < cluster.Length; i++)
                {
                    this.Instance.Clusters[i].Centroid = cluster[i].Centroid;
                    this.Instance.Clusters[i].ClusterData = cluster[i].ClusterData;
                    this.Instance.Clusters[i].ClusterDataDistanceToCentroid = cluster[i].ClusterDataDistanceToCentroid;
                    this.Instance.Clusters[i].ClusterDataOriginalIndex = cluster[i].ClusterDataOriginalIndex;
                    this.Instance.Clusters[i].ClusterOfNearestForeignSample = cluster[i].ClusterOfNearestForeignSample;
                    this.Instance.Clusters[i].DistanceToNearestClusterCentroid = cluster[i].DistanceToNearestClusterCentroid;
                    this.Instance.Clusters[i].DistanceToNearestForeignSample = cluster[i].DistanceToNearestForeignSample;
                    this.Instance.Clusters[i].DistanceToNearestForeignSampleInNearestCluster = cluster[i].DistanceToNearestForeignSampleInNearestCluster;
                    this.Instance.Clusters[i].InClusterFarthestSample = cluster[i].InClusterFarthestSample;
                    this.Instance.Clusters[i].InClusterFarthestSampleIndex = cluster[i].InClusterFarthestSampleIndex;
                    this.Instance.Clusters[i].InClusterMaxDistance = cluster[i].InClusterMaxDistance;
                    this.Instance.Clusters[i].Mean = cluster[i].Mean;
                    this.Instance.Clusters[i].NearestCluster = cluster[i].NearestCluster;
                    this.Instance.Clusters[i].NearestForeignSample = cluster[i].NearestForeignSample;
                    this.Instance.Clusters[i].NearestForeignSampleInNearestCluster = cluster[i].NearestForeignSampleInNearestCluster;
                    this.Instance.Clusters[i].NumberOfSamples = cluster[i].NumberOfSamples;
                    this.Instance.Clusters[i].StandardDeviation = cluster[i].StandardDeviation;
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
        /// is a function that assigns the centroid to the nearest sample to the calculated mean
        /// </summary>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="numClusters">desired number of clusters</param>
        /// <param name="means">mean of clusters</param>
        /// <param name="centroids">centroids of clusters (Updated in the function)</param>
        private static void assignCentroidsToNearestMeanSample(double[][] rawData, int numClusters, double[][] means, double[][] centroids)
        {
            int Code;
            string Message = "Function <>: ";
            try
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
                        currDist[j] = CalculateDistance(rawData[i], means[j]);

                        if (currDist[j] < minDist[j])
                        {
                            minDist[j] = currDist[j];

                            centroids[j] = rawData[i];
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
        /// allocate is a function that creates a double[][] with the specified size (number of clusters x number of attributes) or returns the provided item if it was not null.
        /// </summary>
        /// <param name="numClusters">number of clusters</param>
        /// <param name="numAttributes">number of attributes</param>
        /// <returns>the allocated double[][]</returns>
        private static double[][] allocate(int numClusters, int numAttributes, double[][] initialCentroids)
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

                if (initialCentroids == null)
                {
                    newItem = new double[numClusters][];

                    for (int k = 0; k < numClusters; ++k)
                        newItem[k] = new double[numAttributes];
                }
                else
                    newItem = initialCentroids;

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
        /// <param name="numClusters">Number of clusters</param>
        /// <param name="initialMeans">The initial guess for the means</param></returns>
        private static double[][] calculateInitialMeans(double[][] rawData, int numClusters)
        {
            double[][] initialMeans = new double[numClusters][];
            int Code;
            string Message = "Function <calculateInitialMeans>: ";
            try
            {
                double[] MinValues = new double[rawData[0].Length];
                double[] MaxValues = new double[rawData[0].Length];

                int NumberOfAttributes = rawData[0].Length;

                for (int i = 0; i < numClusters; i++)
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

                for (int i = 0; i < numClusters; i++)
                {
                    for (int j = 0; j < NumberOfAttributes; j++)
                    {
                        initialMeans[i][j] = MinValues[j] + ((MaxValues[j] - MinValues[j]) * (i * 2 + 1)) / (numClusters * 2);
                    }
                }

                return initialMeans;
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
        public static double CalculateDistance(double[] firstElement, double[] secondElement)
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
        /// <param name="previousMeanValue">the mean value of the previous minibatch.</param>
        /// <param name="previousSampleCount">the number of samples in previous minibatch.</param>
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="clustering">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="means">mean of each cluster (Updated in the function)</param>

        public static void UpdateMeans(double[][] rawData, int[] clustering, double[][] means, long previousSampleCount = 0, double[] previousMeanValues = null)
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
                    //for (int j = 0; j < means[0].Length; ++j)
                    // means[k][j] = 0.0;
                    means[k] = new double[rawData[0].Length];
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

                        /*
                        // This code recalculate sum by adding a mean from previous minibatch.
                        if (previousSampleCount != 0 && previousMeanValues != null)
                        {
                            double f = (double)1 / (rawData.Length + previousSampleCount);
                            means[k][j] = f * (previousSampleCount * previousMeanValues[k] + rawData.Length * means[k][j]);
                        }*/
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
        /// <param name="rawData">the samples to be clustered</param>
        /// <param name="clustering">contains the assigned cluster number for each sample of the RawData</param>
        /// <param name="means">mean of each cluster</param>
        /// <param name="centroids">centroid of each cluster (Updated in the function)</param>
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

                    double currDist = CalculateDistance(rawData[i], means[cluster]);  // call helper

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
        /// assignCentroidsToMean is a function that sets the centroids to the means
        /// </summary>
        /// <param name="means">mean of each cluster</param>
        /// <param name="centroids">centroid of each cluster</param>
        public static void assignCentroidsToMean(double[][] means, double[][] centroids)
        {
            int Code;
            string Message = "Function <centroidsAreMeans>: ";
            try
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

                for (int i = 0; i < rawData.Length; ++i)      // walk through each sample
                {
                    for (int k = 0; k < numClusters; ++k)       // compute distances to all centroids
                    {
                        distances[k] = CalculateDistance(rawData[i], centroids[k]);
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

        public static double[][] TransposeFunction(double[][] similarFuncData)
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

        #endregion
    }
}
