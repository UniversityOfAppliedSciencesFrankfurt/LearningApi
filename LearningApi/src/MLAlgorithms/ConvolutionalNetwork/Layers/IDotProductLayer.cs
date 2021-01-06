using System;

namespace ConvolutionalNetworks.Layers
{
    /// <summary>
    /// Dot Product capable Deep Learning Layers
    /// </summary>
    public interface IDotProductLayer
    {
        /// <summary>
        /// Bias Preferences for Layer
        /// </summary>
        double BiasPref { get; set; }
    }
}
