using System;
using LearningAPIFramework.ConvolutionalNetze.Training;
using LearningFoundation;

namespace LearningAPIFramework.ConvolutionalNetze
{
	/// <summary>
	/// Learning Api Implementation of Convolutional Network
	/// </summary>
	public class ConvolutionalNetwork : IAlgorithm
	{
		
		private Tuple<double[][], double[]> labelPixelSeparator(double[][] data)
		{
			double[] labels = new double[data.Length];
			double[][] trainIp = new double[data.Length][];
			for (int i = 0; i < data.Length; i++)
			{
				labels[i] = data[i][0];
				double[] temp = new double[data[0].Length - 1];
				for (int j = 1; j < data[0].Length; j++)
				{
					temp[j - 1] = data[i][j] / 1.0;
				}

				trainIp[i] = temp;
			}

			return new Tuple<double[][], double[]>(trainIp, labels);
		}

		/// <summary>
		/// Stochastic Gradient Descent Algorithm for estimating weights of the convolutional filters 
		/// </summary>
		private GradientDescentTrainer gradientDescentTrainer;
		/// <summary>
		/// 9 layer network consisting of 2 convolutional filters of depth 8 and 16
		/// </summary>
		private NetworkArchitecture networkArch;
		/// <summary>
		/// Training parameters for the gradient descent
		/// Learning Rate:Specifies the rate at which deltas = errors are scaled to alter the weights
		/// Momentum : Specifies the rate at which the previous weight change estimates are weighed to change the currnt weight changes
		/// Batch Size and L2Decay: Specifies the number of images(objects) that are trained in one batch of a stochastic gradient descent iteration
		/// and the Decay rate at which convergence is expected .
		/// </summary>
		private TrainParameters trainParams;

		/// <summary>
		/// Creates a convolutional network with default training parameters and a nework architecture of 9 layers with 
		/// a stochastic gradient descent algorithm trainer
		/// </summary>
		private DeepStackLearn dsLearner;

		public ConvolutionalNetwork(int numClasses = 10,int numOrder = 28, int batchSize = 20)
		{

			this.networkArch = new NetworkArchitecture(numClasses,numOrder);

			this.trainParams = new TrainParameters(batchSize);

			this.gradientDescentTrainer = new GradientDescentTrainer(this.networkArch.stackedNeuralNetwork)
			{
				LearningRate = trainParams.LearningRate,
				BatchSize = trainParams.BatchSize,
				L2Decay = trainParams.L2Decay,
				Momentum = trainParams.Momentum
			};


			this.networkArch.stackedNetworkTrainer = this.gradientDescentTrainer;

			dsLearner = new DeepStackLearn(numClasses, this.networkArch);
		}

		/// <summary>
		/// Returns the label / class of the input data predicted by the trained Convolutional Network
		/// </summary>
		/// <param name="data">Set of all images / objects whose label class have to be determined</param>
		/// <param name="ctx">Context of the input data</param>
		/// <returns>ConvolutionalNetwrokPredictionScore object which has a two tuple predictionScore[label,probability] 
		/// where label is the output class of the data that the network predicts it to lie in and probability is the confidence 
		/// that the network has of correctly estimating the class of the label</returns>
		public IResult Predict(double[][] data, IContext ctx)
		{
			ConvolutionalNetworkPredictionScore predictScore = new ConvolutionalNetworkPredictionScore();
			foreach(double[] image in data)
			{
				var res = dsLearner.Test(image);
				predictScore.addResult((int)res[0], res[1]);

			}
			return predictScore;
		}

		/// <summary>
		/// Trains the Convolutional Network till the network correctly estimates atleast 97% of the data that it was trained with
		/// or the mean squared error in the output class estimate and actual output class probability is lesser than 0.18
		/// </summary>
		/// <param name="data">The training data set</param>
		/// <param name="ctx">Context paraemeters of the training dataset</param>
		/// <returns></returns>
		public IScore Run(double[][] data, IContext ctx)
		{
			return this.Train(data,ctx);
		}

		/// <summary>
		/// Runs the training algorithm till the network correctly estimates atleast 97% of the data that it was trained with
		/// or the mean squared error in the output class estimate and actual output class probability is lesser than 0.18
		/// </summary>
		/// <param name="data">Input training data set for supervised learning with both image(objects) and its labels
		/// (probability classes)</param>
		/// <param name="ctx">Context of the training parameters</param>
		/// <returns></returns>
		public IScore Train(double[][] data, IContext ctx)
		{
			ConvolutionalNetworkTrainingScore trainScore = new ConvolutionalNetworkTrainingScore();
			var dataSeparator = this.labelPixelSeparator(data);
			dsLearner.setSupervisedLearningInputs(dataSeparator.Item1, dataSeparator.Item2);
			//Console.WriteLine("Convolutional Network learning...");
			double lossComputed;
			double trainingConvergence;
			int batchNum = this.trainParams.BatchSize;
			do
			{
				var trainSample = dsLearner.getNextTrainingBatch(this.gradientDescentTrainer.BatchSize);
				trainingConvergence = dsLearner.train(trainSample.Item1, trainSample.Item2, trainSample.Item3);
				lossComputed = this.gradientDescentTrainer.Loss;
				batchNum += this.trainParams.BatchSize;
			} while (lossComputed > 0.18 && trainingConvergence < 97);
			
			trainScore.Loss = lossComputed;
			trainScore.convergenceConfidence = trainingConvergence;
			return trainScore;
		}


	}
}
