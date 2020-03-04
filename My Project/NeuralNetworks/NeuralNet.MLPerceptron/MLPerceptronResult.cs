using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.MLPerceptron
{
    /// <summary>
    /// Prediction result of Multilayer Perceptron algorithm.
    /// </summary>
    public class MLPerceptronResult : IResult
    {
        /// <summary>
        /// Flat array of outpupts. If the output contains two classes then this array contains:
        /// (number of input vectors) * (num of output classes)
        /// </summary>
        public double[] results { get; set; }
    }
}
