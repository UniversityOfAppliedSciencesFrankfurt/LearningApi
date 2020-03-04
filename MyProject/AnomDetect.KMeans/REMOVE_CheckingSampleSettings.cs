using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Clustering.KMeans
{
    /// <summary>
    /// CheckingSampleSettings is a class that contains the desired settings by the user for checking to which cluster a sample belongs
    /// </summary>
    public class CheckingSampleSettings
    {
        /// <summary>
        /// The sample to be checked
        /// </summary>
        public double[] Sample { get; internal set; }

        /// <summary>
        /// A value in % representing the tolerance to possible outliers
        /// </summary>
        public double Tolerance { get; internal set; }

        /// <summary>
        /// Constructor to create the desired settings by the user for checking to which cluster a sample belongs
        /// </summary>
        /// <param name="Sample">The sample to be checked</param>
        /// <param name="tolerance">A value in % representing the tolerance to possible outliers</param>
        public CheckingSampleSettings(double[] Sample, double tolerance = 0)
        {
            this.Sample = Sample;
            this.Tolerance = tolerance;
        }
    }
}
