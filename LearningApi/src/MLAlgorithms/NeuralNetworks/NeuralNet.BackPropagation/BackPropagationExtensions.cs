using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuronalNet.BackPropagation
{
    public static class BackPropagationExtensions
    {
        public static LearningApi UseBackPropagation(this LearningApi api, 
            int hiddenLayerCount, double momentum, double learningRate,
                 IActivationFunction activationFnc)
       
        {
            api.Algorithm = new BackPropagationNetwork(hiddenLayerCount, momentum, learningRate, activationFnc);
            return api;
        }
    }
}
