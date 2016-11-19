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

namespace UnitTests
{
    public class ApiInitializationTests
    {
        public ApiInitializationTests()
        {

        }
        [Fact]
        public bool InitNeuralBackPropagationTest()
        {
            
            //mapper initialization
            var irisMapperFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_mapper.txt");
            var irisMapper = DataMapper.Load(irisMapperFilePath);
            //iris data file
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris.csv");

            //creates learning api object
            LearningApi api = new LearningApi();
            //prepares the ML Algoritm
            api.UseBackPropagation(1, 0.2, 1.0, null);
            
            //connect to data file for streaming the data
            api.UseCsvDataProvider(path, ',', 1);
            
            //api.AddBlobStorageDataSourceProvider();

            //start process of learning
            api.TrainAsync().Wait();

            //  api.Train();
            //   api.TrainSample();

            IScore status = api.GetScore();

            //api.Train(vector)
            return true;
        }

        

        public void LoadModelNeuralBackPropagationTest()
        {
            //LearningApi api = new LearningApi.FromModel();
            //api.LoadData();

        }
    }
}
