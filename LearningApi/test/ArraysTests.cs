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
    public class ArraysTests
    {
        [Fact]
        public void FindFirstTests()
        {
            var dataSample1 = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 5, 6, 7, 2, 5, 7, 8, 3, 4, 5, 2, 3, 4 };
            
            var ocurrences = dataSample1.FindAllOcurrences();
            
            Assert.Equal(ocurrences.Length, 8);
        }



    }
}
