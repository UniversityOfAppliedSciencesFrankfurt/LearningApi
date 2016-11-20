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
    public class ApiInitializationTests
    {
        public ApiInitializationTests()
        {

        }
        [Fact]
        public bool InitNeuralBackPropagationTest()
        {
          //  InitIrisMapperInJsonFormat_helper();
            //mapper initialization
            var irisMapperFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_mapper.txt");
            //iris data file
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris.csv");

            //creates learning api object
            LearningApi api = new LearningApi();
            //prepares the ML Algoritm
            api.UseBackPropagation(1, 0.2, 1.0, null);

            //init mapper for data 
            var dm = DataMapper.Load(irisMapperFilePath);

            //setup normalizer to prepare data for normalization
            api.Normilizer = new DataNormalizer(dm); 

            //connect to data file for streaming the data
            api.UseCsvDataProvider(path, ',', 1);

            //api.AddBlobStorageDataSourceProvider();

            //
            api.DataMapper = dm;
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


        public string InitIrisMapperInJsonFormat_helper()
        {

            var dm = new DataMapper();

            //dm.m_Features = new Column[4];
            //dm.m_Features[0] = new Column { Id = 1, Name = "sepal_length", Index = 0, Normalization = 1, Type = 1, Values = null, DefaultMissingValue = 5.5 };
            //dm.m_Features[1] = new Column { Id = 2, Name = "sepal_width",  Index = 1, Normalization = 1, Type = 1, Values = null, DefaultMissingValue = 4.2 };
            //dm.m_Features[2] = new Column { Id = 3, Name = "petal_length", Index = 2, Normalization = 1, Type = 1, Values = null, DefaultMissingValue = 1.4 };
            //dm.m_Features[3] = new Column { Id = 4, Name = "petal_width",  Index = 3, Normalization = 1, Type = 1, Values = null, DefaultMissingValue = 0.5 };

            
            var jsonString = JsonConvert.SerializeObject(dm);
            return jsonString;
        }
    }
}
