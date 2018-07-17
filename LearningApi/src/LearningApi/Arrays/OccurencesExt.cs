using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Arrays
{
    public static class OccurencesExt
    {
        public static object[] FindAllOcurrences(this object[] items)
        {
            List<object> founds = new List<object>();

            foreach (var item in items)
            {
                if (!founds.Contains(item))
                {
                    founds.Add(item);
                }
            }

            return founds.ToArray();
        }

        public static object[] FindOcurrencesOf(this object[] items, object[] targets)
        {
            if (targets == null)
                throw new NullReferenceException();

            List<object> founds = new List<object>();

            foreach (var item in items)
            {
                if (!founds.Contains(item))
                {
                    founds.Add(item);
                }
            }

            return founds.ToArray();
        }


        /// <summary>
        /// Calculates the hamming distance between arrays.
        /// </summary>
        /// <param name="originArray">Original array to compare from.</param>
        /// <param name="comparingArray">Array to compare to.</param>
        /// <returns>Hamming distance.</returns>
        public static double[] GetHammingDistance(this double[][] originArray, double[][] comparingArray)
        {
            double[][] hDistance = new double[originArray.Length][];
            double[] h = new double[originArray.Length];
            double[] accuracy = new double[originArray.Length];

            for (int i = 0; i < originArray.Length; i++)
            {
                if (originArray[i].Length != comparingArray[i].Length)
                {
                    throw new Exception("Data must be equal length");
                }

                int sum = 0;
                for (int j = 0; j < originArray[i].Length; j++)
                {
                    if (originArray[i][j] == comparingArray[i][j])
                    {
                        sum = sum + 0;
                    }
                    else
                    {
                        sum = sum + 1;
                    }
                }

                h[i] = sum;

                accuracy[i] = ((originArray[i].Length - sum) * 100 / originArray[i].Length);
            }

            return accuracy;
        }

        public static double GetHammingDistance(this double[] originArray, double[] comparingArray)
        {
            double h;
            double accuracy;

            if (originArray.Length != comparingArray.Length)
            {
                throw new Exception("Data must be equal length");
            }

            int sum = 0;
            for (int j = 0; j < originArray.Length; j++)
            {
                if (originArray[j] == comparingArray[j])
                {
                    sum = sum + 0;
                }
                else
                {
                    sum = sum + 1;
                }
            }

            h = sum;

            accuracy = ((originArray.Length - sum) * 100 / originArray.Length);

            return accuracy;
        }
    }
}
