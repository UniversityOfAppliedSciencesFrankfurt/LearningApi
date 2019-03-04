using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;

namespace LearningAPIFramework.ConvolutionalNetze
{
	public class ConvolutionalNetworkTrainingScore : IScore
	{
		public ConvolutionalNetworkTrainingScore() { }

		public double Loss { get; set; }
		public double convergenceConfidence { get; set; }
	}
}
