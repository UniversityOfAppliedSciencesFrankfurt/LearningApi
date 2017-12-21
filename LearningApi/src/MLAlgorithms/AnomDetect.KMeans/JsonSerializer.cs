using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnomalyDetection.Interfaces;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace AnomalyDetectionApi
{
    /// <summary>
    /// JSON_SaveLoad is class that saves to or loads from JSON files.
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// Save is a function that saves an Instance of AnomalyDetectionAPI.
        /// </summary>
        /// <param name="settings">settings to save the AnomalyDetectionAPI instance</param>
        /// <param name="instance">AnomalyDetectionAPI object to be saved</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse Save(string path, Instance instance)
        {
            try
            {
                var absolutePath = path.Substring(0, path.IndexOf(Path.GetFileName(path)));

                var saveInstancePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), absolutePath) + "\\Instance Result";

                if (!Directory.Exists(saveInstancePath))
                    Directory.CreateDirectory(saveInstancePath);

                FileStream fs = new FileStream(saveInstancePath + "\\" + Path.GetFileName(path), FileMode.Create);

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Instance));

                jsonSerializer.WriteObject(fs, instance);

                fs.Dispose();

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <Save -JSON- (AnomalyDetecionAPI)>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// Save is a function that saves the Clustering Results.
        /// </summary>
        /// <param name="settings">settings to save the AnomalyDetectionAPI instance</param>
        /// <param name="clusters">the  clustering results object to be saved</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse Save(string path, Cluster[] clusters)
        {
            try
            {
                var absolutePath = path.Substring(0, path.IndexOf(Path.GetFileName(path)));

                var clusterResultPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), absolutePath) + "\\Cluster Result";

                if (!Directory.Exists(clusterResultPath))
                    Directory.CreateDirectory(clusterResultPath);

                FileStream fs = new FileStream(clusterResultPath + "\\" + Path.GetFileName(path), FileMode.Create);

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Cluster[]));

                jsonSerializer.WriteObject(fs, clusters);

                fs.Dispose();

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <Save -JSON- (ClusteringResults[])>: Unhandled exception:\t" + Ex.ToString());
            }
        }

        /// <summary>
        /// SaveChecks is a function that checks the saving settings for errors. Some errors can be corrected.
        /// </summary>
        /// <param name="saveSettings">settings to save</param>
        /// <param name="settings">the checked settings to save</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse ValidateSaveConditions(SaveLoadSettings saveSettings, out SaveLoadSettings settings)
        {
            var adResponse = SaveLoadSettings.JsonSettings(saveSettings.ModelPath, out settings, saveSettings.Replace);

            if (adResponse.Code != 0)
            {
                settings = null;

                return adResponse;
            }

            return new AnomalyDetectionResponse(0, "OK");
        }


        /// <summary>
        /// Deserializes and loads an AnomalyDetectionAPI object from a JSON file.
        /// </summary>
        /// <param name="settings">Settings to load the AnomalyDetectionAPI instance</param>
        /// <returns>
        /// - Item 1: The loaded AnomalyDetectionAPI object <br />
        /// - Item 2: A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        //Check for another way to keep centroid private
        public Tuple<Instance, AnomalyDetectionResponse> ReadJsonObject(string path)
        {
            Instance instance;

            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Instance));

                instance = (Instance)jsonSerializer.ReadObject(fs);

                fs.Dispose();

                if (instance.Centroids == null)
                {
                    instance = null;

                    return Tuple.Create(instance, new AnomalyDetectionResponse(206, "Function <Load_AnomalyDetectionAPI -JSON- >: Can't deserialize file"));
                }

                return Tuple.Create(instance, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                instance = null;

                if (Ex is System.IO.FileNotFoundException)
                {
                    return Tuple.Create(instance, new AnomalyDetectionResponse(200, "Function<Load_AnomalyDetectionAPI -JSON- >: File not found"));
                }

                if (Ex is System.IO.DirectoryNotFoundException)
                {
                    return Tuple.Create(instance, new AnomalyDetectionResponse(204, "Function<Load_AnomalyDetectionAPI -JSON- >: Directory not found"));
                }

                if (Ex is FileLoadException)
                {
                    return Tuple.Create(instance, new AnomalyDetectionResponse(202, "Function<Load_AnomalyDetectionAPI -JSON- >: File cannot be loaded"));
                }

                if (Ex is SerializationException)
                {
                    return Tuple.Create(instance, new AnomalyDetectionResponse(203, "Function<Load_AnomalyDetectionAPI -JSON- >: File content is corrupted"));
                }

                return Tuple.Create(instance, new AnomalyDetectionResponse(400, "Function<Load_AnomalyDetectionAPI -JSON- >: Unhandled exception:\t" + Ex.ToString()));
            }
        }

        /// <summary>
        /// Deserializes and loads a Clusters object from a JSON file.
        /// </summary>
        /// <param name="settings">Settings to load the ClusteringResults[] object</param>
        /// <returns>
        /// - Item 1: The loaded ClusteringResults[] object <br />
        /// - Item 2: A code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public Tuple<Cluster[], AnomalyDetectionResponse> GetClusters(string path)
        {
            Cluster[] cluster;
            try
            {
                string ResultPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), path);

                FileStream fs = new FileStream(ResultPath, FileMode.Open);

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Cluster[]));

                cluster = (Cluster[])jsonSerializer.ReadObject(fs);

                fs.Dispose();

                if (cluster.Length == 0)
                {
                    cluster = null;

                    return Tuple.Create(cluster, new AnomalyDetectionResponse(206, "Function <Load_ClusteringResults -JSON- >: Can't deserialize file"));
                }

                return Tuple.Create(cluster, new AnomalyDetectionResponse(0, "OK"));
            }
            catch (Exception Ex)
            {
                cluster = null;

                if (Ex is System.IO.FileNotFoundException)
                {
                    return Tuple.Create(cluster, new AnomalyDetectionResponse(200, "Function <Load_ClusteringResults -JSON- >: File not found"));
                }

                if (Ex is System.IO.DirectoryNotFoundException)
                {
                    return Tuple.Create(cluster, new AnomalyDetectionResponse(204, "Function<Load_ClusteringResults -JSON- >: Directory not found"));
                }

                if (Ex is FileLoadException)
                {
                    return Tuple.Create(cluster, new AnomalyDetectionResponse(202, "Function <Load_ClusteringResults -JSON- >: File cannot be loaded"));
                }

                if (Ex is SerializationException)
                {
                    return Tuple.Create(cluster, new AnomalyDetectionResponse(203, "Function <Load_ClusteringResults -JSON- >: File content is corrupted"));
                }

                return Tuple.Create(cluster, new AnomalyDetectionResponse(400, "Function <Load_ClusteringResults -JSON- >: Unhandled exception:\t" + Ex.ToString()));
            }
        }

        /// <summary>
        /// LoadChecks is a function that checks the load settings for errors. Some errors can be corrected.
        /// </summary>
        /// <param name="settings">settings to load</param>
        /// <param name="saveSettings">the checked settings to load</param>
        /// <returns>a code and a message that state whether the function succeeded or encountered an error. When the function succeeds, it will return:
        /// <ul style="list-style-type:none">
        /// <li> - Code: 0, "OK" </li>
        /// </ul>
        /// </returns>
        public AnomalyDetectionResponse Validate(SaveLoadSettings settings, out SaveLoadSettings saveSettings)
        {
            if (!File.Exists(settings.ModelPath))
            {
                saveSettings = null;

                return new AnomalyDetectionResponse(200, "Function <LoadChecks -JSON- >: File not found");
            }

            var adResponse = SaveLoadSettings.JsonSettings(settings.ModelPath, out saveSettings, settings.Replace);

            if (adResponse.Code != 0)
            {
                saveSettings = null;

                return adResponse;
            }

            return new AnomalyDetectionResponse(0, "OK");
        }
    }
}
