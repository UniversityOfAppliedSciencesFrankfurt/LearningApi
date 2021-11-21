using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkov
{
    public class HiddenMarkovScore: IScore
    {
        public double AvgLogLikelihood { get; set; }
    }
}
