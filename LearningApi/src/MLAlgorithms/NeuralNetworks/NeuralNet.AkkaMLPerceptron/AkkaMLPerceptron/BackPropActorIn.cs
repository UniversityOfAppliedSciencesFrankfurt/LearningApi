using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaMLPerceptron
{
    /// <summary>
    /// Used as input message for BackPropagationActor.
    /// </summary>
    public class BackPropActorIn
    {
        public double[] Input { get; set; }

        public double[] PrevLayerWeights { get; set; }

        public int NumOfFeatures { get; set; }
    }
}