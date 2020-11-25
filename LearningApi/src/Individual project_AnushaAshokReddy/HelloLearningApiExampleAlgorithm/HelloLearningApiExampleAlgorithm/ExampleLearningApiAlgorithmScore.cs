using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleLearningApiAlgorithm
{
    public class LinearRegressionScore : IScore
    {
        public double[] Weights { get; set; }

        public double Bias { get; set; }

        public double[] Loss { get; set; }
    }
}

