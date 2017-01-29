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
                IActivationFunction activationFnc = null)

        {
            var alg = new PerceptronAlgorithm(0, learningRate, iterations);
            api.AddModule(alg, "PerceptronAlgorithm");
            return api;
        }
    }
}
