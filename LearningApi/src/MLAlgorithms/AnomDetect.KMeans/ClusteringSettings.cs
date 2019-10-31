using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// ClusteringSettings is a class that contains the desired settings by the user for clustering.
    /// </summary>
    [DataContract]
    public class ClusteringSettings
    {
        /// <summary>
        /// the desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul>
        /// </summary>
        [DataMember]
        public int KmeansAlgorithm { get; internal set; }

        /// <summary>
        /// maximum allowed number of Kmeans iteration for clustering
        /// </summary>
        [DataMember]
        public int KmeansMaxIterations { get; set; }

        /// <summary>
        /// number of clusters
        /// </summary>
        [DataMember]
        public int NumberOfClusters { get; internal set; }

        /// <summary>
        /// a bool, if true Kmeans clustering start with an initial guess for the centroids else it will start with a random assignment
        /// </summary>
        //public bool InitialGuess { get; internal set; }

        /// <summary>
        /// Number of dimensions.
        /// </summary>
        [DataMember]
        public int NumOfDimensions { get; internal set; }

        /// <summary>
        /// If set, it will be used as initial centroids.
        /// </summary>
        [DataMember]
        public double[][] InitialCentroids { get; set; }

        /// <summary>
        /// A value in % representing the tolerance to possible outliers
        /// </summary>
        [DataMember]
        public double Tolerance { get; internal set; }

        /// <summary>
        /// Method to be used in function recognition (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids and In cluster Max Distance</li>
        /// <li> - 2: Min and Max Centoids</li>
        /// </ul>
        /// </summary>
        [DataMember]
        public int FuncRecogMethod { get; internal set; }

        /// <summary>
        /// Constructor to create the desired settings by the user for clustering.
        /// </summary>
        /// <param name="RawData">data to be clustered</param>
        /// <param name="KmeansMaxIterations">maximum allowed number of Kmeans iteration for clustering</param>
        /// <param name="numClusters">number of clusters</param>
        /// <param name="numDims">Number of dimensions.</param>
        /// <param name="KmeansAlgorithm">the desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul></param>
        /// <param name="initialCentroids">???</param>
        /// <param name="funcRecogMethod">Method to be used in function recognition (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids and In cluster Max Distance</li>
        /// <li> - 2: Min and Max Centoids</li>
        /// </ul></param>
        public ClusteringSettings(int KmeansMaxIterations, int numClusters, int numDims, int KmeansAlgorithm = 1, double[][] initialCentroids = null, double tolerance = 0, int funcRecogMethod = 1)
        {
            if (KmeansAlgorithm != 2)
            {
                this.KmeansAlgorithm = 1;
            }
            else
            {
                this.KmeansAlgorithm = 2;
            }
            if (funcRecogMethod != 2)
            {
                this.FuncRecogMethod = 1;
            }
            else
            {
                this.FuncRecogMethod = 2;
            }
            this.KmeansMaxIterations = KmeansMaxIterations;
            this.NumberOfClusters = numClusters;
            this.NumOfDimensions = numDims;
            this.InitialCentroids = initialCentroids;
            this.Tolerance = tolerance;
        }

        /// <summary>
        /// Clonses settings object.
        /// </summary>
        /// <returns></returns>
        internal ClusteringSettings Clone()
        {
            ClusteringSettings sett = this.MemberwiseClone() as ClusteringSettings;
            if (this.InitialCentroids != null)
            {
                List<double[]> copyCentroids = new List<double[]>();

                foreach (var centroid in this.InitialCentroids)
                {
                    Array arr = new double[centroid.Length];
                    centroid.CopyTo(arr, 0);
                    copyCentroids.Add(arr as double[]);
                }

                sett.InitialCentroids = copyCentroids.ToArray();
            }

            return sett;
        }
    }
}
