using System;
using LearningFoundation;
namespace NeuralRegression
{
    /// <summary>
    /// Helps Learning API to use Neural Regression Algorithm
    /// </summary>
    public static class ExtensionNeuralRegression
    {
        /// <summary>
        /// Extension function for using Neural Network in Learning Api
        /// </summary>
        /// <param name="api">Instance of Learning api</param>
        /// <param name="learningRate">Learning rate for Neural Network</param>
        /// <param name="iterations">Iteration number</param>
        /// <returns></returns>
        public static LearningApi UseNeuralRegression(this LearningApi api, double learningRate, int iterations)
            {
            var alg = new NeuralRegression(learningRate, iterations);
            api.AddModule(alg, "Neural Regression");
                return api;
            }

        
    }
}
