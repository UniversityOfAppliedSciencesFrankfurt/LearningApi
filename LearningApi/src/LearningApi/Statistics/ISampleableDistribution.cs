using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation.Math;
namespace LearningFoundation.Statistics
{
    public interface ISampleableDistribution<TObservations> : IDistribution<TObservations>,
       IRandomNumberGenerator<TObservations>
    {
        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <param name="result">The location where to store the sample.</param>
        /// 
        /// <returns>A random observation drawn from this distribution.</returns>
        /// 
        TObservations Generate(TObservations result);
    }
}
