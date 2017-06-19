using System;
using NeuralNetworks.Core.Neurons;
using LearningFoundation;

namespace NeuralNetworks.Core.Layers
{
    /// <summary>
    ///   Stochastic Activation Layer.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class represents a layer of <see cref="StochasticNeuron">stochastic neurons</see>.
    /// </remarks>
    /// 
    [Serializable]
    public class StochasticLayer : ActivationLayer
    {

        private new StochasticNeuron[] neurons;
        private double[] sample;
        /// <summary>
        ///   Gets the layer's neurons.
        /// </summary>
        /// 
        public new StochasticNeuron[] Neurons
        {
            get { return neurons; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="StochasticLayer"/> class.
        /// </summary>
        /// 
        /// <param name="function">The activation function for the neurons in the layer.</param>
        /// <param name="neuronsCount">The neurons count.</param>
        /// <param name="inputsCount">The inputs count.</param>
        /// 
        public StochasticLayer(IStochasticFunction function, int neuronsCount, int inputsCount)
            : base(neuronsCount, inputsCount, function)
        {
            neurons = new StochasticNeuron[neuronsCount];
            for (int i = 0; i < neurons.Length; i++)
                base.neurons[i] = this.neurons[i] =
                    new StochasticNeuron(inputsCount, function);
        }

        /// <summary>
        ///   Compute output vector of the layer.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>
        ///   Returns layer's output vector.
        /// </returns>
        /// 
        public override double[] Compute(double[] input)
        {
            double[] output = new double[neuronsCount];

            for (int i = 0; i < neurons.Length; i++)
                output[i] = neurons[i].Compute(input);

            this.output = output;

            return output;
        }
        /// <summary>
        ///   Compute probability vector of the layer.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>
        ///   Returns layer's probability vector.
        /// </returns>
        ///     
        public double[] Generate(double[] input)
        {
            double[] sample = new double[neuronsCount];
            double[] output = new double[neuronsCount];

            for (int i = 0; i < neurons.Length; i++)
            {
                sample[i] = neurons[i].Generate(input);
                output[i] = neurons[i].Output;
            }

            this.sample = sample;
            this.output = output;

            return sample;
        }
        /// <summary>
        ///   Copy the weights of another layer in reversed order. This
        ///   can be used to update visible layers from hidden layers and
        ///   vice-versa.
        /// </summary>
        /// 
        /// <param name="layer">The layer to copy the weights from.</param>
        /// 
        public void CopyReversedWeightsFrom(StochasticLayer layer)
        {
            for (int i = 0; i < Neurons.Length; i++)
                for (int j = 0; j < inputsCount; j++)
                    this.Neurons[i].Weights[j] = layer.Neurons[j].Weights[i];
        }
    }
}
