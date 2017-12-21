using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AnomalyDetectionApi
{
    [DataContract]
    public class Instance
    {
        /// <summary>
        /// data to be clustered
        /// </summary>
        [DataMember]
        public double[][] RawData { get; internal set; }

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
        /// Constructor for creating AnomalyDetectionAPI object
        /// </summary>
        /// <param name="data">Data to be clustered</param>
        /// <param name="numOfClusters">Desired number of clusters</param>
        /// <param name="centroids">Explicitelly set centroids. This value is typically set in a case of pattern recognition.
        /// For each pattern recognition related clustering, centroids are set on some reference value for each set of data samples.</param>
        public Instance(double[][] data, int numOfClusters, double[][] centroids = null)
        {
            this.RawData = data;
            this.NumberOfClusters = numOfClusters;
            this.Centroids = centroids;
        }

    }
}
