using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Performs data statistics on provided data.
    /// </summary>
    public interface IStatistics : IPipelineModule<double[], double>
    {
        /// <summary>
        /// Does calculation of vector of column data
        /// </summary>
        /// <param name="rawData">Vector with raw scalar values.</param>
        /// <returns></returns>
        double Run(double[] colData, IContext ctx);

    }

}
