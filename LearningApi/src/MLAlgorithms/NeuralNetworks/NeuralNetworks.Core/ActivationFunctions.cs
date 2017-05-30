

namespace NeuralNetworks.Core
{
    using LearningFoundation;
    using System;    
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningFoundation.Math.Random;
    public class ActivationFunctions
    {
        private double alpha;
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }
        public static double Boolean(double val)
        {
            return (val >= 0) ? 1 : 0;
        }

        public static double Sigmoid(double val)
        {
            return 1 / (1 + Math.Exp(-val));
        }

        public double SigmoidFunction(double alpha)
        {
            return alpha;
        }
        public double BernoulliFunction(double alpha)
        {
            return alpha;
        }

        public double Function(double x)
        {
            return (1 / (1 + Math.Exp(-alpha * x)));
        }
        public double Derivative(double x)
        {
            double y = Function(x);

            return (alpha * y * (1 - y));
        }
        public double Derivative2(double y)
        {
            return (alpha * y * (1 - y));
        }
        public double Generate(double x)
        {
            double y = Function(x);
          return y > Generator.Random.NextDouble() ? 1 : 0;
        }
        public double Generate2(double y)
        {
            return y > Generator.Random.NextDouble() ? 1 : 0;
        }

    }
}
