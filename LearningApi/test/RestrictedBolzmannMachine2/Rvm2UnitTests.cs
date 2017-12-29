using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NeuralNet.Perceptron;
using NeuralNet.RestrictedBolzmannMachine2;
using System.IO;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;


namespace test.RestrictedBolzmannMachine
{

    public class RbmUnitTests
    {

        static RbmUnitTests()
        {

        }

        /// <summary>
        /// RBM is not supervised algorithm. This is why we do not have a label.
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptorForRbm_sample1()
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[6];

            // Label not used.
            des.LabelIndex = -1;

            des.Features = new Column[6];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[3] = new Column { Id = 4, Name = "col4", Index = 3, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[4] = new Column { Id = 5, Name = "col5", Index = 4, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[5] = new Column { Id = 6, Name = "col6", Index = 5, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };

            return des;
        }
        

        [Fact]
        public void SimpleRBMTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 12;
                ctx.DataDescriptor = getDescriptorForRbm_sample1();
                double[][] data = new double[maxSamples][];

                data[0] = new double[] { 1, 1, 0, 0, 0, 0 };  // A
                data[1] = new double[] { 0, 0, 1, 1, 0, 0 };  // B
                data[2] = new double[] { 0, 0, 0, 0, 1, 1 };  // C
                
                data[3] = new double[] { 1, 1, 0, 0, 0, 1 };  // noisy A
                data[4] = new double[] { 0, 0, 1, 1, 0, 0 };  // B
                data[5] = new double[] { 0, 0, 0, 0, 1, 1 };  // C
                
                data[6] = new double[] { 1, 0, 0, 0, 0, 0 };  // weak A
                data[7] = new double[] { 0, 0, 1, 0, 0, 0 };  // weak B
                data[8] = new double[] { 0, 0, 0, 0, 1, 0 };  // weak C
                
                data[9] = new double[] { 1, 1, 0, 1, 0, 0 };  // noisy A
                data[10] = new double[] { 1, 0, 1, 1, 0, 0 };  // noisy B
                data[11] = new double[] { 0, 0, 1, 0, 1, 1 };  // noisy C
                return data;
            });

            
            api.UseRbm(0.2, 1000);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[4][];

            testData[0] = new double[] { 1, 1, 0, 0, 0, 0 };
            testData[1] = new double[] { 0, 0, 0, 0, 1, 1 };
            testData[2] = new double[] { 0, 1, 0, 0, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 1, 0 };

            var result = api.Algorithm.Predict(testData, api.Context);

            // NOT FINISHED.
            //Assert.True(result[0] == 1);
            //Assert.True(result[1] == 0);
            //Assert.True(result[2] == 0);
            //Assert.True(result[3] == 0);
            //Assert.True(result[4] == 1);
            //Assert.True(result[5] == 0);
        }

        [Fact]
        public void RBMDataSample1Test()
        {
            var dataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\rbm_sample1.csv");

            LearningApi api = new LearningApi(this.getDescriptorForRbm_sample1());
           
            // Initialize data provider
            api.UseCsvDataProvider(dataPath, ',', false, 1);
            api.UseDefaultDataMapper();
            api.UseRbm(0.2, 1000);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[4][];

            testData[0] = new double[] { 1, 1, 0, 0, 0, 0 };
            testData[1] = new double[] { 0, 0, 0, 0, 1, 1 };
            testData[2] = new double[] { 0, 1, 0, 0, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 1, 1 };

            var result = api.Algorithm.Predict(testData, api.Context);

            // NOT FINISHED.
            //Assert.True(result[0] == 1);
            //Assert.True(result[1] == 0);
            //Assert.True(result[2] == 0);
            //Assert.True(result[3] == 0);
            //Assert.True(result[4] == 1);
            //Assert.True(result[5] == 0);
        }


    }


}
