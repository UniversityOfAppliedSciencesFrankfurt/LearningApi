using LearningFoundation;
using LearningFoundation.DataMappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisonForestRegressionAlgUnitTests
{
    internal class Helpers
    {
        private const string m_DefaultMapperFileName = "samples/CC-data_mapper.json";

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

        public static DataDescriptor GetDescriptorForUnsupervisedTrafficData(string mapperPath = null)
        {
            return new DataDescriptor()
            {
                Features = new Column[1],
                LabelIndex = -1,
            };
        }
    }
}
