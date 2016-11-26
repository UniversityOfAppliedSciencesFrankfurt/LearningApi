using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using System.IO;
using Newtonsoft.Json;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using LearningFoundation.Normalizers;
using LearningFoundation.Statistics;
using NeuralNet.BackPropagation;

namespace UnitTests
{
    public class ApiInitializationTests
    {
        string m_iris_mapper_path;
        string m_iris_data_path;
        BasicStatistics[] m_stats;//basic statistics of the iris data
        public ApiInitializationTests()
        {
            //create stat for IRIS data
            m_stats = new BasicStatistics[5]
            {
                new BasicStatistics(1, 4.3, 7.9, 5.84333333333, 0.681122222),
                new BasicStatistics(2, 2.0, 4.4, 3.05733333333, 0.188712889),
                new BasicStatistics(3, 1.0, 6.9, 3.75800000000, 3.095502667),
                new BasicStatistics(4, 0.1, 2.5, 1.19933333333, 0.577132889),
                new BasicStatistics(5, 0, 0, 0, 0),
            };

            //mapper initialization
            m_iris_mapper_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris_mapper.json");

            //iris data file
            m_iris_data_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"sample_data\iris\iris.csv");

        }
        [Fact]
        public bool InitNeuralBackPropagationTest()
        {
          //  InitIrisMapperInJsonFormat_helper();

            //creates learning api object
            LearningApi api = new LearningApi();

            
            //prepares the ML Algoritm and setup parameters
            api.UseBackPropagation(1, 0.2, 1.0, null);


            //connect to data file for streaming the data
            api.UseCsvDataProvider(m_iris_data_path, ',', 1);


            //use mapper for data 
            api.UseDefaultDataMapper(m_iris_mapper_path);

            //provide basic data statistic
            //api.UseBasicDataStatistics(new BasicStatistics());

            //use MinMax data normalizer
            api.UseMinMaxNormalizer(m_stats.Select(x => x.Min).ToArray(), m_stats.Select(x => x.Max).ToArray());

            //use Gaus data normalizer
            //api.UseGaussNormalizer(m_stats.Select(x => x.Mean).ToArray(), m_stats.Select(x => x.Variance).ToArray());


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

            dm.Features = new Column[4];
            dm.Features[0] = new Column { Id = 1, Name = "sepal_length", Index = 0,  Type =  ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
            dm.Features[1] = new Column { Id = 2, Name = "sepal_width", Index = 1,  Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 4.2 };
            dm.Features[2] = new Column { Id = 3, Name = "petal_length", Index = 2,  Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 1.4 };
            dm.Features[3] = new Column { Id = 4, Name = "petal_width", Index = 3,  Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.5 };
            dm.Features[4] = new Column { Id = 5, Name = "species", Index = 4,  Type = ColumnType.CLASS, Values = null, DefaultMissingValue = 1 };


            var jsonString = JsonConvert.SerializeObject(dm);
            return jsonString;
        }
    }
}
