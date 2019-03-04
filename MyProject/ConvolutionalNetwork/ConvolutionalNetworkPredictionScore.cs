using System;
using System.Collections.Generic;
using LearningFoundation;

namespace LearningAPIFramework.ConvolutionalNetze
{
	public class ConvolutionalNetworkPredictionScore : IResult
	{
		public ConvolutionalNetworkPredictionScore()
		{
			this.predictionScore = new List<Tuple<int, double>>();
		}
		
		public List<Tuple<int, double>> predictionScore;

		/// <summary>
		/// Add Prediction Result
		/// </summary>
		/// <param name="label">Label Predicted</param>
		/// <param name="probability">Probability</param>
		public void addResult(int label,double probability)
		{
			predictionScore.Add(new Tuple<int, double>(label, probability));
		}

	}
}
