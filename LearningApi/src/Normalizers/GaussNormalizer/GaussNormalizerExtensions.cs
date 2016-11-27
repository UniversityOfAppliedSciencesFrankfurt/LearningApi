using LearningFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Normalizers
{
    /// <summary>
    /// Extension method for using Gaus normalizer (Standardization)
    /// </summary>
    public static class GaussNormalizerExtensions
    {

        /// <summary>
        /// Extension method for using GaussNormalizer in LearningAPI
        /// In case of using Gauss normalize you have to provide mean and variace values for all data columns (feature)
        /// </summary>
        /// <param name="api"></param>
        /// <param name="mean"> min for each column in the data set</param>
        /// <param name="var"> variance for each column in the data set</param>
        /// <returns></returns>
        public static LearningApi UseGaussNormalizer(this LearningApi api, double[] mean, double[] var)       
        {
            // Normalizer should not require DataMapper.
            //if (api.DataMapper == null)
            //    throw new Exception("Data Mapper must be initialized before Data Normalizer.");

            //if (api.DataMapper is DataMappers.DataMapper)
            //{
                //create minmax object
                var gn = new GaussNormalizer(null/* TODO datamapper not needed!! */, mean, var);
                // assign to LearningAPI property
                api.AddModule(gn);

                return api;
            //}
            //else
            //    throw new Exception("Data Mapper must be of \"DataMapper\" type.");

        }

        
    }
}
