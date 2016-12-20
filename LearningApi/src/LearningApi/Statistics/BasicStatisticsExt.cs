using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Statistics
{
    /// <summary>
    /// Implement extension methods for statistics clculation eg. mean, median, variance,... 
    /// Modul calculate mean value of array of numbers. 
    /// The mean is the average of the numbers.
    /// </summary>
    public static class BasicStatistics
    {
        /// <summary>
        /// Calculate mean value of array of numbers. 
        /// </summary>
        /// <param name="colData"> array of values </param>
        /// <returns>calculated mean</returns>
        public static double MeanOf(this double[] colData)
        {
            if (colData == null || colData.Length < 2)
                throw new MLException("'coldData' cannot be null or empty!");

            //calculate summ of the values
            double sum = 0;
            for(int i=0; i < colData.Length; i++)
                sum += colData[i];

            //calculate mean
            double retVal = sum / colData.Length;

            return retVal;
        }

        /// <summary>
        /// /// Modul calculate median value of array of numbers. 
        /// If there is an odd number of data values 
        /// then the median will be the value in the middle. 
        /// If there is an even number of data values the median 
        /// is the mean of the two data values in the middle. 
        /// For the data set 1, 1, 2, 5, 6, 6, 9 the median is 5. 
        /// For the data set 1, 1, 2, 6, 6, 9 the median is 4.
        /// </summary>
        /// <param name="colData"></param>
        /// <returns></returns>
        public static double MedianOf(this double[] colData)
        {
            if (colData == null || colData.Length < 2)
                throw new MLException("'coldData' cannot be null or empty!");

            //initial mean value
            double median = 0;
            int medianIndex = colData.Length / 2;

            //sort the values
            Array.Sort(colData);

            //in case we have odd number of elements in data set
            // median is just in the middle of the dataset
            if(colData.Length % 2 == 1)
            {
                // 
                median = colData[medianIndex];
            }
            else//otherwize calculate average between two element in the middle
            {
                var val1 = colData[medianIndex - 1];
                var val2 = colData[medianIndex];

                //calculate the median
                median = (val1 + val2) / 2; 
            }

            return median;
        }

        /// <summary>
        /// Calculate variance for the sample data .
        /// </summary>
        /// <param name="colData"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static double VarianceOfS(this double[] colData)
        {
            if (colData == null || colData.Length < 4)
                throw new MLException("'coldData' cannot be null or less than 4 elements!");
            
            //number of elements
            int count = colData.Length;

            //calculate the mean
            var mean = colData.MeanOf();

            //calculate summ of square 
            double parSum = 0;
            for (int i = 0; i < colData.Length; i++)
            {
                var res = (colData[i] - mean);

                parSum += res*res;
            }
                
            return parSum/(count-1);
        }

        public static double Stdev(this double[] colData)
        {
            if (colData == null || colData.Length < 4)
                throw new MLException("'coldData' cannot be null or less than 4 elements!");

            //number of elements
            int count = colData.Length;

            //calculate the mean
            var vars = colData.VarianceOfS();

            //calculate summ of square 
            return Math.Sqrt(vars);
        }

        /// <summary>
        /// Calculate variance for the whole population.
        /// </summary>
        /// <param name="colData"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static double VarianceOfP(this double[] colData)
        {
            if (colData == null || colData.Length < 4)
                throw new MLException("'coldData' cannot be null or less than 4 elements!");

            //number of elements
            int count = colData.Length;

            //calculate the mean
            var mean = colData.MeanOf();

            //calculate summ of square 
            double parSum = 0;
            for (int i = 0; i < colData.Length; i++)
            {
                var res = (colData[i] - mean);

                parSum += res * res;
            }

            return parSum / count;
        }
    }
}
