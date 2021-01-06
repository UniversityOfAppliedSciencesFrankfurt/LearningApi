using System;
using ConvolutionalNetworks.Tensor;

namespace ConvolutionalNetworks.Layers
{
	/// <summary>
	/// Returns Gradients of bias and weight correction estimates for gradient descent computations.
	/// </summary>
	/// <typeparam name="T">Type Parameter</typeparam>
    public class ParametersAndGradients<T> where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Tensor
        /// </summary>
        public Volume Volume { get; set; }

        /// <summary>
        /// Tensor Gradient
        /// </summary>
        public Volume Gradient { get; set; }
    }
}
