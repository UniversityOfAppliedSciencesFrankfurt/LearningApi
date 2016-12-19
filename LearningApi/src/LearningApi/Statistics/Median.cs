using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Statistics
{
    /// <summary>
    /// Modul calculate median value of array of numbers. 
    /// If there is an odd number of data values 
    /// then the median will be the value in the middle. 
    /// If there is an even number of data values the median 
    /// is the mean of the two data values in the middle. 
    /// For the data set 1, 1, 2, 5, 6, 6, 9 the median is 5. 
    /// For the data set 1, 1, 2, 6, 6, 9 the median is 4.
    /// </summary>
    public class Median : IStatistics
    {
        public double Run(double[] colData, IContext ctx)
        {
            if (colData == null || colData.Length < 2)
                throw new MLException("'coldData' cannot be null or empty!");

            //initial mean value
            double median = 0;
            int medianIndex = colData.Length / 2;

            //sort the values
            Array.Sort(colData);

            if(colData.Length % 2 == 1)
            {
                // 
                median = colData[medianIndex];
            }
            else
            {
                var val1 = colData[medianIndex - 1];
                var val2 = colData[medianIndex];

                //calculate the median
                median = (val1 + val2) / 2; 
            }

            return median;
        }
    }
}
