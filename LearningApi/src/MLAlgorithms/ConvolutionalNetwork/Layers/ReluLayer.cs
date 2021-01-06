using System;
using System.Collections.Generic;
using ConvolutionalNetworks.Tensor;
namespace ConvolutionalNetworks.Layers
{
    /// <summary>
    ///     Implements ReLU nonlinearity elementwise
    ///     x -> max(0, x)
    ///     the output is in [0, inf)
    /// </summary>
    public class ReluLayer : LayerBase
    {
        /// <summary>
        /// Backward Pass
        /// </summary>
        /// <param name="outputGradient">Output Gradient Tensor</param>
        public override void Backward(Volume outputGradient)
        {
            this.OutputActivationGradients = outputGradient;

            this.OutputActivation.ReluGradient(this.InputActivation,
                this.OutputActivationGradients,
                this.InputActivationGradients);
        }

        /// <summary>
        /// Forward Pass
        /// </summary>
        /// <param name="input">Input Tensor</param>
        /// <param name="isTraining">isTraining boolean flag</param>
        /// <returns>Forward Pass Tensor result</returns>
        protected override Volume Forward(Volume input, bool isTraining = false)
        {
            input.Relu(this.OutputActivation);
            return this.OutputActivation;
        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="inputWidth">Input Width</param>
        /// <param name="inputHeight">Input Height</param>
        /// <param name="inputDepth">Input Depth</param>
        public override void Init(int inputWidth, int inputHeight, int inputDepth)
        {
            base.Init(inputWidth, inputHeight, inputDepth);

            this.OutputDepth = inputDepth;
            this.OutputWidth = inputWidth;
            this.OutputHeight = inputHeight;
        }
    }
}
