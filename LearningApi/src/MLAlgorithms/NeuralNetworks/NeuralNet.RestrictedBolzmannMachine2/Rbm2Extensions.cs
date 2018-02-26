using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    public static class RbmExtensions
    {
        public static LearningApi UseRbm(this LearningApi api,
        double learningRate, int iterations, int numVisible, int numHidden,
             IActivationFunction activationFnc = null)

        {
            var alg = new Rbm(numVisible, numHidden, iterations, learningRate);
            api.AddModule(alg, "Rbm");
            return api;
        }
    }
}
