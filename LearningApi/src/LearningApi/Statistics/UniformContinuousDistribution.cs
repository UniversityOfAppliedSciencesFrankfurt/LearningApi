using LearningFoundation.Attributes;
using LearningFoundation.MathFunction;
using System;
using System.ComponentModel;
namespace LearningFoundation.Statistics
{
    /// <summary>
    ///   Continuous Uniform Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The continuous uniform distribution or rectangular distribution is a family of 
    ///   symmetric probability distributions such that for each member of the family, all
    ///   intervals of the same length on the distribution's support are equally probable.
    ///   The support is defined by the two parameters, a and b, which are its minimum and
    ///   maximum values. The distribution is often abbreviated U(a,b). It is the maximum
    ///   entropy probability distribution for a random variate X under no constraint other
    ///   than that it is contained in the distribution's support.</para>
    ///   
    /// 

    [Serializable]
    public class UniformContinuousDistribution : UnivariateContinuousDistribution, ISampleableDistribution<double>
    {
        private double a;
        private double b;

        /// <summary>
        ///   Creates a new uniform distribution defined in the interval [0;1].
        /// </summary>
        /// 
        public UniformContinuousDistribution() : this( 0, 1 ) { }

        /// <summary>
        ///   Creates a new uniform distribution defined in the interval [a;b].
        /// </summary>
        /// 
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// 
        public UniformContinuousDistribution(
            [Real, DefaultValue( 0 )] double a,
            [Real, DefaultValue( 1 )] double b )
        {
            if (a > b)
            {
                throw new ArgumentOutOfRangeException( "b",
                    "The starting number a must be lower than b." );
            }
            this.a = a;
            this.b = b;
        }
        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange( a, b ); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction( double x )
        {
            if (x < a)
                return 0;
            if (x >= b)
                return 1;
            return (x - a) / (b - a);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="a">The starting number a.</param>
        /// <param name="b">The ending number b.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public override double[] Generate( int samples, double[] result )
        {
            return Random( a, b, samples, result );//test
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Uniform distribution with the given parameters.
        /// </summary>
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random( double a, double b, int samples, double[] result )
        {
            var rand = Generator.Random;
            for (int i = 0; i < samples; i++)
                result[i] = rand.NextDouble( ) * (b - a) + a;
            return result;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public override double Generate()
        {
            return Random( a, b );//test
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Uniform distribution with the given parameters.
        /// </summary>
        /// <returns>A random double value sampled from the specified Uniform distribution.</returns>
        /// 
        public static double Random( double a, double b )
        {
            return Generator.Random.NextDouble( ) * (b - a) + a;
        }


    }
}
