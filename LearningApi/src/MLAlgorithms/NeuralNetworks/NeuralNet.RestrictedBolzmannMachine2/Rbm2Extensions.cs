using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    public static class RbmExtensions
    {
        public static LearningApi UseRbm(this LearningApi api,
        double learningRate, int iterations,
             IActivationFunction activationFnc = null)

        {
            var alg = new Rbm(6, 3, iterations, learningRate);
            api.AddModule(alg, "Rbm");
            return api;
        }
    }
}
