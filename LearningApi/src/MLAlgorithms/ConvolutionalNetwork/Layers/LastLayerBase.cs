using System;
using System.Collections.Generic;
using ConvolutionalNetworks.Tensor;

namespace ConvolutionalNetworks.Layers
{
    /// <summary>
    /// LastLayer Abstract Definition
    /// </summary>
    public abstract class LastLayerBase : LayerBase, ILastLayer 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected LastLayerBase()
        {
        }
		
        /// <summary>
        /// BackWard Pass
        /// </summary>
        /// <param name="y">Output</param>
        /// <param name="loss">Loss Parameter Mean Squared</param>
        public abstract void Backward(Volume y, out double loss);
    }
}
