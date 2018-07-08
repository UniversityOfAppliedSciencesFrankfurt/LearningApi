
using LearningFoundation;
using LearningFoundation.Clustering.KMeans;
using LearningFoundation.Clustering.KMeans.FunctionRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MouseGestureRecognition
{
    internal class MouseGestureRecognizer
    {
        private LearningApi m_LearningApi;
        private double[][] m_CurrentData;
        private ClusteringSettings m_Settings;
        public MouseGestureRecognizer()
        {
            m_LearningApi = new LearningApi();

            m_LearningApi.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                return m_CurrentData;
            });

            double[][] initCentroids = new double[2][];
            initCentroids[0] = new double[] { 0.0, 0.0 };
            initCentroids[1] = new double[] { 400.0, 0.0 };
            //initCentroids[2] = new double[] { 0.0, 200.0 };
            //initCentroids[3] = new double[] { 400.0, 200.0 };

            m_Settings = new ClusteringSettings(KmeansMaxIterations: 1000, numClusters: 2, numDims: 2, KmeansAlgorithm: 2, initialCentroids: initCentroids, tolerance: 0);
            
            m_LearningApi.UseKMeansFunctionRecognitionModule(m_Settings);
        }

        public void Train(List<Point> points)
        {
            //double[][] initCentroids = new double[2][];
            //initCentroids[0] = new double[] { points.First().X, points.First().Y };
            //initCentroids[1] = new double[] { points.Last().X, points.Last().Y };
            //m_Settings.InitialCentroids = initCentroids;

            m_CurrentData = getDataFromPoints(points);
            KMeansFunctionRecognitonScore res = m_LearningApi.RunBatch() as KMeansFunctionRecognitonScore;
        }

        private double[][] getDataFromPoints(List<Point> points)
        {
            double[][] data = new double[points.Count][];

            for (int i = 0; i < points.Count; i++)
            {
                data[i] = new double[2];
                data[i][0] = (double)points[i].X;
                data[i][1] = (double)points[i].Y;
            }

            return data;
        }

        public bool IsThisRightGesture(List<Point> points)
        {
            double[][] data = getDataFromPoints(points);

            var predictionResult = m_LearningApi.Algorithm.Predict(data, null) as KMeansFunctionRecognitionResult;

            return predictionResult.Result;
        }
    }
}
