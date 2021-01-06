using System;
using ConvolutionalNetworks.Training;
using ConvolutionalNetworks.Tensor;
using System.Linq;

namespace ConvolutionalNetworks
{
	/// <summary>
	/// Convolutional Network Trainer 
	/// </summary>
	public class DeepStackLearn
	{
		private double[][] imageVal;
		private double[] labelVal;
		private int numClasses;
		private int numTraincases;
		private int width;
		private int height;

		private readonly Random _random = new Random(RandomUtilities.Seed);
		private int startPoint;
		private int epochNum;

		private GradientDescentTrainer deepStackTrainer;
		private Net neuralNetwork;

		private readonly CircularBuffer _testAccWindow = new CircularBuffer(100);
		private readonly CircularBuffer _trainAccWindow = new CircularBuffer(100);

        /// <summary>
        /// Convolutional Network Gradient Descent Learner
        /// </summary>
        /// <param name="numClasses">Number of Output Classes</param>
        /// <param name="networkArch">Network Architecture</param>
		public DeepStackLearn(int numClasses, NetworkArchitecture networkArch)
		{
			this.numClasses = numClasses;
			this.deepStackTrainer = networkArch.stackedNetworkTrainer;
			this.neuralNetwork = networkArch.stackedNeuralNetwork;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="imageVal">Two dimensional data set that has to be learnt by the network</param>
		/// <param name="labelVal">Label of the corresponding 2D Dataset for Supervised Learning</param>
		public void setSupervisedLearningInputs(double[][] imageVal, double[] labelVal)
		{
			this.imageVal = new double[imageVal.Length][];
			this.labelVal = new double[labelVal.Length];

			this.numTraincases = imageVal.Length;

			this.imageVal = imageVal;
			this.labelVal = labelVal;

			this.width = (int)Math.Sqrt(imageVal[0].Length);
			this.height = (int)Math.Sqrt(imageVal[0].Length);

		}
		/// <summary>
		/// Returns a batch of Training label and Training data for supervised Learning
		/// </summary>
		/// <param name="batchSize">Number of images to be taken in one batch of backpropogation Gradient Descent Training of the Convolutional Network</param>
		/// <param name="isShuffle">Boolean flag to shuffle the image datasets in the first iteration of first epoch </param>
		public Tuple<Volume, Volume, int[]> getNextTrainingBatch(int batchSize, bool isShuffle = false)
		{
			var dataShape = new Shape(this.width, this.height, 1, batchSize);
			var labelShape = new Shape(1, 1, this.numClasses, batchSize);
			var data = new double[dataShape.TotalLength];
			var label = new double[labelShape.TotalLength];
			var labels = new int[batchSize];

			// Shuffle if flag is Set
			if (this.startPoint == 0 && this.epochNum == 0 && isShuffle)
			{
				for (int i = this.imageVal.Length - 1; i >= 0; i--)
				{
					var j = this._random.Next(i);
					var temp = this.imageVal[j];
					var temp2 = this.labelVal[j];
					this.imageVal[j] = this.imageVal[i];
					this.labelVal[j] = this.labelVal[i];
					this.imageVal[i] = temp;
					this.labelVal[i] = temp2;
				}
			}

			var dataVolume = BuilderInstance.Volume.From(data, dataShape);

			for (int i = 0; i < batchSize; i++)
			{
				var image = this.imageVal[this.startPoint];

				labels[i] = (int)labelVal[this.startPoint];

				int j = 0;
				for (int y = 0; y < this.height; y++)
				{
					for (var x = 0; x < this.width; x++)
					{
						dataVolume.Set(x, y, 0, i, image[j++] / 255.0);
					}
				}

				label[i * this.numClasses + labels[i]] = 1.0;

				this.startPoint++;
				if (this.startPoint == this.imageVal.Length)
				{
					this.startPoint = 0;
					this.epochNum++;
				}
			}
			var labelVolume = BuilderInstance.Volume.From(label, labelShape);

			return new Tuple<Volume, Volume, int[]>(dataVolume, labelVolume, labels);

		}
		/// <summary>
		/// Test Method
		/// </summary>
		/// <param name="image">Returns the label of the image passed by forward propogating the data through the trained network</param>
		public double[] Test(double[] image)
		{
			double[] result = new double[2];
			var dataShape = new Shape(this.width, this.height, 1, 1);
			var data = new double[dataShape.TotalLength];
			var dataVolume = BuilderInstance.Volume.From(data, dataShape);
			int j = 0;
			for (int y = 0; y < this.height; y++)
			{
				for (var x = 0; x < this.width; x++)
				{
					dataVolume.Set(x, y, 0, 0, image[j++] / 255.0);
				}
			}

			var forwardPass = this.neuralNetwork.Forward(dataVolume);
			var prediction = this.neuralNetwork.GetPrediction();
			result[0] = prediction[0];
			result[1] = forwardPass.Get(prediction[0]);
			return result;

		}

		/// <summary>
		/// Test Method
		/// </summary>
		/// <param name="x">Test Tensor</param>
		/// <param name="labels">Prediction labels</param>
		/// <param name="accuracy">Accuracy out</param>
		/// <param name="forward">Forward Pass flag</param>
		public double Test(Volume x, int[] labels, CircularBuffer accuracy, bool forward = true)
		{
			if (forward)
			{
				this.neuralNetwork.Forward(x);
			}
			
			var prediction = this.neuralNetwork.GetPrediction();

			for (int i = 0; i < labels.Length; i++)
			{
				accuracy.Add(labels[i] == prediction[i] ? 1.0 : 0.0);
			}
			return Math.Round(accuracy.Items.Average() * 100.0, 2);
		}

		/// <summary>
		/// Train Method
		/// </summary>
		/// <param name="x">Input</param>
		/// <param name="y">Training Label</param>
		/// <param name="labels">label indicators</param>
		public double train(Volume x, Volume y, int[] labels)
		{
			this.deepStackTrainer.Train(x, y);

			return Test(x, labels, this._trainAccWindow, false);
		}

	}


}
