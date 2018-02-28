using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    /// <summary>
    /// Result of prediction of RBM.
    /// </summary>
    public class RbmResult : IResult
    {
        /// <summary>
        /// Predictes hidden nodes.
        /// </summary>
        public double[][] HiddenNodesPredictions { get; set; }

        /// <summary>
        /// Predicted visible nodes by backward pass.
        /// </summary>
        public double[][] VisibleNodesPredictions { get; set; }

        /// <summary>
        /// Current wight status.
        /// </summary>
        public double[][] Weights { get; internal set; }
    }
}
