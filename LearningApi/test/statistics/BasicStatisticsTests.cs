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
    /// Unit tests for validation and check corect result for statistics calculation of various operations
    /// </summary>
    public class BasicStatisticsTests
    {
        [Fact]
        public void MeanOfTest()
        {
            var dataSample1 = new double[] { 6, 11, 7, 20 };
            var result = dataSample1.MeanOf();
            Assert.Equal(result, 11);
        }

       
        [Theory]
        [InlineData(new double[] { 6, 11, 7, 20 })]
        [InlineData(new double[] { 2, 3, 1 })]
        public void MeanOfTest1(double[] data)
        {
            var result = data.MeanOf();
            Assert.True(result == 11 || result == 2);
        }

      
        [Fact]
        public bool MeanTest()
        {
            //define stats modul 
            
            //Test 2
            var dataSample1 = new double[] { 6, 11, 7, 20, 34, 56, 2, 4, 56, 78, 65, 23, 456, 56 };
            var result = dataSample1.MeanOf();
            Assert.Equal(Math.Round(result,2), 62.43);


            //Test 3
            dataSample1 = new double[] { 6.54, 11.45, 7.523, 20, 0.534, 2.56, 2, 4, 56, 78, 65, 23, 456.0987, 56.071, 789.0987 };
            result =  dataSample1.MeanOf();
            Assert.Equal(Math.Round(result, 2), 105.19);

            //Test 4
            dataSample1 = new double[] { 6.54, 122.45, 11.45, 7.523, 20, 0.534, 0.534, 2.56, 2, 4, 56, 78, 65, 23, 456.0987, 56.071, 789.0987 };
            result =  dataSample1.MeanOf();
            Assert.Equal(Math.Round(result, 2), 100.05);


            return true;
        }

        [Fact]
        public bool MedianTest()
        {
            //test 1
            var dataSample1 = new double[] { 6, 11, 7, 20 };
            var result = dataSample1.MedianOf();
            Assert.Equal(result, 9);


            //Test 2
            dataSample1 = new double[] { 6, 11, 7, 20, 34, 56, 2, 4, 56, 78, 65, 23, 456, 56 };
            result =  dataSample1.MedianOf();
            Assert.Equal(Math.Round(result, 2), 28.5);


            //Test 3
            dataSample1 = new double[] { 6.54, 11.45, 7.523, 20, 0.534, 2.56, 2, 4, 56, 78, 65, 23, 456.0987, 56.071, 789.0987 };
            result =  dataSample1.MedianOf();
            Assert.Equal(Math.Round(result, 2), 20);

            //Test 4
            dataSample1 = new double[] { 6.54, 122.45, 11.45, 7.523, 20, 0.534, 0.534, 2.56, 2, 4, 56, 78, 65, 23, 456.0987, 56.071, 789.0987 };
            result =  dataSample1.MedianOf();
            Assert.Equal(Math.Round(result, 2), 20);


            return true;
        }

        [Fact]
        public bool VarianceTest()
        {
           
            //test 1
            var dataSample1 = new double[] { 6, 11, 7, 20 };
            var result = dataSample1.VarianceOfS();
            Assert.Equal(Math.Round(result,2), 40.67);


            //Test 2
            dataSample1 = new double[] { 6, 11, 7, 20, 34, 56, 2, 4, 56, 78, 65, 23, 456, 56 };
            result = dataSample1.VarianceOfS();
            Assert.Equal(Math.Round(result, 2), 13492.42);

            //Test 3
            dataSample1 = new double[] { 6.54, 11.45, 7.523, 20, 0.534, 2.56, 2, 4, 56, 78, 65, 23, 456.0987, 56.071, 789.0987 };
            result = dataSample1.VarianceOfS();
            Assert.Equal(Math.Round(result, 2), 48749.90);


            //Test 4
            dataSample1 = new double[] { 6.54, 122.45, 11.45, 7.523, 20, 0.534, 0.534, 2.56, 2, 4, 56, 78, 65, 23, 456.0987, 56.071, 789.0987 };
            result = dataSample1.VarianceOfS();
            Assert.Equal(Math.Round(result, 2), 43331.28);


            return true;
        }
    }
}
