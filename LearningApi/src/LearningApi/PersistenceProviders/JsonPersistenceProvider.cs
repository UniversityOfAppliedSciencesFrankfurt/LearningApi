using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;


namespace LearningFoundation.PersistenceProviders
{
    public class JsonPersistenceProvider : IModelPersistenceProvider
    {
        private JsonSerializerSettings getSettings()
        {
            return new JsonSerializerSettings()
            {
                ContractResolver = new PrivateFieldsContractResolver(),
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


    /// <summary>
    /// Resolves all private fields.
    /// </summary>
    class PrivateFieldsContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            //Func<PropertyInfo, bool> expr1 = (f) => f.Name != "Modules";
            //Func<FieldInfo, bool> expr2 = (f) => f.Name != "<Modules>k__BackingField";

            Func<PropertyInfo, bool> expr1 = (f) => !typeof(Stream).IsAssignableFrom(f.GetType()) && !f.DeclaringType.Name.Contains("FunctionModule") && !f.DeclaringType.Name.Contains("Stream") && f.GetCustomAttributes().Count(att=>att.GetType() == typeof(JsonIgnoreAttribute)) == 0;
            Func<FieldInfo, bool> expr2 = (f) => !typeof(Stream).IsAssignableFrom(f.GetType()) && !f.DeclaringType.Name.Contains("FunctionModule") && !f.DeclaringType.Name.Contains("Stream") && f.GetCustomAttributes().Count(att => att.GetType() == typeof(JsonIgnoreAttribute)) == 0;

            var x = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.Name != "Modules");
            var y = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(expr1)
                .Select(p => base.CreateProperty(p, memberSerialization))
                .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(expr2)
                .Select(f => base.CreateProperty(f, memberSerialization)))
                .ToList();
            props.ForEach(p => { p.Writable = true; p.Readable = true; });

          
            return props;
        }
    }
}
