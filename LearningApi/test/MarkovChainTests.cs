using LearningFoundation.Arrays;
using LearningFoundation.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LearningFoundation.Test
{
    /// <summary>
    /// Unit tests for validation and check corect result for advanced statistics calculation of various operations
    /// </summary>
    [TestClass]
    public class MarkovChainTests
    {
        [TestMethod]
        public void FindFirstOrderTest()
        {
            var dataSample1 = new object[] { 1, 2, 3, 4, 4, 4, 7, 7, 1, 2, 3, 5, 6, 7, 2, 5, 7, 8, 3, 4, 5, 2, 3, 4 };
            var dataSample2 = new object[] { 1, 2, 3, 2, 5, 6, 7, 1, 1, 2, 3, 5, 6, 7,3, 4, 7, 7, 7, 4, 7, 2, 3, 4 };

            var ocurrences = dataSample1.FindAllOcurrences();

            var result = MarkovChain.CalculateFirstOrder(new object[][] { dataSample1, dataSample2 }, ocurrences);

            Assert.AreEqual(ocurrences.Length, 8);
        }



    }
}
