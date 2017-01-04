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
        public void CorrelationCoefficientTests()
        {
            //define stats modul 

            //test 1
            var dataSample1 = new double[] { 180, 176, 144, 195, 159, 185, 166, 173, 149, 168 };
            var dataSample2 = new double[] { 87, 65, 52, 94, 87, 79, 59, 64, 45, 77 };

            var result = dataSample1.CorrCoeffOf(dataSample2);
            Assert.Equal(Math.Round(result, 4), 0.7294);
        }


        [Fact]
        public void CorrelationCoefficientTests2()
        {
            //define stats modul 

            //test 1
            var dataSample1 = new double[] { 180, 176, 144, 195, 159, 185, 166, 173, 149, 168 };
            var dataSample2 = new double[] { 178, 171, 149, 195, 162, 181, 160, 175, 159, 168 };

            var result = dataSample1.CorrCoeffOf(dataSample2);
            Assert.Equal(Math.Round(result, 4), 0.9584);           
        }
        [Fact]
        public void CalculateMeanStDev_Tests2()
        {
            //define stats modul 

            //test 1
            var dataset = getSampleDataSet();

            var result = dataset.calculateMeanStDev();
            //mean
            Assert.Equal(Math.Round(result.Item1[0], 4), -0.2916);
            Assert.Equal(Math.Round(result.Item1[1], 4), 0.55);
            Assert.Equal(Math.Round(result.Item1[2], 4), 1);

            //stdev
            Assert.Equal(Math.Round(result.Item2[0], 4), 2.6234);
            Assert.Equal(Math.Round(result.Item2[1], 4), 0.5104);
            Assert.Equal(Math.Round(result.Item2[2], 4), 0.7947);
        }


        private double[][] getSampleDataSet()
        {
            var data = new double[20][] {
                                    new double[]{+1.283,1,0},
                                    new double[]{-0.843,1,1},
                                    new double[]{+2.364,0,0},
                                    new double[]{+4.279,1,1},
                                    new double[]{+3.383,0,2},
                                    new double[]{-1.624,1,2},
                                    new double[]{-2.628,1,0},
                                    new double[]{+2.847,1,2},
                                    new double[]{+1.362,0,1},
                                    new double[]{+2.640,0,1},
                                    new double[]{-4.188,1,2},
                                    new double[]{-1.161,0,1},
                                    new double[]{+0.825,1,0},
                                    new double[]{-0.253,0,1},
                                    new double[]{-2.286,0,2},
                                    new double[]{-3.162,1,2},
                                    new double[]{-4.714,1,1},
                                    new double[]{-0.242,0,1},
                                    new double[]{-0.400,0,0},
                                    new double[]{-3.315,1,0},
                                         };
            //
            return data;
        }
    }
}
