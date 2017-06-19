using System;
namespace LearningFoundation.Statistics
{
    /// <summary>
    ///   Normal (Gaussian) distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory, the normal (or Gaussian) distribution is a very 
    ///   commonly occurring continuous probability distribution—a function that
    ///   tells the probability that any real observation will fall between any two
    ///   real limits or real numbers, as the curve approaches zero on either side.
    ///   Normal distributions are extremely important in statistics and are often 
    ///   used in the natural and social sciences for real-valued random variables
    ///   whose distributions are not known.</para>
    /// <para>
    ///   The normal distribution is immensely useful because of the central limit 
    ///   theorem, which states that, under mild conditions, the mean of many random 
    ///   variables independently drawn from the same distribution is distributed 
    ///   approximately normally, irrespective of the form of the original distribution:
    ///   physical quantities that are expected to be the sum of many independent processes
    ///   (such as measurement errors) often have a distribution very close to the normal.
    ///   Moreover, many results and methods (such as propagation of uncertainty and least
    ///   squares parameter fitting) can be derived analytically in explicit form when the
    ///   relevant variables are normally distributed.</para>
    /// <para>
    ///   The Gaussian distribution is sometimes informally called the bell curve. However,
    ///   many other distributions are bell-shaped (such as Cauchy's, Student's, and logistic).
    ///   The terms Gaussian function and Gaussian bell curve are also ambiguous because they
    ///   sometimes refer to multiples of the normal distribution that cannot be directly
    ///   interpreted in terms of probabilities.</para>
    ///   
    /// <para>
    ///   The Gaussian is the most widely used distribution for continuous
    ///   variables. In the case of a single variable, it is governed by
    ///   two parameters, the mean and the variance.</para>
    ///   
    /// <para> 
    /// 
    [Serializable]
    public class NormalDistribution
    {

        // Distribution parameters
        private double mean = 0;   // mean μ
        private double stdDev = 1; // standard deviation σ

        // Distribution measures
        private double? entropy;

        // Derived measures
        private double variance = 1; // σ²
        private double lnconstant;   // log(1/sqrt(2*pi*variance))

        private bool immutable;

        // 97.5 percentile of standard normal distribution
        private const double p95 = 1.95996398454005423552;

        [ThreadStatic]
        private static bool useSecond = false;

        [ThreadStatic]
        private static double secondValue = 0;



        /// <summary>
        ///   Generates a random value from a standard Normal 
        ///   distribution (zero mean and unit standard deviation).
        /// </summary>
        /// 
        public static double Random()
        {
            var rand = MathFunction.Generator.Random;

            // check if we can use second value
            if (useSecond)
            {
                // return the second number
                useSecond = false;
                return secondValue;
            }

            double x1, x2, w, firstValue;

            // generate new numbers
            do
            {
                x1 = rand.NextDouble( ) * 2.0 - 1.0;
                x2 = rand.NextDouble( ) * 2.0 - 1.0;
                w = x1 * x1 + x2 * x2;
            }
            while (w >= 1.0);

            w = Math.Sqrt( (-2.0 * Math.Log( w )) / w );

            // get two standard random numbers
            firstValue = x1 * w;
            secondValue = x2 * w;

            useSecond = true;

            // return the first number
            return firstValue;
        }
    }
}
