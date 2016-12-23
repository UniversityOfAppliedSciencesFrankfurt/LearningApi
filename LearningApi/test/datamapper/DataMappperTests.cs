

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Xunit;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using LearningFoundation.Statistics;
using LearningFoundation.Normalizers;

namespace UnitTests
{
    /// <summary>
    /// Test class for testing data mapper - transformation of the RealData in to numeric ML ready data.
    /// </summary>
    public class DataMapperTests
    {
        
        /// <summary>
        /// COnstructor which initializes all neccessery data for test
        /// </summary>
        public DataMapperTests()
        {
            
        }

        [Fact]
        public void MinMaxNormalization_test1()
        {
            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData1());

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return GetRealDataSample();
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            //
           var result =  api.Run() as double [][];

        
            //Test result for normalization
           var expected = GetTransformedNumericDataSample();

            for(int i = 0; i < expected.Length; i++)
            {
                for(int j = 0; j < expected[0].Length; j++)
                {

                    Assert.Equal(Math.Round(result[i][j], 5), expected[i][j]);
                }
                
                
            }

            //
            return;
        }

        #region Helpers
        private DataDescriptor loadMetaData1()
        {  
            var des = new DataDescriptor();
            
            des.Features = new Column[4];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.BINARY, Values = new string[] { "no", "yes" }, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.STRING, Values = null, DefaultMissingValue = 1.4 };
            des.Features[3] = new Column { Id = 1, Name = "col4", Index = 0, Type = ColumnType.CLASS, Values = new string[] {"red", "green", "blue" }, DefaultMissingValue = 1 };

            return des;
        }

        private double[][] GetNormalizedDataSample()
        {
            var data = new double[20][] {
                                            new double[]{0.667,1,1,0,0},
                                            new double[]{0.430,1,0,1,0},
                                            new double[]{0.787,0,1,0,0},
                                            new double[]{1.000,1,0,1,0},
                                            new double[]{0.900,0,0,0,1},
                                            new double[]{0.344,1,0,0,1},
                                            new double[]{0.232,1,1,0,0},
                                            new double[]{0.841,1,0,0,1},
                                            new double[]{0.676,0,0,1,0},
                                            new double[]{0.818,0,0,1,0},
                                            new double[]{0.059,1,0,0,1},
                                            new double[]{0.395,0,0,1,0},
                                            new double[]{0.616,1,1,0,0},
                                            new double[]{0.496,0,0,1,0},
                                            new double[]{0.270,0,0,0,1},
                                            new double[]{0.173,1,0,0,1},
                                            new double[]{0.000,1,0,1,0},
                                            new double[]{0.497,0,0,1,0},
                                            new double[]{0.480,0,1,0,0},
                                            new double[]{0.156,1,1,0,0},

                                         };
            //
            return data;
        }

        private double[][] GetTransformedNumericDataSample()
        {
            var data = new double[20][] {       
                                    new double[]{+1.283,1,1,0,0},
                                    new double[]{-0.843,1,0,1,0},
                                    new double[]{+2.364,0,1,0,0},
                                    new double[]{+4.279,1,0,1,0},
                                    new double[]{+3.383,0,0,0,1},
                                    new double[]{-1.624,1,0,0,1},
                                    new double[]{-2.628,1,1,0,0},
                                    new double[]{+2.847,1,0,0,1},
                                    new double[]{+1.362,0,0,1,0},
                                    new double[]{+2.640,0,0,1,0},
                                    new double[]{-4.188,1,0,0,1},
                                    new double[]{-1.161,0,0,1,0},
                                    new double[]{+0.825,1,1,0,0},
                                    new double[]{-0.253,0,0,1,0},
                                    new double[]{-2.286,0,0,0,1},
                                    new double[]{-3.162,1,0,0,1},
                                    new double[]{-4.714,1,0,1,0},
                                    new double[]{-0.242,0,0,1,0},
                                    new double[]{-0.400,0,1,0,0},
                                    new double[]{-3.315,1,1,0,0},
                                         };
            //
            return data;
        }

        private object[][] GetRealDataSample()
        {
            var data = new object[20][] {   
                            new object[] { +1.283, "yes", "This id description of the column and can be ignored.", "red" },
                            new object[] { -0.843, "yes", "This id description of the column and can be ignored.", "green" },
                            new object[] { +2.364, "no", "This id description of the column and can be ignored.", "red" },
                            new object[] { +4.279, "yes", "This id description of the column and can be ignored.", "green" },
                            new object[] { +3.383, "no", "This id description of the column and can be ignored.", "blue" },
                            new object[] { -1.624, "yes", "This id description of the column and can be ignored.", "blue" },
                            new object[] { -2.628, "yes", "This id description of the column and can be ignored.", "red" },
                            new object[] { +2.847, "yes", "This id description of the column and can be ignored.", "blue" }, 
                            new object[] { +1.362, "no", "This id description of the column and can be ignored.", "green" },
                            new object[] { +2.640, "no", "This id description of the column and can be ignored.", "green" },
                            new object[] { -4.188, "yes", "This id description of the column and can be ignored.", "blue" },
                            new object[] { -1.161, "no", "This id description of the column and can be ignored.", "green" },
                            new object[] { +0.825, "yes", "This id description of the column and can be ignored.", "red" },
                            new object[] { -0.253, "no", "This id description of the column and can be ignored.", "green" },
                            new object[] { -2.286, "no", "This id description of the column and can be ignored.", "blue" },
                            new object[] { -3.162, "yes", "This id description of the column and can be ignored.", "blue" },
                            new object[] { -4.714, "yes", "This id description of the column and can be ignored.", "green" },
                            new object[] { -0.242, "no", "This id description of the column and can be ignored.", "green" },
                            new object[] { -0.400, "no", "This id description of the column and can be ignored.", "red" },
                            new object[] { -3.315, "yes", "This id description of the column and can be ignored.", "red" }
                            };

            //
            return data;
        }

        #endregion

    }
}
