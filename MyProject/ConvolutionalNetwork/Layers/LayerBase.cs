using System;
using System.Collections.Generic;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
    public abstract class LayerBase
    {
        protected bool IsInitialized;

        protected LayerBase()
        {
        }
		

        public Volume InputActivation { get; protected set; }

        public Volume InputActivationGradients { get; protected set; }

        public Volume OutputActivation { get; protected set; }

        public Volume OutputActivationGradients { get; protected set; }

        public int OutputDepth { get; protected set; }

        public int OutputWidth { get; protected set; }

        public int OutputHeight { get; protected set; }

        public int InputDepth { get; private set; }

        public int InputWidth { get; private set; }

        public int InputHeight { get; private set; }

        public LayerBase Child { get; set; }

        public List<LayerBase> Parents { get; set; } = new List<LayerBase>();

        public abstract void Backward(Volume outputGradient);

        internal void ConnectTo(LayerBase layer)
        {
            this.Child = layer;
            layer.Parents.Add(this);

            layer.Init(this.OutputWidth, this.OutputHeight, this.OutputDepth);
        }

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

        protected abstract Volume Forward(Volume input, bool isTraining = false);

        public virtual Volume Forward(bool isTraining)
        {
            return DoForward(this.Parents[0].Forward(isTraining), isTraining);
        }

        public static LayerBase FromData(IDictionary<string, object> dico)
        {
            var typeName = dico["Type"] as string;
            var type = Type.GetType(typeName);
            var t = (LayerBase)Activator.CreateInstance(type, dico);
            return t;
        }

        public virtual List<ParametersAndGradients<double>> GetParametersAndGradients()
        {
            return new List<ParametersAndGradients<double>>();
        }

        public virtual void Init(int inputWidth, int inputHeight, int inputDepth)
        {
            this.InputWidth = inputWidth;
            this.InputHeight = inputHeight;
            this.InputDepth = inputDepth;
            this.IsInitialized = true;
        }
    }
}