using System;
using ConvolutionalNetworks.Tensor;

namespace ConvolutionalNetworks.Layers
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

        /// <summary>
        /// Pool Layer Constructor
        /// </summary>
        /// <param name="width">Pool Layer width</param>
        /// <param name="height">Pool layer height</param>
        public PoolLayer(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
        
        /// <summary>
        /// Pool Layer Width
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Pool layer height
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Pool layer Stride
        /// </summary>
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

        /// <summary>
        /// Pool layer padding width
        /// </summary>
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
		/// <param name="outputGradient">Output gradients in downstream computation</param>

        public override void Backward(Volume outputGradient)
        {
            this.OutputActivationGradients = outputGradient;

            this.InputActivationGradients.Clear();

            this.OutputActivation.PoolGradient(this.InputActivation, this.OutputActivationGradients, this.Width,
                this.Height, this.Pad, this.Pad, this.Stride, this.Stride, this.InputActivationGradients);
        }

        /// <summary>
        /// Forward Pass
        /// </summary>
        /// <param name="input">INput Tensor</param>
        /// <param name="isTraining">IsTraining Boolean Flag</param>
        /// <returns>Forward Pass Tensor result</returns>
        protected override Volume Forward(Volume input, bool isTraining = false)
        {
            input.Pool(this.Width, this.Height, this.Pad, this.Pad, this.Stride, this.Stride, this.OutputActivation);
            return this.OutputActivation;
        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="inputWidth">Input Width</param>
        /// <param name="inputHeight">Input Height</param>
        /// <param name="inputDepth"> Input Depth</param>
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
