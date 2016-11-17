using LearningFoundation;
using LearningFoundation.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeuronalNet.BackPropagation;


namespace UnitTests
{
    public class ApiInitializationTests
    {
        public ApiInitializationTests()
        {

        }

        public void InitNeuralBackPropagationTest()
        {
            LearningApi api = new LearningApi();
            api.UseBackPropagation(1, 0.2, 1.0, null);
            api.UseCsvDataProvider("", ',', 0);
            
            //api.AddBlobStorageDataSourceProvider();

            api.TrainAsync().Wait();

            //  api.Train();
            //   api.TrainSample();

            IScore status = api.GetScore();

            //api.Train(vector)

        }

        public void LoadModelNeuralBackPropagationTest()
        {
            //LearningApi api = new LearningApi.FromModel();
            //api.LoadData();

        }
    }
}
