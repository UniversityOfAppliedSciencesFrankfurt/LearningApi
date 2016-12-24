

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

namespace test.scalers
{
    /// <summary>
    /// Test class for testing Normalizers in LearningAPI.
    /// </summary>
    public class NormalizationTests
    {
        /// <summary>
        /// COnstructor which initializes all neccessery data for test
        /// </summary>
        public NormalizationTests()
        {
            
        }

  
        //Tests MinMax denoramlizer 
        // 1. Loads the real data
        // 2. Transforms the data in to numeric form
        // 3. Checck if the numeric data is corectly converted by comparing with the data which is alredy pre-transformed.
        // 4. Transform the data in to normalized form
        // 5. Check if the normalized data is corectly calculated by comparing with the data which is alredy pre-normalized.
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
            api.UseMinMaxNormalizer();

            //
           var result =  api.Run() as double [][];

        
            //Test result for normalization
           var expected = GetNormalizedDataSample();

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

        //Tests MinMax noramlizer 
        // 1. Loads the real data
        // 2. Transforms the data in to numeric form
        // 3. Checck if the numeric data is corectly converted by comparing with the data which is alredy pre-transformed.
        // 4. Transform the data in to normalized form
        // 5. Transform data in to denormalized form (numeric form)
        // 6. Check if the normalized data is corectly calculated by comparing with the data which is alredy pre-transformed.
        [Fact]
        public void MinMaxDeNormalization_test1()
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
            api.UseMinMaxNormalizer();

            //use denormalizer on normalized data
            api.UseMinMaxDeNormalizer();

            //
            var result = api.Run() as double[][];


            //Test result for normalization
            var expected = GetTransformedNumericDataSample();

            for (int i = 0; i < expected.Length; i++)
            {
                for (int j = 0; j < expected[0].Length; j++)
                {

                    Assert.Equal(Math.Round(result[i][j],4), expected[i][j]);
                }

            }

            //
            return;
        }


        #region Helpers
        private DataDescriptor loadMetaData1()
        {  
            var des = new DataDescriptor();
            
            des.Features = new Column[3];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 4.2 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 1.4 };
           
            return des;
        }

        private double[][] GetNormalizedDataSample()
        {
            var data = new double[20][] {
                            new double[] {0.70830,0.46035,0.02506},
                            new double[] {0.90269,0.17596,0.98948},
                            new double[] {0.84380,0.00000,0.74926},
                            new double[] {0.09911,0.15149,0.14045},
                            new double[] {0.14094,1.00000,0.47930},
                            new double[] {0.29494,0.15908,0.97459},
                            new double[] {0.00000,0.13423,0.32963},
                            new double[] {0.94677,0.92789,0.50154},
                            new double[] {0.43342,0.84938,0.36993},
                            new double[] {0.59274,0.38423,1.00000},
                            new double[] {0.20954,0.22981,0.02335},
                            new double[] {1.00000,0.97624,0.77512},
                            new double[] {0.88538,0.34722,0.79811},
                            new double[] {0.25485,0.06008,0.28850},
                            new double[] {0.38789,0.56458,0.31134},
                            new double[] {0.76089,0.03685,0.44707},
                            new double[] {0.56843,0.80946,0.87389},
                            new double[] {0.50096,0.62136,0.00000},
                            new double[] {0.57480,0.48569,0.44144},
                            new double[] {0.82428,0.50246,0.09700}
            };

            return data;
        }

        private double[][] GetTransformedNumericDataSample()
        {
            var data = new double[20][] {       new double[] {4.1220,-0.6311,-8.9094 },
                                                new double[] { 7.9604,-6.1778,9.7382 },
                                                new double[] { 6.7976,-9.6096,5.0934},
                                                new double[] { -7.9072,-6.6549,-6.6783},
                                                new double[] { -7.0811,9.8941,-0.1264 },
                                                new double[] { -4.0402,-6.5070,9.4502},
                                                new double[] { -9.8642,-6.9916,-3.0204},
                                                new double[] { 8.8309,8.4876,0.3037},
                                                new double[] { -1.3059,6.9565,-2.2412},
                                                new double[] { 1.8402,-2.1157,9.9416},
                                                new double[] { -5.7267,-5.1274,-8.9425 },
                                                new double[] { 9.8819,9.4306,5.5934 },
                                                new double[] { 7.6187,-2.8375,6.0380},
                                                new double[] { -4.8319,-8.4378,-3.8157},
                                                new double[] {-2.2049,1.4018,-3.3739 },
                                                new double[] { 5.1604,-8.8909,-0.7496},
                                                new double[] { 1.3601,6.1779,7.5032},
                                                new double[] { 0.0278,2.5092,-9.3939},
                                                new double[] { 1.4859,-0.1368,-0.8585},
                                                new double[] { 6.4121,0.1903,-7.5184},
                                            };
            //
            return data;
        }

        private object[][] GetRealDataSample()
        {
            var data = new object[20][] {   new object[] {4.1220,-0.6311,-8.9094 },
                                                new object[] { 7.9604,-6.1778,9.7382 },
                                                new object[] { 6.7976,-9.6096,5.0934},
                                                new object[] { -7.9072,-6.6549,-6.6783},
                                                new object[] { -7.0811,9.8941,-0.1264 },
                                                new object[] { -4.0402,-6.5070,9.4502},
                                                new object[] { -9.8642,-6.9916,-3.0204},
                                                new object[] { 8.8309,8.4876,0.3037},
                                                new object[] { -1.3059,6.9565,-2.2412},
                                                new object[] { 1.8402,-2.1157,9.9416},
                                                new object[] { -5.7267,-5.1274,-8.9425 },
                                                new object[] { 9.8819,9.4306,5.5934 },
                                                new object[] { 7.6187,-2.8375,6.0380},
                                                new object[] { -4.8319,-8.4378,-3.8157},
                                                new object[] {-2.2049,1.4018,-3.3739 },
                                                new object[] { 5.1604,-8.8909,-0.7496},
                                                new object[] { 1.3601,6.1779,7.5032},
                                                new object[] { 0.0278,2.5092,-9.3939},
                                                new object[] { 1.4859,-0.1368,-0.8585},
                                                new object[] { 6.4121,0.1903,-7.5184},
                                            };
            //
            return data;
        }

        #endregion

    }
}
