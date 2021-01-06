using LearningFoundation.DimensionalityReduction.PCA.Exceptions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.DimensionalityReduction.PCA.Utils
{
    /// <summary>
    /// This class provide an implementation of GeneralLinearAlgebraUtils,
    /// which use Math.NET Numerics as the backend.
    /// </summary>
    public class MathNetNumericsLinearAlgebraUtils : GeneralLinearAlgebraUtils
    {
        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][] CalculateCovarienceMatrix(double[][] inputMatrix)
        {
            checkMatrixDimension(inputMatrix);

            int N = inputMatrix.GetLength(0);
            double[] meanVector = CalculateMeanVector(inputMatrix);
            double[][] xHat = SubstractMatrixVector(inputMatrix, meanVector);
            Matrix<double> xHatDotxHatT = DenseMatrix.OfColumnArrays(DotProductM2M(xHat, Transpose(xHat)));
            return xHatDotxHatT.Multiply(1.0 / (double)N).ToColumnArrays();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][] CalculateEigenVectors(double[][] inputMatrix)
        {
            checkMatrixDimension(inputMatrix);

            double[][] covarianceMatrix = CalculateCovarienceMatrix(inputMatrix);
            double[][][] svdMatrices = SVD(covarianceMatrix);
            double[][] eigenVector = svdMatrices[0];
            return eigenVector;
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[] CalculateMeanVector(double[][] inputMatrix)
        {
            checkMatrixDimension(inputMatrix);

            int N = inputMatrix.GetLength(0);
            int D = inputMatrix[0].GetLength(0);
            Vector<double> sumOfVectors = Vector<double>.Build.Dense(D);
            foreach (double[] vector in inputMatrix)
            {
                Vector<double> v = DenseVector.OfArray(vector);
                sumOfVectors = sumOfVectors + v;
            }
            sumOfVectors = sumOfVectors.Multiply(1.0 / (double)N);
            return sumOfVectors.ToArray();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][] DotProductM2M(double[][] inputMatrix1, double[][] inputMatrix2)
        {
            checkMatrixDimension(inputMatrix1);
            checkMatrixDimension(inputMatrix2);

            Matrix<double> m1 = DenseMatrix.OfColumnArrays(inputMatrix1);
            Matrix<double> m2 = DenseMatrix.OfColumnArrays(inputMatrix2);
            return (m1 * m2).ToColumnArrays();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[] DotProductM2V(double[][] inputMatrix, double[] inputVector)
        {
            checkMatrixDimension(inputMatrix);

            Matrix<double> m = DenseMatrix.OfColumnArrays(inputMatrix);
            Vector<double> v = DenseVector.OfArray(inputVector);
            return (m * v).ToArray();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[] DotProductV2M(double[] inputVector, double[][] inputMatrix)
        {
            checkMatrixDimension(inputMatrix);

            Vector<double> v = DenseVector.OfArray(inputVector);
            Matrix<double> m = DenseMatrix.OfColumnArrays(inputMatrix);
            return (v * m).ToArray();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][] MatrixAbs(double[][] inputMatrix)
        {
            checkMatrixDimension(inputMatrix);

            double[][] result = new double[inputMatrix.GetLength(0)][];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                result[i] = VectorAbs(inputMatrix[i]);
            }

            return result;
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[] Substract2Vectors(double[] inputVector1, double[] inputVector2)
        {
            if (inputVector1.GetLength(0) != inputVector2.GetLength(0))
            {
                throw new InvalidDimensionException("Dimension of two vectors are not agree with each other");
            }

            Vector<double> v1 = DenseVector.OfArray(inputVector1);
            Vector<double> v2 = DenseVector.OfArray(inputVector2);
            return (v1 - v2).ToArray();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][] SubstractMatrixScala(double[][] inputMatrix, double scalar)
        {
            checkMatrixDimension(inputMatrix);

            Matrix<double> m = DenseMatrix.OfColumnArrays(inputMatrix);
            return m.Subtract(scalar).ToColumnArrays();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][] SubstractMatrixVector(double[][] inputMatrix, double[] inputVector)
        {
            checkMatrixDimension(inputMatrix);

            double[][] resultMatrix = new double[inputMatrix.GetLength(0)][];
            Vector<double> _inputVector = DenseVector.OfArray(inputVector);
            for(int i = 0; i< inputMatrix.GetLength(0); i++)
            {
                Vector<double> v1 = DenseVector.OfArray(inputMatrix[i]);
                resultMatrix[i] = (v1 - _inputVector).ToArray();
            }
            return resultMatrix;

        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][] AddMatrixVector(double[][] inputMatrix, double[] inputVector)
        {
            checkMatrixDimension(inputMatrix);

            double[][] resultMatrix = new double[inputMatrix.GetLength(0)][];
            Vector<double> _inputVector = DenseVector.OfArray(inputVector);
            for(int i = 0; i< inputMatrix.GetLength(0); i++)
            {
                Vector<double> v1 = DenseVector.OfArray(inputMatrix[i]);
                resultMatrix[i] = (v1 + _inputVector).ToArray();
            }
            return resultMatrix;
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[] SubstractVectorScala(double[] inputVector, double scalar)
        {
            Vector<double> v = DenseVector.OfArray(inputVector);
            return v.Subtract(scalar).ToArray();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][][] SVD(double[][] inputMatrix)
        {
            checkMatrixDimension(inputMatrix);

            Matrix<double> m = DenseMatrix.OfColumnArrays(inputMatrix);
            Svd<double> SVD = m.Svd();
            double[][][] result = new double[4][][];
            result[0] = SVD.U.ToColumnArrays();
            result[1] = SVD.W.ToColumnArrays();
            result[2] = SVD.VT.ToColumnArrays();
            result[3] = SVD.S.ToColumnMatrix().ToColumnArrays();
            return result;
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[][] Transpose(double[][] inputMatrix)
        {
            checkMatrixDimension(inputMatrix);

            Matrix<double> m = DenseMatrix.OfColumnArrays(inputMatrix);
            return m.Transpose().ToColumnArrays();
        }

        /// <summary>
        /// See LearningFoundation.DimensionalityReduction.PCA.Utils.GeneralLinearAlgebraUtils
        /// </summary>
        public override double[] VectorAbs(double[] inputVector)
        {
            double[] result = new double[inputVector.GetLength(0)];

            for (int i = 0; i < inputVector.GetLength(0); i++)
            {
                result[i] = Math.Abs(inputVector[i]);
            }

            return result;
        }
    }
}
