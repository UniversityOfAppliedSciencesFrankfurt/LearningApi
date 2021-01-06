using DimensionalityReduction.PCA.Tests.Const;
using DimensionalityReduction.PCA.Tests.EqualityComparers;
using DimensionalityReduction.PCA.Utils;
using LearningFoundation.DimensionalityReduction.PCA.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace DimensionalityReduction.PCA.Tests
{
    /// <summary>
    /// Test Utils classes CsvUtils, DataParserUtils, ImageUtils
    /// </summary>
   [TestClass]
    public class UtilitiesClassTest
    {
        private static DoubleVectorComparer DVectorComparer = new DoubleVectorComparer();
        private static DoubleMatrixComparer DMatrixComparer = new DoubleMatrixComparer();
        private static DoubleCubicComparer DCubicComparer = new DoubleCubicComparer();

        /// <summary>
        /// Test to use CsvUtils to read csv file into double[][]
        /// </summary>
        [TestMethod]
        public static void Test_ReadCSVToDouble2DArray()
        {
            double[][] expectedResult = new double[2][];
            expectedResult[0] = new double[] { 1, 2, 3, 4 };
            expectedResult[1] = new double[] { 5, 6, 7, 8 };

            double[][] result = CsvUtils.ReadCSVToDouble2DArray(Path.Combine(TestConstants.INPUT_DATA_DIR, "csv.for.testing.csv.utils.csv"), ',');
            Assert.AreEqual(expectedResult, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test to use CsvUtils to write double[][] to csv file 
        /// </summary>
        [TestMethod]
        public static void Test_WriteCSVFromDouble2DArray()
        {
            double[][] expectedResult = new double[2][];
            expectedResult[0] = new double[] { 1, 2, 3, 4 };
            expectedResult[1] = new double[] { 5, 6, 7, 8 };

            CsvUtils.WriteCSVFromDouble2DArray(Path.Combine(TestConstants.OUTPUT_RESULT_DIR, "csv.for.testing.csv.utils.csv"), ',', expectedResult);

            double[][] result = CsvUtils.ReadCSVToDouble2DArray(Path.Combine(TestConstants.OUTPUT_RESULT_DIR, "csv.for.testing.csv.utils.csv"), ',');
            Assert.AreEqual(expectedResult, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test to parse and array in object to double[][]
        /// </summary>
        [TestMethod]
        public static void Test_ParseObjectToDouble2DArray()
        {
            double[][] expectedResult = new double[2][];
            expectedResult[0] = new double[] { 1, 2, 3, 4 };
            expectedResult[1] = new double[] { 5, 6, 7, 8 };

            object tmp = expectedResult;

            double[][] result = DataParserUtils.ParseObjectToDouble2DArray(tmp);
            Assert.AreEqual(expectedResult, result);//, DMatrixComparer);
        }

        /// <summary>
        /// Test to parse and array in object[][] to double[][]
        /// </summary>
        [TestMethod]
        public static void Test_ParseObject2DArrayToDouble2DArray()
        {
            double[][] expectedResult = new double[2][];
            expectedResult[0] = new double[] { 1, 2, 3, 4 };
            expectedResult[1] = new double[] { 5, 6, 7, 8 };

            object[][] tmp = new object[expectedResult.GetLength(0)][];
            for (int i = 0; i < expectedResult.GetLength(0); i++)
            {
                tmp[i] = new object[expectedResult[0].GetLength(0)];
                for (int j = 0; j < expectedResult[0].GetLength(0); j++)
                {
                    tmp[i][j] = expectedResult[i][j];
                }
            }

            double[][] result = DataParserUtils.ParseObject2DArrayToDouble2DArray(tmp);
            Assert.AreEqual(expectedResult, result);//, DMatrixComparer);
        }

        /// <summary>
        /// These test use csv which is generated from python to test ImageUtils to read a bmp file to double[][] array
        /// </summary>
        [TestMethod]
        public static void Test_ReadBMPImageToDoubleArray()
        {
            double[][] imageData = ImageUtils.getImageAsDoubleArray(Path.Combine(TestConstants.INPUT_DATA_DIR, "face1.bmp"));

            // csv file is generated from original image using python code.
            double[][] expectedData = CsvUtils.ReadCSVToDouble2DArray(Path.Combine(TestConstants.INPUT_DATA_DIR, "face1.bmp.csv"), ',');

            //Here we compare data which is generated from python code with data read by Utils class
            Assert.AreEqual(expectedData, imageData);// DMatrixComparer);
        }

        /// <summary>
        /// These test use csv which is generated from python to test ImageUtils to save a double[][] array into bmp file
        /// </summary>
        [TestMethod]
        public static void Test_SaveDoubleArrayToBMPImage()
        {
            // csv file is generated from original image using python code.
            double[][] exampleImageData = CsvUtils.ReadCSVToDouble2DArray(Path.Combine(TestConstants.INPUT_DATA_DIR, "face1.bmp.csv"), ',');

            //write image data to a new image file
            ImageUtils.saveDoubleArrayToImage(Path.Combine(TestConstants.OUTPUT_RESULT_DIR, "face1.test.doublearray.to.image.bmp"), exampleImageData);

            //Now read back from that file
            double[][] writtenImageData = ImageUtils.getImageAsDoubleArray(Path.Combine(TestConstants.OUTPUT_RESULT_DIR, "face1.test.doublearray.to.image.bmp"));

            //We expect that exampleImageData should equal writtenImageData
            Assert.AreEqual(exampleImageData, writtenImageData);//, DMatrixComparer);
        }
    }
}
