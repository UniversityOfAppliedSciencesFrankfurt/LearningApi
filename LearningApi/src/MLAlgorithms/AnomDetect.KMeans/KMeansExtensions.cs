using LearningFoundation;
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
        /// UseKMeans is an extansion that call KMeans through LearningAPI
        /// </summary>
        /// <param name="api">the LearningAPI object</param>
        /// <param name="settings">the desired clustering settings</param>
        /// <returns></returns>
        public static LearningApi UseKMeans(this LearningApi api, ClusteringSettings settings, double[][] centroids = null, double[] maxDistance = null)
        {
            var alg = new KMeans(settings, centroids, maxDistance);
            api.AddModule(alg, "Rbm");
            return api;
        }
    }
}
