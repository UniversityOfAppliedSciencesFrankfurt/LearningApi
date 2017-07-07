using LearningFoundation;
using System;

namespace NeuralNetworks.Core.Neurons
{
    /// <summary>
    /// Activation neuron.
    /// </summary>
    /// 
    /// <remarks><para>Activation neuron computes weighted sum of its inputs, adds
    /// threshold value and then applies activation function.
    /// The neuron isusually used in multi-layer neural networks.</para></remarks>
    /// 

    [Serializable]
    public class ActivationNeuron : Neuron
    {
        /// <summary>
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks>The value is added to inputs weighted sum before it is passed to activation
        /// function.</remarks>
        /// 
        protected double threshold = 0.0;

        /// <summary>
        /// Activation function.
        /// </summary>
        /// 
        /// <remarks>The function is applied to inputs weighted sum plus
        /// threshold value.</remarks>
        /// 
        protected IActivationFunction function = null;

        /// <summary>
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks>The value is added to inputs weighted sum before it is passed to activation
        /// function.</remarks>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        /// Neuron's activation function.
        /// </summary>
        /// 
        public IActivationFunction ActivationFunction
        {
            get { return function; }
            set { function = value; }

        }

        /// <summary>
        /// Initializes a new instance of the Activation Neuron class.
        /// </summary>
        /// 
        /// <param name="inputs">Neuron's inputs count.</param>
        /// <param name="function">Neuron's activation function.</param>
        /// 
        public ActivationNeuron(int inputs, IActivationFunction function)
            : base(inputs)
        {
            this.function = function;
        }

        /// <summary>
        /// Randomize neuron.
        /// </summary>
        /// 
        /// <remarks>Calls base class Neuron.Randomizemethod
        /// to randomize neuron's weights and then randomizes threshold's value.</remarks>
        /// 
        public override void Randomize()
        {
            // randomize weights
            base.Randomize();

            // randomize threshold
            threshold = rand.Generate();
        }


        /// <summary>
        /// Computes output value of neuron.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns neuron's output value.</returns>
        ///        
        public override double Compute(double[] input)
        {
            // check for corrent input vector
            if (input.Length != inputsCount)
                throw new ArgumentException("Wrong length of the input vector.");

            // initial sum value
            double sum = 0.0;

            // compute weighted sum of inputs
            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * input[i];
            sum += threshold;

            // local variable to avoid mutlithreaded conflicts
            double output = function.Function(sum);

            // assign output property as well (works correctly for single threaded usage)
            this.output = output;

            return output;
        }
    }
}
