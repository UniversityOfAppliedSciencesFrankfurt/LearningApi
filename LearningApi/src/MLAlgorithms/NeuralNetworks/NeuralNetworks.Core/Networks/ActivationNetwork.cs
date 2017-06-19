using System;
using LearningFoundation;
using NeuralNetworks.Core.Layers;
namespace NeuralNetworks.Core.Networks
{
    /// <summary>
    /// Activation network.
    /// </summary>
    /// 
    /// <remarks><para>Activation network is a base for multi-layer neural network
    /// with activation functions. It consists of activation layers
    /// 
    

    [Serializable]
    public class ActivationNetwork : Network
    {
        /// <summary>
        /// Initializes a new instance of the Activation Network class.
        /// </summary>
        /// 
        /// <param name="function">Activation function of neurons of the network.</param>
        /// <param name="inputsCount">Network's inputs count.</param>
        /// <param name="neuronsCount">Array, which specifies the amount of neurons in
        /// each layer of the neural network.</param>
        /// 
        /// <remarks>The new network is randomized (see <see cref="ActivationNeuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
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
        
    }
}
