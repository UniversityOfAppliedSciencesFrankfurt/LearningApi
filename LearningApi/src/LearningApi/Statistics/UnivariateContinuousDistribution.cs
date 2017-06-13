using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using LearningFoundation.Statistics;
using LearningFoundation.Fitting;
using LearningFoundation.MathFunction;
using System.Runtime.Serialization.Formatters;
namespace LearningFoundation.Statistics
{
    [Serializable]
    public abstract class UnivariateContinuousDistribution : DistributionBase,
        IDistribution, IUnivariateDistribution, IUnivariateDistribution<double>,
        ISampleableDistribution<double>,
       IFormattable //TODO remove
    {
        [NonSerialized]
        private double? m_Median;

        [NonSerialized]
        private double? stdDev;

        [NonSerialized]
        private double? m_Mode;

        [NonSerialized]
        private DoubleRange? quartiles;
        protected UnivariateContinuousDistribution()
        {
        }
        public abstract double Mean { get; }
        public abstract double Variance { get; }

        public abstract double Entropy { get; }
        public abstract DoubleRange Support { get; }
        public virtual double Mode
        {
            get
            {
                if (m_Mode == null)
                    m_Mode = BrentSearch.Maximize(ProbabilityDensityFunction, Quartiles.Min, Quartiles.Max, 1e-10);

                return m_Mode.Value;
            }
        }
        public virtual DoubleRange Quartiles
        {
            get
            {
                if (quartiles == null)
                {
                    double min = InverseDistributionFunction(0.25);
                    double max = InverseDistributionFunction(0.75);
                    quartiles = new DoubleRange(min, max);
                }

                return quartiles.Value;
            }
        }
        public virtual DoubleRange GetRange(double percentile)
        {
            if (percentile <= 0 || percentile > 1)
                throw new ArgumentOutOfRangeException("percentile", "The percentile must be between 0 and 1.");

            double a = InverseDistributionFunction(1.0 - percentile);
            double b = InverseDistributionFunction(percentile);

            if (b > a)
                return new DoubleRange(a, b);
            return new DoubleRange(b, a);
        }
        public virtual double Median
        {
            get
            {
                if (m_Median == null)
                    m_Median = InverseDistributionFunction(0.5);

                return m_Median.Value;
            }
        }
        public virtual double StandardDeviation
        {
            get
            {
                if (!stdDev.HasValue)
                    stdDev = System.Math.Sqrt(this.Variance);
                return stdDev.Value;
            }
        }
        #region IDistribution explicit members
        double IDistribution.DistributionFunction(double[] x)
        {
            return DistributionFunction(x[0]);
        }
        double IDistribution.ComplementaryDistributionFunction(double[] x)
        {
            return ComplementaryDistributionFunction(x[0]);
        }
        double IDistribution.ProbabilityFunction(double[] x)
        {
            return ProbabilityDensityFunction(x[0]);
        }
        double IUnivariateDistribution.ProbabilityFunction(double x)
        {
            return ProbabilityDensityFunction(x);
        }
        double IDistribution.LogProbabilityFunction(double[] x)
        {
            return LogProbabilityDensityFunction(x[0]);
        }
        double IUnivariateDistribution.LogProbabilityFunction(double x)
        {
            return LogProbabilityDensityFunction(x);
        }
        void IDistribution.Fit(Array observations)
        {
            (this as IDistribution).Fit(observations, (IFittingOptions)null);
        }
        void IDistribution.Fit(Array observations, double[] weights)
        {
            (this as IDistribution).Fit(observations, weights, (IFittingOptions)null);
        }
        void IDistribution.Fit(Array observations, int[] weights)
        {
            (this as IDistribution).Fit(observations, weights, (IFittingOptions)null);
        }
        void IDistribution.Fit(Array observations, IFittingOptions options)
        {
            (this as IDistribution).Fit(observations, (double[])null, options);
        }
        void IDistribution.Fit(Array observations, double[] weights, IFittingOptions options)
        {
            double[] univariate = observations as double[];
            if (univariate != null)
            {
                Fit(univariate, weights, options);
                return;
            }

            double[][] multivariate = observations as double[][];
            if (multivariate != null)
            {
                Fit(Matrix.Concatenate(multivariate), weights, options);
                return;
            }

            throw new ArgumentException("Invalid input type.", "observations");
        }
        void IDistribution.Fit(Array observations, int[] weights, IFittingOptions options)
        {
            double[] univariate = observations as double[];
            if (univariate != null)
            {
                Fit(univariate, weights, options);
                return;
            }

            double[][] multivariate = observations as double[][];
            if (multivariate != null)
            {
                Fit(Matrix.Concatenate(multivariate), weights, options);
                return;
            }

            throw new ArgumentException("Invalid input type.", "observations");
        }
        #endregion

        public abstract double DistributionFunction(double x);
        /// 
        public virtual double DistributionFunction(double a, double b)
        {
            if (a > b)
            {
                throw new ArgumentOutOfRangeException("b",
                    "The start of the interval a must be smaller than b.");
            }
            else if (a == b)
            {
                return 0;
            }

            return DistributionFunction(b) - DistributionFunction(a);
        }
        public virtual double ComplementaryDistributionFunction(double x)
        {
            return 1.0 - DistributionFunction(x);
        }
        public virtual double InverseDistributionFunction(

            double p)
        {
            if (p < 0.0 || p > 1.0)
                throw new ArgumentOutOfRangeException("p", "Value must be between 0 and 1.");

            if (Double.IsNaN(p))
                throw new ArgumentOutOfRangeException("p", "Value is Not-a-Number (NaN).");

            if (p == 0)
                return Support.Min;

            else if (p == 1)
                return Support.Max;

            bool lowerBounded = !Double.IsInfinity(Support.Min);
            bool upperBounded = !Double.IsInfinity(Support.Max);

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

                f = DistributionFunction(lower);

                if (f > p)
                {
                    while (f > p && !Double.IsInfinity(upper))
                    {
                        upper += 2 * (upper - lower) + 1;
                        f = DistributionFunction(upper);
                    }
                }
                else
                {
                    while (f < p && !Double.IsInfinity(upper))
                    {
                        upper += 2 * (upper - lower) + 1;
                        f = DistributionFunction(upper);
                    }
                }
            }

            else if (!lowerBounded && upperBounded)
            {
                upper = Support.Max;
                lower = upper - 1;

                f = DistributionFunction(upper);

                if (f > p)
                {
                    while (f > p && !Double.IsInfinity(lower))
                    {
                        lower = lower - 2 * lower;
                        f = DistributionFunction(lower);
                    }
                }
                else
                {
                    while (f < p && !Double.IsInfinity(lower))
                    {
                        lower = lower - 2 * lower;
                        f = DistributionFunction(lower);
                    }
                }
            }

            else // completely unbounded
            {
                lower = 0;
                upper = 0;

                f = DistributionFunction(0);

                if (f > p)
                {
                    while (f > p && !Double.IsInfinity(lower))
                    {
                        upper = lower;
                        lower = 2 * lower - 1;
                        f = DistributionFunction(lower);
                    }
                }
                else
                {
                    while (f < p && !Double.IsInfinity(upper))
                    {
                        lower = upper;
                        upper = 2 * upper + 1;
                        f = DistributionFunction(upper);
                    }
                }
            }

            if (Double.IsNegativeInfinity(lower))
                lower = Double.MinValue;

            if (Double.IsPositiveInfinity(upper))
                upper = Double.MaxValue;

            double value = BrentSearch.Find(DistributionFunction, p, lower, upper);

            return value;
        }

         
        public virtual double QuantileDensityFunction(double p)
        {
            return 1.0 / ProbabilityDensityFunction(InverseDistributionFunction(p));
        }

     
        public abstract double ProbabilityDensityFunction(double x);
      
