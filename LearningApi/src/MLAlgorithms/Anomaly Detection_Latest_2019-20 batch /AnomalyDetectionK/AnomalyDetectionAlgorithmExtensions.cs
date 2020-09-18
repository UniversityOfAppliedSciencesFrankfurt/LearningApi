using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetectionK
{
    /// <summary>
    /// Anomaly Detection with k-mean clustering Algorithom Extensions
    /// </summary>
    public static class AnomalyDetectionAlgorithmExtensions
    {
        
        /// <summary>
        /// Deserializing and parsing of the csv file.
        /// </summary>
        /// <param name="api">Object</param>
        /// <param name="learningRate">learningRate</param>
        public static LearningApi UseADAlgorithm(this LearningApi api, double learningRate)
        {
            Project ADAlgorithm = new Project(learningRate, 2.3);
            api.AddModule(ADAlgorithm, "Iproject");
            return api;
        }
    }
}
