using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;
using static NeuralNet.RestrictedBolzmannMachine2.DeepRbm;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    /// <summary>
    /// Defines the result of training RbmDeep network.
    /// </summary>
    public class RbmDeepResult : IResult
    {
        public List<List<RbmLayerResult>> Results { get; set; }
    }
}
