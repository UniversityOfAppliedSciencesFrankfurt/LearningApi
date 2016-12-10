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

        public Task<double> Run(double[] featureValues, IContext ctx)
        {
            return Train(featureValues, 1, ctx:ctx);
        }

      

        public abstract Task<double> Train(double[] featureValues, double label, IContext ctx);
    }
}
