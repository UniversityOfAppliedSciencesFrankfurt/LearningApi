using System;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
	/// <summary>
	/// Returns Gradients of bias and weight correction estimates for gradient descent computations.
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class ParametersAndGradients<T> where T : struct, IEquatable<T>, IFormattable
    {
        public Volume Volume { get; set; }

        public Volume Gradient { get; set; }
    }
}