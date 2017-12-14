using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.Statistics
{
    /// <summary>
    /// Implement extension methods for statistics calculation between two data sets X and Y eg. sum of square error, Pearson coeff,... 
    /// 
    /// </summary>
    public static class AdvancedStatistics
    {
        /// <summary>
        /// Calculates sum of squares residuals of the two datasets
        /// </summary>
        /// <param name="data1">first data set</param>
        /// <param name="data2">second dataset</param>
        /// <returns></returns>
        public static double SSROf(this double[] data1, double[] data2)
        {
            if (data1 == null || data1.Length < 2)
                throw new MLException("'xData' cannot be null or empty!");

            if (data2 == null || data2.Length < 2)
                throw new MLException("'yData' cannot be null or empty!");

            if (data1.Length != data2.Length)
                throw new MLException("Both datasets must be of the same size!");

            //calculate sum of the square residuals
            double ssr = 0;
            for(int i=0; i < data1.Length; i++)
            {
                var r = (data1[i] - data2[i]);
                ssr += r * r;
            }
               

            return ssr;
        }

        /// <summary>
        /// Calculates Pearson correlation coefficient of two data sets
        /// </summary>
        /// <param name="data1"> first data set</param>
        /// <param name="data2">second data set </param>
        /// <returns></returns>
        public static double CorrCoeffOf(this double[] data1, double[] data2)
        {
            if (data1 == null || data1.Length < 2)
                throw new MLException("'xData' cannot be null or empty!");

            if (data2 == null || data2.Length < 2)
                throw new MLException("'yData' cannot be null or empty!");

            if (data1.Length != data2.Length)
                throw new MLException("Both datasets must be of the same size!");

            //calculate average for each dataset
            double aav = data1.MeanOf();
            double bav = data2.MeanOf();

            double corr = 0;
            double ab = 0, aa = 0, bb = 0;
            for (int i = 0; i < data1.Length; i++)
            {
                var a = data1[i] - aav;
                var b = data2[i] - bav;

                ab += a * b;
                aa += a * a;
                bb += b * b;
            }

            corr = ab / Math.Sqrt(aa * bb);

            return corr;
        }
        /// <summary>
        /// Calculates the Spearman correlation coefficient of two data sets
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public static double CorrCoeffRankOf(this double[] data1, double[] data2)
        {
            if (data1 == null || data1.Length < 2)
                throw new MLException("'xData' cannot be null or empty!");

            if (data2 == null || data2.Length < 2)
                throw new MLException("'yData' cannot be null or empty!");

            if (data1.Length != data2.Length)
                throw new MLException("Both datasets must be of the same size!");

            var n = data1.Length;
            var list = new List<DataPoint>();
            for (var i = 0; i < n; i++)
            {
                list.Add(new DataPoint(){ xValue = data1[i], yValue = data2[i] });
            }
            var byXList = list.OrderBy(r => r.xValue).ToArray();
            var byYList = list.OrderBy(r => r.yValue).ToArray();
            for (var i = 0; i < n; i++)
            {
                byXList[i].RankByX = i + 1;
                byYList[i].RankByY = i + 1;
            }
            //fix rank for the same value
            foreach(var g in byXList.GroupBy(x=>x.xValue))
            {
                var cc = g.Count();
                if (g.Count() > 1)
                {
                    foreach (var gg in g)
                    {
                        gg.RankByX = g.Sum(x => x.RankByX) / (float)cc;
                    }

                }
            }
            //fix rank for the same value
            foreach (var g in list.GroupBy(y => y.yValue))
            {
                var cc = g.Count();
                var rankSum = g.Sum(y => y.RankByY);
                if (g.Count() > 1)
                {
                    foreach(var gg in g)
                    {
                       gg.RankByY= rankSum / (float)cc;
                    }

                }
            }
            //
            var sumRankDiff
              = list.Aggregate(0f, (total, r) => total += (r.RankByX - r.RankByY)* (r.RankByX - r.RankByY));
            var rankCorrelation 
              = 1 - (double)(6 * sumRankDiff) / (n * (n * n - 1L));

            return rankCorrelation;
        }

        class DataPoint
        {
            public double xValue,yValue;
            public float RankByX, RankByY;
        }
    }
}
