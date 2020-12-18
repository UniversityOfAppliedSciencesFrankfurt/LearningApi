using LearningFoundation;
using LearningFoundation.DataMappers;
using Newtonsoft.Json;
using System;
using System.Globalization;
using LearningFoundation.Helpers;
using System.Collections.Generic;
using System.Text;
namespace SupportVectorMachineAlgorithmUnitTests
{
    internal class Helpers
    {

        private const string T_DefaultMapperFileName = "samples/IrisData/iris_mapper.json";

        public static DataDescriptor GetDescriptor(string mapperPath = null)
        {
            if (string.IsNullOrEmpty(mapperPath))
            {
                mapperPath = T_DefaultMapperFileName;
            }

            string strContent = System.IO.File.ReadAllText(mapperPath);

            var dm = JsonConvert.DeserializeObject(strContent, typeof(DataDescriptor));

            if (dm is DataDescriptor)
            {
                return (DataDescriptor)dm;
            }
            else
            {
                return null;
            }
        }
    }
}
