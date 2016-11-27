using LearningFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Normalizers
{
    /// <summary>
    /// Extension method for using MinMax normalizer
    /// </summary>
    public static class MinMaxNormalizerExtensions
    {
        /// <summary>
        /// Extension methods of LearningAPI for using MinMax normalizer
        /// </summary>
        /// <param name="api">LearningAPI</param>
        /// <returns></returns>
        public static LearningApi UseMinMaxNormalizer(this LearningApi api, double [] min, double [] max)       
        {
            // Normalizer should not require DataMapper.
            //if (api.DataMapper == null)
            //    throw new Exception("Data Mapper must be initialized before Data Normalizer.");

            //if (api.DataMapper is DataMappers.DataMapper)
            //{
            //create minmax object
            var mn = new MinMaxNormalizer(null/* TODO datamapper not needed!! */, min, max);

                // assign to LearningAPI property
                api.Normalizer = mn;

                return api;
            //}
            //else
            //    throw new Exception("Data Mapper must be of \"DataMapper\" type.");
            
        }

        
    }
}
