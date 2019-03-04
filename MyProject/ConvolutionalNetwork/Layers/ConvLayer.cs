using System;
using System.Collections.Generic;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
	/// <summary>
	/// Convolution Filter Layer Class.Design  the feature filters which you wish to extract from the images.
	/// Or you could backpropogate the filter co-efficients to learn image features.
	/// Convolutional layers apply a convolution operation to the input, passing the result to the next layer.
	/// The convolution emulates the response of an individual neuron to visual stimuli
	/// </summary>
	public class ConvLayer : LayerBase, IDotProductLayer
    {
        private double _biasPref;
        private int _pad;
        private int _stride = 1;
        public ConvLayer(int width, int height, int filterCount)
        {
			this.L1DecayMul = 0;
			this.L2DecayMul = 1;

            this.FilterCount = filterCount;
            this.Width = width;
            this.Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public Volume Bias { get; private set; }

        public Volume BiasGradient { get; private set; }

        public Volume Filters { get; private set; }

        public Volume FiltersGradient { get; private set; }

        public int FilterCount { get; }

        public double L1DecayMul { get; set; }

        public double L2DecayMul { get; set; }

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

        public double BiasPref
        {
            get { return this._biasPref; }
            set
            {
                this._biasPref = value;

                if (this.IsInitialized)
                {
                    UpdateOutputSize();
                }
            }
        }

		/// <summary>
		/// BackPropogate through a convolutional Layer
		/// </summary>
		/// <param name="outputGradient">Tensor of the output gradient of the upstream layer used to estimate weights by gradient descent downstream.</param>
		public override void Backward(Volume outputGradient)
        {
            this.OutputActivationGradients = outputGradient;

            // compute gradient wrt weights and data
            this.InputActivation.ConvolutionGradient(this.Filters, this.OutputActivationGradients,
                this.FiltersGradient, this.Pad, this.Stride, this.InputActivationGradients);
            this.OutputActivationGradients.BiasGradient(this.BiasGradient);
        }


		/// <summary>
		/// Forward propogate through a convolutional layer.
		/// </summary>
		/// <param name="input">Tensor of the input from doenstream layer used to compute output based on Activation function used in the current layer</param>
		protected override Volume Forward(Volume input, bool isdoubleraining = false)
        {
            input.Convolution(this.Filters, this.Pad, this.Stride, this.OutputActivation);
            this.OutputActivation.Add(this.Bias, this.OutputActivation);
            return this.OutputActivation;
        }

        /// <summary>
		/// Returns the list of gradients and biases at the Convolutional Filter Layer.
		/// </summary>
		/// <returns></returns>
        public override List<ParametersAndGradients<double>> GetParametersAndGradients()
        {
            var response = new List<ParametersAndGradients<double>>
            {
                new ParametersAndGradients<double>
                {
                    Volume = this.Filters,
                    Gradient = this.FiltersGradient,
                },
                new ParametersAndGradients<double>
                {
                    Volume = this.Bias,
                    Gradient = this.BiasGradient,
                }
            };

            return response;
        }

        public override void Init(int inputWidth, int inputHeight, int inputDepth)
        {
            base.Init(inputWidth, inputHeight, inputDepth);

            UpdateOutputSize();
        }

		/// <summary>
		/// Updates the size of the Tensor object after a downstream pass.
		/// newWidth = [(oldWidth - FilterWidth + (pad*2))/(stride + 1)]
		/// newHeight = [(oldHeight - FilterHeight + (pad*2))/(stride + 1)]
		/// [.] denotes largest integer function .
		/// </summary>
		internal void UpdateOutputSize()
        {
            // required
            this.OutputDepth = this.FilterCount;

            // note we are doing floor, so if the strided convolution of the filter doesnt fit into the input
            // volume exactly, the output volume will be trimmed and not contain the (incomplete) computed
            // final application.
            this.OutputWidth =
                (int) Math.Floor((this.InputWidth + this.Pad * 2 - this.Width) / (double) this.Stride + 1);
            this.OutputHeight =
                (int) Math.Floor((this.InputHeight + this.Pad * 2 - this.Height) / (double) this.Stride + 1);

            // initializations
            var scale = Math.Sqrt(2.0 / (this.Width * this.Height * this.InputDepth));

            var shape = new Shape(this.Width, this.Height, this.InputDepth, this.OutputDepth);
            if (this.Filters == null || !this.Filters.Shape.Equals(shape))
            {
                this.Filters?.Dispose();
                this.Filters = BuilderInstance .Volume.Random(shape, 0.0, scale);
                this.FiltersGradient?.Dispose();
                this.FiltersGradient = BuilderInstance .Volume.SameAs(shape);
            }

            this.Bias = BuilderInstance .Volume.From(new double[this.OutputDepth].Populate(this.BiasPref),
                new Shape(1, 1, this.OutputDepth));
            this.BiasGradient = BuilderInstance .Volume.SameAs(new Shape(1, 1, this.OutputDepth));
        }
    }
}