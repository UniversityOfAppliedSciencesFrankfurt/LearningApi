using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaMLPerceptron
{
    public class BackPropActorOut
    {
        public long Test { get; set; }

        public string Status { get; set; }

        public double[][] LayerOutput { get; set; }

        public double[][] LayerNeuronSum { get; set; }
    }
}
