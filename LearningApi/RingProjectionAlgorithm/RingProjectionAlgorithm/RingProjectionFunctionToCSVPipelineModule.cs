using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LearningFoundation;

namespace RingProjectionAlgorithm
{
    /// <summary>
    /// Save the result from RingProjecitonPipelineComponent into CSV file
    /// </summary>
    public class RingProjectionFunctionToCSVPipelineModule : IPipelineModule<double[], object>
    {
        public string Label { get; set; }
        public int Index { get; set; }
        public string BasePath { get; set; }
        public string Delimter { get; set; }

        /// <summary>
        /// Generate CSV file from the result of ring projection
        /// </summary>
        /// <param name="label">Image name</param>
        /// <param name="index">Index of image with the same content</param>
        /// <param name="basePath">Save path of CSV file</param>
        public RingProjectionFunctionToCSVPipelineModule(string label, int index, string delimiter, string basePath)
        {
            this.Label = label;
            this.Index = index;
            this.Delimter = delimiter;
            this.BasePath = basePath;
        }

        /// <summary>
        /// What actually happens inside pipeline component
        /// </summary>
        /// <param name="data">output data from the previous compatible pipeline component</param>
        /// <param name="ctx">data description</param>
        /// <returns></returns>
        public object Run(double[] data, IContext ctx)
        {
            string savePath = Path.Combine(BasePath, $"{Label}.{Index}.csv");

            if (!File.Exists(savePath))
            {
                File.Create(savePath).Dispose();
            }

            using (StreamWriter streamWriter = new StreamWriter(savePath))
            {
                streamWriter.WriteLine("{0}{1}{2}", "Radius", Delimter, "Value");
                for (int i = 0; i < data.Length; i++)
                {
                    streamWriter.WriteLine($"{i}{Delimter}{data[i]}");
                }
            }
            return null;
        }
    }
}
