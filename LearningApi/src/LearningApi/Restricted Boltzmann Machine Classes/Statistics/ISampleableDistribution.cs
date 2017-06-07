using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation.Math;
namespace LearningFoundation.Statistics
{
    public interface ISampleableDistribution<TObservations> : IDistribution<TObservations>,
       IRandomNumberGenerator<TObservations>
    {
        
        TObservations Generate(TObservations result);
    }
}
