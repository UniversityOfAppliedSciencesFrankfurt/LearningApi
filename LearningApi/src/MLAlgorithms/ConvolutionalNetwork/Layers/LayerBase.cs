using System;
using System.Collections.Generic;
using ConvolutionalNetworks.Tensor;

namespace ConvolutionalNetworks.Layers
{
    /// <summary>
    /// Layer Implementation abstract definition
    /// </summary>
    public abstract class LayerBase
    {
        /// <summary>
        /// IsInitialized Boolean flag
        /// </summary>
        protected bool IsInitialized;

        /// <summary>
        /// Constructor
        /// </summary>
        protected LayerBase()
        {
        }
		
        /// <summary>
        /// Input Layer Activation
        /// </summary>
        public Volume InputActivation { get; protected set; }

        /// <summary>
        /// Input Layer Activation Gradient
        /// </summary>
        public Volume InputActivationGradients { get; protected set; }

        /// <summary>
        /// Output Activation
        /// </summary>
        public Volume OutputActivation { get; protected set; }

        /// <summary>
        /// Output LAyer Activation Gradient
        /// </summary>
        public Volume OutputActivationGradients { get; protected set; }

        /// <summary>
        /// Output Layer depth
        /// </summary>
        public int OutputDepth { get; protected set; }

        /// <summary>
        /// Output Layer Width
        /// </summary>
        public int OutputWidth { get; protected set; }

        /// <summary>
        /// Output Layer Height
        /// </summary>
        public int OutputHeight { get; protected set; }

        /// <summary>
        /// Input Layer Depth
        /// </summary>
        public int InputDepth { get; private set; }

        /// <summary>
        /// Input Layer Width
        /// </summary>
        public int InputWidth { get; private set; }

        /// <summary>
        /// Input Layer Height
        /// </summary>
        public int InputHeight { get; private set; }

        /// <summary>
        /// Child Layer Description
        /// </summary>
        public LayerBase Child { get; set; }

        /// <summary>
        /// Parent Layer Description
        /// </summary>
        public List<LayerBase> Parents { get; set; } = new List<LayerBase>();

        /// <summary>
        /// Backward Pass
        /// </summary>
        /// <param name="outputGradient">Output Layer Gradients</param>
        public abstract void Backward(Volume outputGradient);

        internal void ConnectTo(LayerBase layer)
        {
            this.Child = layer;
            layer.Parents.Add(this);

            layer.Init(this.OutputWidth, this.OutputHeight, this.OutputDepth);
        }

        /// <summary>
        /// Forward Pass
        /// </summary>
        /// <param name="input">Input Parameters</param>
        /// <param name="isTraining">IsTraiing Boolean Flag</param>
        /// <returns>Forward Pass Tensor result</returns>
        public virtual Volume DoForward(Volume input, bool isTraining = false)
        {

            this.InputActivation = input;

            var outputShape = new Shape(this.OutputWidth, this.OutputHeight, this.OutputDepth, input.Shape.Dimensions[3]);

            if (this.OutputActivation == null ||
                !this.OutputActivation.Shape.Equals(outputShape))
            {
                this.OutputActivation = BuilderInstance .Volume.SameAs(input.Storage, outputShape);
            }

            if (this.InputActivationGradients == null ||
                !this.InputActivationGradients.Shape.Equals(input.Shape))
            {
                this.InputActivationGradients = BuilderInstance .Volume.SameAs(this.InputActivation.Storage,
                    this.InputActivation.Shape);
            }

            this.OutputActivation = Forward(input, isTraining);

            return this.OutputActivation;
        }

        /// <summary>
        /// Forward Pass
        /// </summary>
        /// <param name="input">Input Tensor</param>
        /// <param name="isTraining">IsTraining flag</param>
        /// <returns>Forward pass tensor result</returns>
        protected abstract Volume Forward(Volume input, bool isTraining = false);

        /// <summary>
        /// Forward Pass
        /// </summary>
        /// <param name="isTraining">IsTraining Boolean flag</param>
        /// <returns>Forward pass tensor result</returns>
        public virtual Volume Forward(bool isTraining)
        {
            return DoForward(this.Parents[0].Forward(isTraining), isTraining);
        }

        /// <summary>
        /// Get Gradients from computing upstream layers
        /// </summary>
        /// <returns>Tangent Points and derivatives</returns>        
        public virtual List<ParametersAndGradients<double>> GetParametersAndGradients()
        {
            return new List<ParametersAndGradients<double>>();
        }

        /// <summary>
        /// Initialize the layer
        /// </summary>
        /// <param name="inputWidth">Tensor Input Width</param>
        /// <param name="inputHeight">Tensor Input Height</param>
        /// <param name="inputDepth">Tensor Input Depth</param>
        public virtual void Init(int inputWidth, int inputHeight, int inputDepth)
        {
            this.InputWidth = inputWidth;
            this.InputHeight = inputHeight;
            this.InputDepth = inputDepth;
            this.IsInitialized = true;
        }
    }
}
