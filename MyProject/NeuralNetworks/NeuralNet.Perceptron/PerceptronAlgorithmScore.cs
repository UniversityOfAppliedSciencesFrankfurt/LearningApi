using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNet.Perceptron
{
    public class PerceptronAlgorithmScore : IScore
    {
        public double[] Errors { get; set; }

        /// <summary>
        /// On how many iterations error has converged to zero.
        /// </summary>
        public int Iterations { get; internal set; }
        public double TotolEpochError { get; internal set; }
        public double[] Weights { get; set; }
        public Func<double, double>  ActivationFunction { get; set; }

    }
}
