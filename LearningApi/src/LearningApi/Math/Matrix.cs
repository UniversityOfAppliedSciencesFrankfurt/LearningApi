﻿using System;
namespace LearningFoundation.MathFunction
{
    public enum MatrixOrder
    {
        CRowMajor = 1,
        FortranColumnMajor = 0,
        Default = CRowMajor
    }

    public static class Matrix
    {
        static T cast<T>( this object value )
        {
            return (T)Convert.ChangeType( value, typeof( T ) );
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value that is not a number (NaN).
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        /// 
        public static bool HasNaN( this double[] matrix )
        {
            foreach (var e in matrix)
                if (Double.IsNaN( e ))
                    return true;
            return false;
        }

        /// <summary>
        ///   Returns a value indicating whether the specified
        ///   matrix contains a value that is not a number (NaN).
        /// </summary>
        /// 
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// 
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        /// 
        public static bool HasNaN( this double[][] matrix )
        {
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    if (Double.IsNaN( matrix[i][j] ))
                        return true;
            return false;
        }


        /// <summary>
        ///   Combine vectors horizontally.
        /// </summary>
        /// 
        public static T[] Concatenate<T>( this T[][] vectors )
        {
            int size = 0;
            for (int i = 0; i < vectors.Length; i++)
                size += vectors[i].Length;

            T[] r = new T[size];

            int c = 0;
            for (int i = 0; i < vectors.Length; i++)
                for (int j = 0; j < vectors[i].Length; j++)
                    r[c++] = vectors[i][j];

            return r;
        }

        /// <summary>
        ///   Gets the maximum element in a vector.
        /// </summary>
        /// 
        public static T Max<T>( this T[] values, out int imax )
            where T : IComparable<T>
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo( max ) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }
            return max;
        }

        public static T Max<T>( this T[] values )
            where T : IComparable<T>
        {
            int imax;
            return Max( values, out imax );
        }
        /// <summary>
        ///   Gets the minimum element in a vector.
        /// </summary>
        /// 
        public static T Min<T>( this T[] values, out int imin )
            where T : IComparable<T>
        {
            imin = 0;
            T min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo( min ) < 0)
                {
                    min = values[i];
                    imin = i;
                }
            }
            return min;
        }

        public static T Min<T>( this T[] values )
            where T : IComparable<T>
        {
            int imin;
            return Min( values, out imin );
        }
    }
}
