using System;
using LearningFoundation.MathFunction;
using LearningFoundation;

namespace NeuralNetworks.Core.ActivationFunctions
{

    //public class BernoulliFunctionGen : BernoulliFunction
    //{
    //    private double alpha = 0.5;
    //    public override double Function(double x)
    //    {
    //        double y = base.Function(x);
    //        return y > Generator.Random.NextDouble() ? 1 : 0;
    //    }
    //}
    /// <summary>
    ///   Bernoulli stochastic activation function.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Bernoulli activation function can be used to create 
    ///   Stochastic Neurons, which can in turn be used to create 
    ///   Deep Belief Networks and Restricted Boltzmann
    ///   Machines.
    ///   The use of a Bernoulli function is indicated when the inputs of a problem
    ///   are discrete, it is, are either 0 or 1.
    ///   As a stochastic activation function, the Bernoulli
    ///   function is able to generate values following a statistic probability distribution.In
    ///   this case, the Bernoulli function follows a Bernoulli
    ///   distribution with its mean value given by
    ///   the output of this class' sigmoidal function</para>
    /// </remarks>
    /// 
    [Serializable]
    public class BernoulliFunction : IStochasticFunction
    {
        /// <summary>
        ///   Sigmoid's alpha value.
        /// </summary>
        /// 
        /// <remarks><para>The value determines steepness of the function. Increasing value of
        /// this property changes sigmoid to look more like a threshold function. Decreasing
        /// value of this property makes sigmoid to be very smooth (slowly growing from its
        /// minimum value to its maximum value).</para>
        ///
        private double alpha;
        
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the BernoulliFunction class.
        /// </summary>
        ///        
        
        public BernoulliFunction(double alpha)
        {
            this.alpha = alpha;
        }

        /// <summary>
        /// Calculates function value.
        /// </summary>
        ///
        /// Input value: x
        /// 
        /// Output value: f(x)
        ///
        /// The method calculates function value at point x
        ///
        public virtual double Function(double x)
        {
            return (1 / (1 + Math.Exp(-alpha * x)));

        }

        /// <summary>
        ///   Samples a value from the function given a input value.
        /// </summary>
        /// 
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Draws a random value from the function.</returns>
        /// 

        public double Generate(double x)
        {
            double y = Function(x);
            return y > Generator.Random.NextDouble() ? 1 : 0;
        }

        /// <summary>
        ///   Samples a value from the function given a function output value.
        /// </summary>
        /// 
        /// <param name="y">The function output value. 
        /// 
        /// <remarks><para>The method calculates the same output value as the
        /// Generate method, but it takes not the input x value
        /// itself, but the function value, which was calculated previously with help
        /// of the IActivation.Function method.</para>
        /// </remarks>
        /// 
        /// <returns>Draws a random value from the function.</returns>
        /// 

        public double Generate2(double y)
        {
            return y > Generator.Random.NextDouble() ? 1 : 0;
        }
        /// <summary>
        /// Calculates function derivative.
        /// </summary>
        /// 
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function derivative, <i>f'(x)</i>.</returns>
        /// 
        /// <remarks>The method calculates function derivative at point <paramref name="x"/>.</remarks>
        ///
        public double Derivative( double x )
        {
            double y = Function( x );

            return (alpha * y * (1 - y));
        }

        /// <summary>
        /// Calculates function derivative.
        /// </summary>
        /// 
        /// <param name="y">Function output value - the value, which was obtained
        /// with the help of <see cref="Function"/> method.</param>
        /// 
        /// <returns>Function derivative, <i>f'(x)</i>.</returns>
        /// 
        /// <remarks><para>The method calculates the same derivative value as the
        /// <see cref="Derivative"/> method, but it takes not the input <b>x</b> value
        /// itself, but the function value, which was calculated previously with
        /// the help of <see cref="Function"/> method.</para>
        /// 
        /// <para><note>Some applications require as function value, as derivative value,
        /// so they can save the amount of calculations using this method to calculate derivative.</note></para>
        /// </remarks>
        /// 
        public double Derivative2( double y )
        {
            return (alpha * y * (1 - y));
        }


    }
   
}
