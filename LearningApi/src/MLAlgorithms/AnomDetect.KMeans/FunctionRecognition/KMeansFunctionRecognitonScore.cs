using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomDetect.KMeans.FunctionRecognition
{
    /// <summary>
    /// KMeansScore is a class that contains the response object of the KMeansFunctionRecognitionModule' training function. 
    /// </summary>
    public class KMeansFunctionRecognitonScore : IScore
    {
        public KMeansFunctionRecognitonScore()
        {
        }

        /// <summary>
        /// Number of data samples across all trained functions.
        /// </summary>
        public int NomOfSamples { get; set; }

        /// <summary>
        /// Number of trained functions.
        /// </summary>
        public int NomOfTrainedFunctions { get; set; }

        /// <summary>
        /// The centroid calculated as middle between min and max value of every dimension..
        /// </summary>
        public double[][] Centroids { get; set; }

        /// <summary>
        /// The minimum centroid value per each dimension.
        /// </summary>
        public double[][] MinCentroid { get; set ; }

        /// <summary>
        /// The maximum centroid value per each dimension.
        /// </summary>
        public double[][] MaxCentroid { get; set; }
    }
}
