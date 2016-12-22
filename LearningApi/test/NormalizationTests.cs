
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Xunit;
using System.IO;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using LearningFoundation.Statistics;
using LearningFoundation.Normalizers;
using test;

namespace UnitTests
{
    /// <summary>
    /// Test class for testing Normalizers in LearningAPI.
    /// The test contains two test methods for normalization: one for MinMax and One for Gaus (Stanardized) normalizer
    /// The test contains two test methods for denormalization: one for MinMax and One for Gaus (Stanardized) normalizer 
    /// </summary>
    public class NormalizationTests
    {
        //helper for test run
        LearningApi m_Api;
       // Mean[] m_Stats;//basic statistics of the iris data

        //file path for data and results data
        string m_irisNumericDataFilePath;
        string m_irisMinMaxNormalizedDataFilePath;
        string m_irisGaussNormalizedDataFilePath;

        /// <summary>
        /// COnstructor which initializes all neccessery data for test
        /// </summary>
        public NormalizationTests()
        {
            
            //iris data file paths
            var irisRealDataFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris.csv");
            m_irisNumericDataFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_numeric.csv");
            m_irisMinMaxNormalizedDataFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_minmax_normalized.csv");
            m_irisGaussNormalizedDataFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_gauss_normalized.csv");
            //iris mapper
            var irisMapperFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_mapper.json");

            m_Api = new LearningApi(TestHelpers.GetDescriptor());
            
            //create datamapper 
            m_Api.UseDefaultDataMapper();
          
            //create dataprovider
            m_Api.UseCsvDataProvider(irisRealDataFilePath, ',', 1);
        }


        //Tests MinMax noramlizer 
        // 1. Loads the real data
        // 2. Transforms the data in to numeric form
        // 3. Checck if the numeric data is corectly converted by comparing with the data which is alredy pre-transformed.
        // 4. Transform the data in to normalized form
        // 5. Check if the normalized data is corectly calculated by comparing with the data which is alredy pre-normalized.
        [Fact]
        public bool NormalizeData_With_MinMax_Normalized_Test()
        {
            //TODO: We need to make this test simpler. It should test normalization only without of need to 
            // use data source provider sand data mapper.
            throw new NotImplementedException("TODO");
            /*
            var minmax = new MinMaxNormalizer(m_Api.DataMapper as DataMapper, m_Stats.Select(x=>x.Min).ToArray(), m_Stats.Select(x => x.Max).ToArray());

            //numeric data
            var numeric = CsvDataProviderExtensions.LoadDataFromFile(m_irisNumericDataFilePath, ',');
            var normalized = CsvDataProviderExtensions.LoadDataFromFile(m_irisMinMaxNormalizedDataFilePath, ',');

            //
            int numOfFeatures = m_Api.DataMapper.NumOfFeatures;
            int labelIndx = m_Api.DataMapper.LabelIndex;
            int index = 1;//first row is header
            do
            {
                var rawData = m_Api.DataProvider.Current;

                if (rawData != null)
                {
                    //transform read data in to numeric data 
                    // numeric data stay the same
                    //(binary data are transformed in to 0 and 1)
                    // (Category data transform in to class number, R,G,B in to 0,1,2)
                    object[] data = m_Api.DataMapper.RunAsync(rawData);

                    //test corectness of mapped data
                    var numericRow = numeric.ElementAt(index);
                    for (int i = 0; i < numericRow.Length; i++)
                    {
                        Assert.True(double.Parse(numericRow[i].ToString()) == Convert.ToDouble(data[i]), "Inconsistent data while transform real in to numeric data");
                    }


                    double[] featureVector = new double[numOfFeatures];
                    for (int i = 0; i < numOfFeatures; i++)
                    {
                        featureVector[i] = (double)data[m_Api.DataMapper.GetFeatureIndex(i)];
                    }



                    //transform the numeric data in to normalized data based on MinMax normalizer
                    // numeric data are transformed in to numeric data in range of 0-1
                    //binary data stay the same 0 and 1
                    // Category data are transformed in to 1-N binary number. In case of R,G,B classes 
                    // R normalized value id (1,0,0)
                    // G normalized in to (0,1,0)
                    // B normalized int to (0,0,1)
                    var normFeatureVector = minmax.Normalize(featureVector);
                    //test corectness of normalized data
                    var normRow = normalized.ElementAt(index);
                    for (int i = 0; i < normFeatureVector.Length; i++)
                    {
                        var testValue = double.Parse(normRow[i].ToString());
                        var normValue = double.Parse(normFeatureVector[i].ToString());

                        Assert.True(Math.Round(normValue, 9) == testValue, "Inconsistent data while normalization data");

                    }
                } 
                else
                    break;//if the next item is null, we reached the end of the list

                index++;
            } while (m_Api.DataProvider.MoveNext());
            */
            return true;
        
        }

