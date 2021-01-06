using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;

namespace ConvolutionalNetworks
{
    /// <summary>
    /// IScore Implementation for Convolutional Network Training Results
    /// </summary>
	public class ConvolutionalNetworkTrainingScore : IScore
	{
        /// <summary>
        /// Constructor
        /// </summary>
		public ConvolutionalNetworkTrainingScore() { }

        /// <summary>
        /// Loss Parameter of the Network Training
        /// </summary>
		public double Loss { get; set; }
        /// <summary>
        /// Convergence parameters of the Training  Network
        /// </summary>
		public double convergenceConfidence { get; set; }
	}
}
