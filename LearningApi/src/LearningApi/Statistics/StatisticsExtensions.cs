using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Statistics
{
    /// <summary>
    /// Implements a set of useful statistic methods on most used types.
    /// </summary>
    public static class StatisticsExtensions
    {
        public static double MeanOf(this double[] data)
        {
            if (data == null || data.Length < 2)
                throw new MLException("'coldData' cannot be null or empty!");

            //calculate summ of the values
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
                sum += data[i];

            //calculate mean
            double retVal = sum / data.Length;

            return retVal;
        }
    }
}
