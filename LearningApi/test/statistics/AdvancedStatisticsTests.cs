using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using LearningFoundation;
using LearningFoundation.Statistics;

namespace test.statistics
{
    /// <summary>
    /// Unit tests for validation and check corect result for advanced statistics calculation of various operations
    /// </summary>
    public class AdvancedStatisticsTests
    {

        [Fact]
        public bool CorrelatioCoefficientTests()
        {
            //define stats modul 
            
            //test 1
            var dataSample1 = new double[]{ 180, 176, 144, 195, 159, 185, 166, 173, 149, 168 };
            var dataSample2 = new double[] { 87,  65,  52,  94,  87,  79,  59,  64,  45,  77 };

            var result = dataSample1.CorrCoeffOf(dataSample2);
            Assert.Equal(Math.Round(result, 4), 0.7294);

            

            return true;
        }
    }
}
