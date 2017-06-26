using LearningFoundation;
using NeuralNetworks.Core.ActivationFunctions;
using NeuralNetworks.Core.Layers;
using NeuralNetworks.Core.Networks;
using NeuralNetworks.Core.Neurons;

namespace NeuralNet.RestrictedBoltzmannMachine
{
    public class RestrictedBoltzmannMachine : ActivationNetwork
    {

        private StochasticLayer visible;
        private StochasticLayer hidden;

        ///   Gets the visible layer of the machine.
        public StochasticLayer Visible
        {
            get { return visible; }
        }

        ///   Gets the hidden layer of the machine.
        public StochasticLayer Hidden
        {
            get { return hidden; }
        }

        ///   Creates a new RBM Class with input number and hidden neuron number      
        ///   
        /// <param name="inputsCount">The number of inputs for the machine.</param>
        /// <param name="hiddenNeurons">The number of hidden neurons in the machine.</param>
        /// 
        public RestrictedBoltzmannMachine(int inputsCount, int hiddenNeurons)
            : this(new BernoulliFunction(alpha: 1), inputsCount, hiddenNeurons) { }

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

        ///Compute the network output vector using the input vector
        public override double[] Compute(double[] input)
        {
            return hidden.Compute(input);
        }

        /// Resconstruc an input vector using a given output
        public double[] Reconstruct(double[] output)
        {
            return visible.Compute(output);
        }

        /////   Samples an output vector from the network with an input vector.
        /////   Output is A possible output considering the stochastic activations of the network.
        public double[] GenerateOutput(double[] input)
        {
            return hidden.Generate(input);
        }

        ///   Samples an input vector from the network  given an output vector.
        ///   Output is a possible reconstruction considering the stochastic activations of the network.
        public double[] GenerateInput(double[] output)
        {
            return visible.Generate(output);
        }

    }
}
