using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaMLPerceptron
{
    public class ForwardPropActorOut
    {
        public int actorNum { get; set; }

        public double[,] LayerOutput { get; set; }

        public double[,] LayerNeuronSum { get; set; }
    }
}
