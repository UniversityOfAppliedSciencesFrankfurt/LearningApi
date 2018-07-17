using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    public class RbmLayer
    {

        /// <summary>
        /// Number of visible nodes.
        /// </summary>
        public int NumVisible;

        /// <summary>
        /// Number of hidden nodes.
        /// </summary>
        public int NumHidden;

        /// <summary>
        /// Visible node values (0, 1)
        /// </summary>
        public double[] VisibleValues;
        //public double[] VisProbs;
        public double[] VisBiases;

        public double[] HidValues;
   
        public double[] HidBiases;

        public double[][] VHWeights;

        public double Loss { get; internal set; }
    }
}
