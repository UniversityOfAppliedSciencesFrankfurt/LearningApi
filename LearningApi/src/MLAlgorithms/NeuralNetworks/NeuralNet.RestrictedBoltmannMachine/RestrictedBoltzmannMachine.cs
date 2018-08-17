using LearningFoundation;
using NeuralNetworks.Core.ActivationFunctions;
using NeuralNetworks.Core.Layers;
using NeuralNetworks.Core.Networks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("test")]

namespace NeuralNet.RestrictedBoltzmannMachine
{
    internal class RestrictedBoltzmannMachine : ActivationNetwork
    {

        private StochasticLayer visible;
        private StochasticLayer hidden;

        /// <summary>
        /// Gets the visible layer of the machine.
        /// </summary>
        public StochasticLayer Visible
        {
            get { return visible; }
        }

        /// <summary>
        /// Gets the hidden layer of the machine.
        /// </summary>
        public StochasticLayer Hidden
        {
            get { return hidden; }
        }

      
        ///   Creates a new RBM class with the activation function, inputsCount and hidden Neurons
        ///   
        /// <param name="function">The activation function to use in the network neurons.</param>
        /// <param name="inputsCount">The number of inputs for the machine.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in the machine.</param>
        /// 
        public RestrictedBoltzmannMachine(IStochasticFunction function, int inputsCount, int hiddenNeurons)
        : base(function, inputsCount, 1)
        {
            this.visible = new StochasticLayer(function, inputsCount, hiddenNeurons);
            this.hidden = new StochasticLayer(function, hiddenNeurons, inputsCount);
            base.layers[0] = hidden;
        }

        
        /// <summary>
        /// Compute the network output vector using the input vector
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override double[] Compute(double[] input)
        {
            return hidden.Compute(input);
        }


        #region Extra function
        ///// <summary>
        /////Creates a new RBM Class with input number and hidden neuron number      
        /////
        ///// </summary>
        ///// <param name="inputsCount">The number of inputs for the machine.</param>
        ///// <param name="hiddenNeurons">The number of hidden neurons in the machine.</param>
        ///// 
        //public RestrictedBoltzmannMachine(int inputsCount, int hiddenNeurons)
        //    : this(new BernoulliFunction(alpha: 0.5), inputsCount, hiddenNeurons) { }


        ///// <summary>
        /////  Resconstruc an input vector using a given output
        ///// </summary>
        ///// <param name="output"></param>
        ///// <returns></returns>
        //public double[] Reconstruct(double[] output)
        //{
        //    return visible.Compute(output);
        //}


        ///// <summary>
        ///// Samples an output vector from the network with an input vector.
        ///// Output is a possible output considering the stochastic activations of the network. 
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public double[] GenerateOutput(double[] input)
        //{
        //    return hidden.Generate(input);
        //}
        #endregion

        /// <summary>
        ///Samples an input vector from the network  given an output vector.
        /// Output is a possible reconstruction considering the stochastic activations of the network.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public double[] GenerateInput(double[] output)
        {
            return visible.Generate(output);
        }

    }
}
