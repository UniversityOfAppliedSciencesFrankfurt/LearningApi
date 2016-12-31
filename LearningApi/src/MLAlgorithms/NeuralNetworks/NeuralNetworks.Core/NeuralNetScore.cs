using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetworks.Core
{
    public class NeuralNetScore : IScore
    {

        public double[] Errors { get; set; }

        public double[] Weights { get; set; }

    }
}
