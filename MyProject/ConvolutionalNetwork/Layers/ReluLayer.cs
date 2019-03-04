using System;
using System.Collections.Generic;
using LearningAPIFramework.Tensor;
namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
    /// <summary>
    ///     Implements ReLU nonlinearity elementwise
    ///     x -> max(0, x)
    ///     the output is in [0, inf)
    /// </summary>
    public class ReluLayer : LayerBase
    {
        public ReluLayer()
        {
            
        }
		
        public override void Backward(Volume outputGradient)
        {
            this.OutputActivationGradients = outputGradient;

            this.OutputActivation.ReluGradient(this.InputActivation,
                this.OutputActivationGradients,
                this.InputActivationGradients);
        }

        protected override Volume Forward(Volume input, bool isTraining = false)
        {
            input.Relu(this.OutputActivation);
            return this.OutputActivation;
        }

        public override void Init(int inputWidth, int inputHeight, int inputDepth)
        {
            base.Init(inputWidth, inputHeight, inputDepth);

            this.OutputDepth = inputDepth;
            this.OutputWidth = inputWidth;
            this.OutputHeight = inputHeight;
        }
    }
}