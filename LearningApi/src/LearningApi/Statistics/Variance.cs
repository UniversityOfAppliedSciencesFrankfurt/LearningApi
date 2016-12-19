using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Statistics
{
    /// <summary>
    /// Modul calculate variance value of array of numbers. 
    ///The variance(σ2) is a measure of how far each value in the data set is from the mean.
    ///The variance (σ2), is defined as the sum of the squared distances of each
    ///value in the data from the mean (μ), divided by the number of elements in the data (N).
    /// </summary>
    public class Variance : IStatistics
    {
        /// <summary>
        /// Calculate variance for the sample data .
        /// </summary>
        /// <param name="colData"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public double Run(double[] colData, IContext ctx)
        {
            if (colData == null || colData.Length < 4)
                throw new MLException("'coldData' cannot be null or less than 4 elements!");
            
            //number of elements
            int count = colData.Length;

            //calculate the mean
            Mean m = new Mean();
            var mean = m.Run(colData, ctx);

            //calculate summ of square 
            double parSum = 0;
            for (int i = 0; i < colData.Length; i++)
            {
                var res = (colData[i] - mean);

                parSum += res*res;
            }
                

            return parSum/(count-1);
        }
    }
}
