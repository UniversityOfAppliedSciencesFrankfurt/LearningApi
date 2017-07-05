using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NeuralNet.Perceptron;


namespace test.Perceptron
{
    public class PerceptronUnitTests
    {

        static PerceptronUnitTests()
        {

        }

        private DataDescriptor getDescriptor()
        {
            DataDescriptor desc = new DataDescriptor();
            desc.Features = new LearningFoundation.DataMappers.Column[1];
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "X",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 1;

            return desc;
        }

        private DataDescriptor get2DDescriptor()
        {
            DataDescriptor desc = new DataDescriptor();
            desc.Features = new LearningFoundation.DataMappers.Column[2];
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "X",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };
            desc.Features[1] = new LearningFoundation.DataMappers.Column()
            {
                Id = 1,
                Name = "Y",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 2;

            return desc;
        }

        [Fact]
        public void SimpleSequenceTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 100000;
                ctx.DataDescriptor = getDescriptor();
                double[][] data = new double[maxSamples][];

                //
                // We generate following input vectors: 
                // IN Val - Expected OUT Val 
                // 1 - 0
                // 2 - 0,
                // ...
                // maxSamples / 2     - 1,
                // maxSamples / 2 + 1 - 1,
                // maxSamples / 2 + 2 - 1,

                for (int i = 0; i < maxSamples; i++)
                {
                    data[i] = new double[2];
                    data[i][0] = i;
                    data[i][1] = (i > (maxSamples / 2)) ? 1 : 0;
                }

                return data;
            });

            api.UsePerceptron(0.01, 100000);
          
            IScore score = api.Run() as IScore;

            double[][] testData = new double[10][];
            testData[0] = new double[] { 20.0, 0.0 };
            testData[1] = new double[] { 1020.0, 0.0 };
            testData[2] = new double[] { 20.0, 0.0 };
            testData[3] = new double[] { 20.0, 0.0 };
            testData[4] = new double[] { 20000.0, 0.0 };
            testData[5] = new double[] { 20.0, 0.0 };
            testData[6] = new double[] { 20.0, 0.0 };
            testData[7] = new double[] { 60000.0, 0.0 };
            testData[8] = new double[] { 20.0, 0.0 };
            testData[9] = new double[] { 20.0, 0.0 };

            //testData[1] = new double[] { 2000.0, 0.0 };
            //testData[2] = new double[] { 6000.0, 0.0 };
            //testData[3] = new double[] { 5001, 0.0 };
            //testData[4] = new double[] { 20.0, 0.0 };
            //testData[5] = new double[] { 100.0, 0.0 };
            //testData[6] = new double[] { 200.0, 0.0 };
            //testData[7] = new double[] { 500.0, 0.0 };
            //testData[8] = new double[] { 1000.0, 0.0 };
            //testData[9] = new double[] { 2500.0, 0.0 };


            var result = api.Algorithm.Predict(testData, api.Context);

            Assert.True(result[0] == 0);
            Assert.True(result[1] == 1);
            Assert.True(result[2] == 1);
            Assert.True(result[3] == 1);
        }

        [Fact]
        public void SimpleSequenceWithGapsTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 10000;
                ctx.DataDescriptor = getDescriptor();
                double[][] data = new double[maxSamples / 3][];

                //
                // We generate following input vectors: 
                // IN Val - Expected OUT Val 
                // Every 3th number is given

                for (int i = 0; i < maxSamples / 3; i++)
                {
                    data[i] = new double[2];
                    data[i][0] = i * 3;
                    data[i][1] = ((i * 3) > (maxSamples / 2)) ? 1 : 0;
                }

                return data;
            });

            api.UsePerceptron(0.2, 1000);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[6][];
            testData[0] = new double[] { 2.0, 0.0 };
            testData[1] = new double[] { 1, 0.0 };
            testData[2] = new double[] { 3, 0.0 };
            testData[3] = new double[] { 3002, 0.0 };
            testData[4] = new double[] { 6002.0, 0.0 };
            testData[5] = new double[] { 9005, 0.0 };

            var result = api.Algorithm.Predict(testData, api.Context);

            Assert.True(result[0] == 0);
            Assert.True(result[1] == 0);
            Assert.True(result[2] == 0);
            Assert.True(result[3] == 1);
            Assert.True(result[4] == 1);
            Assert.True(result[5] == 1);
        }



        [Fact]
        public void SimpleSequence2DTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 10000;
                ctx.DataDescriptor = get2DDescriptor();
                double[][] data = new double[maxSamples][];

                for (int i = 0; i < maxSamples/2; i++)
                {
                    data[2*i] = new double[3];
                    data[2*i][0] = i;
                    data[2*i][1] = 5.0;
                    data[2*i][2] = 1.0;

                    data[2 * i+1] = new double[3];
                    data[2*i+1][0] = i;
                    data[2*i+1][1] = -5.0;
                    data[2*i+1][2] = 0.0;
                }
                return data;
            });

            api.UsePerceptron(0.2, 1000);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[6][];
            testData[0] = new double[] { 2.0, 5.0, 0.0 };
            testData[1] = new double[] { 2, -5.0, 0.0 };
            testData[2] = new double[] { 100, -5.0, 0.0 };
            testData[3] = new double[] { 100, -5.0, 0.0 };
            testData[4] = new double[] { 490, 5.0, 0.0 };
            testData[5] = new double[] { 490, -5.0, 0.0 };

            var result = api.Algorithm.Predict(testData, api.Context);

            Assert.True(result[0] == 1);
            Assert.True(result[1] == 0);
            Assert.True(result[2] == 0);
            Assert.True(result[3] == 0);
            Assert.True(result[4] == 1);
            Assert.True(result[5] == 0);
        }
    }


}
