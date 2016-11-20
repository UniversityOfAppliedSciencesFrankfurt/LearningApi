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
        /// <returns></returns>
        double[] Normalize(double[] rawData );

        /// <summary>
        /// Denormilizes normalized data.
        /// </summary>
        /// <param name="normilizedData"></param>
        /// <returns></returns>
        double[] DeNormalize(double[] normilizedData);
    }
}
