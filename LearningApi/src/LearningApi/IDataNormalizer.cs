using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{

    /// <summary>
    /// Performs data normalization and denormalization.
    /// </summary>
    public interface IDataNormalizer
    {
        /// <summary>
        /// Does normalization of vector in rawData
        /// </summary>
        /// <param name="rawData">Vector with raw scalar values.</param>
        /// <param name="statsistics">Basic statistics for each feture (columns).</param>
        /// <returns></returns>
        double[] Normalize(IStatistics[] statsistics, double[] rawData );

        /// <summary>
        /// Denormilizes normalized data.
        /// <param name="statsistics">Basic statistics for each feture (columns).</param>
        /// </summary>
        /// <param name="normilizedData"></param>
        /// <returns></returns>
        double[] DeNormalize(IStatistics[] statsistics, double[] normilizedData);
    }
}