        public virtual double LogProbabilityDensityFunction(double x)
        {
            return Math.Log(ProbabilityDensityFunction(x));
        }

      
        public virtual double HazardFunction(double x)
        {
            double f = ProbabilityDensityFunction(x);
            if (f == 0)
                return 0;

            double s = ComplementaryDistributionFunction(x);
            return f / s;
        }

       
        public virtual double CumulativeHazardFunction(double x)
        {
            return Math.Log(ComplementaryDistributionFunction(x));
        }

        
        public virtual double LogCumulativeHazardFunction(double x)
        {
            return Math.Log(-System.Math.Log(ComplementaryDistributionFunction(x)));
        }

        
        public virtual void Fit(double[] observations)
        {
            Fit(observations, (IFittingOptions)null);
        }

        
        public virtual void Fit(double[] observations, double[] weights)
        {
            Fit(observations, weights, (IFittingOptions)null);
        }

       
        public virtual void Fit(double[] observations, int[] weights)
        {
            Fit(observations, weights, (IFittingOptions)null);
        }

        
        public virtual void Fit(double[] observations, IFittingOptions options)
        {
            Fit(observations, (double[])null, options);
        }

       
        public virtual void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            throw new NotSupportedException();
        }

       
        public virtual void Fit(double[] observations, int[] weights, IFittingOptions options)
        {
            if (weights != null)
                throw new NotSupportedException();

            Fit(observations, (double[])null, options);
        }
     
        public double[] Generate(int samples)
        {
            return Generate(samples, new double[samples]);
        }

        
        public virtual double[] Generate(int samples, double[] result)
        {
            var random = Generator.Random;
            for (int i = 0; i < samples; i++)
                result[i] = InverseDistributionFunction(random.NextDouble());
            return result;
        }

      
        public virtual double Generate()
        {
            return InverseDistributionFunction(Generator.Random.NextDouble());
        }

        double ISampleableDistribution<double>.Generate(double result)
        {
            return Generate();
        }

        double IDistribution<double>.ProbabilityFunction(double x)
        {
            return ProbabilityDensityFunction(x);
        }

        double IDistribution<double>.LogProbabilityFunction(double x)
        {
            return LogProbabilityDensityFunction(x);
        }

    }
}