using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    public static class RbmExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="api">Instance of LearningAPI.</param>
        /// <param name="learningRate">Defines how fast the RBM algorithm will learn. </param>
        /// <param name="iterations">How many iterations are done per given dataset.</param>
        /// <param name="numVisible">Number of visible nodes should be equal to number of inputs.</param>
        /// <param name="numHidden">Number of hidden nodes.</param>
        /// <param name="activationFnc">Optionaly, you can specify some activation function.</param>
        /// <returns></returns>
        public static LearningApi UseRbm(this LearningApi api,
        double learningRate, int iterations, int numVisible, int numHidden,
             IActivationFunction activationFnc = null)

        {
            var alg = new Rbm(numVisible, numHidden, iterations, learningRate/*, bool writeLossToFile = false*/);
            api.AddModule(alg, $"Rbm_{Guid.NewGuid()}");
            return api;
        }


        public static LearningApi UseCRbm(this LearningApi api, double[] featureVector,
        double learningRate, int iterations, int numVisible, int numHidden,
             IActivationFunction activationFnc = null)

        {
            var alg = new CRbm(featureVector, numVisible, numHidden, iterations, learningRate/*, bool writeLossToFile = false*/);
            api.AddModule(alg, $"Rbm_{Guid.NewGuid()}");
            return api;
        }

        public static LearningApi UseDeepRbm(this LearningApi api,
       double learningRate, int iterations, int[] layers,
            IActivationFunction activationFnc = null)

        {
            var alg = new DeepRbm(layers, iterations, learningRate/*, bool writeLossToFile = false*/);
            api.AddModule(alg, $"Rbm_{Guid.NewGuid()}");
            return api;
        }
    }
}
