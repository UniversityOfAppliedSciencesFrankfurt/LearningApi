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
            int hiddenLayerCount, double momentum, double learningRate,
                 IActivationFunction activationFnc)
       
        {
            var alg = new BackPropagationNetwork(hiddenLayerCount, momentum, learningRate, activationFnc);
            api.Modules.Add("Algorithm", alg);
            return api;
        }
    }
}
