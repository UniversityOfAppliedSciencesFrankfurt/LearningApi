
using ConvolutionalNetworks.Layers;
using ConvolutionalNetworks.Training;

namespace ConvolutionalNetworks
{

	/// <summary>
	/// Network Architecture creates a nine layer sequential convolutional
	/// neural network with two convolutional filters of depth 8 and 16 with 2 Pooling layers
	/// and 2 Rectified Linear Unit(ReLu) layer.
	/// </summary>
	public class NetworkArchitecture
	{
        /// <summary>
        /// Neural Network Stack
        /// </summary>
		public Net stackedNeuralNetwork { get; set; }

        /// <summary>
        /// Trainer for the stacked Neural Net
        /// </summary>
		public GradientDescentTrainer stackedNetworkTrainer { get; set; }

        /// <summary>
        /// Network Architecture
        /// </summary>
        /// <param name="numClasses">Number of Output Classes</param>
        /// <param name="numOrder">Order of the input image</param>
		public NetworkArchitecture(int numClasses = 10,int numOrder = 28)
		{
			this.stackedNeuralNetwork = new Net();
			this.stackedNeuralNetwork.AddLayer(new InputLayer(numOrder, numOrder, 1));
			this.stackedNeuralNetwork.AddLayer(new ConvLayer(5, 5, 8) { Stride = 1, Pad = 2 });
			this.stackedNeuralNetwork.AddLayer(new ReluLayer());
			this.stackedNeuralNetwork.AddLayer(new PoolLayer(2, 2) { Stride = 2 });
			this.stackedNeuralNetwork.AddLayer(new ConvLayer(5, 5, 16) { Stride = 1, Pad = 2 });
			this.stackedNeuralNetwork.AddLayer(new ReluLayer());
			this.stackedNeuralNetwork.AddLayer(new PoolLayer(3, 3) { Stride = 3 });
			this.stackedNeuralNetwork.AddLayer(new FullyConnLayer(numClasses));
			this.stackedNeuralNetwork.AddLayer(new SoftmaxLayer(numClasses));
		}


	}
}
