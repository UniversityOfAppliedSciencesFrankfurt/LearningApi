using LearningFoundation.MathFunction;
using System;

namespace LearningFoundation.Statistics
{
    /// <summary>
    ///   Common interface for sampleable distributions (i.e. distributions that
    ///   allow the generation of new samples through the IRandomNumberGenerator{T}.Generate()"/>
    ///   method.
    /// </summary>
    /// 
    /// <typeparam name="TObservations">The type of the observations, such as System.Double
    /// 
    public interface ISampleableDistribution<TObservations> : //IDistribution<TObservations>,
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
        TObservations Generate( TObservations result );        
       
    }
}
