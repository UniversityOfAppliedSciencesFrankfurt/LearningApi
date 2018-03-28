using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Matrix
{
    public static class MatrixMath
    {
        public static double[][] CreateEmptyMatrix(int rows, int cols)
        {
            // allocates/creates a matrix initialized to all 0.0
            // do error checking here
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }
    }
}
