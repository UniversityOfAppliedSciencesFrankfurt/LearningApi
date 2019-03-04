namespace LearningAPIFramework.ConvolutionalNetze
{
	/// <summary>
	/// Sets up the basic training parameters Learning Rate,Momentum,L2Decay and BatchSize
	/// for gradient descent training.
	/// </summary>
	public class TrainParameters
	{
		public TrainParameters(int batchSize = 20)
		{
			LearningRate = 0.01;
			BatchSize = batchSize;
			L2Decay = 0.001;
			Momentum = 0.9;
		}

		public double LearningRate { get; set; }
		public double Momentum { get; set; }
		public double L2Decay { get; set; }
		public int BatchSize { get; set; }
	}
}