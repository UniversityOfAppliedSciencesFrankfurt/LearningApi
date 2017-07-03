using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;

namespace NeuralNetworks.Core.Neurons
{
    /// <summary>
    ///   Stochastic Activation Neuron.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Stochastic Activation Neuron is an activation neuron
    ///   which activates (returns 1) only within a given probability.
    ///   The neuron has a random component in the activation function,
    ///   and the neuron fires only if the total sum, after applied
    ///   to a logistic activation function, is greater than a randomly
    ///   sampled value.
    /// </remarks>
    /// 
    [Serializable]
    public class StochasticNeuron : ActivationNeuron
    {

        private double sample;
        private new IStochasticFunction function;

        /// <summary>
        ///   Gets or sets the stochastic activation 
        ///   function for this stochastic neuron.
        /// </summary>
        /// 

        public new IStochasticFunction ActivationFunction
        {
            get { return function; }
            set { base.function = this.function = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="StochasticNeuron"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">Number of inputs for the neuron.</param>
        /// <param name="function">Activation function for the neuron.</param>
        /// 
        public StochasticNeuron( int inputs, IStochasticFunction function )
            : base( inputs, function )
        {
            this.ActivationFunction = function;
            this.Threshold = 0;
        }

        /// <summary>
        ///   Computes output value of neuron.
        /// </summary>
        /// 
        /// Input  : An input vector
        /// 
        /// <returns>Returns the neuron's output value for the given input.</returns>
        /// 
           //this.hidden = new StochasticLayer(function, hiddenNeurons, inputsCount);
        public override double Compute( double[] input ) //1st: 111000
        {
            double sum = threshold; //1st :0
            for (int i = 0; i < weights.Length; i++) // Length -6
                sum += weights[i] * input[i];
            double output = function.Function( sum );   
            this.output = output;
            return output;
        }

        /// <summary>
        ///   Samples the neuron output considering
        ///   the stochastic activation function.
        /// </summary>
        /// 
        /// <param name="input">An input vector.</param>
        /// 
        /// <returns>A possible output for the neuron drawn
        /// from the neuron's stochastic function.</returns>
        /// 

        public double Generate( double[] input )
        {
            double sum = threshold;
            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * input[i];
            double output = function.Function( sum );
            double sample = function.Generate2( output );
            this.output = output;
            this.sample = sample;
            return sample;
        }
        /// <summary>
        ///   Samples the neuron output considering
        ///   the stochastic activation function.
        /// </summary>
        /// 
        /// <param name="output">The (previously computed) neuron output.</param>
        /// 
        /// <returns>A possible output for the neuron drawn
        /// from the neuron's stochastic function.</returns>
        /// 

        public double Generate( double output )
        {
            double sample = function.Generate2( output );
            this.output = output;
            this.sample = sample;
            return sample;
        }

    }
}
