using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticRegression
{
    public class LogisticRegressionResult : IResult
    {
        public double[] PredictedValues { get; set; }
    }
}
