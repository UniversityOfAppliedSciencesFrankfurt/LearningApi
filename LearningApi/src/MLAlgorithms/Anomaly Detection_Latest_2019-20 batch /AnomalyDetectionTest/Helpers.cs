using LearningFoundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iprojecthelp
{
    /// <summary>
    /// Helpers to Convert CSV file
    /// </summary>
    internal class Helpers
    {
        /// <summary>
        /// Path of mapping file
        /// </summary>
        private const string m_DefaultMapperFileName = "Samples/iris_mapper.json";

        /// <summary>
        /// Data Descriptor
        /// </summary>
        /// <param name="mapperPath">The path for mapping configuration.</param>
        public static DataDescriptor GetDescriptor(string mapperPath = null)
        {
            if (string.IsNullOrEmpty(mapperPath))
                mapperPath = m_DefaultMapperFileName;

            string strContent = System.IO.File.ReadAllText(mapperPath);

            var dm = JsonConvert.DeserializeObject(strContent, typeof(DataDescriptor));

            if (dm is DataDescriptor)
                return (DataDescriptor)dm;
            else
                return null;
        }
    }
}
