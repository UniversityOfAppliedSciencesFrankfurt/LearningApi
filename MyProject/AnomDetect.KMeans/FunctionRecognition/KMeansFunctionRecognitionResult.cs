using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;


//namespace AnomDetect.KMeans.FunctionRecognition
namespace LearningFoundation.Clustering.KMeans.FunctionRecognition
{
    public class KMeansFunctionRecognitionResult : IResult
    {
        /// <summary>
        /// TRUE if all clusters fits between min and max.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// (result of cluster 1 + .. + result of clusterN) / N
        /// where N is number of clusters.
        /// result of cluster K is 1 if the cluster fits between Min and Max.
        /// </summary>
        public float Loss { get; set; }

        /// <summary>
        /// TRUE if the cluster fits between Min and Max.
        /// </summary>
        public bool[] ResultsPerCluster { get; set; }
    }
}
