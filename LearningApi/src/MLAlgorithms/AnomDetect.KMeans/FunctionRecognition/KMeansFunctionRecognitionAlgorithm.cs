using LearningFoundation;
using LearningFoundation.Clustering.KMeans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;


namespace AnomDetect.KMeans.FunctionRecognition
{
    /// <summary>
    /// Module, which implements algorithm for function recognition based on KMeans.
    /// This module must be invoked by RunBatch (Run is not supported) to train the model.
    /// Model training gets as data function data in a batch. It means every batch contains data of a single function.
    /// The module internally uses KMeansAlgorithm to calculate clusters for every single function (batch) and keeps minimum, maximum and
    /// center value of every centroid of ecvery cluster.
    /// When the Predict method is called, it also uses KMeansAlgorithm to calculate clusters for predicting function, which is input of the method <see cref="Predict(double[][], IContext)"/>.
    /// Prediction will check if the calculated clusters of predicting function fits in relation MinCentroids less or equal predicted centroids less or equal MaxCentroids.
    /// Intuitivelly, predicted clusters (clusters) must fit between minimum and maximum centroids of all trained functions.
    /// </summary>
    public class KMeansFunctionRecognitionAlgorithm : IAlgorithm
    {
        private ClusteringSettings m_Settings;

        private KMeansFunctionRecognitonScore m_Score;

        public KMeansFunctionRecognitionAlgorithm(ClusteringSettings settings)
        {
            this.m_Settings = settings;
            this.m_Score = new KMeansFunctionRecognitonScore();
            this.m_Score.MaxCentroid = new double[settings.NumberOfClusters][];
            this.m_Score.MinCentroid = new double[settings.NumberOfClusters][];
            this.m_Score.Centroids = new double[settings.NumberOfClusters][];

            for (int i = 0; i < settings.NumberOfClusters; i++)
            {
                this.m_Score.MaxCentroid[i] = new double[settings.NumOfDimensions];
                this.m_Score.MinCentroid[i] = new double[settings.NumOfDimensions];
                this.m_Score.Centroids[i] = new double[settings.NumOfDimensions];

                for (int dim = 0; dim < settings.NumOfDimensions; dim++)
                {
                    this.m_Score.MaxCentroid[i][dim] = double.MinValue;
                    this.m_Score.MinCentroid[i][dim] = double.MaxValue;
                }
            }
        }


        /// <summary>
        /// Data of a single function for which KMeans will be calculated.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IScore Run(double[][] data, IContext ctx)
        {
            KMeansAlgorithm kmeans = new KMeansAlgorithm(this.m_Settings);

            KMeansScore res = kmeans.Train(data, ctx) as KMeansScore;

            //Debug.WriteLine($"C0: {res.Model.Clusters[0].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
            //Debug.WriteLine($"C1: {res.Model.Clusters[1].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
            //Debug.WriteLine($"C2: {res.Model.Clusters[2].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
            //Debug.WriteLine($"C3: {res.Model.Clusters[3].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");

            for (int clusterIndx=0; clusterIndx < res.Model.Clusters.Length;clusterIndx++)
            {
                for (int dim = 0; dim < this.m_Settings.NumOfDimensions; dim++)
                {
                    if (res.Model.Clusters[clusterIndx].Centroid[dim] > this.m_Score.MaxCentroid[clusterIndx][dim])
                    {
                        this.m_Score.MaxCentroid[clusterIndx][dim] = res.Model.Clusters[clusterIndx].Centroid[dim];
                    }

                    if (res.Model.Clusters[clusterIndx].Centroid[dim] < this.m_Score.MinCentroid[clusterIndx][dim])
                    {
                        this.m_Score.MinCentroid[clusterIndx][dim] = res.Model.Clusters[clusterIndx].Centroid[dim];
                    }
                }
            }

            this.m_Score.NomOfSamples += data.Length;

            for (int clusterIndex = 0; clusterIndex < res.Model.Clusters.Length; clusterIndex++)
            {
                this.m_Score.Centroids[clusterIndex] = new double[m_Settings.NumOfDimensions];

                for (int dim = 0; dim < m_Settings.NumOfDimensions; dim++)
                {
                    if (this.m_Score.MinCentroid[clusterIndex][dim] >= 0)
                    {
                        this.m_Score.Centroids[clusterIndex][dim] = (this.m_Score.MaxCentroid[clusterIndex][dim] + this.m_Score.MinCentroid[clusterIndex][dim])/2;
                    }
                    else
                    {
                        this.m_Score.Centroids[clusterIndex][dim] = ((this.m_Score.MaxCentroid[clusterIndex][dim] - this.m_Score.MinCentroid[clusterIndex][dim]) / 2) + this.m_Score.MinCentroid[clusterIndex][dim];
                    }
                }
            }

            return m_Score;
        }

        /// <summary>
        /// Invokes Run.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IScore Train(double[][] data, IContext ctx)
        {
            return Run(data, ctx);
        }


        /// <summary>
        /// Predicts if the specified function fits in the trainined MIN-MAX cluster interval.
        /// All calculated clusters must fit in trained cluster MIN-MAX intervals.
        /// </summary>
        /// <param name="funcData"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IResult Predict(double[][] funcData, IContext ctx)
        {
            KMeansAlgorithm kmeans = new KMeansAlgorithm(this.m_Settings);

            KMeansScore res = kmeans.Train(funcData, ctx) as KMeansScore;

            int scores = 0;

            KMeansFuncionRecognitionResult predRes = new KMeansFuncionRecognitionResult();
            predRes.ResultsPerCluster = new bool[m_Settings.NumberOfClusters];
     
            double[][] results = new double[m_Settings.NumberOfClusters][];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new double[m_Settings.NumOfDimensions];
                for (int dim = 0; dim < m_Settings.NumOfDimensions; dim++)
                {
                    if (res.Model.Clusters[i].Centroid[dim] >= m_Score.MinCentroid[i][dim] &&
                        res.Model.Clusters[i].Centroid[dim] <= m_Score.MaxCentroid[i][dim])
                    {
                        results[i][dim] = 1;
                        scores++;                       
                    }
                    else
                    {
                        results[i][dim] = 0;
                    }

                    //
                    // We calculate here the result of cluster over all dimensions. If all dimensions fits then cluster result is true.
                    if (results[i].Count(r => r == 1) == m_Settings.NumOfDimensions)
                        predRes.ResultsPerCluster[i] = true;
                    else
                        predRes.ResultsPerCluster[i] = false;
                }     
            }

           
            predRes.Result = (scores == m_Settings.NumberOfClusters * m_Settings.NumOfDimensions);
            predRes.Loss = ((float)scores) / (m_Settings.NumberOfClusters * m_Settings.NumOfDimensions);
   
            return predRes;
        }
    }
}
