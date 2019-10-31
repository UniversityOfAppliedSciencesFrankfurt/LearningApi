using LearningFoundation;
using LearningFoundation.Clustering.KMeans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;

//namespace AnomDetect.KMeans.FunctionRecognition
namespace LearningFoundation.Clustering.KMeans.FunctionRecognition
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
            this.Score.Centroids = new double[settings.NumberOfClusters][];

            if(this.Settings.FuncRecogMethod == 1)
            {
                this.Score.InClusterMaxDistance = new double[settings.NumberOfClusters];
                this.Score.MaxCentroid = null;
                this.Score.MinCentroid = null;

                for (int i = 0; i < settings.NumberOfClusters; i++)
                {
                    this.Score.Centroids[i] = new double[settings.NumOfDimensions];
                }
            }
            else
            {
                this.Score.InClusterMaxDistance = null;
                this.Score.MaxCentroid = new double[settings.NumberOfClusters][];
                this.Score.MinCentroid = new double[settings.NumberOfClusters][];

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
  
        }


        /// <summary>
        /// Data of a single function for which KMeans will be calculated.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IScore Run(double[][] data, IContext ctx)
        {
            /*
            if (this.Score.NomOfTrainedFunctions == 1)
            {
                this.Settings.InitialCentroids = new double[this.Settings.NumberOfClusters][];
                for (int i = 0; i < this.Settings.NumberOfClusters; i++)
                {
                    this.Settings.InitialCentroids[i] = this.Score.Centroids[i];
                }
            }*/

            this.Settings.InitialCentroids = null;
            KMeansAlgorithm kmeans = new KMeansAlgorithm(this.Settings.Clone());

            KMeansScore res = kmeans.Train(data, ctx) as KMeansScore;
            this.Score.NomOfTrainedFunctions += 1;

            

            if (this.Settings.FuncRecogMethod == 1)
            {
                double[][] oldCentroids = this.Score.Centroids;
                for (int clusterIndx = 0; clusterIndx < res.Model.Clusters.Length; clusterIndx++)
                {
                    if (this.Score.NomOfTrainedFunctions == 1)
                    {
                        this.Score.Centroids[clusterIndx] = res.Model.Clusters[clusterIndx].Centroid;
                        //this.Score.InClusterMaxDistance[clusterIndx] = res.Model.Clusters[clusterIndx].InClusterMaxDistance;
                    }
                    else
                    {
                        for (int d = 0; d < this.Settings.NumOfDimensions; d++)
                        {
                            this.Score.Centroids[clusterIndx][d] = (res.Model.Clusters[clusterIndx].Centroid[d] + oldCentroids[clusterIndx][d] * (this.Score.NomOfTrainedFunctions - 1)) / this.Score.NomOfTrainedFunctions;
                        }

                        adjustInClusterMaxDistance(clusterIndx, res, oldCentroids);
                    }
                }
            }
            else
            {
                //Debug.WriteLine($"C0: {res.Model.Clusters[0].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
                //Debug.WriteLine($"C1: {res.Model.Clusters[1].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
                //Debug.WriteLine($"C2: {res.Model.Clusters[2].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");
                //Debug.WriteLine($"C3: {res.Model.Clusters[3].Centroid[0]},{res.Model.Clusters[0].Centroid[1]}");

                for (int clusterIndx = 0; clusterIndx < res.Model.Clusters.Length; clusterIndx++)
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



                for (int clusterIndex = 0; clusterIndex < res.Model.Clusters.Length; clusterIndex++)
                {
                    this.Score.Centroids[clusterIndex] = new double[Settings.NumOfDimensions];

                    for (int dim = 0; dim < Settings.NumOfDimensions; dim++)
                    {
                        if (this.Score.MinCentroid[clusterIndex][dim] >= 0)
                        {
                            this.Score.Centroids[clusterIndex][dim] = (this.Score.MaxCentroid[clusterIndex][dim] + this.Score.MinCentroid[clusterIndex][dim]) / 2;
                        }
                        else
                        {
                            this.Score.Centroids[clusterIndex][dim] = ((this.Score.MaxCentroid[clusterIndex][dim] - this.Score.MinCentroid[clusterIndex][dim]) / 2) + this.Score.MinCentroid[clusterIndex][dim];
                        }
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
            /*
            for (int i = 0; i < this.Settings.NumberOfClusters; i++)
            {
                this.Settings.InitialCentroids[i] = this.Score.Centroids[i];
            }*/
            this.Settings.InitialCentroids = null;
            KMeansAlgorithm kmeans = new KMeansAlgorithm(this.Settings.Clone());
            kmeans.Instance = null;
            KMeansScore res = kmeans.Train(funcData, ctx) as KMeansScore;

            int scores = 0;

            KMeansFunctionRecognitionResult predRes = new KMeansFunctionRecognitionResult();
            predRes.ResultsPerCluster = new bool[Settings.NumberOfClusters];
     
            double[][] results = new double[Settings.NumberOfClusters][];

            if (this.Settings.FuncRecogMethod == 1)
            {
                //double[] currDistance = new double[results.Length];
                double currDistance;
                for (int i = 0; i < results.Length; i++)
                {
                    currDistance = KMeansAlgorithm.CalculateDistance(Score.Centroids[i], res.Model.Clusters[i].Centroid);

                    if (currDistance <= Score.InClusterMaxDistance[i] * (1.0 + this.Settings.Tolerance / 100.0))
                    {
                        predRes.ResultsPerCluster[i] = true;
                        scores++;
                    }
                    else
                    {
                        predRes.ResultsPerCluster[i] = false;
                    }
                }
                predRes.Result = (scores == Settings.NumberOfClusters);
                predRes.Loss = ((float)scores) / (Settings.NumberOfClusters);
            }
            else
            {
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

            }


            return predRes;

        }

        public static int OptimalNumberOfClusters_TwoFunctions(double[][] functions1, double[][] functions2, ClusteringSettings settings, int MinNumClusters, int MaxNumClusters, out double[] Fmins_k)
        {
            if (MinNumClusters < 2)
            {
                MinNumClusters = 2;
            }
            
            double[][] oneFunction = new double[settings.NumOfDimensions][];
            int numFun = 0;
            double dist;
            int score = 0;
            KMeansFunctionRecognitionAlgorithm kMeansFR1, kMeansFR2;
            KMeansFunctionRecognitonScore res1, res2;

            double Fmax = 0;
            double Fmin = double.MaxValue;
            double[] Fmins;
            Fmins_k = new double[MaxNumClusters - MinNumClusters + 1];
            double F = 0;
            int cluster = -1;

            for (int k = MinNumClusters; k <= MaxNumClusters; k++)
            {
                
                settings.InitialCentroids = null;
                settings.NumberOfClusters = k;

                kMeansFR1 = new KMeansFunctionRecognitionAlgorithm(settings);
                kMeansFR2 = new KMeansFunctionRecognitionAlgorithm(settings);
                res1 = new KMeansFunctionRecognitonScore();
                res2 = new KMeansFunctionRecognitonScore();

                numFun = functions1.Length / settings.NumOfDimensions;
                for (int f = 0; f < numFun; f++)
                {
                    oneFunction = KMeansAlgorithm.TransposeFunction(KMeansAlgorithm.selectFunction(functions1, f + 1, settings.NumOfDimensions));
                    res1 = kMeansFR1.Run(oneFunction, null) as KMeansFunctionRecognitonScore;
                }

                numFun = functions2.Length / settings.NumOfDimensions;

                settings.InitialCentroids = null;
                for (int f = 0; f < numFun; f++)
                {
                    oneFunction = KMeansAlgorithm.TransposeFunction(KMeansAlgorithm.selectFunction(functions2, f + 1, settings.NumOfDimensions));
                    res2 = kMeansFR2.Run(oneFunction, null) as KMeansFunctionRecognitonScore;
                }

                Fmins = new double[k];
                // check if there is a non intersection cluster
                for (int i = 0; i < k; i++)
                {
                    Fmin = double.MaxValue;
                    score = 0;
                    for (int j = 0; j < k; j++)
                    {
                        dist = KMeansAlgorithm.CalculateDistance(res1.Centroids[i], res2.Centroids[j]);
                        if (dist > res1.InClusterMaxDistance[i] + res2.InClusterMaxDistance[j])
                        {
                            score++;
                        }
                    }

                    if (score == k)
                    {
                        //calculate F;
                        for (int j = 0; j < k; j++)
                        {
                            F = KMeansAlgorithm.CalculateDistance(res1.Centroids[i], res2.Centroids[j])/(res1.InClusterMaxDistance[i] + res2.InClusterMaxDistance[j]);
                            // select min F among the two functions
                            if (F < Fmin)
                            {
                                Fmin = F;
                            }
                        }
                    }
                    //save Fmin of each centroid
                    if (Fmin!=double.MaxValue)
                    {
                        Fmins[i] = Fmin;
                    }
                    
                }
                // save max Fmin per number of clusters
                for (int i = 0; i < k; i++)
                {
                    if (Fmins[i]>Fmins_k[k-MinNumClusters])
                    {
                        Fmins_k[k - MinNumClusters] = Fmins[i];
                    }
                }
            }

            //select max F among different number of cluters
            for (int i = 0; i < Fmins_k.Length; i++)
            {
                if (Fmins_k[i] > Fmax)
                {
                    Fmax = Fmins_k[i];
                    cluster = i + MinNumClusters;
                }
            }

            return cluster;
        }
       

        /// <summary>
        /// adjustInClusterMaxDistance is a function that recalculates/approximate the maximum distance in the cluster for partial clustering
        /// </summary>
        /// <param name="cluster">index of the cluster</param>
        private void adjustInClusterMaxDistance(int cluster, KMeansScore res, double[][] oldCentroids)
        {
            // calculate new in cluster max distance
            double curDistance = KMeansAlgorithm.CalculateDistance(res.Model.Clusters[cluster].Centroid, this.Score.Centroids[cluster]);
            

            // compare to max possible old in cluster max distance
            double oldDistance = this.Score.InClusterMaxDistance[cluster] + KMeansAlgorithm.CalculateDistance(this.Score.Centroids[cluster], oldCentroids[cluster]);

            if (oldDistance > curDistance)
            {
                curDistance = oldDistance;
            }

            this.Score.InClusterMaxDistance[cluster] = curDistance;
        }

    }
}
