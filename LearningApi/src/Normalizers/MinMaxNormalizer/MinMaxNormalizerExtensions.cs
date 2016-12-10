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
        public static LearningApi UseMinMaxNormalizer(this LearningApi api, double[] min, double[] max)
        {           
            var minMaxNorm = new MinMaxNormalizer(min, max);

            api.Modules.Add("Normilizer", minMaxNorm);

            return api;
        }
    }
}
