using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{

    /// <summary>
    /// Performs data normalization and denormalization.
    /// </summary>
    public interface IDataDeNormalizer : IPipelineModule<double[][], double[][]>
    {
        /// <summary>
        /// Denormilizes normalized data.
        /// </summary>
        /// <param name="normilizedData"></param>
        /// <returns></returns>
        double[][] DeNormalize(double[][] normilizedData, IContext ctx);
    }
}
