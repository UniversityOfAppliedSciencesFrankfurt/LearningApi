using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LearningFoundation.RingProjectionAlgorithm
{
    /// <summary>
    /// Save the result from RingProjecitonPipelineComponent into CSV file
    /// </summary>
    public class RingProjectionFunctionToCSVPipelineModule : IPipelineModule<double[], object>
    {
        private string m_label;
        private int m_index;
        private string m_basePath;
        private string m_delimiter;

        /// <summary>
        /// Generate CSV file from the result of ring projection
        /// </summary>
        /// <param name="label">Image name</param>
        /// <param name="index">Index of image with the same content</param>
        /// <param name="delimiter">Separator of CSV file</param>
        /// <param name="basePath">Save path of CSV file</param>
        public RingProjectionFunctionToCSVPipelineModule(string label, int index, string delimiter, string basePath)
        {
            this.m_label = label;
            this.m_index = index;
            this.m_delimiter = delimiter;
            this.m_basePath = basePath;
        }

        /// <summary>
        /// What actually happens inside pipeline component
        /// </summary>
        /// <param name="data">output data from the previous compatible pipeline component</param>
        /// <param name="ctx">data description</param>
        /// <returns></returns>
        public object Run(double[] data, IContext ctx)
        {
            string savePath = Path.Combine(m_basePath, $"{m_label}.{m_index}.csv");

            if (!File.Exists(savePath))
            {
                File.Create(savePath).Dispose();
            }

            using (StreamWriter streamWriter = new StreamWriter(savePath))
            {
                streamWriter.WriteLine("{0}{1}{2}", "Radius", m_delimiter, "Value");
                for (int i = 0; i < data.Length; i++)
                {
                    streamWriter.WriteLine($"{i}{m_delimiter}{data[i]}");
                }
            }
            return null;
        }
    }
}
