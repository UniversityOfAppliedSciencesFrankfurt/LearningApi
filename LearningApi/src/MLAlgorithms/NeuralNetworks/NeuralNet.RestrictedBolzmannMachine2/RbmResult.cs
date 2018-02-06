using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    public class RbmResult : IResult
    {
        public double[][] HiddenNodesPredictions { get; set; }

        public double[][] VisibleNodesPredictions { get; set; }
    }
}
