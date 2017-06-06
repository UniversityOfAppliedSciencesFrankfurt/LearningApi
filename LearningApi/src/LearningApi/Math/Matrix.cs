using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace LearningFoundation.Math
{
    public enum MatrixOrder
    {

        CRowMajor = 1,

        FortranColumnMajor = 0,

        Default = CRowMajor
    }


    public static partial class Matrix
    {
        static T cast<T>(this object value)
        {
            return (T)System.Convert.ChangeType(value, typeof(T));
        }
        public static T[,] InsertColumn<T, TSource>(this T[,] matrix, TSource[] column, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (column == null)
                throw new ArgumentNullException("column");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            int maxRows = System.Math.Max(rows, column.Length);

            T[,] X = new T[maxRows, cols + 1];

            // Copy original matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < index; j++)
                    X[i, j] = matrix[i, j];
                for (int j = index; j < cols; j++)
                    X[i, j + 1] = matrix[i, j];
            }

            // Copy additional column
            for (int i = 0; i < column.Length; i++)
                X[i, index] = cast<T>(column[i]);

            return X;
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at a given index.
        /// </summary>
        /// 
        public static T[,] InsertColumn<T, TSource>(this T[,] matrix, TSource value, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            T[,] X = new T[rows, cols + 1];

            // Copy original matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < index; j++)
                    X[i, j] = matrix[i, j];
                for (int j = index; j < cols; j++)
                    X[i, j + 1] = matrix[i, j];
            }

            // Copy additional column
            for (int i = 0; i < rows; i++)
                X[i, index] = cast<T>(value);

            return X;
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at a given index.
        /// </summary>
        /// 
        public static T[][] InsertColumn<T, TSource>(this T[][] matrix, TSource[] column, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");
            if (column == null)
                throw new ArgumentNullException("column");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            int maxRows = System.Math.Max(rows, column.Length);

            T[][] X = new T[maxRows][];
            for (int i = 0; i < X.Length; i++)
                X[i] = new T[cols + 1];

            // Copy original matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < index; j++)
                    X[i][j] = matrix[i][j];

                for (int j = index; j < cols; j++)
                    X[i][j + 1] = matrix[i][j];
            }

            // Copy additional column
            for (int i = 0; i < column.Length; i++)
                X[i][index] = cast<T>(column[i]);

            return X;
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at a given index.
        /// </summary>
        /// 
        public static T[][] InsertColumn<T, TSource>(this T[][] matrix, TSource value, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            int rows = matrix.Length;
            int cols = matrix[0].Length;

            T[][] X = new T[rows][];
            for (int i = 0; i < X.Length; i++)
                X[i] = new T[cols + 1];

            // Copy original matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < index; j++)
                    X[i][j] = matrix[i][j];

                for (int j = index; j < cols; j++)
                    X[i][j + 1] = matrix[i][j];
            }

            // Copy additional column
            for (int i = 0; i < rows; i++)
                X[i][index] = cast<T>(value);

            return X;
        }
        public static T[,] InsertColumn<T>(this T[,] matrix)
        {
            return InsertColumn(matrix, new T[matrix.Length], matrix.GetLength(1));
        }

        /// <summary>
        ///   Returns a new matrix with a new column vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[][] InsertColumn<T>(this T[][] matrix)
        {
            return InsertColumn(matrix, new T[matrix.Length], matrix[0].Length);
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[,] InsertColumn<T, TSource>(this T[,] matrix, TSource[] column)
        {
            return InsertColumn(matrix, column, matrix.GetLength(1));
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[,] InsertColumn<T, TSource>(this T[,] matrix, TSource value)
        {
            return InsertColumn(matrix, value, matrix.GetLength(1));
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[][] InsertColumn<T, TSource>(this T[][] matrix, TSource[] column)
        {
            return InsertColumn(matrix, column, matrix[0].Length);
        }

        /// <summary>
        ///   Returns a new matrix with a given column vector inserted at the end of the original matrix.
        /// </summary>
        /// 
        public static T[][] InsertColumn<T, TSource>(this T[][] matrix, TSource value)
        {
            return InsertColumn(matrix, value, matrix[0].Length);
        }


        public static T[] Concatenate<T>(this T[] a, T[] b)
        {
            T[] r = new T[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                r[i + a.Length] = b[i];

            return r;
        }

       
        public static T[] Concatenate<T>(this T[] vector, T element)
        {
            T[] r = new T[vector.Length + 1];
            for (int i = 0; i < vector.Length; i++)
                r[i] = vector[i];

            r[vector.Length] = element;

            return r;
        }

     
        public static T[] Concatenate<T>(this T element, T[] vector)
        {
            T[] r = new T[vector.Length + 1];

            r[0] = element;

            for (int i = 0; i < vector.Length; i++)
                r[i + 1] = vector[i];

            return r;
        }

        
        public static T[,] Concatenate<T>(this T[,] matrix, T[] vector)
        {
            return matrix.InsertColumn(vector);
        }

     
        public static T[,] Concatenate<T>(this T[,] a, T[,] b)
        {
            return Concatenate(new[] { a, b });
        }

       
        public static T[][] Concatenate<T>(this T[][] a, T[][] b)
        {
            return Concatenate(new[] { a, b });
        }

      
        public static T[,] Concatenate<T>(params T[][,] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                cols += matrices[i].GetLength(1);
                if (matrices[i].GetLength(0) > rows)
                    rows = matrices[i].GetLength(0);
            }

            T[,] r = new T[rows, cols];


            int c = 0;
            for (int k = 0; k < matrices.Length; k++)
            {
                int currentRows = matrices[k].GetLength(0);
                int currentCols = matrices[k].GetLength(1);

                for (int j = 0; j < currentCols; j++)
                {
                    for (int i = 0; i < currentRows; i++)
                    {
                        r[i, c] = matrices[k][i, j];
                    }
                    c++;
                }
            }

            return r;
        }

      
        public static T[][] Concatenate<T>(params T[][][] matrices)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < matrices.Length; i++)
            {
                cols += matrices[i][0].Length;
                if (matrices[i].Length > rows)
                    rows = matrices[i].Length;
            }

            T[][] r = new T[rows][];
            for (int i = 0; i < r.Length; i++)
                r[i] = new T[cols];


            int c = 0;
            for (int k = 0; k < matrices.Length; k++)
            {
                int currentRows = matrices[k].Length;
                int currentCols = matrices[k][0].Length;

                for (int j = 0; j < currentCols; j++)
                {
                    for (int i = 0; i < currentRows; i++)
                    {
                        r[i][c] = matrices[k][i][j];
                    }
                    c++;
                }
            }

            return r;
        }

        public static T[] Concatenate<T>(this T[][] vectors)
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

      
    }
}