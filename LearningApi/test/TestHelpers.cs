using LearningFoundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace test
{
    public class TestHelpers
    {
        private static string m_iris_mapper_path;

        static TestHelpers()
        {
            //mapper initialization
            m_iris_mapper_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"SampleData\iris\iris_mapper.json");

        }

        public static DataDescriptor GetDescriptor(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = m_iris_mapper_path;

            string strContent = System.IO.File.ReadAllText(path);

            var dm = JsonConvert.DeserializeObject(strContent, typeof(DataDescriptor));

            if (dm is DataDescriptor)
                return (DataDescriptor)dm;
            else
                return null;
        }

    }
}
