using System;




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
        
        public static T Max<T>(this T[] values, out int imax)
            where T : IComparable<T>
        {
            imax = 0;
            T max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                    imax = i;
                }
            }
            return max;
        }
        
        public static T Max<T>(this T[] values)
            where T : IComparable<T>
        {
            int imax;
            return Max(values, out imax);
        }
        
        public static T Min<T>(this T[] values, out int imin)
            where T : IComparable<T>
        {
            imin = 0;
            T min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                    imin = i;
                }
            }
            return min;
        }
        
        public static T Min<T>(this T[] values)
            where T : IComparable<T>
        {
            int imin;
            return Min(values, out imin);
        }
    }
}
