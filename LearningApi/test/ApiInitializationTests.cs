using LearningFoundation;
using LearningFoundation.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeuronalNet.BackPropagation;
using Xunit;
using System.IO;

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
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris_multi_class_data.csv");
            
            LearningApi api = new LearningApi();
            api.UseBackPropagation(1, 0.2, 1.0, null);
            api.UseCsvDataProvider("", ',', 0);
            
            //api.AddBlobStorageDataSourceProvider();

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
