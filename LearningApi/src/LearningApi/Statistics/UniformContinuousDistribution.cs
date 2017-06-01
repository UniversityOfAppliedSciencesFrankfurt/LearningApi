using System;
using System.ComponentModel;
using LearningFoundation.Attributes;
using LearningFoundation.Statistics;
using LearningFoundation.Fitting;


namespace LearningFoundation.Statistics
{
    [Serializable]
    public class UniformContinuousDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, IFittingOptions>,
        ISampleableDistribution<double>
    {
        private double a;
        private double b;
        private bool immutable;

        public UniformContinuousDistribution() : this(0, 1) { }
        public UniformContinuousDistribution(DoubleRange range)
            : this(range.Min, range.Max)
        {
        }
        public UniformContinuousDistribution(
            [Real, DefaultValue(0)] double a,
            [Real, DefaultValue(1)] double b)
        {
            if (a > b)
            {
                throw new ArgumentOutOfRangeException("b",
                    "The starting number a must be lower than b.");
            }

            this.a = a;
            this.b = b;
        }

        public double Minimum { get { return a; } }
        public double Maximum { get { return b; } }
        public double Length { get { return b - a; } }
        public override double Mean
        {
            get { return (a + b) / 2; }
        }
        public override double Variance
        {
            get { return ((b - a) * (b - a)) / 12.0; }
        }
        public override double Mode
        {
            get { return Mean; }
        }
        public override double Entropy
        {
            get { return System.Math.Log(b - a); }
        }
        public override DoubleRange Support
        {
            get { return new DoubleRange(a, b); }
        }
        public override double DistributionFunction(double x)
        {
            if (x < a)
                return 0;

            if (x >= b)
                return 1;

            return (x - a) / (b - a);
        }
        public override double ProbabilityDensityFunction(double x)
        {
            if (x > a && x <= b)
                return 1.0 / (b - a);
            else return 0;
        }
        public override double LogProbabilityDensityFunction(double x)
        {
            if (x >= a && x <= b)
                return -System.Math.Log(b - a);
            else return double.NegativeInfinity;
        }
        public override void Fit(double[] observations, double[] weights, Fitting.IFittingOptions options)
        {
            if (immutable)
                throw new InvalidOperationException("This object can not be modified.");

            if (options != null)
                throw new ArgumentException("This method does not accept fitting options.");

            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            a = observations.Min();
            b = observations.Max();
        }

        public override object Clone()
        {
            return new UniformContinuousDistribution(a, b);
        }

        public static UniformContinuousDistribution Standard { get { return standard; } }

        private static readonly UniformContinuousDistribution standard = new UniformContinuousDistribution() { immutable = true };

        public static UniformContinuousDistribution Estimate(double[] observations)
        {
            var n = new UniformContinuousDistribution();
            n.Fit(observations);
            return n;
        }
        public override double[] Generate(int samples, double[] result)
        {
            return Random(a, b, samples, result);
        }
        public override double Generate()
        {
            return Random(a, b);
        }

        public static double[] Random(double a, double b, int samples)
        {
            return Random(a, b, samples, new double[samples]);
        }
        public static double[] Random(double a, double b, int samples, double[] result)
        {
            var rand = LearningFoundation.Math.Generator.Random;
            for (int i = 0; i < samples; i++)
                result[i] = rand.NextDouble() * (b - a) + a;
            return result;
        }

        public static double[] Random(int samples)
        {
            return Random(samples, new double[samples]);
        }
        public static double[] Random(int samples, double[] result)
        {
            var random = LearningFoundation.Math.Generator.Random;
            for (int i = 0; i < samples; i++)
                result[i] = random.NextDouble();
            return result;
        }
        public static double Random()
        {
            return LearningFoundation.Math.Generator.Random.NextDouble();
        }
        public static double Random(double a, double b)
        {
            return LearningFoundation.Math.Generator.Random.NextDouble() * (b - a) + a;
        }

        //public override string ToString(string format, IFormatProvider formatProvider)
        //{
        //    return String.Format(formatProvider, "U(x; a = {0}, b = {1})",
        //        a.ToString(format, formatProvider),
        //        b.ToString(format, formatProvider));
        //}

    }
}
