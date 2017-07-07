using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using NeuralNetworks.Core.ActivationFunctions;

namespace NeuralNet.RestrictedBoltzmannMachine
{
    public static class RBMExtensions
    {
        /// <summary>
        /// Create a Restricted Boltzmann Machine algorithm with parameters
        /// </summary>
        /// <param name="api">Using the LearningAPI</param>
        /// <param name="InputsCount"> </param>
        /// <param name="HiddenNeurons"></param>
        /// <param name="Iterations"></param>
        /// <param name="LearningRates"></param>
        /// <param name="Momentums"></param>
        /// <param name="Decays"></param>
        /// <returns></returns>
        public static LearningApi UseRestrictedBoltzmannMachine( this LearningApi api, int InputsCount, 
            int HiddenNeurons,int Iterations, double LearningRates, double Momentums, double Decays )
        {
          
        var alg = new RBMAlgorithm(InputsCount, HiddenNeurons)
            {
                LearningRate = LearningRates,
                Momentum = Momentums,
                Decay = Decays,
                Iteration = Iterations
            };

            api.AddModule( alg, "RBMAlgorithm" );
            return api;
        }
    }
}
