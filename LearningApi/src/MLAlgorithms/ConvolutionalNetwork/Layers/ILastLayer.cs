using System;
using ConvolutionalNetworks.Tensor;
namespace ConvolutionalNetworks.Layers
{
    /// <summary>
    /// Last Layer Interface
    /// </summary>
    public interface ILastLayer 
    {
        /// <summary>
        /// Backward Propogate from last layer
        /// </summary>
        /// <param name="y">Output of the network</param>
        /// <param name="loss">Mean Squared Loss Parameter</param>
        void Backward(Volume y, out double loss);
    }
}
