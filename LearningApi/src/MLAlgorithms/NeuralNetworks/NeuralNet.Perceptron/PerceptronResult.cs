using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.Perceptron
{
    public class PerceptronResult : IResult
    {
        public double[] PredictedValues { get; set; }
    }
}
