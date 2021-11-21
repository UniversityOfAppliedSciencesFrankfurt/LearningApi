using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkov
{
    public class HiddenMarkovResult: IResult
    {
        public List<double> Probability { get; set; }
    }
}
