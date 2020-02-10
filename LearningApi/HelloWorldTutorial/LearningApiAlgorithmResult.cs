using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorldTutorial
{
    /// <summary>
    /// Maintaing the double type array names as PredictedValuesto set and get the values
    /// </summary>
    public class LearningApiAlgorithmResult:IResult
    {
        public double[] PredictedValue { get; set; }
    }
}
