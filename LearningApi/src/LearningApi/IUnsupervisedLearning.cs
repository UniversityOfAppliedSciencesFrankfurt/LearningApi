using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation
{
    public interface IUnsupervisedLearning
    {
        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns learning error.</returns>
        /// 
        double Run(double[] input);

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        ///
        /// <returns>Returns sum of learning errors.</returns>
        /// 
        double RunEpoch(double[][] input);
    }
}
