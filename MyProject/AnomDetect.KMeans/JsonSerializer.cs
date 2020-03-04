using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// JSONSerializer is a class that saves to or loads from JSON files.
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// Save is a function that saves a clustering Instance.
        /// </summary>
        /// <param name="path">path to save the clustering Instance object</param>
        /// <param name="instance">the clustering Instance object to be saved</param>
        public void Save(string path, KMeansModel instance)
        {
            int Code;
            string Message = "Function <Save>: ";
            try
            {
                var absolutePath = path.Substring(0, path.IndexOf(Path.GetFileName(path)));

                var saveInstancePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), absolutePath) + "KMeans Clustering Results";

                if (!Directory.Exists(saveInstancePath))
                    Directory.CreateDirectory(saveInstancePath);

                FileStream fs = new FileStream(saveInstancePath + "\\" + Path.GetFileName(path), FileMode.Create);

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(KMeansModel));

                jsonSerializer.WriteObject(fs, instance);

                fs.Dispose();
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// Deserializes and loads a clustering Instance object from a JSON file.
        /// </summary>
        /// <param name="path">path to load the clustering Instance object</param>
        /// <returns>The loaded clustering Instance object</returns>
        public KMeansModel Load(string path)
        {
            KMeansModel instance;
            int Code;
            string Message = "Function <LoadInstance>: ";

            try
            {
                var absolutePath = path.Substring(0, path.IndexOf(Path.GetFileName(path)));

                var InstanceResultPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), absolutePath) + "KMeans Clustering Results\\" + Path.GetFileName(path);

                FileStream fs = new FileStream(InstanceResultPath, FileMode.Open);

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(KMeansModel));

                instance = (KMeansModel)jsonSerializer.ReadObject(fs);

                fs.Dispose();

                if (instance.Clusters[0].Centroid == null)
                {
                    Code = 206;
                    Message += "Can't deserialize file";
                    throw new KMeansException(Code, Message);
                }

                return instance;
            }
            catch (Exception Ex)
            {
                if (Ex is System.IO.FileNotFoundException)
                {
                    Code = 200;
                    Message += "File not found";
                    throw new KMeansException(Code, Message);
                }

                if (Ex is System.IO.DirectoryNotFoundException)
                {
                    Code = 204;
                    Message += "Directory not found";
                    throw new KMeansException(Code, Message);
                }

                if (Ex is FileLoadException)
                {
                    Code = 202;
                    Message += "File cannot be loaded";
                    throw new KMeansException(Code, Message);
                }

                if (Ex is SerializationException)
                {
                    Code = 203;
                    Message += "File content is corrupted";
                    throw new KMeansException(Code, Message);
                }

                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /*
        /// <summary>
        /// Save is a function that saves the Cluster object.
        /// </summary>
        /// <param name="path">path to save the Cluster object</param>
        /// <param name="clusters">the array of Cluster object to be saved</param>
        public void Save(string path, Cluster[] clusters)
        {
            int Code;
            string Message = "Function <Save>: ";
            try
            {
                var absolutePath = path.Substring(0, path.IndexOf(Path.GetFileName(path)));

                var clusterResultPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), absolutePath) + "Cluster Result";

                if (!Directory.Exists(clusterResultPath))
                    Directory.CreateDirectory(clusterResultPath);

                FileStream fs = new FileStream(clusterResultPath + "\\" + Path.GetFileName(path), FileMode.Create);

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Cluster[]));

                jsonSerializer.WriteObject(fs, clusters);

                fs.Dispose();
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }

        /// <summary>
        /// Deserializes and loads a Cluster object from a JSON file.
        /// </summary>
        /// <param name="path">path to load the Cluster object</param>
        /// <returns>The loaded array of Cluster objects</returns>
        public Cluster[] LoadClusters(string path)
        {
            Cluster[] cluster;
            int Code;
            string Message = "Function <LoadClusters>: ";

            try
            {
                var absolutePath = path.Substring(0, path.IndexOf(Path.GetFileName(path)));

                var clusterResultPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), absolutePath) + "Cluster Result\\" + Path.GetFileName(path);

                FileStream fs = new FileStream(clusterResultPath, FileMode.Open);

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Cluster[]));

                cluster = (Cluster[])jsonSerializer.ReadObject(fs);

                fs.Dispose();

                if (cluster.Length == 0)
                {
                    Code = 206;
                    Message += "Can't deserialize file";
                    throw new KMeansException(Code, Message);
                }

                return cluster;
            }
            catch (Exception Ex)
            {
                cluster = null;

                if (Ex is System.IO.FileNotFoundException)
                {
                    Code = 200;
                    Message += "File not found";
                    throw new KMeansException(Code, Message);
                }

                if (Ex is System.IO.DirectoryNotFoundException)
                {
                    Code = 204;
                    Message += "Directory not found";
                    throw new KMeansException(Code, Message);
                }

                if (Ex is FileLoadException)
                {
                    Code = 202;
                    Message += "File cannot be loaded";
                    throw new KMeansException(Code, Message);
                }

                if (Ex is SerializationException)
                {
                    Code = 203;
                    Message += "File content is corrupted";
                    throw new KMeansException(Code, Message);
                }

                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new KMeansException(Code, Message);
            }
        }
        */
        
    }
}
