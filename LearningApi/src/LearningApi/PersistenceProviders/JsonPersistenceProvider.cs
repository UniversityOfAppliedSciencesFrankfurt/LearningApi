using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LearningFoundation.PersistenceProviders
{
    public class JsonPersistenceProvider : IModelPersistenceProvider
    {
        public LearningApi Load(string name)
        {
            using (FileStream fs = new FileStream($"{name}-.json", FileMode.Open))
            {
                StreamReader sw = new StreamReader(fs, Encoding.UTF8);
                {
                    var jsonApi = sw.ReadToEnd();

                    JsonSerializerSettings sett = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };


                    return JsonConvert.DeserializeObject<LearningApi>(jsonApi, sett);
                }
            }
        }

        public void Save(string name, LearningApi api)
        {
            JsonSerializerSettings sett = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All  
            };

       
            var jsonApi = JsonConvert.SerializeObject(api, api.GetType(), sett);
            using (FileStream fs = new FileStream($"{name}-.json", FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(jsonApi);
                }
            }
        }
    }
}
