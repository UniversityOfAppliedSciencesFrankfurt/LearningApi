using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    public interface IAlgorithm : IPipelineModule<double[][], IScore>
    {
        IScore Train(double[][] data, IContext ctx);
    }
}
