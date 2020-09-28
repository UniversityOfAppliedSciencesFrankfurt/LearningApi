using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.MlAlgorithms.SupportVectorMachineAlgorithm
{
    /// <summary>
    /// MyAlgorithmScore is a class that contains the response object of the SVM training function.
    /// </summary>
    public class SVMScore  : IScore
    {
        /// <summary>
        /// Constructor of class SVMScore
        /// </summary>
        public SVMScore()
        {
        }
        /// <summary>
        /// The value of bias
        /// </summary>
        public double Bias { get; set; }
        
        /// <summary>
        /// The value of alphas
        /// </summary>
        public double[] Alphas { get; set; }

        /// <summary>
        /// The value of weight
        /// </summary>
        public double[] Weight { get; set; }
        
        /// <summary>
        /// A message to the user
        /// </summary>
        public string Message { get; internal set; }


    }
}
