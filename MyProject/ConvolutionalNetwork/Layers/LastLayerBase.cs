using System;
using System.Collections.Generic;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
    public abstract class LastLayerBase : LayerBase, ILastLayer 
    {
        protected LastLayerBase()
        {
        }
		

        public abstract void Backward(Volume y, out double loss);
    }
}