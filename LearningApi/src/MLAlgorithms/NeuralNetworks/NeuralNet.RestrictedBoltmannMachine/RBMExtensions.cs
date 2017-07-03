using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using NeuralNetworks.Core.ActivationFunctions;

namespace NeuralNet.RestrictedBoltzmannMachine
{
    public static class RBMExtensions
    {
        public static LearningApi UseRestrictedBoltzmannMachine( this LearningApi api, int inputscount, int hiddenneurons,
            int iterations, double learningrates, double momentums, double decays )
        {
            #region testing
            //var function = new BernoulliFunction( alpha: 0.5 );
            //var rbm = new RestrictedBoltzmannMachine( function, inputscount, hiddenneurons ); //rbm(function,6,2)
            //public RBMAlgorithm( int iterations, double learningrate, double momentum, double decay,
            //double[][] inputs, IStochasticFunction activationfunction = null )
            #endregion
      
        var alg = new RBMAlgorithm(inputscount, hiddenneurons)
            {
                LearningRate = learningrates,
                Momentum = momentums,
                Decay = decays,
                Iteration = iterations
            }
                ;
            api.AddModule( alg, "RBMAlgorithm" );
            return api;
        }
    }
}
