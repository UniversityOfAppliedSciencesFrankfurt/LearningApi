using test;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        string m_iris_data_path;

      
        public ApiInitializationTests()
        {
            //iris data file
            m_iris_data_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"SampleData\iris\iris.csv");
        }

        /// <summary>
        /// Demonstrates how to inject a data provider as an action.
        /// </summary>
        [Fact]
        public void ActionModuleTest()
        {
            LearningApi api = new LearningApi(null);
            api.UseActionModule<double[], double[]>((input, ctx) =>
            {
                return new double[] { 1.1, 2.2, 3.3, 4.4 };
            });

            var result = api.Run();

            Assert.Equal(1.1, ((double[])result)[0]);
            Assert.Equal(4.4, ((double[])result)[3]);
        }


        /// <summary>
        /// Demonstrates ho to setup a chain of action modules.
        /// </summary>
        [Fact]
        public void ActionModuleChainTest()
        {
            LearningApi api = new LearningApi(null);
            api.UseActionModule<double[], double[]>((input, ctx) =>
            {
                // This module provides some data.
                return new double[] { 1.1, 2.2, 3.3, 4.4 };
            });

            api.UseActionModule<double[], double[]>((input, ctx) =>
            {
                // This module manipulate the data.
                return new double[] { input[0] + 1, input[1] + 1, input[2] + 1, input[3] + 1 };
            });

            var result = api.Run();

            Assert.Equal(2.1, ((double[])result)[0]);
            Assert.Equal(5.4, ((double[])result)[3]);
        }


        /// <summary>
        /// Runs BackPropagation algorithm.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public bool InitNeuralBackPropagationTest()
        {
            //  InitIrisMapperInJsonFormat_helper();

            // Creates learning api object
            LearningApi api = new LearningApi(TestHelpers.GetDescriptor());

            // Initialize data provider
            api.UseCsvDataProvider(m_iris_data_path, ',', 1);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            // Use MinMax data normalizer
            //api.UseMinMaxNormalizer();

            // We could also use some other normalizer like Gaus data normalizer
            //api.UseGaussNormalizer(m_stats.Select(x => x.Mean).ToArray(), m_stats.Select(x => x.StDev).ToArray());

            // Prepares the ML Algoritm and setup parameters
            api.UseBackPropagation(1, 0.2, 1.0, null);

            api.Run();

            IScore status = api.GetScore();

            //api.Train(vector)
            return true;
        }

        [Fact]
        public void RunPipelineTest()
        {
            // Creates learning api object
            LearningApi api = new LearningApi(TestHelpers.GetDescriptor());

            // Initialize data provider
            api.UseCsvDataProvider(m_iris_data_path, ',', 1);

            // Use mapper for data, which will extract (map) required columns 
            api.UseDefaultDataMapper();

            // Use MinMax data normalizer
            //api.UseMinMaxNormalizer(m_stats.Select(x => x.Min).ToArray(), m_stats.Select(x => x.Max).ToArray());

            // We could also use some other normalizer like Gaus data normalizer
            //api.UseGaussNormalizer(m_stats.Select(x => x.Mean).ToArray(), m_stats.Select(x => x.StDev).ToArray());

            // Prepares the ML Algoritm and setup parameters
            api.UseBackPropagation(1, 0.2, 1.0, null);

            //start process of learning
            api.Run();

            //  api.Train();
            //   api.TrainSample();

            IScore status = api.GetScore();

            //api.Train(vector)
            return;
        }


        public void LoadModelNeuralBackPropagationTest()
        {
            //LearningApi api = new LearningApi.FromModel();
            //api.LoadData();

        }

        public string InitIrisMapperInJsonFormat_helper()
        {
            var descriptor = TestHelpers.GetDescriptor();

            var dm = new DataMapper();

            descriptor.Features = new Column[4];
            descriptor.Features[0] = new Column { Id = 1, Name = "sepal_length", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 5.5 };
            descriptor.Features[1] = new Column { Id = 2, Name = "sepal_width", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 4.2 };
            descriptor.Features[2] = new Column { Id = 3, Name = "petal_length", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 1.4 };
            descriptor.Features[3] = new Column { Id = 4, Name = "petal_width", Index = 3, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.5 };
            descriptor.Features[4] = new Column { Id = 5, Name = "species", Index = 4, Type = ColumnType.CLASS, Values = null, DefaultMissingValue = 1 };
            descriptor.LabelIndex = 1;

            var jsonString = JsonConvert.SerializeObject(dm);
            return jsonString;
        }
    }
}
