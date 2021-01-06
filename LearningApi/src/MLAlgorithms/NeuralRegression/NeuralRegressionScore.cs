using System;
using LearningFoundation;
namespace NeuralRegression
{
    /// <summary>
    /// NeuralRegressionScore class implements IScore interface
    /// </summary>
    public class NeuralRegressionScore : IScore
    {
        /// <summary>
        /// Initializing Double type variable for storing Errors
        /// </summary>
        public double[] Errors { get; set; }
        /// <summary>
        /// Initializing Double type variable for storing Weights
        /// </summary>
        public double[] Weights { get; set; }
    }
}
