using System;
using LearningFoundation;

namespace NeuralNetworks.Core.ActivationFunctions
{
    /// <summary>
    /// Sigmoid activation function.
    /// </summary>
    ///   
    /// 
    [Serializable]
    public class SigmoidFunction : IActivationFunction
    {
        // sigmoid's alpha value
        private double alpha = 2;

        /// <summary>
        /// Sigmoid's alpha value.
        /// </summary>
        /// 
        /// <remarks><para>The value determines steepness of the function. Increasing value of
        /// this property changes sigmoid to look more like a threshold function. Decreasing
        /// value of this property makes sigmoid to be very smooth (slowly growing from its
        /// minimum value to its maximum value).</para>
        ///
        /// <para>Default value is set to <b>2</b>.</para>
        /// </remarks>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }


        /// <summary>
        /// Initializes a new instance of the SigmoidFunction class.
        /// </summary>
        public SigmoidFunction() { }

        /// <summary>
        /// Initializes a new instance of the Sigmoid Function class with specific alpha
        /// </summary>
        /// 
        /// <param name="alpha">Sigmoid's alpha value.</param>
        /// 
        public SigmoidFunction(double alpha)
        {
            this.alpha = alpha;
        }


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
            return (1 / (1 + Math.Exp(-alpha * x)));
        }

    }
}
