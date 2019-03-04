using System;
using System.Collections.Generic;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
	/// <summary>
	/// Fully connected layers connect every neuron in one layer to every neuron in another layer.
	/// It is in principle the same as the traditional multi-layer perceptron neural network (MLP)
	/// </summary>
	public class FullyConnLayer : LayerBase, IDotProductLayer 
    {
        private double _biasPref;
		
        public FullyConnLayer(int neuronCount)
        {
            this.NeuronCount = neuronCount;

			this.L1DecayMul = 0;
			this.L2DecayMul = 1;
        }

        public Volume Bias { get; private set; }

        public Volume BiasGradient { get; private set; }

        public Volume Filters { get; private set; }

        public Volume FiltersGradient { get; private set; }

        public double L1DecayMul { get; set; }

        public double L2DecayMul { get; set; }

        public int NeuronCount { get; }

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
		/// BackPropogate through a Fully Connected Layer
		/// </summary>
		/// <param name="outputGradient">Tensor of the output gradient of the upstream layer used to estimate weights of the perceptron layer.</param>
		public override void Backward(Volume outputGradient)
        {
            this.OutputActivationGradients = outputGradient;

            // compute gradient wrt weights and data
            using (var reshapedInput = this.InputActivation.ReShape(1, 1, -1, this.InputActivation.Shape.Dimensions[3]))
            using (var reshapedInputGradients = this.InputActivationGradients.ReShape(1, 1, -1, this.InputActivationGradients.Shape.Dimensions[3]))
            {
                reshapedInput.ConvolutionGradient(
                    this.Filters, this.OutputActivationGradients,
                    this.FiltersGradient,
                    0, 1, reshapedInputGradients);

                this.OutputActivationGradients.BiasGradient(this.BiasGradient);
            }
        }

        protected override Volume Forward(Volume input, bool isTraining = false)
        {
            using (var reshapedInput = input.ReShape(1, 1, -1, input.Shape.Dimensions[3]))
            {
                reshapedInput.Convolution(this.Filters, 0, 1, this.OutputActivation);
                this.OutputActivation.Add(this.Bias, this.OutputActivation);
                return this.OutputActivation;
            }
        }

		/// <summary>
		/// Returns the list of gradients and biases at the FullyConnected(MLP) Layer.
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

        internal void UpdateOutputSize()
        {
            this.OutputWidth = 1;
            this.OutputHeight = 1;
            this.OutputDepth = this.NeuronCount;

            // computed
            var inputCount = this.InputWidth * this.InputHeight * this.InputDepth;

            // Full-connected <-> 1x1 convolution
            if (this.Filters == null || !Equals(this.Filters.Shape, new Shape(1, 1, inputCount, this.NeuronCount)))
            {
                var scale = Math.Sqrt(2.0 / inputCount);
                this.Filters = BuilderInstance .Volume.Random(new Shape(1, 1, inputCount, this.NeuronCount), 0, scale);
                this.FiltersGradient = BuilderInstance .Volume.SameAs(new Shape(1, 1, inputCount, this.NeuronCount));
            }

            this.Bias = BuilderInstance  .Volume.From(new double[this.NeuronCount].Populate(this.BiasPref), new Shape(1, 1, this.NeuronCount));
            this.BiasGradient = BuilderInstance  .Volume.SameAs(new Shape(1, 1, this.NeuronCount));
        }
    }
}