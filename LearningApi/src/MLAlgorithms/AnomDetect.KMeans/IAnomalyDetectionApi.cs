
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnomalyDetection.Interfaces
{
    /// <summary>
    /// Interface for AnomalyDetectionAPI
    /// </summary>
    public interface IAnomalyDetectionApi

    {
        /// <summary>
        /// ImportNewDataForClustering is a function that start a new clustering instance or add to an existing one. It saves the results automatically.
        /// </summary>
        /// <param name="Settings">Contains the desired settings for the clustering process</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "Clustering Complete. K-means stopped at the maximum allowed iteration: " + Maximum_Allowed_Iteration </li>
        /// <li> or </li>
        /// <li> - Code: 0, "Clustering Complete. K-means converged at iteration: " + Iteration_Reached </li>
        /// </ul>
        /// </returns>
        AnomalyDetectionResponse Training(double[][] rawData, double[][] centroids = null);

        /// <summary>
        /// CheckSample is a function that detects to which cluster the given sample belongs to.
        /// </summary>
        /// <param name="Settings">Contains the desired settings for detecting to which, if any, cluster the sample belongs</param>
        /// <param name="ClusterIndex">The cluster number to which the sample belongs (-1 if the sample doesn't belong to any cluster or if an error was encountered).</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "This sample belongs to cluster: " + Cluster_Number </li>
        /// <li> or </li>
        /// <li> - Code: 1, "This sample doesn't belong to any cluster" </li>
        /// </ul>       
        /// </returns>
        AnomalyDetectionResponse CheckSample(CheckingSampleSettings Settings, out int ClusterIndex);

        /// <summary>
        /// GetResults is a function that returns the results of an existing clustering instance 
        /// </summary>
        /// <param name="LoadSettings">Settings to load the clustering instance</param>
        /// <param name="Result">The variable through which the clustering result are returned</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        AnomalyDetectionResponse GetClusters(string path, out Cluster[] Result);

        /// <summary>
        /// GetPreviousSamples is a function that loads samples from a previous clustering instance
        /// </summary>
        /// <param name="path">Instance path</param>
        /// <param name="OldSamples">The variable through which the samples are returned</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        AnomalyDetectionResponse GetPreviousSamples(string path, out double[][] OldSamples);

        /// <summary>
        /// RecommendedNumberOfClusters is a function that returns a recommended number of clusters for the given samples.
        /// </summary>
        /// <param name="RawData">The samples to be clustered</param>
        /// <param name="KmeansMaxIterations">Maximum allowed number of Kmeans iteration for clustering</param>
        /// <param name="KmeansAlgorithm">The desired Kmeans clustering algorithm (1 or 2)
        /// <ul style="list-style-type:none">
        /// <li> - 1: Centoids are the nearest samples to the means</li>
        /// <li> - 2: Centoids are the means</li>
        /// </ul></param>
        /// <param name="NumberOfAttributes">Number of attributes for each sample</param>
        /// <param name="MaxNumberOfClusters">Maximum desired number of clusters</param>
        /// <param name="MinNumberOfClusters">Minimum desired number of clusters</param>
        /// <param name="Method">Integer 0,1 or 2 representing the method to be used. 
        /// <ul style = "list-style-type:none" >
        /// <li> - Method 0: Radial method in which the farthest sample of each cluster must be closer to the cluster centoid than the nearest foreign sample of the other clusters </li>
        /// <li> - Method 1: Standard Deviation method in which the standard deviation in each cluster must be less than the desired standard deviation </li>
        /// <li> - Method 2: Both. uses radial and standard deviation methods at the same time </li>
        /// </ul>
        /// </param>
        /// <param name="StdDev">The desired standard deviation upper limit in each cluster</param>
        /// <param name="RecNumberOfClusters">The variable through which the recommended number of clusters is returned</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style = "list-style-type:none" >
        /// <li> - Code: 0, "OK" </li>
        /// <li> or </li>
        /// <li> - Code: 1, "Could not find a recommended number of clusters based on the desired constraints" </li>
        /// </ul>
        /// </returns>
        AnomalyDetectionResponse RecommendedNumberOfClusters(double[][] RawData, int KmeansMaxIterations, int NumberOfAttributes, int MaxNumberOfClusters, int MinNumberOfClusters, int Method, double[] StdDev, out int RecNumberOfClusters, int KmeansAlgorithm = 1, double[][] centroids = null);
    }
}
