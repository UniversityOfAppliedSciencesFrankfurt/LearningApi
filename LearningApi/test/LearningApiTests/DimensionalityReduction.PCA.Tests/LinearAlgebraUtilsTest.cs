using DimensionalityReduction.PCA.Tests.EqualityComparers;
using LearningFoundation.DimensionalityReduction.PCA.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DimensionalityReduction.PCA.Tests
{
    /// <summary>
    /// Test all mesthod of the implementation of GeneralLinearAlgebraUtils
    /// </summary>
    [TestClass]
    public class LinearAlgebraUtilsTest
    {
        private static GeneralLinearAlgebraUtils Linag = GeneralLinearAlgebraUtils.getImpl();
        private static DoubleVectorComparer DVectorComparer = new DoubleVectorComparer();
        private static DoubleMatrixComparer DMatrixComparer = new DoubleMatrixComparer();
        private static DoubleCubicComparer DCubicComparer = new DoubleCubicComparer();

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductM2V() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void DotProductM2V_RR()
        {
            double[][] matrix = new double[3][];
            matrix[0] = new double[] { 1.0, 0.0 };
            matrix[1] = new double[] {-1.0, -3.0 };
            matrix[2] = new double[] { 2.0, 2.0 };

            double[] result = Linag.DotProductM2V(matrix, new double[] { 2.0, 1.0, 0.0 });

            double[] expect = new double[] { 1.0, -3.0 };
            Assert.AreEqual(expect, result);//, DVectorComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductM2V() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void DotProductM2V_WR()
        {
            double[][] matrix = new double[3][];
            matrix[0] = new double[] { 1.0, 0.0 };
            matrix[1] = new double[] { -1.0, -3.0 };
            matrix[2] = new double[] { 2.0, 2.0 };
            double[] result = Linag.DotProductM2V(matrix, new double[] { 2.0, 1.0, 0.0 });
            double[] expect = new double[] { 1.0, 0.0 };
            Assert.AreNotEqual(expect, result);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductM2V() method
        /// Type: WD - Wrong Dimension (Expected for an exception)
        /// </summary>
        [TestMethod]
        public void DotProductM2V_WD()
        {
            double[][] matrix = new double[3][];
            matrix[0] = new double[] { 1, 0 };
            matrix[1] = new double[] { 2, 2 };
            Action Code = () => { Linag.DotProductM2V(matrix, new double[] { 2, 1, 0 }); };
            //var ex = Record.Exception(Code) ;
            //Assert.NotNull(ex);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.Substract2Vectors() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void Substract2Vectors_RR()
        {
            double[] A = new double[] { 1, 2 };
            double[] B = new double[] { 3, 4 };

            double[] expect = new double[] { -2, -2 };

            double[] result = Linag.Substract2Vectors(A, B);
            Assert.AreEqual(expect, result);//, DVectorComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.Substract2Vectors() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void Substract2Vectors_WR()
        {
            double[] A = new double[] { 1, 2 };
            double[] B = new double[] { 3, 4 };

            double[] expect = new double[] { 2, 2 };

            double[] result = Linag.Substract2Vectors(A, B);
            Assert.AreNotEqual(expect, result);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.Substract2Vectors() method
        /// Type: WD - Wrong Dimension (Expected for an exception)
        /// </summary>
        [TestMethod]
        public void Substract2Vectors_WD()
        {
            double[] A = new double[] { 1, 2 };
            double[] B = new double[] { 3, 4, 5 };
            Action Code = () => { Linag.Substract2Vectors(A, B); };
            //var ex = Record.Exception(Code);
            //Assert.NotNull(ex);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.SubstractMatrixScala() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void SubstractMatrixScala_RR()
        {
            double A = 4.0;
            double[][] B = new double[2][];
            B[0] = new double[] { 1, 3 };
            B[1] = new double[] { 2, 4 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { -3, -1 };
            expect[1] = new double[] { -2, 0 };

            double[][] result = Linag.SubstractMatrixScala(B, A);
            Assert.AreEqual(expect, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.SubstractMatrixScala() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void SubstractMatrixScala_WR()
        {
            double A = 4.0;
            double[][] B = new double[2][];
            B[0] = new double[] { 1, 3 };
            B[1] = new double[] { 2, 4 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { -3, -1 };
            expect[1] = new double[] { -2, 1 };

            double[][] result = Linag.SubstractMatrixScala(B, A);
            Assert.AreNotEqual(expect, result);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.SubstractMatrixScala() method
        /// Type: WD - Wrong Dimension (Expected for an exception)
        /// </summary>
        [TestMethod]
        public void SubstractMatrixScala_WD()
        {
            double A = 4.0;
            double[][] B = new double[2][];
            B[0] = new double[] { 1, 3 };
            B[1] = new double[] { 2, 4, 11 };

            Action Code = () => { Linag.SubstractMatrixScala(B, A); };
            //var ex = Record.Exception(Code);
            //Assert.NotNull(ex);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.Transpose() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void Transpose_RR()
        {
            double[][] B = new double[2][];
            B[0] = new double[] { 1, 3 };
            B[1] = new double[] { 2, 4 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { 1, 2 };
            expect[1] = new double[] { 3, 4 };

            double[][] result = Linag.Transpose(B);
            Assert.AreEqual(expect, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.Transpose() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void Transpose_WR()
        {
            double[][] B = new double[2][];
            B[0] = new double[] { 1, 3 };
            B[1] = new double[] { 2, 4 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { 1, 2 };
            expect[1] = new double[] { 3, -4 };

            double[][] result = Linag.Transpose(B);
            Assert.AreNotEqual(expect, result);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.Transpose() method
        /// Type: WD - Wrong Dimension (Expected for an exception)
        /// </summary>
        [TestMethod]
        public void Transpose_WD()
        {
            double[][] B = new double[3][];
            B[0] = new double[] { 1, 3 };
            B[1] = new double[] { 2, 4 };
            B[2] = new double[] { 3, 5, 5, 12 };

            Action Code = () => { Linag.Transpose(B); };
            //var ex = Record.Exception(Code);
            //Assert.NotNull(ex);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductV2M() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void VectorMatrixDotProduct_RR()
        {
            double[] A = new double[] { 6, 4, 24 };
            double[][] B = new double[2][];

            B[0] = new double[] { 6, 4, 24 };
            B[1] = new double[] { 1, -9, 8 };

            double[] expect = new double[] { 628, 162 };
            double[] result = Linag.DotProductV2M(A, B);

            Assert.AreEqual(expect, result);//
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductV2M() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void VectorMatrixDotProduct_WR()
        {
            double[] A = new double[] { 6, 4, 24 };
            double[][] B = new double[2][];

            B[0] = new double[] { 6, 4, 24 };
            B[1] = new double[] { 1, -9, 8 };

            double[] expect = new double[] { 628, 162.01 };
            double[] result = Linag.DotProductV2M(A, B);

            Assert.AreNotEqual(expect, result);//, DVectorComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductV2M() method
        /// Type: WD - Wrong Dimension (Expected for an exception)
        /// </summary>
        [TestMethod]
        public void VectorMatrixDotProduct_WD()
        {
            double[] A = new double[] { 6, 4, 24 };
            double[][] B = new double[2][];

            B[0] = new double[] { 6, 4};
            B[1] = new double[] { 1, -9};

            Action Code = () => { Linag.DotProductV2M(A, B); };
            //var ex = Record.Exception(Code);
            //Assert.NotNull(ex);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.SubstractMatrixVector() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void SubstractMatrixVector_RR()
        {
            double[] A = new double[] { 1, 2 };
            double[][] B = new double[2][];
            B[0] = new double[] { 1, 3 };
            B[1] = new double[] { 2, 4 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { 0, 1 };
            expect[1] = new double[] { 1, 2 };

            double[][] result = Linag.SubstractMatrixVector(B, A);
            Assert.AreEqual(expect, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.SubstractMatrixVector() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void SubstractMatrixVector_WR()
        {
            double[] A = new double[] { 1, 2 };
            double[][] B = new double[2][];
            B[0] = new double[] { 1, 3 };
            B[1] = new double[] { 2, 4 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { 0, 1 };
            expect[1] = new double[] { 1, -2 };

            double[][] result = Linag.SubstractMatrixVector(B, A);
            Assert.AreNotEqual(expect, result);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.Substract2Vectors() method
        /// Type: WD - Wrong Dimension (Expected for an exception)
        /// </summary>
        [TestMethod]
        public void SubstractMatrixVector_WD()
        {
            double[] A = new double[] { 1, 2 };
            double[][] B = new double[2][];
            B[0] = new double[] { 1, 3 , 10};
            B[1] = new double[] { 2, 4 };

            Action Code = () => { Linag.SubstractMatrixVector(B, A); };
            //var ex = Record.Exception(Code);
            //Assert.NotNull(ex);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.SubstractVectorScala() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void SubstractVectorScala_RR()
        {
            double A = 2;
            double[] B = new double[] { 1, 3 };

            double[] expect = new double[] { -1, 1 };

            double[] result = Linag.SubstractVectorScala(B, A);
            Assert.AreEqual(expect, result);//
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.SubstractVectorScala() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void SubstractVectorScala_WR()
        {
            double A = 2;
            double[] B = new double[] { 1, 3 };

            double[] expect = new double[] { -1, -1 };

            double[] result = Linag.SubstractVectorScala(B, A);
            Assert.AreNotEqual(expect, result);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductM2M() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void MatrixMatrixDotProduct_RR()
        {
            double[][] A = new double[3][];
            double[][] B = new double[2][];
            double[][] expect = new double[2][];

            A[0] = new double[] { 0, -4 };
            A[1] = new double[] { 4, -3 };
            A[2] = new double[] { -2, 0 };

            B[0] = new double[] { 0, 1, 2 };
            B[1] = new double[] { 1, -1, 3};

            expect[0] = new double[] { 0, -3 };
            expect[1] = new double[] { -10, -1};

            double[][] result = Linag.DotProductM2M(A, B);
            Assert.AreEqual(expect, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductM2M() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void MatrixMatrixDotProduct_WR()
        {
            double[][] A = new double[3][];
            double[][] B = new double[2][];
            double[][] expect = new double[2][];

            A[0] = new double[] { 0, -4 };
            A[1] = new double[] { 4, -3 };
            A[2] = new double[] { -2, 0 };

            B[0] = new double[] { 0, -6, 2 };
            B[1] = new double[] { 1, 8, 3 };

            expect[0] = new double[] { 0, -3 };
            expect[1] = new double[] { -10, -1 };

            double[][] result = Linag.DotProductM2M(A, B);
            Assert.AreNotEqual(expect, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.DotProductM2M() method
        /// Type: WD - Wrong Dimension (Expect for an exception)
        /// </summary>
        [TestMethod]
        public void MatrixMatrixDotProduct_WD()
        {
            double[][] A = new double[3][];
            double[][] B = new double[2][];
            double[][] expect = new double[2][];

            A[0] = new double[] { 0, -4 };
            A[1] = new double[] { 4, -3 };
            A[2] = new double[] { -2, 0 };

            B[0] = new double[] { 0, 1};
            B[1] = new double[] { 1, -1};

            Action Code = () => { Linag.DotProductM2M(A, B); };
            //var ex = Record.Exception(Code);
            //Assert.NotNull(ex);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.CalculateCovarienceMatrix() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void CalculateCovarienceMatrix_RR()
        {
            double[][] A = new double[3][];
            A[0] = new double[] { 0, 2 };
            A[1] = new double[] { 1, 1 };
            A[2] = new double[] { 2, 0 };


            double[][] expect = new double[2][];
            expect[0] = new double[] { 0.666666666666667, -0.666666666666667 };
            expect[1] = new double[] { -0.666666666666667, 0.666666666666667 };


            double[][] result = Linag.CalculateCovarienceMatrix(A);
            Assert.AreEqual(expect, result);//,DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.CalculateMeanVector() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void CalculateMeanVector_RR()
        {
            double[][] A = new double[2][];
            A[0] = new double[] { 1, 3 };
            A[1] = new double[] { 2, 4 };

            double[] expect = new double[] {1.5, 3.5};
            double[] result = Linag.CalculateMeanVector(A);

            Assert.AreEqual(expect, result);//, DVectorComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.CalculateEigenVectors() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void CalculateEigenVectors_RR()
        {
            double[][] A = new double[3][];
            A[0] = new double[] { 0, 2};
            A[1] = new double[] { 1, 1};
            A[2] = new double[] { 2, 0 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { -0.707106781, 0.707106781 };
            expect[1] = new double[] { 0.707106781, 0.707106781 };

            double[][] result = Linag.CalculateEigenVectors(A);

            double[][] _I = new double[2][];
            _I[0] = new double[] { 1, 0 };
            _I[1] = new double[] { 0, 1 };

            Assert.AreEqual(_I, Linag.DotProductM2M(result, Linag.Transpose(result)));//,DMatrixComparer);
            Assert.AreEqual(expect, result);// DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.MatrixAbs() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void MatrixAbs_RR()
        {
            double[][] A = new double[2][];
            A[0] = new double[] { 1, -3 };
            A[1] = new double[] { -2, 4 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { 1, 3 };
            expect[1] = new double[] { 2, 4 };

            double[][] result = Linag.MatrixAbs(A);

            Assert.AreEqual(expect, result);//DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.MatrixAbs() method
        /// Type: WR - Wrong Return (expect != result)
        /// </summary>
        [TestMethod]
        public void MatrixAbs_WR()
        {
            double[][] A = new double[2][];
            A[0] = new double[] { 1, -5 };
            A[1] = new double[] { -8, 4 };

            double[][] expect = new double[2][];
            expect[0] = new double[] { 1, 3 };
            expect[1] = new double[] { 2, 4 };

            double[][] result = Linag.MatrixAbs(A);

            Assert.AreNotEqual(expect, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.MatrixAbs() method
        /// Type: WD - Wrong Dimension (Expected for an exception)
        /// </summary>
        [TestMethod]
        public void MatrixAbs_WD()
        {
            double[][] A = new double[2][];
            A[0] = new double[] { 1, -5 };
            A[1] = new double[] { -8 };

            Action Code = () => { Linag.MatrixAbs(A); };
            //var ex = Record.Exception(Code);
            //Assert.NotNull(ex);
        }

        /// <summary>
        /// Test GeneralLinearAlgebraUtils.SVD() method
        /// Type: RR - Right Return (expect == result)
        /// </summary>
        [TestMethod]
        public void SVD_RR()
        {

            double[][] A = new double[2][];
            A[0] = new double[] { 2.0,  1.0 };
            A[1] = new double[] { 1.0, 6.0 };

           //////////////////////////////////////
            // U
            double[][] U = new double[2][];
            U[0] = new double[] { 0.22975292, 0.97324899};
            U[1] = new double[] { 0.97324899, -0.22975292};
            // W
            double[][] W = new double[2][];
            W[0] = new double[] { 6.236068, 0.0};
            W[1] = new double[] { 0.0, 1.763932 };
            // VT
            double[][] VT = new double[2][];
            VT[0] = new double[] { 0.22975292, 0.97324899 };
            VT[1] = new double[] { 0.97324899, -0.22975292 };
            // S
            double[][] S = new double[1][];
            S[0] = new double[2] { 6.236068, 1.763932 };
            /////////////////////////////////////////////

            double[][][] expect = new double[4][][];
            expect[0] = U;
            expect[1] = W;
            expect[2] = VT;
            expect[3] = S;

            double[][][] result = Linag.SVD(A);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.GetLength(0));
            Assert.AreEqual(expect.GetLength(0), result.GetLength(0));

            for (int i = 0; i < result.GetLength(0); i++)
            {
                Assert.AreEqual(Linag.MatrixAbs(expect[i]), Linag.MatrixAbs(result[i]));//, DMatrixComparer);
            } 
        }
    }
}
