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
      
        /// <summary>
        /// the obtained clustering instance
        /// </summary>
        public KMeansModel Model { get; internal set; }

        /// <summary>
        /// A message to the user
        /// </summary>
        public string Message { get; internal set; }
    }
}
