using LearningFoundation.MathFunction;
using LearningFoundation.Statistics;
using System;
namespace NeuralNetworks.Core.Neurons
{
    /// <summary>
    /// Base neuron class.
    /// </summary>
    /// 
    /// <remarks>This is a base neuron class, which encapsulates such
    /// common properties, like neuron's input, output and weights.</remarks>
    /// 
    [Serializable]
    public abstract class Neuron
    {
        /// <summary>
        /// Neuron's inputs count.
        /// </summary>
        protected int inputsCount = 0;
        /// <summary>
        /// Neuron's weights.
        /// </summary>
        protected double[] weights = null;
        /// <summary>
        /// Neuron's output value.
        /// </summary>
        protected double output = 0;
        /// <summary>
        /// Random number generator.
        /// </summary>
        /// 
        /// <remarks>The generator is used for neuron's weights randomization.</remarks>
        /// 
        protected IRandomNumberGenerator<double> rand = new UniformContinuousDistribution( );
        /// <summary>
        /// Neuron's output value.
        /// </summary>
        /// 
        /// <remarks>The calculation way of neuron's output value is determined by inherited class.</remarks>
        /// 
        public double Output
        {
            get { return output; }
        }
        /// <summary>
        /// Neuron's weights.
        /// </summary>
        public double[] Weights
        {
            get { return weights; }
        }
        /// <summary>
        /// Initializes a new instance of the Neuronclass.
        /// </summary>
        ///
        /// <param name="inputs">Neuron's inputs count.</param>
        /// 
        /// <remarks>The new neuron will be randomized 
        /// after it is created.</remarks>
        ///
        protected Neuron( int inputs )
        {
            // allocate weights
            inputsCount = Math.Max( 1, inputs );
            weights = new double[inputsCount];
            // randomize the neuron
            Randomize( );
        }
        /// <summary>
        /// Randomize neuron.
        /// </summary>
        /// 
        /// <remarks>
        ///   Initialize neuron's weights with random values specified
        ///   by the RandGenerator
        /// 
        public virtual void Randomize()
        {
            rand.Generate( weights.Length, weights );
        }
        /// <summary>
        /// Computes output value of neuron.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns neuron's output value.</returns>
        /// 
        /// <remarks>The actual neuron's output value is determined by inherited class.
        /// The output value is also stored in the output property.</remarks>
        /// 
        public abstract double Compute( double[] input );
    }
}
