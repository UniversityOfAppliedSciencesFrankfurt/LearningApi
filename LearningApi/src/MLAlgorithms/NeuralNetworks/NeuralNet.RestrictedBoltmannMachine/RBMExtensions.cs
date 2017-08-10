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
        /// <param name="InputsCount"> Number of feature </param>
        /// <param name="HiddenNeurons"> Number of desire class</param>
        /// <param name="Iterations"> Number of iteration</param>
        /// <param name="LearningRates">The training learning rate </param>
        /// <param name="Momentums"> The training Momentums</param>
        /// <param name="Decays">The training decay </param>
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
