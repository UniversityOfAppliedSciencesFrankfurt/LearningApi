using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using LearningFoundation.Statistics;

namespace NeuralNetworks.Core.ActivationFunctions
{
    /// <summary>
    ///   Gaussian stochastic activation function.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Gaussian activation function can be used to create 
    ///   Stochastic Neurons, which can in turn be used to create 
    ///   Deep Belief Networks and Restricted Boltzmann
    ///   Machines. In contrast to the BernoulliFunction, the Gaussian can be used
    ///   to model continuous inputs in Deep Belief Networks. If, however, the inputs of the problem
    ///   being learned are discrete in nature, the use of a Bernoulli function would be more indicated.</para>
    ///   
    /// <para>
    ///   The Gaussian activation function is modeled after a 
    ///   Gaussian (Normal) probability distribution</see>.
    /// </para>
    /// 
    /// <para>
    ///   This function assumes output variables have been 
    ///   normalized to have zero mean and unit variance.</para>
    /// </remarks>
    /// 
    [Serializable]
   
    public class GaussianFunction : IStochasticFunction
    {

        // linear slope value
        private double alpha = 1;

        // function output range
        private DoubleRange range = new DoubleRange(-1, +1);

        /// <summary>
        /// Linear slope value.
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///   Creates a new Gaussian Function
        /// </summary>
        /// 
        /// <param name="alpha">The linear slope value.</param>
        /// 
        public GaussianFunction(double alpha)
        {
            this.alpha = alpha;
        }

        /// <summary>
        ///   Creates a new Gaussian Function with the default value
        /// </summary>
        /// 

        public GaussianFunction()
            : this(1.0) { }


        /// <summary>
        /// Calculates function value.
        /// </summary>
        ///
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function output value, <i>f(x)</i>.</returns>
        ///
        /// <remarks>The method calculates function value at point <paramref name="x"/>.</remarks>
        ///

        public double Function(double x)
        {
            double y = alpha * x;

            if (y > range.Max)
                return range.Max;
            else if (y < range.Min)
                return range.Min;
            return y;
        }

        /// <summary>
        ///   Samples a value from the function given a input value.
        /// </summary>
        /// 
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>
        ///   Draws a random value from the function.
        /// </returns>
        /// 

        public double Generate(double x)
        {
            // assume zero-mean noise
            double y = alpha * x + NormalDistribution.Random();

            if (y > range.Max)
                y = range.Max;
            else if (y < range.Min)
                y = range.Min;

            return y;
        }


        /// <summary>
        ///   Samples a value from the function given a function output value.
        /// </summary>
        /// 
        /// <param name="y">Function output value - the value, which was obtained
        /// with the help of <see cref="Function"/> method.</param>
        /// 
        /// <returns>
        ///   Draws a random value from the function.
        /// </returns>
        /// 
        public double Generate2(double y)
        {
            y = y + NormalDistribution.Random();

            if (y > range.Max)
                y = range.Max;
            else if (y < range.Min)
                y = range.Min;

            return y;
        }



    }
}



