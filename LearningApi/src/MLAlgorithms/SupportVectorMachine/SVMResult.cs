using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.MlAlgorithms.SupportVectorMachineAlgorithm
{
    /// <summary>
    /// SVMResult is a class that contains the response object of the SVM's Predict function.
    /// </summary>
    public class SVMResult : IResult
    {
        /// <summary>
        /// List of results. 1 and -1 are different value of two classes. 
        /// </summary>
        public int[] PredictedResult { get; set; }
        
    }
}
