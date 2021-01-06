using LearningFoundation.DimensionalityReduction.PCA.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LearningFoundation.DimensionalityReduction.PCA.Utils
{
    /// <summary>
    /// This class provide a common interface for other component,
    /// which wishes to do Linear Algebraic operation.
    ///
    /// All matrix in this should be column vector matrix of type double[][],
    /// which is an array of column vector.
    /// </summary>
    public abstract class GeneralLinearAlgebraUtils
    {
        private static GeneralLinearAlgebraUtils IMPL = null;

        /// <summary>
        /// Get the Implementation instance of this abstract class
        /// </summary>
        /// <returns>object inherit type GeneralLinearAlgebraUtils</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static GeneralLinearAlgebraUtils getImpl()
        {
            if (IMPL == null)
            {
                IMPL = new MathNetNumericsLinearAlgebraUtils();
            }
            return IMPL;
        }

        /// <summary>
        /// Calculating dot product of two matrices
        /// </summary>
        /// <param name="inputMatrix1">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <param name="inputMatrix2">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <returns>inputMatrix1.inputMatrix2</returns>
        public abstract double[][] DotProductM2M(double[][] inputMatrix1, double[][] inputMatrix2);

        /// <summary>
        /// Calculate dot product of maxtrix m with vector v
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <param name="inputVector"></param>
        /// <returns>inputMatrix.inputVector</returns>
        public abstract double[] DotProductM2V(double[][] inputMatrix, double[] inputVector);

        /// <summary>
        /// Calculate dot product of vector v with matrix m
        /// </summary>
        /// <param name="inputVector"></param>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <returns>inputVector.inputMatrix</returns>
        public abstract double[] DotProductV2M(double[] inputVector, double[][] inputMatrix);

        /// <summary>
        /// Calculate the mean vector of all vectors of the input matrix
        /// </summary>
        /// <param name="inputMatrix">input matrix with row vectors</param>
        /// <returns>mean vector of the input matrix</returns>
        public abstract double[] CalculateMeanVector(double[][] inputMatrix);

        /// <summary>
        /// Substracting two vectors element-wise.
        /// </summary>
        /// <param name="inputVector1">vector 1</param>
        /// <param name="inputVector2">vector 2</param>
        /// <returns>inputVector1 - inputVector2</returns>
        public abstract double[] Substract2Vectors(double[] inputVector1, double[] inputVector2);

        /// <summary>
        /// Substract each vector of the matrix with the given vector
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <param name="inputVector">vector</param>
        /// <returns>double[][] with each vector has been substracted with the input vector</returns>
        public abstract double[][] SubstractMatrixVector(double[][] inputMatrix, double[] inputVector);

        /// <summary>
        /// Add each vector of the matrix with the given vector
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <param name="inputVector">vector</param>
        /// <returns>double[][] with each vector has been added with the input vector</returns>
        public abstract double[][] AddMatrixVector(double[][] inputMatrix, double[] inputVector);

        /// <summary>
        /// Substract each element in the matrix with the given scala value
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <param name="scalar">scala</param>
        /// <returns>double[][] with each element has been substracted with the given scalar</returns>
        public abstract double[][] SubstractMatrixScala(double[][] inputMatrix, double scalar);

        /// <summary>
        /// Substract each element in the vector with the given scalar value
        /// </summary>
        /// <param name="inputVector"></param>
        /// <param name="scalar"></param>
        /// <returns>double[] with each element has been substracted with the given scalar</returns>
        public abstract double[] SubstractVectorScala(double[] inputVector, double scalar);

        /// <summary>
        /// Calculate the covariance matrix
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <returns>covariance matrix double[][], which output[i] represent the column vector of the covariance matrix </returns>
        public abstract double[][] CalculateCovarienceMatrix(double[][] inputMatrix);

        /// <summary>
        /// So Singular Value Decomposition on inputMatrix
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <returns>
        /// SVD matrix contains 4 sub matrix
        /// SVD[0] is the U matrix
        /// SVD[1] is the W mnatrix
        /// SVD[2] is the VT matrix
        /// SVD[3] is the S matrix
        /// </returns>
        public abstract double[][][] SVD(double[][] inputMatrix);

        /// <summary>
        /// Calculate the eigen vector of the covariance matrix
        /// </summary>
        /// <param name="covarianceMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <returns>double[][] array with output[i] represent the i column vector of the new matrix</returns>
        public abstract double[][] CalculateEigenVectors(double[][] covarianceMatrix);

        /// <summary>
        /// Matrix Transpose
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <returns>double[][] matrix is the transpose of the input matrix</returns>
        public abstract double[][] Transpose(double[][] inputMatrix);

        /// <summary>
        /// Take absolute value of all element of the matrix
        /// output[i][j] = |input[i][j]|
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <returns>new matrix in double[][] array with each element = |input[i][j]|</returns>
        public abstract double[][] MatrixAbs(double[][] inputMatrix);

        /// <summary>
        /// Take asolute value of all element of the input vector
        /// output[i] = |input[i]|
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        /// <returns>new vector in double[] array with each element = |input[i]|</returns>
        public abstract double[] VectorAbs(double[] inputMatrix);

        /// <summary>
        /// We are using jagged array, that why we have to check whether is there any column
        /// that the dimensions does not agree with each other.
        /// </summary>
        /// <param name="inputMatrix">double[][] matrix which each input[i] is the column vector the the data</param>
        protected void checkMatrixDimension(double[][] inputMatrix)
        {
            int firstDimension = inputMatrix[0].GetLength(0);
            foreach (double[] vector in inputMatrix)
            {
                if (firstDimension != vector.GetLength(0))
                {
                    throw new InvalidDimensionException("Dimensions of vectors in matrix do not agree with each other");
                }
            }
        }
    }
}
