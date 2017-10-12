using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNet.BackPropagation
{
    public static class BackPropagationExtensions
    {
        public static LearningApi UseBackPropagation(this LearningApi api, 
            int hiddenLayerCount, double momentum, double learningRate,int iteration,
                 IActivationFunction activationFnc)
       
        {
           
            var alg = new BackPropagationNetwork(hiddenLayerCount)
            {
                LearningRate = learningRate,
                Momentum = momentum,
                Iteration = iteration
            };
            api.AddModule(alg, "Algorithm");
            return api;
        }
    }
}



