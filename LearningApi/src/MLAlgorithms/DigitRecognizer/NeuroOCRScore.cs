using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitRecognizer
{
    public class NeuroOCRScore : IScore
    {
        public double[] Error { get; set; }
        public int Iterations { get; set; }
    }
}

