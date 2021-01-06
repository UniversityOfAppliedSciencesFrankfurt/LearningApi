using LearningFoundation;
using Newtonsoft.Json;

namespace DecisionTreeForLearningAPI.BinaryDecisionTreeTests
{
    internal class Helpers
    {
        private const string m_DefaultMapperFileName = "samples/dataMapper.json";

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
