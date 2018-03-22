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
        /// the resulting centroids
        /// </summary>
        [DataMember]
        public double[][] Centroids { get; internal set; }

        /// <summary>
        /// distance between the centroid and the farthest smaple in each cluster
        /// </summary>
        [DataMember]
        public double[] InClusterMaxDistance { get; internal set; }

        /// <summary>
        /// contains the assigned cluster number for each sample of the RawData
        /// </summary>
        [DataMember]
        public int[] DataToClusterMapping { get; internal set; }

        /// <summary>
        /// Number of currentlly calculated sampes in cluster.
        /// </summary>
        [DataMember]
        public double NumOfSamples { get; set; }

        /// <summary>
        /// Currentlly calculated mean values of clusters.
        /// M1 = mean of numOfSamples/2 (minibatch 1)
        /// M2 = mean for numbers from numOfSamples/2 to numOfSamples (minibatch 2)
        /// mean is batch for numbers from 1 to numOfSamples
        /// (1/q1+q2)[q1*M1+q2*M2]
        /// where q1 is number of elements inside of M1 and q2 number of elements inside of M2
        /// </summary>
        [DataMember]
        public double[] MeanValues { get; set; }


        /// <summary>
        /// Cluster statistics.
        /// </summary>
        [DataMember]
        public Cluster[] Clusters { get; set; }


        /// <summary>
        /// Constructor for creating instance object
        /// </summary>
        /// <param name="data">Data to be clustered</param>
        /// <param name="numOfClusters">Desired number of clusters</param>
        /// <param name="centroids">Explicitelly set centroids. This value is typically set in a case of pattern recognition.
        /// For each pattern recognition related clustering, centroids are set on some reference value for each set of data samples.</param>
        public KMeansModel(double[][] data, int numOfClusters, double[][] centroids = null)
        {
            //this.RawData = data;
            this.NumberOfClusters = numOfClusters;
            this.Centroids = centroids;
        }
    }
}
