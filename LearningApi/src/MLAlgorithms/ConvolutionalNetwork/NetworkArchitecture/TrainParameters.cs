namespace ConvolutionalNetworks
{
	/// <summary>
	/// Sets up the basic training parameters Learning Rate,Momentum,L2Decay and BatchSize
	/// for gradient descent training.
	/// </summary>
	public class TrainParameters
	{
        /// <summary>
        /// Train Parameters Constructor
        /// </summary>
        /// <param name="batchSize">Training Batch Size</param>
		public TrainParameters(int batchSize = 20)
		{
			LearningRate = 0.01;
			BatchSize = batchSize;
			L2Decay = 0.001;
			Momentum = 0.9;
		}

        /// <summary>
        /// Learning Rate
        /// </summary>
		public double LearningRate { get; set; }

        /// <summary>
        /// Momentum
        /// </summary>
		public double Momentum { get; set; }

        /// <summary>
        /// Layer2 Decay
        /// </summary>
		public double L2Decay { get; set; }

        /// <summary>
        /// Training Batch Size
        /// </summary>
		public int BatchSize { get; set; }
	}
}
