using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using Newtonsoft.Json;
using LearningFoundation;

namespace LearningFoundation.Statistics
{

    public static class BasicStatisticsExtensions
    {
        ///// <summary>
        ///// provide basic statistics for the loaded data: MIn, Max, Average and Variance
        ///// </summary>
        ///// <returns></returns>
        //public static LearningApi UseBasicDataStatistics(this LearningApi api, IStatistics statistics)
        //{
        //    if (api.DataMapper == null)
        //        throw new Exception("Data Mapper must be initialized before Data Normalizer.");

        //    if (statistics is BasicStatistics)
        //    {
        //        // assign statistics to LearningAPI property
        //        api.Statistics = statistics;

        //        return api;
        //    }
        //    else
        //        throw new Exception("statistics  must be of \"BasicStatistics\" type.");
        //}
    }
}