        //Tests MinMax noramlizer 
        // 1. Loads the real data
        // 2. Transforms the data in to numeric form
        // 3. Checck if the numeric data is corectly converted by comparing with the data which is alredy pre-transformed.
        // 4. Transform the data in to normalized form
        // 5. Check if the normalizeddata is corectly calculated by comparing with the data which is alredy pre-normalized.
        [Fact]
        public bool NormalizeData_With_Gauss_Test()
        {
            //TODO: We need to make this test simpler. It should test normalization only without of need to 
            // use data source provider sand data mapper.
            throw new NotImplementedException("TODO");

            /*
            var gaus = new GaussNormalizer(m_Api.DataMapper as DataMapper,m_Stats.Select(x => x.Mean).ToArray(), m_Stats.Select(x => x.Variance).ToArray());
            

            //numeric data
            var numeric = CsvDataProviderExtensions.LoadDataFromFile(m_irisNumericDataFilePath, ',');
            var normalized = CsvDataProviderExtensions.LoadDataFromFile(m_irisGaussNormalizedDataFilePath, ',');

            
            //
            int numOfFeatures = m_Api.DataMapper.NumOfFeatures;
            int labelIndx = m_Api.DataMapper.LabelIndex;
            int index = 1;//first row is header

            ///
            do
            {
                var rawData = m_Api.DataProvider.Current;

                if (rawData != null)
                {
                    //transform read data in to numeric data 
                    // numeric data stay the same
                    //(binary data are transformed in to 0 and 1)
                    // (Category data transform in to class number, R,G,B in to 0,1,2)
                    object[] data = m_Api.DataMapper.RunAsync(rawData);
                    
                    //test corectness of mapped data
                    var numericRow = numeric.ElementAt(index);
                    for(int i=0; i<numericRow.Length; i++)
                    {
                        Assert.True(double.Parse(numericRow[i].ToString()) == Convert.ToDouble(data[i]), "Inconsistent data while transform real in to numeric data");
                    }

                    //
                    double[] featureVector = new double[numOfFeatures];
                    for (int i = 0; i < numOfFeatures; i++)
                    {
                        featureVector[i] = (double)data[m_Api.DataMapper.GetFeatureIndex(i)];
                    }



                    //transform the numeric data in to normalized data based on MinMax normalizer
                    // numeric data are transformed in to numeric data in range of 0-1
                    //binary data stay the same 0 and 1
                    // Category data are transformed in to 1-N binary number. In case of R,G,B classes 
                    // R normalized value id (1,0,0)
                    // G normalized in to (0,1,0)
                    // B normalized int to (0,0,1)
                    var normFeatureVector = gaus.Normalize(featureVector);

                    //test corectness of normalized data
                    var normRow = normalized.ElementAt(index);
                    for (int i = 0; i < normFeatureVector.Length; i++)
                    {
                        var testValue = double.Parse(normRow[i].ToString());
                        var normValue = double.Parse(normFeatureVector[i].ToString());

                        Assert.True(Math.Round(testValue/normValue,6) == 1, "Inconsistent data while normalization data");

                    }

                }
                else
                    break;//if the next item is null, we reached the end of the list

                index++;

            } while (m_Api.DataProvider.MoveNext());
            */
            return true;
        }

        [Fact]
        public void NormalizeData_With_MinMax_Test()
        {
            // Creates learning api object
            LearningApi api = new LearningApi(loadMetaData1());

            //Real dataset must be defined as object type, because data can be numeric, binary and classification
            api.UseActionModule<object[][], object[][]>((input, ctx) =>
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
            });

            //this call must be first in the pipeline
            api.UseDefaultDataMapper();

            //
            api.UseMinMaxNormalizer();

            //
             api.Run();

            //
            return;
        }

        [Fact]
        public bool DeNormalizeData_With_Gauss_Normalized_Test()
        {

            return false;
        }


        private DataDescriptor loadMetaData1()
        {  
            var des = new DataDescriptor();
            
            des.Features = new Column[3];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 4.2 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 1.4 };
           
            return des;
        }

        //private DataDescriptor loadMetaData1()
        //{
        //    var des = new DataDescriptor();
        //    des.Features = new Column[4];
        //    des.Features[0] = new Column { Id = 1, Name = "sepal_length", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
        //    des.Features[1] = new Column { Id = 2, Name = "sepal_width", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 4.2 };
        //    des.Features[2] = new Column { Id = 3, Name = "petal_length", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 1.4 };
        //    des.Features[3] = new Column { Id = 4, Name = "petal_width", Index = 3, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.5 };
        //    des.Features[4] = new Column { Id = 5, Name = "species", Index = 4, Type = ColumnType.CLASS, Values = null, DefaultMissingValue = 1 };

        //    des.

        //    return des;
        //}


    }
}
