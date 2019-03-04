using System;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
	/// <summary>
	/// Convolutional networks may include local or global pooling layers,
	/// which combine the outputs of neuron clusters at one layer into a single neuron in the next layer.
	/// For example, max pooling uses the maximum value from each of a cluster of neurons at the previous layer.
	/// </summary>
	
	public class PoolLayer : LayerBase
    {
        private int _pad;
        private int _stride = 2;

        public PoolLayer(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public int Stride
        {
            get { return this._stride; }
            set
            {
                this._stride = value;
                if (this.IsInitialized)
                {
                    UpdateOutputSize();
                }
            }
        }

        public int Pad
        {
            get { return this._pad; }
            set
            {
                this._pad = value;
                if (this.IsInitialized)
                {
                    UpdateOutputSize();
                }
            }
        }

		/// <summary>
		/// Calculate downstream estimates for pool layer based on the output gradient of the layer upstream
		/// </summary>
		/// <param name="outputGradient"></param>

        public override void Backward(Volume outputGradient)
        {
            this.OutputActivationGradients = outputGradient;

            this.InputActivationGradients.Clear();

            this.OutputActivation.PoolGradient(this.InputActivation, this.OutputActivationGradients, this.Width,
                this.Height, this.Pad, this.Pad, this.Stride, this.Stride, this.InputActivationGradients);
        }

        protected override Volume Forward(Volume input, bool isTraining = false)
        {
            input.Pool(this.Width, this.Height, this.Pad, this.Pad, this.Stride, this.Stride, this.OutputActivation);
            return this.OutputActivation;
        }

        public override void Init(int inputWidth, int inputHeight, int inputDepth)
        {
            base.Init(inputWidth, inputHeight, inputDepth);

            UpdateOutputSize();
        }

        private void UpdateOutputSize()
        {
            // computed
            this.OutputDepth = this.InputDepth;
            this.OutputWidth = (int)Math.Floor((this.InputWidth + this.Pad * 2 - this.Width) / (double)this.Stride + 1);
            this.OutputHeight = (int)Math.Floor((this.InputHeight + this.Pad * 2 - this.Height) / (double)this.Stride + 1);
        }
    }
}