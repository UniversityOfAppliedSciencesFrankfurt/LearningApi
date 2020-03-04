using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;

namespace DeltaLearning
{
    public class DeltaLearningScore : IScore
    {
        public int Iterations { get; internal set; }
        public double[] Errors { get; set; }
        /// <summary>
        /// On how many iterations error has converged to zero.
        /// </summary>
        public double[] Weights { get; set; }

    }
}
