
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
	/// <summary>
	/// Specifies the size and dimesnions of the square input image whose features have to be extracted and learned by the Convolutional Network.
	/// </summary>
    public class InputLayer : LayerBase
    {
        public InputLayer()
        {

        }

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
		/// <param name="outputGradient"></param>
        public override void Backward(Volume outputGradient)
        {
        }
		/// <summary>
		/// Upstream output calculation for the input layer is simply the input itself.
		/// </summary>
		/// <param name="input">Input image tensor</param>
		/// <returns></returns>
        protected override Volume Forward(Volume input, bool isTraining = false)
        {
            this.OutputActivation = input;
            return this.OutputActivation;
        }

        public override Volume Forward(bool isTraining)
        {
            return this.OutputActivation;
        }
    }
}