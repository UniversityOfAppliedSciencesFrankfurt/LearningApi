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
    /// <summary>
    /// Trains the K-Means algorithm using coordinates and
    /// also get prediction results from it.
    /// </summary>
    internal class MouseGestureRecognizer
    {
        private LearningApi m_LearningApi;
        private double[][] m_CurrentData;
        private ClusteringSettings m_Settings;

        /// <summary>
        /// Provide settings for K-Means algorithm.
        /// </summary>
        public MouseGestureRecognizer()
        {
            m_LearningApi = new LearningApi();

            m_LearningApi.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                return m_CurrentData;
            });

            double[][] initCentroids = new double[2][];
            initCentroids[0] = new double[] { 0.0, 0.0, 0.0 };
            initCentroids[1] = new double[] { 400.0, 0.0, 0.0 };

            m_Settings = new ClusteringSettings(KmeansMaxIterations: 1000, numClusters: 2, numDims: 3, KmeansAlgorithm: 2, initialCentroids: initCentroids, tolerance: 0);
            
            m_LearningApi.UseKMeansFunctionRecognitionModule(m_Settings);
        }

        /// <summary>
        /// Trains the K-Means algorithm using coordinates.
        /// </summary>
        /// <param name="points">Coordinate Points</param>
        public void Train(List<Coordinates> points)
        {
            m_CurrentData = GetDataFromPoints(points);
            KMeansFunctionRecognitonScore res = m_LearningApi.RunBatch() as KMeansFunctionRecognitonScore;
        }

        /// <summary>
        /// Converts coordinate points to double[][] data for
        /// training and prediction.
        /// </summary>
        /// <param name="points">Coordinate Points</param>
        /// <returns>Returns data for Training and Prediction</returns>
        private double[][] GetDataFromPoints(List<Coordinates> points)
        {
            double[][] data = new double[points.Count][];

            for (int i = 0; i < points.Count; i++)
            {
                data[i] = new double[3];
                data[i][0] = (double)points[i].X;
                data[i][1] = (double)points[i].Y;
                data[i][2] = (double)points[i].T;
            }
            return data;
        }

        /// <summary>
        /// Gives the prediction result.
        /// </summary>
        /// <param name="points">Coordinate Points</param>
        /// <returns>Returns Prediction result as True or False</returns>
        public bool IsThisRightGesture(List<Coordinates> points)
        {
            double[][] data = GetDataFromPoints(points);

            var predictionResult = m_LearningApi.Algorithm.Predict(data, null) as KMeansFunctionRecognitionResult;

            return predictionResult.Result;
        }
    }
}
