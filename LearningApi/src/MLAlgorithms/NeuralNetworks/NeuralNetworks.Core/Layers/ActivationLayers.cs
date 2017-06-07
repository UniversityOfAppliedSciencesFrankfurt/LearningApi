using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using NeuralNetworks.Core.Neurons;

namespace NeuralNetworks.Core.Layers
{
    [Serializable]
    public class ActivationLayer : Layer
    {
        
        public ActivationLayer(int neuronsCount, int inputsCount, IActivationFunction function)
            : base(neuronsCount, inputsCount)
        {
            // create each neuron
            for (int i = 0; i < neurons.Length; i++)
                neurons[i] = new ActivationNeuron(inputsCount, function);
        }

        public void SetActivationFunction(IActivationFunction function)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                ((ActivationNeuron)neurons[i]).ActivationFunction = function;
            }
        }
    }
}
