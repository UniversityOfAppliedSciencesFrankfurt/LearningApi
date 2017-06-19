using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using NeuralNetworks.Core.Neurons;

namespace NeuralNetworks.Core.Layers
{
    /// <summary>
    /// Activation layer.
    /// </summary>
    /// 
    /// <remarks>Activation layer is a layer of <see cref="ActivationNeuron">activation neurons</see>.
    /// The layer is usually used in multi-layer neural networks.</remarks>
    /// 
    [Serializable]
    public class ActivationLayer : Layer
    {
        /// <summary>
        /// Initializes a new instance of the ActivationLayer class.
        /// </summary>
        /// 
        /// <param name="neuronsCount">Layer's neurons count.</param>
        /// <param name="inputsCount">Layer's inputs count.</param>
        /// <param name="function">Activation function of neurons of the layer.</param>
        /// 
        /// <remarks>The new layer is randomized (ActivationNeuron.Randomize
        /// method) after it is created.</remarks>
        /// 
        public ActivationLayer(int neuronsCount, int inputsCount, IActivationFunction function)
            : base(neuronsCount, inputsCount)
        {
            // create each neuron
            for (int i = 0; i < neurons.Length; i++)
                neurons[i] = new ActivationNeuron(inputsCount, function);
        }
        /// <summary>
        /// Set new activation function for all neurons of the layer.
        /// </summary>
        /// 
        /// <param name="function">Activation function to set.</param>
        /// 
        /// <remarks><para>The methods sets new activation function for each neuron by setting
        /// their Activation Function property.</remarks>
        /// 
        public void SetActivationFunction(IActivationFunction function)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                ((ActivationNeuron)neurons[i]).ActivationFunction = function;
            }
        }
    }
}
