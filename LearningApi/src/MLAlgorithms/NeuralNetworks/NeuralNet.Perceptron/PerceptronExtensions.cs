using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNet.Perceptron
{
    public static class PerceptronExtensions
    {
        public static LearningApi UsePerceptron(this LearningApi api,
           double learningRate, int iterations,
                Func<double, double> activationFunction = null,
                bool traceTotalError = false)

        {
            var alg = new PerceptronAlgorithm(0, learningRate, iterations, activationFunction, traceTotalError);
            api.AddModule(alg, "PerceptronAlgorithm");
            return api;
        }
    }
}
