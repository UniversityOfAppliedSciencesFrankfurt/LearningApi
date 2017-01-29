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
        public double TotolEpochError { get; internal set; }
        public double[] Weights { get; set; }
    
    }
}
