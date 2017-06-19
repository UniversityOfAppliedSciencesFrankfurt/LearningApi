using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBoltmannMachine
{
    class RBMScore : IScore
    {
        public double[] Errors { get; set; }

        /// <summary>
        /// On how many iterations error has converged to zero.
        /// </summary>
        public int Iterations { get; internal set; }
        public double TotolEpochError { get; internal set; }
        public double[] Weights { get; set; }

    }
}
