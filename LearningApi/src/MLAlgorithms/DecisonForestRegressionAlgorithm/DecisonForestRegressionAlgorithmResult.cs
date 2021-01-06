using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisonForestRegressionAlgorithm
{
    /// <summary>
/// Results type float[]
/// </summary>
    public class DecisonForestRegressionAlgorithmResult : IResult
    {
        /// <summary>
        /// List of results. -1 means no result for slot.
        /// </summary>
        public float[] Results { get; internal set; }
    }
}
