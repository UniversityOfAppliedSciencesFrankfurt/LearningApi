using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaMLPerceptron
{
    public class BackPropActorOut
    {
        public int actorNum { get; set; }
        public double[,] Errors { get; set; }
        public double[] CostChangeDueToBiases { get; set; }
        public double[,] CostChangeDueToWeights { get; set; }
        public bool[] result { get; set; }
    }
}