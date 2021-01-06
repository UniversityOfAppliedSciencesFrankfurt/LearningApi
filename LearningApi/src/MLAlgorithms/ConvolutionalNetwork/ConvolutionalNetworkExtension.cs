
using LearningFoundation;

namespace ConvolutionalNetworks
{
	/// <summary>
    /// Extension Class for Convolutional Networks
    /// </summary>
	public static class ConvolutionalNetworkExtension
	{
		/// <summary>
		/// The Extension method for Implementing a Convolutional Network inside the Learning Api Framework
		/// </summary>
		/// <param name="api">Learning API instance</param>
		/// <param name="numOpClasses">Number of output classes of the supervised learning instance.Default = 10</param>
		/// <param name="numOrder">Order of the input object which is to be trained by the network.
		/// (numOrder*numOrder) = Length(feature points of object).Default = 28</param>
		/// <param name="batchSize">Number of training objects sets which the algorithm takes in one pass while the network learns.</param>
		/// <returns>Learning Api</returns>
		public static LearningApi UseConvolutionalNetwork(this LearningApi api,int numOpClasses = 10,int numOrder = 28,int batchSize = 20)
		{
			ConvolutionalNetwork convNetzeNetwork = new ConvolutionalNetwork(numOpClasses,numOrder,batchSize);
			api.AddModule(convNetzeNetwork, "ConvolutionsNetze");
			return api;
		}
	}
}
