using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaMLPerceptron
{
    /// <summary>
    /// Used as input message for BackPropagationActor.
    /// </summary>
    public class BiasUpdateBackPropActorOut
    {
        public int actorNum { get; set; }
        public double[][] newbiases { get; set; }
    }
}
