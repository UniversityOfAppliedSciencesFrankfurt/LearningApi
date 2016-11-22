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
    public class NormalizationTests
    {
        //init mapper for data 
        DataMapper dm;
        CsvDataProvider dp;

        public NormalizationTests()
        {
            //mapper initialization
            var irisMapperFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_mapper.txt");
            //iris data file
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris.csv");

            //create datamapper 
            dm = DataMapper.Load(irisMapperFilePath);
            LearningApi api = new LearningApi();
            api.UseCsvDataProvider(path, ',', 1);
            dp=api.DataProvider as CsvDataProvider;

        }
        [Fact]
        public bool NormalizeNumericData_MinMax_Test()
        {
            var minmax = new MinMaxNormalizer(dm);
            int numOfFeatures = dm.NumOfFeatures;
            int labelIndx = dm.LabelIndex;
            while (dp.MoveNext())
            {
                var rawData = dp.Current;

                if (rawData != null)
                {
                    double[] data = dm.MapInputVector(rawData);

                    double[] featureVector = new double[numOfFeatures];

                    for (int i = 0; i < numOfFeatures; i++)
                    {
                        featureVector[i] = data[dm.GetFeatureIndex(i)];
                    }

                    var normFeatureVector = minmax.Normalize(featureVector);

                }
                else
                    break;//if the next item is null, we reached the end of the list
            }

            return true;
        }

        [Fact]
        public bool NormalizeNumericData_Gauss_Test()
        {
            var gauss = new GaussNormalizer(dm);

            return true;
        }

        [Fact]
        public bool NormalizeCategoryData_Test()
        {

            return true;
        }


        public bool DeNormalizeNumericData_MinMax_Test()
        {

            return true;
        }

        [Fact]
        public bool DeNormalizeNumericData_Gauss_Test()
        {

            return true;
        }

        [Fact]
        public bool DeNormalizeCategoryData_Test()
        {

            return true;
        }
    }
}
