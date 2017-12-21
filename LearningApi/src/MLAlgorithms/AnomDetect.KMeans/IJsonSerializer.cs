using AnomalyDetection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetectionApi
{
    /// <summary>
    /// ISaveLoad is the defined interface for saving and loading methods.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Save is a function that saves an instance of AnomalyDetectionAPI.
        /// </summary>
        /// <param name="SaveObject">Settings to save the AnomalyDetectionAPI instance</param>
        /// <param name="Instance">AnomalyDetectionAPI object to be saved</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        AnomalyDetectionResponse Save(string path, Instance instance);

        /// <summary>
        /// Save is a function that saves the clustering results.
        /// </summary>
        /// <param name="SaveObject">Settings to save the AnomalyDetectionAPI instance</param>
        /// <param name="Results">The  clustering results object to be saved</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        AnomalyDetectionResponse Save(string path, Cluster[] Results);

        /// <summary>
        /// SaveChecks is a function that checks the saving settings for errors. Some errors can be corrected.
        /// </summary>
        /// <param name="SaveObject">Settings to save</param>
        /// <param name="CheckedSaveObject">The checked settings to save</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        AnomalyDetectionResponse ValidateSaveConditions(SaveLoadSettings SaveObject, out SaveLoadSettings CheckedSaveObject);

        /// <summary>
        /// LoadJSON_AnomalyDetectionAPI is a function that deserializes and loads an AnomalyDetectionAPI object from a JSON file.
        /// </summary>
        /// <param name="LoadObject">Settings to load the AnomalyDetectionAPI instance</param>
        /// <returns>
        /// - Item 1: The loaded AnomalyDetectionAPI object <br />
        /// - Item 2: A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        Tuple<Instance, AnomalyDetectionResponse> ReadJsonObject(string path);

        /// <summary>
        /// LoadJSON_ClusteringResults is a function that deserializes and loads a ClusteringResults[] object from a JSON file.
        /// </summary>
        /// <param name="LoadObject">Settings to load the ClusteringResults[] object</param>
        /// <returns>
        /// - Item 1: The loaded ClusteringResults[] object <br />
        /// - Item 2: A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        Tuple<Cluster[], AnomalyDetectionResponse> GetClusters(string path);

        /// <summary>
        /// LoadChecks is a function that checks the load settings for errors. Some errors can be corrected.
        /// </summary>
        /// <param name="LoadObject">Settings to load</param>
        /// <param name="CheckedLoadObject">tThe checked settings to load</param>
        /// <returns>A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        AnomalyDetectionResponse Validate(SaveLoadSettings LoadObject, out SaveLoadSettings CheckedLoadObject);
    }
}