using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// Instance is a class representing an instance of a clustering run (preliminary results). 
    /// </summary>
    [DataContract]
    public class KMeansModel : IModel
    {
        /// <summary>
        /// desired number of clusters
        /// </summary>
        [DataMember]
        public int NumberOfClusters { get; internal set; }


        /// <summary>
        /// Contains the assigned cluster number for each sample of the RawData.
        /// </summary>
        /// <remarks>Not persistable.</remarks>
        public int[] DataToClusterMapping { get; internal set; }

        /// <summary>
        /// Number of currentlly calculated samples in cluster.
        /// </summary>
        public int NumOfSamples { get; set; }

  
        /// <summary>
        /// Cluster statistics.
        /// </summary>
        [DataMember]
        public Cluster[] Clusters { get; set; }

        public double[] Fmin { get; set; }

        public double[] D { get; set; }

        public double[] DPrime { get; set; }

        /// <summary>
        /// Constructor for creating instance object
        /// </summary>
        /// <param name="numOfClusters">Desired number of clusters</param>
        /// <param name="numOfSamples">number of samples</param>
        public KMeansModel(int numOfClusters, int numOfSamples = 0)
        {
            //this.RawData = data;
            this.NumberOfClusters = numOfClusters;
            this.NumOfSamples = numOfSamples;            
            this.Clusters = new Cluster[numOfClusters];
            for (int i = 0; i < numOfClusters; i++)
            {
                this.Clusters[i] = new Cluster();
            }
        }
    }
}
