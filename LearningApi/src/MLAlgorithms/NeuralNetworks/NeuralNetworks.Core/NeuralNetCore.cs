using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetworks.Core
{
    public abstract class NeuralNetCore : IAlgorithm
    {
        public NeuralNetCore()
        {
        }

        public abstract Task<double> Train(double[] featureValues, double label);
    }
}
