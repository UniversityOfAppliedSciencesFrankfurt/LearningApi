using ExampleLearningApiAlgorithm;
using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleLearningApiAlgorithmExtension
{
    public static class LinearRegressionExtension
    {
        /// <summary>
        /// The implementation of Linear Regression machine learning algorithm
        /// </summary>
        /// <param name="api"></param>
        /// <param name="learningRate"></param>
        /// <returns></returns>
        public static LearningApi UseLinearRegression(this LearningApi api, double learningRate, int epochs)

        {
            var alg = new LinearRegression(learningRate, epochs);
            api.AddModule(alg, "Linear Regression");
            return api;
        }
    }
}
