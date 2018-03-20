using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// KMeansResult is a class that contains the response object of the KMeans' Predict function. 
    /// </summary>
    public class KMeansResult : IResult
    {
        /// <summary>
        /// Array of the cluster prediction
        /// </summary>
        public int[] PredictedClusters { get; set; }
    }
}
