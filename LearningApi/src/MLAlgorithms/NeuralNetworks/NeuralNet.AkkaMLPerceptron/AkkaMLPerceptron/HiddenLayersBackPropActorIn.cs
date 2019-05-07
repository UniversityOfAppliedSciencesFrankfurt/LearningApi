using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaMLPerceptron
{
    /// <summary>
    /// Used as input message for BackPropagationActor.
    /// </summary>
    public class FirstHiddenLayerBackPropActorIn
    {
        public int actorNum { get; set; }

        public int batchSize { get; set; }

        public int batchIndex { get; set; }

        public int NeuronsCurrentActor { get; set; }

        public int StartIndex { get; set; }

        public double[,] FollLayerErrors { get; set; }

        public double[,] FollLayerWeights { get; set; }

        public double[,] LayerSum { get; set; }

        public double[][] PrevLayerOutput { get; set; }
    }
}
