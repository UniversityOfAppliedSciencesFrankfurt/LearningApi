using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    public class RbmScore : IScore
    {
        public double Loss { get; set; }
        public double[] HiddenValues { get; internal set; }
        public double[] HiddenBisases { get; internal set; }
        public double[][] Weights { get; internal set; }

    

    }
}
