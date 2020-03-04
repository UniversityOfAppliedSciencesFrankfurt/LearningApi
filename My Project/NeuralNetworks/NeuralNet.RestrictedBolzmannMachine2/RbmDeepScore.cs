using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;
using static NeuralNet.RestrictedBolzmannMachine2.DeepRbm;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    public class RbmDeepScore : IScore
    {
        public List<RbmLayer> Layers { get; set; }
    }
}
