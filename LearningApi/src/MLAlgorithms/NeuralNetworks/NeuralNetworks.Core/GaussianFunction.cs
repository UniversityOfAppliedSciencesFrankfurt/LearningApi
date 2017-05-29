
//using System;
//using AForge;
//using Accord.Neuro.Neurons;
//using Accord.Neuro.Networks;
//using Accord.Neuro.Learning;
//using Accord.Statistics.Distributions.Univariate;
//using Accord.Math.Random;
//using Accord.Math;

namespace NeuralNetworks.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using LearningFoundation;
    public class GaussianFunction : IStochasticFunction
    {

        // linear slope value
        private double alpha = 1;

        // function output range
        private DoubleRange range = new DoubleRange(-1, +1);


        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }


        public DoubleRange Range
        {
            get { return range; }
            set { range = value; }
        }


        public GaussianFunction(double alpha)
        {
            this.alpha = alpha;
        }


        public GaussianFunction()
            : this(1.0) { }


        public GaussianFunction(double alpha, DoubleRange range)
        {
            this.Alpha = alpha;
            this.Range = range;
        }



        public double Function(double x)
        {
            double y = alpha * x;

            if (y > range.Max)
                return range.Max;
            else if (y < range.Min)
                return range.Min;
            return y;
        }


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


        public double Generate2(double y)
        {
            y = y + NormalDistribution.Random();

            if (y > range.Max)
                y = range.Max;
            else if (y < range.Min)
                y = range.Min;

            return y;
        }


        public double Derivative(double x)
        {
            double y = alpha * x;

            if (y <= range.Min || y >= range.Max)
                return 0;
            return alpha;
        }


        public double Derivative2(double y)
        {
            if (y <= range.Min || y >= range.Max)
                return 0;
            return alpha;
        }

    }
}



