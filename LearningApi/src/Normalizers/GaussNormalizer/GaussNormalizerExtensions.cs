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
        /// Extension methods of LearningAPI for Gauss MinMax normalizer
        /// </summary>
        /// <param name="api">LearningAPI</param>
        /// <returns></returns>
        public static LearningApi UseGaussNormalizer(this LearningApi api)
        {
            var minMaxNorm = new GaussNormalizer();
            api.Modules.Add("GaussNormilizer", minMaxNorm);

            return api;
        }

        /// <summary>
        /// Extension methods of LearningAPI for using Gauss denormalizer
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static LearningApi UseGaussDeNormalizer(this LearningApi api)
        {
            var minMaxDeNorm = new GaussDeNormalizer();
            api.Modules.Add("GaussDenormilizer", minMaxDeNorm);
            return api;
        }


    }
}
