using System;
using LearningFoundation.Math;
using LearningFoundation;

namespace NeuralNetworks.Core.ActivationFunctions
{
    [Serializable]
    public class BernoulliFunction : IStochasticFunction
    {

        private double alpha; // sigmoid's alpha value


        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }


        public BernoulliFunction(double alpha)
        {
            this.alpha = alpha;
        }


        public BernoulliFunction()
            : this(alpha: 1) { }

        
        public double Function(double x)
        {
            return (1 / (1 + Math.Exp(-alpha * x))); 

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


        public double Derivative(double x)
        {
            double y = Function(x);

            return (alpha * y * (1 - y));
        }


        public double Derivative2(double y)
        {
            return (alpha * y * (1 - y));
        }

    }
}
