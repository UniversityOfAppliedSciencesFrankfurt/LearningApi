using System;
using System.Collections.Generic;
using System.Text;

namespace DimensionalityReduction.PCA.Tests.EqualityComparers
{
    /// <summary>
    /// This class is used to compare whether two double[][] arrays are equal
    /// </summary>
    class DoubleMatrixComparer : IEqualityComparer<double[][]>
    {
        private DoubleVectorComparer DVectorComparer = new DoubleVectorComparer();

        /// <summary>
        /// check if double[][] x = double[][] y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if the substraction of two matrices is not more or less epsilon</returns>
        public bool Equals(double[][] x, double[][] y)
        {
            if (x == null | y == null)
            {
                return false;
            }

            if (x.GetLength(0) != y.GetLength(0))
            {
                return false;
            }

            for (int i = 0; i < x.GetLength(0); i++)
            {
                if (!DVectorComparer.Equals(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the hash code of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(double[][] obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// This class is used to compare two double[][][] martrices
    /// </summary>
    class DoubleCubicComparer : IEqualityComparer<double[][][]>
    {
        private DoubleMatrixComparer DMatrixComparer = new DoubleMatrixComparer();

        /// <summary>
        /// check if double[][][] x = double[][][] y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if the substraction of two matrices is not more or less epsilon</returns>
        public bool Equals(double[][][] x, double[][][] y)
        {
            if (x == null | y == null)
            {
                return false;
            }

            if (x.GetLength(0) != y.GetLength(0))
            {
                return false;
            }

            for (int i = 0; i < x.GetLength(0); i++)
            {
                if (!DMatrixComparer.Equals(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the hash code of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(double[][][] obj)
        {
            return obj.GetHashCode();
        }
    }
}
