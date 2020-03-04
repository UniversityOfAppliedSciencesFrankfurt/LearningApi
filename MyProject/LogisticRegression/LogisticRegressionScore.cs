using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LearningFoundation;
namespace LogisticRegression
{
    public class LogisticRegressionScore : IScore
    {
        public double[] Errors {get; set;}

        public double[] Weights { get; set; }
    }
}
