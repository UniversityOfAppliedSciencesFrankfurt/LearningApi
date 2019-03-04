using System;
using System.Collections.Generic;
using System.IO;
using LearningAPIFramework.ConvolutionalNetze.Layers;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze
{
	/// <summary>
	/// Creates a stack of different types of layers for upstream label recognition and downstream feature 
	/// learning.
	/// </summary>
    public class Net : INet
    {
		public Net()
		{ }
        public List<LayerBase> Layers { get; } = new List<LayerBase>();

        public Volume Forward(Volume input, bool isTraining = false)
        {
            var activation = this.Layers[0].DoForward(input, isTraining);

            for (var i = 1; i < this.Layers.Count; i++)
            {
                var layer = this.Layers[i];
                activation = layer.DoForward(activation, isTraining);
            }

            return activation;
        }

		/// <summary>
		/// Returns the loss value =  Mean Squared(Error) for a given input vector.
		/// </summary>
		/// <param name="input">Input Vector/Tensor whose class estimate is to be found by the CNN</param>
		/// <param name="y">Last Layer Expected probability class output</param>
		/// <returns></returns>
        public double GetCostLoss(Volume input, Volume y)
        {
            Forward(input);

            var lastLayer = this.Layers[this.Layers.Count - 1] as ILastLayer;
            if (lastLayer != null)
            {
                double loss;
                lastLayer.Backward(y, out loss);
                return loss;
            }

            throw new Exception("Last layer doesn't implement ILastLayer interface");
        }
		/// <summary>
		/// BackPropogates through the network structure
		/// </summary>
		/// <param name="y">Expected class output for the given input excitation</param>
		/// <returns></returns>
        public double Backward(Volume y)
        {
            var n = this.Layers.Count;
            var lastLayer = this.Layers[n - 1] as ILastLayer;
            if (lastLayer != null)
            {
                double loss;
                lastLayer.Backward(y, out loss); // last layer assumed to be loss layer
                for (var i = n - 2; i >= 0; i--)
                {
                    // first layer assumed input
                    this.Layers[i].Backward(this.Layers[i + 1].InputActivationGradients);
                }
                return loss;
            }

            throw new Exception("Last layer doesn't implement ILastLayer interface");
        }

		/// <summary>
		// This is a convenience function for returning the argmax
		// prediction, assuming the last layer of the net is a softmax
		/// </summary>
		/// <returns></returns>
		public int[] GetPrediction()
        {
            
            var softmaxLayer = this.Layers[this.Layers.Count - 1] as SoftmaxLayer;
            if (softmaxLayer == null)
            {
                throw new Exception("GetPrediction function assumes softmax as last layer of the net!");
            }

            var activation = softmaxLayer.OutputActivation;
            var N = activation.Shape.Dimensions[3];
            var C = activation.Shape.Dimensions[2];
            var result = new int[N];

            for (var n = 0; n < N; n++)
            {
                var maxv = activation.Get(0, 0, 0, n);
                var maxi = 0;

                for (var i = 1; i < C; i++)
                {
                    var output = activation.Get(0, 0, i, n);
                    if (Ops<double>.GreaterThan(output, maxv))
                    {
                        maxv = output;
                        maxi = i;
                    }
                }

                result[n] = maxi;
            }

            return result;
        }
		/// <summary>
		/// Returns the list of parameters and gradients
		/// </summary>
		/// <returns></returns>
        public List<ParametersAndGradients<double>> GetParametersAndGradients()
        {
            var response = new List<ParametersAndGradients<double>>();

            foreach (var t in this.Layers)
            {
                var parametersAndGradients = t.GetParametersAndGradients();
                response.AddRange(parametersAndGradients);
            }

            return response;
        }

		/// <summary>
		/// Adds a new layer to the network stack.
		/// </summary>
		/// <param name="layer">Type of layer that is to be added to the network.</param>
        public void AddLayer(LayerBase layer)
        {
            int inputWidth = 0, inputHeight = 0, inputDepth = 0;
            LayerBase lastLayer = null;

            if (this.Layers.Count > 0)
            {
                inputWidth = this.Layers[this.Layers.Count - 1].OutputWidth;
                inputHeight = this.Layers[this.Layers.Count - 1].OutputHeight;
                inputDepth = this.Layers[this.Layers.Count - 1].OutputDepth;
                lastLayer = this.Layers[this.Layers.Count - 1];
            }
            else if (!(layer is InputLayer))
            {
                throw new ArgumentException("First layer should be an InputLayer");
            }

            var classificationLayer = layer as IClassificationLayer;
            if (classificationLayer != null)
            {
                var fullconLayer = lastLayer as FullyConnLayer;
                if (fullconLayer == null)
                {
                    throw new ArgumentException(
                        $"Previously added layer should be a FullyConnLayer with {classificationLayer.ClassCount} Neurons");
                }

                if (fullconLayer.NeuronCount != classificationLayer.ClassCount)
                {
                    throw new ArgumentException(
                        $"Previous FullyConnLayer should have {classificationLayer.ClassCount} Neurons");
                }
            }

            if (layer is ReluLayer )
            {
                if (lastLayer is IDotProductLayer dotProductLayer)
                {
                    // relus like a bit of positive bias to get gradients early
                    // otherwise it's technically possible that a relu unit will never turn on (by chance)
                    // and will never get any gradient and never contribute any computation. Dead relu.

                    dotProductLayer.BiasPref = (double)Convert.ChangeType(0.1, typeof(double)); // can we do better?
                }
            }

            if (this.Layers.Count > 0)
            {
                layer.Init(inputWidth, inputHeight, inputDepth);
            }

            this.Layers.Add(layer);
        }

		/// <summary>
		/// Dispose the bias gradient and filter gradient objects
		/// </summary>
		/// <param name="filename">Filepath</param>
        public void Dump(string filename)
        {
            using (var stream = File.Create(filename))
            using (var sw = new StreamWriter(stream))
            {
                for (var index = 0; index < this.Layers.Count; index++)
                {
                    var layerBase = this.Layers[index];
                    sw.WriteLine($"=== Layer {index}");
                    sw.WriteLine("Input");
                    sw.Write(layerBase.InputActivation.ToString());

                    //if (layerBase.InputActivationGradients != null)
                    //{
                    //    sw.Write(layerBase.InputActivationGradients.ToString());
                    //}

                    //var input = layerBase as InputLayer;
                    //if (input != null)
                    //{
                    //    sw.WriteLine("Input");
                    //    sw.Write(input.InputActivation.ToString());
                    //}

                    var conv = layerBase as ConvLayer;
                    if (conv != null)
                    {
                        sw.WriteLine("Filter");
                        sw.Write(conv.Filters.ToString());
                        //sw.Write(conv.FiltersGradient.ToString());

                        sw.WriteLine("Bias");
                        sw.Write(conv.Bias.ToString());
                        //sw.Write(conv.BiasGradient.ToString());
                    }

                    var full = layerBase as FullyConnLayer;
                    if (full != null)
                    {
                        sw.WriteLine("Filter");
                        sw.Write(full.Filters.ToString());
                        //sw.Write(full.FiltersGradient.ToString());

                        sw.WriteLine("Bias");
                        sw.Write(full.Bias.ToString());
                        //sw.Write(full.BiasGradient.ToString());
                    }
                }
            }
        }
		/// <summary>
		/// Forward Propogate through the network.
		/// </summary>
		/// <param name="inputs">Input Tensor</param>
		/// <param name="isTraining">Boolean flag to override training impementation</param>
		/// <returns></returns>
        public Volume Forward(Volume [] inputs, bool isTraining = false)
        {
            return Forward(inputs[0], isTraining);
        }

        //public static Net FromData(IDictionary<string, object> dico)
        //{
        //    var net = new Net();

        //    var layers = dico["Layers"] as IEnumerable<IDictionary<string, object>>;
        //    foreach (var layerData in layers)
        //    {
        //        var layer = LayerBase.FromData(layerData);
        //        net.Layers.Add(layer);
        //    }

        //    return net;
        //}
    }
}