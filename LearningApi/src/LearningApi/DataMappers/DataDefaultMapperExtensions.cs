using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using Newtonsoft.Json;

namespace LearningFoundation.DataMappers
{

    public static class DefaultDataMapperExtensions
    {
        /// <summary>
        /// Loads and initializes data mapper
        /// </summary>
        /// <param name="api">LearningAPI </param>
        /// <param name="dmFilePath">file path of the json data mapper</param>
        /// <returns></returns>
        public static LearningApi UseDefaultDataMapper(this LearningApi api, string dmFilePath)       
        {
            var dm = DefaultDataMapperExtensions.Load(dmFilePath);
           
            api.AddModule(dm);
           
            return api;
        }

        /// <summary>
        /// Initialize mapper from file
        /// </summary>
        /// <param name="filePath">path of the file contining mapper configuration</param>
        /// <returns>.Net data mapper object</returns>
        internal static DataMapper Load(string filePath)
        {
            string strContent = System.IO.File.ReadAllText(filePath);
            //
            var dm = JsonConvert.DeserializeObject(strContent, typeof(DataMapper));
            //
            if (dm is DataMapper)
                return (DataMapper)dm;
            else
                return null;
        }
    }
}
