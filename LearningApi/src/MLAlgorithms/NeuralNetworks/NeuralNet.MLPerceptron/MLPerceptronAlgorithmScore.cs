using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLPerceptron
{
    /// <summary>
    /// Class MLPerceptronAlgorithmScore inherits from interface IScore and implements properties to access the members of the interface
    /// </summary>
    public class MLPerceptronAlgorithmScore : IScore
    {
        /// <summary>
        /// Get/Set the property Errors
        /// </summary>
        public double[] Errors { get; set; }

        /// <summary>
        /// Calculated as Log(sum(Errors of each neuron at output layer))
        /// </summary>
        public double Loss{ get; set; }

        /// <summary>
        /// Get/Set the property Iterations
        /// </summary>
        public int Iterations { get; internal set; }

        /// <summary>
        /// Get/Set the property TotalEpochError
        /// </summary>
        public double TotolEpochError { get; internal set; }

        /// <summary>
        /// Get/Set the property Weights
        /// </summary>
        public double[] Weights { get; set; }

    }
}
