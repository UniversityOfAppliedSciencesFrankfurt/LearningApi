using System;
using System.Collections.Generic;
using LearningFoundation;

namespace ConvolutionalNetworks
{
    /// <summary>
    /// IResult Implementation for COnvolutional Network Prediction score
    /// </summary>
	public class ConvolutionalNetworkPredictionScore : IResult
	{
        /// <summary>
        /// Prediction Score Constructor
        /// </summary>
		public ConvolutionalNetworkPredictionScore()
		{
			this.predictionScore = new List<Tuple<int, double>>();
		}
		
        /// <summary>
        /// Prediction Score List
        /// </summary>
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
