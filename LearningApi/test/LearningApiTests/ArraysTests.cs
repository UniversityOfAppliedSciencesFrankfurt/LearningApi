using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LearningFoundation;
using LearningFoundation.Statistics;
using LearningFoundation.Arrays;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LearningFoundation.Test.Statistics
{
    /// <summary>
    /// Unit tests for validation and check corect result for advanced statistics calculation of various operations
    /// </summary>
    [TestClass]
    public class ArraysTests
    {
        [TestMethod]
        public void FindFirstTest()
        {
            var dataSample1 = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 5, 6, 7, 2, 5, 7, 8, 3, 4, 5, 2, 3, 4 };

            var ocurrences = dataSample1.FindAllOcurrences();

            Assert.Equals(ocurrences.Length, 8);
        }

        /// <summary>
        /// Calculates hamming distance odf sparse arrays.
        /// </summary>
        [TestMethod]
        public void HemmingDistanceTest()
        {
            var dataSample1 = new double[] { 1.0, 1.0, 0.0, 0.0, 1.0 };
            var dataSample2 = new double[] { 1.0, 1.0, 0.0, 0.0, 1.0 };

            var acc = dataSample1.GetHammingDistance(dataSample2);

            Assert.Equals(acc, 100);

            dataSample2 = new double[] { 1.0, 1.0, 0.0, 0.0, 0.0 };

            acc = dataSample1.GetHammingDistance(dataSample2);

            Assert.Equals(acc, 80);

            dataSample2 = new double[] { 1.0, 0.0, 1.0, 1.0, 0.0 };

            acc = dataSample1.GetHammingDistance(dataSample2);

            Assert.Equals(acc, 20);
        }

        [TestMethod]
        public void BinaryConversionTest()
        {
            Assert.Equals(new double[] { 1, 1, 1 }.ToBinary(), 7);
            Assert.Equals(new double[] { 0, 1, 0 }.ToBinary(), 2);
            Assert.Equals(new double[] { 0, 0, 1 }.ToBinary(), 4); 
        }

    }
}
