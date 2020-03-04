using LearningFoundation;
using System;

namespace SurvivalAnalysis
{
    public static class SurvivalAnalysisExtensions
    {
        /// <summary>
        /// The implementation of Logistics Regression machine learning algorithm
        /// </summary>
        /// <param name="api"></param>
        /// <param name="learningRate"></param>
        /// <returns></returns>
        public static LearningApi UseSurvivalAnalysis(this LearningApi api, int iterations)

        {
            var alg = new SurvivalAnalysis();
            alg.Iterations = iterations;
            api.AddModule(alg, "Logistic Regression");
            return api;
        }
    }
}
