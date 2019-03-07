using Microsoft.VisualStudio.TestTools.UnitTesting;
using CenterModule;

namespace CenterModuleUnitTests
{
    [TestClass]
    public class MethodUnitTest
    {  //This Method shows how the Calculation of the Average works
        [TestMethod]
        public void TestCalculateAverage()
        {

            double[][] data = new double[8][];
            double average;
            for (int x = 0; x < 8; x++)
            {
                data[x] = new double[8];
                for (int y = 0; y < 8; y++)
                {
                    data[x][y] = 1;
                }
            }

            CenterAlgorithm center = new CenterAlgorithm();
            average = center.GetAverage(data);
            Assert.AreEqual(1, average);
        }/// <summary>
        /// This method sum all x- and y-Values in a array
        /// </summary>
        [TestMethod]
        
        public void TestSumXYValues()
        {
            float[] x = { 1, 2, 3, 4, 5, 6, 7 };
            float[] y = { 1, 2, 3, 4, 5, 6, 7 };
            int countOnes = 7;
            CenterAlgorithm center = new CenterAlgorithm();
            float xEnd = center.GetSumXValues(x, countOnes);
            float yEnd = center.GetSumYValues(y, countOnes);



            Assert.AreEqual(28, xEnd);
            Assert.AreEqual(28, yEnd);
        }
        /// <summary>
        /// Test if all Values a correct replaced with "1"
        /// </summary>
        [TestMethod]
        public void TestBinarization()
        {
            double[][] data = new double[8][];
            for (int x = 0; x < 8; x++)
            {
                data[x] = new double[8];
                for (int y = 0; y < 8; y++)
                {
                    data[x][y] = 1;
                }
            }

            CenterAlgorithm center = new CenterAlgorithm();
            double[][] binarizedData = center.Binarization(data);
            Assert.AreEqual(data, binarizedData);

        }
        // This Test shows if the outputarray has the same size as the origin array
        [TestMethod]
        public void TestInitiateOutputArray()
        {
            int widthOriginArray = 8;
            int heightOriginArray = 8;
            CenterAlgorithm center = new CenterAlgorithm();
            double[][] centeredData = center.InitiateCenteredArray(widthOriginArray, heightOriginArray);
            int widthCenteredArray = centeredData[0].Length;
            int heightCenteredArray = centeredData.Length;
            Assert.AreEqual(widthOriginArray, widthCenteredArray);
            Assert.AreEqual(heightOriginArray, heightCenteredArray);
        }

    }
}
