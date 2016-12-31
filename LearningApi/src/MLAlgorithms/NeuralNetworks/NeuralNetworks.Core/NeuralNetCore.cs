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


        public abstract IScore Run(double[][] featureValues, IContext ctx);
     
        public IScore Train(double[][] featureValues, IContext ctx)
        {
            return Run(featureValues, ctx);
        }


        public double[] Predict(double[][] data, IContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
