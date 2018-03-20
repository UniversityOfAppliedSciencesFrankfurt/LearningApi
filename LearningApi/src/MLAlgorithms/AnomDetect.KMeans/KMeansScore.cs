using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// KMeansScore is a class that contains the response object of the KMeans' training function. 
    /// </summary>
    public class KMeansScore : IScore
    {
        public KMeansScore()
        {
        }
        public double[] Errors { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double[] Weights { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Array of the obtained clusters
        /// </summary>
        public Cluster[] Clusters { get; internal set; }

        /// <summary>
        /// the obtained clustering instance
        /// </summary>
        public Instance instance { get; internal set; }

        /// <summary>
        /// A message to the user
        /// </summary>
        public string Message { get; internal set; }
    }
}
