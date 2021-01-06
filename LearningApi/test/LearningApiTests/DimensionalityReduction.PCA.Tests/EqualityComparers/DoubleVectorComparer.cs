using DimensionalityReduction.PCA.Tests.Const;
using System;
using System.Collections.Generic;
using System.Text;

namespace DimensionalityReduction.PCA.Tests.EqualityComparers
{
    /// <summary>
    /// This class is used to compare two double[] vectors
    /// </summary>
    class DoubleVectorComparer : IEqualityComparer<double[]>
    {
        /// <summary>
        /// compare if abs (x - y) ~= epsilon
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true of abs(x - y) ~= epsilon</returns>
        public bool Equals(double[] x, double[] y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (x.Length != y.Length)
            {
                return false;
            }

            for (int i = 0; i < x.Length; i++)
            {
                if (Math.Abs(x[i] - y[i]) > TestConstants.Epsilon)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the hash code of object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(double[] obj)
        {
            return obj.GetHashCode();
        }
    }
}
