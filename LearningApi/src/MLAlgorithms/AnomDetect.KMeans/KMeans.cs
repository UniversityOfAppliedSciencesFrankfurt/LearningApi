using AnomalyDetection.Interfaces;
using AnomalyDetectionApi;
using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AnomDetect.KMeans
{
    public class KMeans : IAlgorithm
    {
        private AnomalyDetectionAPI kmeanApi;

        private ClusteringSettings m_Settings;

        public KMeans(ClusteringSettings settings)
        {
            this.m_Settings = settings;
            
            kmeanApi = new AnomalyDetectionAPI(settings); 
        }

     

        public IResult Predict(double[][] data, IContext ctx)
        {
            KMeansResult res = new KMeansResult()
            {
                PredictedClusters = new int[data.Length],
            };

            int clusterIndex = -1;
            List<double> list = new List<double>();

            int n = 0;
            foreach (var item in data)
            {
                var r = kmeanApi.CheckSample(new CheckingSampleSettings(null, item, 0), out clusterIndex);
                res.PredictedClusters[n++] = clusterIndex;
            }

            return res;
        }

        public IScore Run(double[][] data, IContext ctx)
        {
            return Train(data, ctx);
        }

        public IScore Train(double[][] data, IContext ctx)
        {
            var r = this.kmeanApi.Training(data);
            KMeansScore res = new KMeansScore();
            res.Clusters = this.kmeanApi.Clusters;

            return res;
        }

        //public object GetModel()
        //{

        //}
    }
}
