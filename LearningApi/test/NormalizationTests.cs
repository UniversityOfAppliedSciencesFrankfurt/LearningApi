using LearningFoundation;
using LearningFoundation.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeuronalNet.BackPropagation;
using Xunit;
using System.IO;
using LearningFoundation.DataMappers;
using Newtonsoft.Json;
using LearningFoundation.DataNormalizers;

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
        LearningApi m_api;
        IStatistics[] m_stats;//basic statistics of the columns data

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
            var irisMapperFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_mapper.txt");

            m_api = new LearningApi();
            //create datamapper 
            m_api.DataMapper = DataMapper.Load(irisMapperFilePath);
            //create dataprovider
            m_api.UseCsvDataProvider(irisRealDataFilePath, ',', 1);

            //create statistics of the data
            m_stats = DataMapper.CalculateStatistics(m_api.DataProvider, m_api.DataMapper);
            m_api.DataMapper.Statistics = m_stats;

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
            var minmax = new MinMaxNormalizer(m_api.DataMapper as DataMapper);

            //numeric data
            var numeric = CsvDataProviderExtensions.LoadDataFromFile(m_irisNumericDataFilePath, ',');
            var normalized = CsvDataProviderExtensions.LoadDataFromFile(m_irisMinMaxNormalizedDataFilePath, ',');

            //
            int numOfFeatures = m_api.DataMapper.NumOfFeatures;
            int labelIndx = m_api.DataMapper.LabelIndex;
            int index = 1;//first row is header
            do
            {
                var rawData = m_api.DataProvider.Current;

                if (rawData != null)
                {
                    //transform read data in to numeric data 
                    // numeric data stay the same
                    //(binary data are transformed in to 0 and 1)
                    // (Category data transform in to class number, R,G,B in to 0,1,2)
                    double[] data = m_api.DataMapper.MapInputVector(rawData);

                    //test corectness of mapped data
                    var numericRow = numeric.ElementAt(index);
                    for (int i = 0; i < numericRow.Length; i++)
                    {
                        Assert.True(double.Parse(numericRow[i].ToString()) == data[i], "Inconsistent data while transform real in to numeric data");
                    }


                    double[] featureVector = new double[numOfFeatures];
                    for (int i = 0; i < numOfFeatures; i++)
                    {
                        featureVector[i] = data[m_api.DataMapper.GetFeatureIndex(i)];
                    }



                    //transform the numeric data in to normalized data based on MinMax normalizer
                    // numeric data are transformed in to numeric data in range of 0-1
                    //binary data stay the same 0 and 1
                    // Category data are transformed in to 1-N binary number. In case of R,G,B classes 
                    // R normalized value id (1,0,0)
                    // G normalized in to (0,1,0)
                    // B normalized int to (0,0,1)
                    var normFeatureVector = minmax.Normalize(m_api.DataMapper.Statistics,featureVector);
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
            } while (m_api.DataProvider.MoveNext());

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
            var gaus = new GaussNormalizer(m_api.DataMapper as DataMapper);
            

            //numeric data
            var numeric = CsvDataProviderExtensions.LoadDataFromFile(m_irisNumericDataFilePath, ',');
            var normalized = CsvDataProviderExtensions.LoadDataFromFile(m_irisGaussNormalizedDataFilePath, ',');

            
            //
            int numOfFeatures = m_api.DataMapper.NumOfFeatures;
            int labelIndx = m_api.DataMapper.LabelIndex;
            int index = 1;//first row is header

            ///
            do
            {
                var rawData = m_api.DataProvider.Current;

                if (rawData != null)
                {
                    //transform read data in to numeric data 
                    // numeric data stay the same
                    //(binary data are transformed in to 0 and 1)
                    // (Category data transform in to class number, R,G,B in to 0,1,2)
                    double[] data = m_api.DataMapper.MapInputVector(rawData);
                    
                    //test corectness of mapped data
                    var numericRow = numeric.ElementAt(index);
                    for(int i=0; i<numericRow.Length; i++)
                    {
                        Assert.True(double.Parse(numericRow[i].ToString()) == data[i], "Inconsistent data while transform real in to numeric data");
                    }

                    //
                    double[] featureVector = new double[numOfFeatures];
                    for (int i = 0; i < numOfFeatures; i++)
                    {
                        featureVector[i] = data[m_api.DataMapper.GetFeatureIndex(i)];
                    }



                    //transform the numeric data in to normalized data based on MinMax normalizer
                    // numeric data are transformed in to numeric data in range of 0-1
                    //binary data stay the same 0 and 1
                    // Category data are transformed in to 1-N binary number. In case of R,G,B classes 
                    // R normalized value id (1,0,0)
                    // G normalized in to (0,1,0)
                    // B normalized int to (0,0,1)
                    var normFeatureVector = gaus.Normalize(m_api.DataMapper.Statistics, featureVector);

                    //test corectness of normalized data
                    var normRow = normalized.ElementAt(index);
                    for (int i = 0; i < numericRow.Length; i++)
                    {
                        var testValue = double.Parse(normRow[i].ToString());
                        var normValue = double.Parse(normFeatureVector[i].ToString());

                        Assert.True(Math.Round(normValue,9) == testValue, "Inconsistent data while normalization data");

                    }

                }
                else
                    break;//if the next item is null, we reached the end of the list

                index++;

            } while (m_api.DataProvider.MoveNext());

            return true;
        }

        [Fact]
        public bool DeNormalizeData_With_MinMax_Test()
        {

            return false;
        }

        [Fact]
        public bool DeNormalizeData_With_Gauss_Normalized_Test()
        {

            return false;
        }

    }
}
