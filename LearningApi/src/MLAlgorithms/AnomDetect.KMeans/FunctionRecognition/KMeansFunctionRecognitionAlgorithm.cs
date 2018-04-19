using LearningFoundation;
using LearningFoundation.Clustering.KMeans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;

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
        [DataMember]
        public ClusteringSettings Settings;

        [DataMember]
        public KMeansFunctionRecognitonScore Score;

        public KMeansFunctionRecognitionAlgorithm(ClusteringSettings settings)
        {
            this.Settings = settings;
            this.Score = new KMeansFunctionRecognitonScore();
            this.Score.MaxCentroid = new double[settings.NumberOfClusters][];
            this.Score.MinCentroid = new double[settings.NumberOfClusters][];
            this.Score.Centroids = new double[settings.NumberOfClusters][];

            for (int i = 0; i < settings.NumberOfClusters; i++)
            {
                this.Score.MaxCentroid[i] = new double[settings.NumOfDimensions];
                this.Score.MinCentroid[i] = new double[settings.NumOfDimensions];
                this.Score.Centroids[i] = new double[settings.NumOfDimensions];

                for (int dim = 0; dim < settings.NumOfDimensions; dim++)
                {
                    this.Score.MaxCentroid[i][dim] = double.MinValue;
                    this.Score.MinCentroid[i][dim] = double.MaxValue;
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
            KMeansAlgorithm kmeans = new KMeansAlgorithm(this.Settings);

            KMeansScore res = kmeans.Train(data, ctx) as KMeansScore;

            //Debug.WriteLine($"C0: {res.Model.Clusters[0].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
            //Debug.WriteLine($"C1: {res.Model.Clusters[1].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
            //Debug.WriteLine($"C2: {res.Model.Clusters[2].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
            //Debug.WriteLine($"C3: {res.Model.Clusters[3].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");

            for (int clusterIndx=0; clusterIndx < res.Model.Clusters.Length;clusterIndx++)
            {
                for (int dim = 0; dim < this.Settings.NumOfDimensions; dim++)
                {
                    if (res.Model.Clusters[clusterIndx].Centroid[dim] > this.Score.MaxCentroid[clusterIndx][dim])
                    {
                        this.Score.MaxCentroid[clusterIndx][dim] = res.Model.Clusters[clusterIndx].Centroid[dim];
                    }

                    if (res.Model.Clusters[clusterIndx].Centroid[dim] < this.Score.MinCentroid[clusterIndx][dim])
                    {
                        this.Score.MinCentroid[clusterIndx][dim] = res.Model.Clusters[clusterIndx].Centroid[dim];
                    }
                }
            }

            this.Score.NomOfSamples += data.Length;

            for (int clusterIndex = 0; clusterIndex < res.Model.Clusters.Length; clusterIndex++)
            {
                this.Score.Centroids[clusterIndex] = new double[Settings.NumOfDimensions];

                for (int dim = 0; dim < Settings.NumOfDimensions; dim++)
                {
                    if (this.Score.MinCentroid[clusterIndex][dim] >= 0)
                    {
                        this.Score.Centroids[clusterIndex][dim] = (this.Score.MaxCentroid[clusterIndex][dim] + this.Score.MinCentroid[clusterIndex][dim])/2;
                    }
                    else
                    {
                        this.Score.Centroids[clusterIndex][dim] = ((this.Score.MaxCentroid[clusterIndex][dim] - this.Score.MinCentroid[clusterIndex][dim]) / 2) + this.Score.MinCentroid[clusterIndex][dim];
                    }
                }
            }

            return Score;
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
            KMeansAlgorithm kmeans = new KMeansAlgorithm(this.Settings);

            KMeansScore res = kmeans.Train(funcData, ctx) as KMeansScore;

            int scores = 0;

            KMeansFuncionRecognitionResult predRes = new KMeansFuncionRecognitionResult();
            predRes.ResultsPerCluster = new bool[Settings.NumberOfClusters];
     
            double[][] results = new double[Settings.NumberOfClusters][];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new double[Settings.NumOfDimensions];
                for (int dim = 0; dim < Settings.NumOfDimensions; dim++)
                {
                    if (res.Model.Clusters[i].Centroid[dim] >= Score.MinCentroid[i][dim] &&
                        res.Model.Clusters[i].Centroid[dim] <= Score.MaxCentroid[i][dim])
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
                    if (results[i].Count(r => r == 1) == Settings.NumOfDimensions)
                        predRes.ResultsPerCluster[i] = true;
                    else
                        predRes.ResultsPerCluster[i] = false;
                }     
            }

           
            predRes.Result = (scores == Settings.NumberOfClusters * Settings.NumOfDimensions);
            predRes.Loss = ((float)scores) / (Settings.NumberOfClusters * Settings.NumOfDimensions);
   
            return predRes;
        }
    }
}
