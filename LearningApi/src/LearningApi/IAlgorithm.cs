using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    public interface IAlgorithm : IPipelineModule<double[][], Task<IScore>>
    {
        Task<double> Train(double[] featureValues, double label, IContext ctx);
    }
}
