using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LearningFoundation.PersistenceProviders
{
    public class JsonPersistenceProvider : IModelPersistenceProvider
    {
        private JsonSerializerSettings getSettings()
        {

            return new JsonSerializerSettings()
            {
                Culture = CultureInfo.InvariantCulture,
                TypeNameHandling = TypeNameHandling.All
            };

        }

        public LearningApi Load(string name)
        {
            using (FileStream fs = new FileStream($"{name}.json", FileMode.Open))
            {
                StreamReader sw = new StreamReader(fs, Encoding.UTF8);
                {
                    var jsonApi = sw.ReadToEnd();

                    return JsonConvert.DeserializeObject<LearningApi>(jsonApi, getSettings());
                }
            }
        }

        public void Save(string name, LearningApi api)
        {

            var jsonApi = JsonConvert.SerializeObject(api, api.GetType(), getSettings());
            using (FileStream fs = new FileStream($"{name}.json", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(jsonApi);
                }
            }
        }
    }
}
