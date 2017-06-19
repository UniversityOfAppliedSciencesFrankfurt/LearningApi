using LearningFoundation.MathFunction;
using System;
namespace LearningFoundation.Statistics
{
    /// <summary>
    ///   Abstract class for univariate continuous probability Distributions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A probability distribution identifies either the probability of each value of an
    ///   unidentified random variable (when the variable is discrete), or the probability
    ///   of the value falling within a particular interval (when the variable is continuous).</para>
    /// <para>
    ///   The probability distribution describes the range of possible values that a random
    ///   variable can attain and the probability that the value of the random variable is
    ///   within any (measurable) subset of that range.</para>  
    /// <para>
    ///   The function describing the probability that a given value will occur is called
    ///   the probability function (or probability density function, abbreviated PDF), and
    ///   the function describing the cumulative probability that a given value or any value
    ///   smaller than it will occur is called the distribution function (or cumulative
    ///   distribution function, abbreviated CDF).</para>  
    ///   

    [Serializable]
    public abstract class UnivariateContinuousDistribution : ISampleableDistribution<double>
    {
        /// <summary>
        ///   Constructs a new UnivariateDistribution class.
        /// </summary>
        /// 
        protected UnivariateContinuousDistribution()
        {
        }
        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>A <see cref="DoubleRange"/> containing
        ///  the support interval for this distribution.</value>
        ///  
        public abstract DoubleRange Support { get; }
        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// Need for InverseDistributionFunction
        public abstract double DistributionFunction( double x );
        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function 
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.
        /// </remarks>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>A sample which could original the given probability 
        ///   value when applied in the Distribution Function/>.</returns>
        /// 
        public virtual double InverseDistributionFunction(
            double p )
        {
            if (p < 0.0 || p > 1.0)
                throw new ArgumentOutOfRangeException( "p", "Value must be between 0 and 1." );
            if (Double.IsNaN( p ))
                throw new ArgumentOutOfRangeException( "p", "Value is Not-a-Number (NaN)." );
            if (p == 0)
                return Support.Min;
            else if (p == 1)
                return Support.Max;
            bool lowerBounded = !Double.IsInfinity( Support.Min );
            bool upperBounded = !Double.IsInfinity( Support.Max );
            double lower;
            double upper;
            double f;
            if (lowerBounded && upperBounded)
            {
                lower = Support.Min;
                upper = Support.Max;
                f = 0.5;
            }
            else if (lowerBounded && !upperBounded)
            {
                lower = Support.Min;
                upper = lower + 1;
                f = DistributionFunction( lower );
                if (f > p)
                {
                    while (f > p && !Double.IsInfinity( upper ))
                    {
                        upper += 2 * (upper - lower) + 1;
                        f = DistributionFunction( upper );
                    }
                }
                else
                {
                    while (f < p && !Double.IsInfinity( upper ))
                    {
                        upper += 2 * (upper - lower) + 1;
                        f = DistributionFunction( upper );
                    }
                }
            }
            else if (!lowerBounded && upperBounded)
            {
                upper = Support.Max;
                lower = upper - 1;
                f = DistributionFunction( upper );
                if (f > p)
                {
                    while (f > p && !Double.IsInfinity( lower ))
                    {
                        lower = lower - 2 * lower;
                        f = DistributionFunction( lower );
                    }
                }
                else
                {
                    while (f < p && !Double.IsInfinity( lower ))
                    {
                        lower = lower - 2 * lower;
                        f = DistributionFunction( lower );
                    }
                }
            }
            else // completely unbounded
            {
                lower = 0;
                upper = 0;
                f = DistributionFunction( 0 );
                if (f > p)
                {
                    while (f > p && !Double.IsInfinity( lower ))
                    {
                        upper = lower;
                        lower = 2 * lower - 1;
                        f = DistributionFunction( lower );
                    }
                }
                else
                {
                    while (f < p && !Double.IsInfinity( upper ))
                    {
                        lower = upper;
                        upper = 2 * upper + 1;
                        f = DistributionFunction( upper );
                    }
                }
            }
            if (Double.IsNegativeInfinity( lower ))
                lower = Double.MinValue;
            if (Double.IsPositiveInfinity( upper ))
                upper = Double.MaxValue;
            double value = BrentSearch.Find( DistributionFunction, p, lower, upper );
            return value;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate( int samples )
        {
            return Generate( samples, new double[samples] );
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public virtual double[] Generate( int samples, double[] result )
        {
            var random = Generator.Random;
            for (int i = 0; i < samples; i++)
                result[i] = InverseDistributionFunction( random.NextDouble( ) );
            return result;
        }
        
        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public virtual double Generate()
        {
            return InverseDistributionFunction( Generator.Random.NextDouble( ) );
        }

        double ISampleableDistribution<double>.Generate( double result )
        {
            return Generate( );
        }

    }
}