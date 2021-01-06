
using ConvolutionalNetworks.Tensor;

namespace ConvolutionalNetworks.Layers
{
	/// <summary>
	/// Specifies the size and dimesnions of the square input image whose features have to be extracted and learned by the Convolutional Network.
	/// </summary>
    public class InputLayer : LayerBase
    {
        /// <summary>
        /// Constructor for Input Layer
        /// </summary>
        /// <param name="inputWidth">Input Width of tensor</param>
        /// <param name="inputHeight">Input Height of tensor</param>
        /// <param name="inputDepth">Input depth of tensor</param>
        public InputLayer(int inputWidth, int inputHeight, int inputDepth)
        {
            Init(inputWidth, inputHeight, inputDepth);

            this.OutputWidth = inputWidth;
            this.OutputHeight = inputHeight;
            this.OutputDepth = inputDepth;
        }
		/// <summary>
		/// No downstream weight estimation for input layer.
		/// </summary>
		/// <param name="outputGradient">Output Gradient Tensor</param>
        public override void Backward(Volume outputGradient)
        {
        }
		/// <summary>
		/// Upstream output calculation for the input layer is simply the input itself.
		/// </summary>
		/// <param name="input">Input image tensor</param>
        /// <param name="isTraining">Boolean is Training flag</param>
		/// <returns>Tensor</returns>
        protected override Volume Forward(Volume input, bool isTraining = false)
        {
            this.OutputActivation = input;
            return this.OutputActivation;
        }

        /// <summary>
        /// Forward PAss
        /// </summary>
        /// <param name="isTraining">Boolean Training flag</param>
        /// <returns>Tensor</returns>
        public override Volume Forward(bool isTraining)
        {
            return this.OutputActivation;
        }
    }
}
