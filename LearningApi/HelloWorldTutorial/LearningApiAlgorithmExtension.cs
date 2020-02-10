using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorldTutorial
{
    /// <summary>
    /// Adds the Functionalities to learning api
    /// </summary>
    public static class LearningApiAlgorithmExtension
    {
        /// <summary>
        /// The implementation of LearningApi machine learning algorithm and adding the name to algorithm
        /// </summary>
        /// <param name="api"></param>
        /// <returns> api after adding the functionalities to it</returns>
        public static LearningApi UseLearningApiAlgorithm(this LearningApi api)
        {
            var alg = new LearningApiAlgorithm();
            api.AddModule(alg, "LearningApiAlgorithm");
            return api;
        }
    }

}
