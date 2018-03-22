using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// ClusteringSettings is a class that contains the desired settings by the user for clustering.
    /// </summary>
    public class ClusteringSettings
    {
        /// <summary>
        /// the desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul>
        /// </summary>
        public int KmeansAlgorithm { get; internal set; }

        /// <summary>
        /// maximum allowed number of Kmeans iteration for clustering
        /// </summary>
        public int KmeansMaxIterations { get; internal set; }

        /// <summary>
        /// number of clusters
        /// </summary>
        public int NumberOfClusters { get; internal set; }

        /// <summary>
        /// a bool, if true Kmeans clustering start with an initial guess for the centroids else it will start with a random assignment
        /// </summary>
        //public bool InitialGuess { get; internal set; }

        /// <summary>
        /// number of attributes for each sample
        /// </summary>
        public int NumberOfAttributes { get; internal set; }

        /// <summary>
        /// If set, it will be used as initial centroids.
        /// </summary>
        public double[][] InitialCentroids { get; set; }

        /// <summary>
        /// Constructor to create the desired settings by the user for clustering.
        /// </summary>
        /// <param name="RawData">data to be clustered</param>
        /// <param name="KmeansMaxIterations">maximum allowed number of Kmeans iteration for clustering</param>
        /// <param name="NumberOfClusters">number of clusters</param>
        /// <param name="NumberOfAttributes">number of attributes for each sample</param>
        /// <param name="KmeansAlgorithm">the desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul></param>
        /// <param name="InitialGuess">a bool, if true Kmeans clustering start with an initial guess for the centroids else it will start with a random assignment.</param>
        /// <param name="Replace"></param>
        public ClusteringSettings(int KmeansMaxIterations, int NumberOfClusters, int NumberOfAttributes, int KmeansAlgorithm = 1,  bool Replace = false,double[][] initialCentroids = null)
        {
            if (KmeansAlgorithm != 2)
            {
                this.KmeansAlgorithm = 1;
            }
            else
            {
                this.KmeansAlgorithm = 2;
            }
            this.KmeansMaxIterations = KmeansMaxIterations;
            this.NumberOfClusters = NumberOfClusters;
            this.NumberOfAttributes = NumberOfAttributes;
            this.InitialCentroids = initialCentroids;
        }

    }
}
