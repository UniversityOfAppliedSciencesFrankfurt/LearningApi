using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LearningFoundation
{
    /// <summary>
    ///   Common interface for stochastic activation functions.
    /// </summary>
    /// 
    /// <seealso cref="BernoulliFunction"/>
    /// <seealso cref="GaussianFunction"/>
    /// 
    public interface IStochasticFunction : IActivationFunction
    {
        /// <summary>
        ///   Samples a value from the function given a input value.
        /// </summary>
        /// 
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Draws a random value from the function.</returns>
        /// 
        double Generate( double x );

        /// <summary>
        ///   Samples a value from the function given a function output value.
        /// </summary>
        /// 
        /// <param name="y">The function output value. This is the value which was obtained
        /// with the help of the IActivationFunction method.</param>
        /// 
        /// The method calculates the same output value as the
        /// Generate method, but it takes not the input <b>x</b> value
        /// itself, but the function value, which was calculated previously with help
        /// of the IActivationFunction method.
        /// 
        /// <returns>Draws a random value from the function.</returns>
        /// 
        double Generate2( double y );
    }
}
