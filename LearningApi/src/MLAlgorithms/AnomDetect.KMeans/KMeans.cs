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

        public double[] Predict(double[][] data, IContext ctx)
        {
            int clusterIndex = -1;
            List<double> list = new List<double>();

            foreach (var item in data)
            {
                var res = kmeanApi.CheckSample(new CheckingSampleSettings(null, item, 0), out clusterIndex);
                list.Add(clusterIndex);
            }
            
            return list.ToArray();
        }

        public IScore Run(double[][] data, IContext ctx)
        {
            return Train(data, ctx);
        }

        public IScore Train(double[][] data, IContext ctx)
        {
            var r = this.kmeanApi.Training(data);
            KMeansResult res = new KMeansResult();
            res.Clusters = this.kmeanApi.Clusters;

            return res;
        }
    }
}
