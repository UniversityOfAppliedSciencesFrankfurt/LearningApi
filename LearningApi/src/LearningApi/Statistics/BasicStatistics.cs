using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Statistics
{
    /// <summary>
    /// Class repr
    /// </summary>
    public class BasicStatistics : IStatistics
    {
        /// <summary>
        /// Constructor which accept column data, and stats will be automaticaly
        /// </summary>
        /// <param name="colId">Column Id which stat is related</param>
        /// <param name="colData"> column data</param>
        public BasicStatistics(int colId, double[] colData)
        {
            ColumnId = colId;
            Min = colData.Min();
            Max = colData.Max();
            Mean = colData.Average();
            Median = (int)(colData.Length + 1 / 2);
            Range = Max - Min;
            Variance = colData.Sum(x => (x - Mean) * (x - Mean) / colData.Length);
        }

        /// <summary>
        /// Constructor to construct the objact with pre-calculated statistics
        /// </summary>
        /// <param name="colId">column Id statistic is related</param>
        /// <param name="min">min value of the col data</param>
        /// <param name="max">max value of the col data</param>
        /// <param name="mean">mean value of the col data</param>
        /// <param name="variance">variance value of the col data</param>
        public BasicStatistics(int colId, double min, double max, double mean, double variance)
        {
            ColumnId = colId;
            Min = min;
            Max = max;
            Mean = mean;
            Range = Max - Min;
            Variance = variance;
        }

        /// <summary>
        /// Calculates the basic statistics for each column in dataset
        /// </summary>
        /// <param name="dataSet">data set</param>
        /// <param name="dm">datamaper related to dataSet</param>
        /// <returns></returns>
        public static IStatistics[] CalculateStatistics(IEnumerable<object[]> dataSet, IDataMapper dm)
        {
            //
            List<double[]> data = new List<double[]>();
            foreach(var row in dataSet)
            {
                var r = dm.MapInputVector(row);
                data.Add(r);
            } 

            IStatistics[] stats = new BasicStatistics[data[0].Length];

            for (int j = 0; j < stats.Length; j++)
            {
                var col = new BasicStatistics(j + 1, data.Select(x => x[j]).ToArray());
                stats[j] = col;
            }

            return stats;
        }

        /// <summary>
        /// Column Id
        /// </summary>
        public int ColumnId { get; set; }
        /// <summary>
        /// Minimum value of the list
        /// </summary>
        public double Min { get; set; }
        /// <summary>
        /// Maximum value of the list
        /// </summary>
        public double Max { get; set; }
        /// <summary>
        /// Average or Mean value in the list
        /// </summary>
        public double Mean { get; set; }
        /// <summary>
        /// Value of the middle position in the list
        ///  "( [the number of data points] + 1) ÷ 2"
        /// </summary>
        public double Median { get; set; }

        /// <summary>
        /// Maximum - Min = Range
        /// </summary>
        public double Range { get; set; }

        /// <summary>
        /// Variance = 1/n S(xi-Mean)^2
        /// S - sum
        /// </summary>
        public double Variance { get; set; }


    }
}
