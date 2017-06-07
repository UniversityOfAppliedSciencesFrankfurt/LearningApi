 using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using NeuralNetworks.Core.Layers;
namespace NeuralNetworks.Core.Networks
{
    [Serializable]
    public class ActivationNetwork : Network
    {
        
        public ActivationNetwork(IActivationFunction function, int inputsCount, params int[] neuronsCount)
            : base(inputsCount, neuronsCount.Length)
        {
            // create each layer
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new ActivationLayer(
                    // neurons count in the layer
                    neuronsCount[i],
                    // inputs count of the layer
                    (i == 0) ? inputsCount : neuronsCount[i - 1],
                    // activation function of the layer
                    function);
            }
        }

        public void SetActivationFunction(IActivationFunction function)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                ((ActivationLayer)layers[i]).SetActivationFunction(function);
            }
        }
    }
}
