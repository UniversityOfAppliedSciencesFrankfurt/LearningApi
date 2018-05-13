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
            for (int i = 0; i < data1.Length; i++)
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
                list.Add(new DataPoint() { xValue = data1[i], yValue = data2[i] });
            }
            var byXList = list.OrderBy(r => r.xValue).ToArray();
            var byYList = list.OrderBy(r => r.yValue).ToArray();
            for (var i = 0; i < n; i++)
            {
                byXList[i].RankByX = i + 1;
                byYList[i].RankByY = i + 1;
            }
            //fix rank for the same value
            foreach (var g in byXList.GroupBy(x => x.xValue))
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
                    foreach (var gg in g)
                    {
                        gg.RankByY = rankSum / (float)cc;
                    }

                }
            }
            //
            var sumRankDiff
              = list.Aggregate(0f, (total, r) => total += (r.RankByX - r.RankByY) * (r.RankByX - r.RankByY));
            var rankCorrelation
              = 1 - (double)(6 * sumRankDiff) / (n * (n * n - 1L));

            return rankCorrelation;
        }


        /// <summary>
        /// Point Biserial Correlation coefficient
        /// https://en.wikipedia.org/wiki/Point-biserial_correlation_coefficient
        ///When you choose to analyse your data using a point-biserial correlation, part of the process involves checking to make sure that the data you want to analyse can actually be analysed using a point-biserial correlation. 
        ///You need to do this because it is only appropriate to use a point-biserial correlation if your data "passes" five assumptions 
        ///that are required for a point-biserial correlation to give you a valid result. 
        ///In practice, checking for these five assumptions just adds a little bit more time to your analysis, requiring you to click 
        ///a few more buttons in SPSS Statistics when performing your analysis, as well as think a little bit more about 
        ///your data, but it is not a difficult task.When you choose to analyse your data using a point-biserial correlation,
        ///part of the process involves checking to make sure that the data you want to analyse can actually be analysed 
        ///using a point-biserial correlation. You need to do this because it is only appropriate to use a point-biserial 
        ///correlation if your data "passes" five assumptions that are required for a point-biserial correlation to give you a 
        ///valid result. In practice, checking for these five assumptions just adds a little bit more time to your analysis, 
        ///requiring you to click a few more buttons in SPSS Statistics when performing your analysis, as well as think a 
        ///little bit more about your data, but it is not a difficult task.
        /// Assumption #1: One of your two variables should be measured on a continuous scale. Examples of continuous variables include
        ///             revision time (measured in hours), intelligence (measured using IQ score), exam performance (measured from 0 to 100), 
        ///             weight (measured in kg), and so forth. You can learn more about continuous variables in our article: Types of Variable.
        /// Assumption #2: Your other variable should be dichotomous. Examples of dichotomous variables include gender 
        ///             (two groups: male or female), employment status (two groups: employed or unemployed), smoker (two groups: yes or no), and so forth.
        /// Assumption #3: There should be no outliers for the continuous variable for each category of the dichotomous variable. 
        ///             You can test for outliers using boxplots.
        /// Assumption #4: Your continuous variable should be approximately normally distributed for each category of the dichotomous 
        ///             variable. You can test this using the Shapiro-Wilk test of normality.
        /// Assumption #5: Your continuous variable should have equal variances for each category of the dichotomous variable.
        ///             You can test this using Levene's test of equality of variances.
        ///
        /// </summary>
        /// <param name="data1">numeric array</param>
        /// <param name="data2">array of 0 and 1s</param>
        /// <returns></returns>
        public static double CorrCoeffPBOf(this double[] data1, int[] data2)
        {
            if (data1 == null || data1.Length < 2)
                throw new MLException("'xData' cannot be null or empty!");

            if (data2 == null || data2.Length < 2)
                throw new MLException("'yData' cannot be null or empty!");

            if (data1.Length != data2.Length)
                throw new MLException("Both datasets must be of the same size!");

            if (data2.Distinct().Count() != 2)
                throw new MLException("Data2 must be dichotomous!");

            //implementation in excel
            //http://www.real-statistics.com/correlation/biserial-correlation/
            //rb=(m1-m0)p0p1/sy

            int n = data1.Length;
            int n1 = 0;
            int n0 = 0;
            double s1 = 0;
            double s0 = 0;
            for (int i = 0; i < data1.Length; i++)
            {
                if (data2[i] == 1)
                {
                    n1++;
                    s1 = s1 + data1[i];
                }
                else
                {
                    n0++;
                    s0 = s0 + data1[i];
                }
            }
            if (n != n0 + n1)
                throw new MLException("Data2 must be dichotomous!");
            //
            var m1 = s1 / (double)n1;
            var m0 = s0 / (double)n0;
            var p0 = n0 / (double)n;
            var p1 = n1 / (double)n;
            var s = data1.Stdev();
            var z = Distributions.NormSInverse(p1);
            var y = Distributions.NormDist(z, 0, 1, false);
            return (m1 - m0) * p0 * p1 / (s * y);
        }


        /// <summary>
        /// Autocorrelation function.
        /// </summary>
        /// <param name="fncData">The function to be autocorrelated.</param>
        /// <returns>Autocorrelation values.</returns>
        public static double[] Autocorelate(this double[] fncData)
        {
            double mean = fncData.MeanOf();

            double[] autocorrelation = new double[fncData.Length / 2];

            for (int i = 0; i < autocorrelation.Length; i++)
            {
                double n = 0; // Numerator
                double d = 0; // Denominator

                for (int j = 0; j < fncData.Length; j++)
                {
                    // diff from mean = delta
                    double deltaX = fncData[j] - mean;

                    n += deltaX * (fncData[(j + i) % fncData.Length] - mean);

                    d += deltaX * deltaX;
                }

                // autocorr(i)=
                autocorrelation[i] = n / d;
            }

            return autocorrelation;
        }
    }

    class DataPoint
    {
        public double xValue, yValue;
        public float RankByX, RankByY;
    }

}
