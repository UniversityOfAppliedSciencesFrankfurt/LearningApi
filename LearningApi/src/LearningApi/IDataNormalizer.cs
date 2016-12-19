using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{

    /// <summary>
    /// Performs data normalization and denormalization.
    /// </summary>
    public interface IDataNormalizer : IPipelineModule<double[][], double[][]>
    {
        /// <summary>
        /// Does normalization of vector in rawData
        /// </summary>
        /// <param name="rawData">Vector with raw scalar values.</param>
        /// <returns></returns>
        double[][] Run(double[][] rawData, IContext ctx);

    }
}
