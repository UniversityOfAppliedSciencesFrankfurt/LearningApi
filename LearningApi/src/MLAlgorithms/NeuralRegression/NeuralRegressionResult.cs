using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;

namespace NeuralRegression
{
    /// <summary>
    /// NeuralRegressionResult class implements IResult interface
    /// </summary>
    public class NeuralRegressionResult : IResult
    {
        /// <summary>
        /// Intializing Double type variable for storing Predicted Values
        /// </summary>
        public double[][] PredictedValues { get; set; }
    }
}

