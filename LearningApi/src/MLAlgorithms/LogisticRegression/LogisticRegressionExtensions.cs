using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using LearningFoundation;

namespace LogisticRegression
{
    public static class LogistiRegressionExtensions
    {
        /// <summary>
        /// The implementation of Logistics Regression machine learning algorithm
        /// </summary>
        /// <param name="api"></param>
        /// <param name="learningRate"></param>
        /// <returns></returns>
        public static LearningApi UseLogisticRegression(this LearningApi api, double learningRate, int iterations)

        {
            var alg = new LogisticRegression(learningRate);
            alg.Iterations = iterations;
            api.Modules.Add("Logistic Regression", alg);
            return api;
        }
    }
}
