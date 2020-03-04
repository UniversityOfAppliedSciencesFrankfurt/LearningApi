//using AnomDetect.KMeans;
//using AnomDetect.KMeans.FunctionRecognition;
using LearningFoundation;
using LearningFoundation.Clustering.KMeans.FunctionRecognition;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// KMeansExtensions is an extension class for KMeans so it can be called through LearningAPI.
    /// </summary>
    public static class KMeansExtensions
    {
        /// <summary>
        /// UseKMeans is an extension that call KMeans through LearningAPI
        /// </summary>
        /// <param name="api">the LearningAPI object</param>
        /// <param name="settings">the desired clustering settings</param>
        /// <returns></returns>
        public static LearningApi UseKMeans(this LearningApi api, ClusteringSettings settings, double[] maxDistance = null)
        {
            var alg = new KMeansAlgorithm(settings.Clone(), maxDistance);
            api.AddModule(alg, "Rbm");
            return api;
        }


        /// <summary>
        /// Installs the KMeanFunctionRecognitionModule in the LearningApi pipeline.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static LearningApi UseKMeansFunctionRecognitionModule(this LearningApi api, ClusteringSettings settings)
        {
            var alg = new KMeansFunctionRecognitionAlgorithm(settings);
            api.AddModule(alg, "KMeanFunctionRecognitionModule");
            return api;
        }
    }
}
