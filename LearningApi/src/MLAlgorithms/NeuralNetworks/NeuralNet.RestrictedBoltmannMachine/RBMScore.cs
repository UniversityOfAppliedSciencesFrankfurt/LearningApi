using LearningFoundation;

namespace NeuralNet.RestrictedBoltmannMachine
{
    public class RBMScore : IScore
    {

        public double[] Errors { get; set; }

        /// <summary>
        /// On how many iterations error has converged to zero.
        /// </summary>
        public int Iterations { get; internal set; }


        /// <summary>
        /// Total errors through the whole epoch
        /// </summary>
        public double TotalEpochError { get; internal set; }


        /// <summary>
        /// Total Visible Neutron Weight
        /// </summary>
        public double[] Weights { get; set; }

        /// <summary>
        /// The class vector of the machine
        /// </summary>
        public double[][] Class { get; set; }

    }
}
