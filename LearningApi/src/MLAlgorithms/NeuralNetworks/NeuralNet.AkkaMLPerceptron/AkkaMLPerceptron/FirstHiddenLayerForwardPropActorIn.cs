using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaMLPerceptron
{
    /// <summary>
    /// Used as input message for BackPropagationActor.
    /// </summary>
    public class FirstHiddenLayerForwardPropActorIn
    {

        public int actorNum { get; set; }

        public int batchSize { get; set; }

        public int batchIndex { get; set; }

        public int StartIndex { get; set; }

        public int NeuronsCurrentActor { get; set; }

        public double[][] Input { get; set; }

        public double[] PreviousLayerBiases { get; set; }

        public double[,] PrevLayerWeights { get; set; }

    }
}
