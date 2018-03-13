using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using LearningFoundation;
using LearningFoundation.Statistics;
using LearningFoundation.Arrays;

namespace test.statistics
{
    /// <summary>
    /// Unit tests for validation and check corect result for advanced statistics calculation of various operations
    /// </summary>
    public class MarkovChainTests
    {
        [Fact]
        public void FindFirstOrderTest()
        {
            var dataSample1 = new object[] { 1, 2, 3, 4, 4, 4, 7, 7, 1, 2, 3, 5, 6, 7, 2, 5, 7, 8, 3, 4, 5, 2, 3, 4 };
            var dataSample2 = new object[] { 1, 2, 3, 2, 5, 6, 7, 1, 1, 2, 3, 5, 6, 7,3, 4, 7, 7, 7, 4, 7, 2, 3, 4 };

            var ocurrences = dataSample1.FindAllOcurrences();

            var result = MarkovChain.CalculateFirstOrder(new object[][] { dataSample1, dataSample2 }, ocurrences);

            Assert.Equal(ocurrences.Length, 8);
        }



    }
}
