using AnomalyDetection.Interfaces;
using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomDetect.KMeans
{
    public static class KMeansExtensions
    {
        public static LearningApi UseKMeans(this LearningApi api, ClusteringSettings settings)
        {
            var alg = new KMeans(settings);
            api.AddModule(alg, "Rbm");
            return api;
        }
    }
}
