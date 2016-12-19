using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Statistics
{
    /// <summary>
    /// Modul calculate mean value of array of numbers. 
    /// The mean is the average of the numbers.
    /// </summary>
    public class Mean : IStatistics
    {
        public double Run(double[] colData, IContext ctx)
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
    }
}
