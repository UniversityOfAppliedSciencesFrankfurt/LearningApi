using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnomalyDetection.Interfaces
{
    /// <summary>
    /// The desired settings by the user for checking to which cluster a sample belongs
    /// </summary>
    public class CheckingSampleSettings
    {
        /// <summary>
        /// Settings to the clustering intance that contains the clusters data
        /// </summary>
        public string Path{ get; internal set; }

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
        /// <param name="LoadProjectSettings">Settings to the clustering intance that contains the clusters data</param>
        /// <param name="Sample">The sample to be checked</param>
        /// <param name="tolerance">A value in % representing the tolerance to possible outliers</param>
        public CheckingSampleSettings(string path, double[] Sample, double tolerance = 0)
        {
            this.Path =  path;
            this.Sample = Sample;
            this.Tolerance = tolerance;
        }
    }
}
