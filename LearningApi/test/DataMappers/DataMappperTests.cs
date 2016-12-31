

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

namespace test.datamapper
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
        public void NumericTransformation_test1()
        {
            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData_with_CategoricLabel());

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample();
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            //
           var result =  api.Run() as double [][];

        
            //Test result for normalization
           var expected = transformedData_with_CategoricLabel();

            for(int i = 0; i < expected.Length; i++)
            {
                for(int j = 0; j < expected[0].Length; j++)
                {

                    Assert.Equal(result[i][j], expected[i][j]);
                }
                
                
            }

            //
            return;
        }

        [Fact]
        public void NumericTransformation_test2()
        {
            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData_with_CategoricFeature());

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample();
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            //
            var result = api.Run() as double[][];


            //Test result for normalization
            var expected = transformedData_with_CategoricFeature();

            for (int i = 0; i < expected.Length; i++)
            {
                for (int j = 0; j < expected[0].Length; j++)
                {

                    Assert.Equal(result[i][j], expected[i][j]);
                }
            }

            //
            return;
        }

        //feature count when category column is feature  
        [Fact]
        public void Feature_and_LabelIndex_Mapping_Test2()
        {
            var desc = loadMetaData_with_CategoricFeature();
            // Creates learning api object
            LearningApi api = new LearningApi(desc);

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample();
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            //
            var result = api.Run() as double[][];

            var featureNumber = result[0].Count() - 1;//- labelcolumn

            //string column is not counted because it is ignored column
            //there is one category column in features so the number of features is increased with (calssCount-1)
            // featureCount = columnCount- strinCoulumnCount-LabelColumn + calssCount-1
            var featureCount = 4 - 1 - 1 + (3 - 1);
            Assert.Equal(featureNumber, featureCount);

        }
        //feature count when category column is a label and noone feature is class column.
        [Fact]
        public void Feature_and_LabelIndex_Mapping_Test1()
        {
            var desc = loadMetaData_with_CategoricLabel();
            // Creates learning api object
            LearningApi api = new LearningApi(desc);

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
            {
                return loadRealDataSample();
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            //
            var result = api.Run() as double[][];

            var featureNumber = result[0].Count() - 1;//- labelcolumn

            //string column is not counted because it is ignored column
            //there is no category column in features so the number of features is 
            //  featureCount = columnCount-IngoredCoulmnCount-1 labelcolumn
            var featureCount = 4-1-1 ;
            Assert.Equal(featureNumber, featureCount);

        }

        #region Sample data with Categorical Feature
        private DataDescriptor loadMetaData_with_CategoricLabel()
        {
            var des = new DataDescriptor();

            des.Features = new Column[4];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.BINARY, Values = new string[] { "no", "yes" }, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.STRING, Values = null, DefaultMissingValue = 1.4 };
            des.Features[3] = new Column { Id = 4, Name = "col4", Index = 3, Type = ColumnType.CLASS, Values = new string[] { "red", "green", "blue" }, DefaultMissingValue = 1 };

            des.LabelIndex = 3;
            return des;
        }
       
        private double[][] transformedData_with_CategoricFeature()
        {
            var data = new double[20][] {
                                    new double[]{+1.283,1,0,0,1},
                                    new double[]{-0.843,0,1,0,1},
                                    new double[]{+2.364,1,0,0,0},
                                    new double[]{+4.279,0,1,0,1},
                                    new double[]{+3.383,0,0,1,0},
                                    new double[]{-1.624,0,0,1,1},
                                    new double[]{-2.628,1,0,0,1},
                                    new double[]{+2.847,0,0,1,1},
                                    new double[]{+1.362,0,1,0,0},
                                    new double[]{+2.640,0,1,0,0},
                                    new double[]{-4.188,0,0,1,1},
                                    new double[]{-1.161,0,1,0,0},
                                    new double[]{+0.825,1,0,0,1},
                                    new double[]{-0.253,0,1,0,0},
                                    new double[]{-2.286,0,0,1,0},
                                    new double[]{-3.162,0,0,1,1},
                                    new double[]{-4.714,0,1,0,1},
                                    new double[]{-0.242,0,1,0,0},
                                    new double[]{-0.400,1,0,0,0},
                                    new double[]{-3.315,1,0,0,1},
                                         };
            //
            return data;
        }
        #endregion


        #region Sample data with Categorical Label
        private DataDescriptor loadMetaData_with_CategoricFeature()
        {  
            var des = new DataDescriptor();
            
            des.Features = new Column[4];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.BINARY, Values = new string[] { "no", "yes" }, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.STRING, Values = null, DefaultMissingValue = 1.4 };
            des.Features[3] = new Column { Id = 4, Name = "col4", Index = 3, Type = ColumnType.CLASS, Values = new string[] {"red", "green", "blue" }, DefaultMissingValue = 1 };

            des.LabelIndex = 1;
            return des;
        }

        private double[][] transformedData_with_CategoricLabel()
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
        #endregion


        #region Real Data Sample
        /// <summary>
        /// return real data set
        /// </summary>
        /// <returns></returns>
        private object[][] loadRealDataSample()
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
