using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SumAlgorithm
{
    public static class SumExtension
    {
        /// <summary>
        /// The implementation of Sum algorithm
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static LearningApi UseSum(this LearningApi api)
        {
            var alg = new Sum();
            api.AddModule(alg, "Sum");
            return api;
        }
    }
}
