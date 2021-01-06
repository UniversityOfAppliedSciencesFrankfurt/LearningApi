using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisonForestRegressionAlgorithm
{
    /// <summary>
    /// DecisonForestRegressionAlgorithmExtensions
    /// </summary>
    public static class DecisonForestRegressionAlgorithmExtensions
    {
        /// <summary>
        /// UseDecisonForestRegressionAlgorithm take api and learning rate
        /// </summary>
        /// <param name="api"></param>
        /// <param name="learningRate"></param>
        /// <returns></returns>
        public static LearningApi UseDecisonForestRegressionAlgorithm(this LearningApi api, double learningRate)
        {

            DecisonForestRegressionAlgorithm alg = new DecisonForestRegressionAlgorithm(learningRate, 2.3);
            api.AddModule(alg, "MyAlgorithm");
            return api;
        }
    }
}
