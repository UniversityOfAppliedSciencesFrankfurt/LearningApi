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
        /// <returns></returns>
        public static LearningApi UseDefaultDataMapper(this LearningApi api)
        {
            var dm = new DataMapper();
            api.Modules.Add("DataMapper", dm);

            return api;
        }
    }
}
