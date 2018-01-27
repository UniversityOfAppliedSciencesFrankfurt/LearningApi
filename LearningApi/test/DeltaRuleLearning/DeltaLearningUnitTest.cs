using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using DeltaLearning;


namespace test
{
    public class DeltaLearningUnitTest
    {

        static DeltaLearningUnitTest()
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
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 1,
                Name = "Y",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 2;

            return desc;
        }
        private DataDescriptor get3DDescriptor()
        {
            DataDescriptor desc = new DataDescriptor();
            desc.Features = new LearningFoundation.DataMappers.Column[3];
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "X",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 1,
                Name = "Y",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 2,
                Name = "Z",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 3;

            return desc;
        }

        [Fact]
        public void SimpleSequenceTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 10;
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

            api.UseDeltaLearning(0.2, 1000);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[4][];
            testData[0] = new double[] { 2.0, 0.0 };
            testData[1] = new double[] { 4.0, 0.0 };
            testData[2] = new double[] {6.0, 0.0 };
            testData[3] = new double[] { 8.0, 0.0 };
            

            var result = api.Algorithm.Predict(testData, api.Context);


            Assert.True(result[0] == 0);
            Assert.True(result[1] == 0);
            Assert.True(result[2] == 1);
            Assert.True(result[3] == 1);

        }

        /// <summary>
        /// for Even number of samples we should get 5.0 1.0
        /// for odd number of samples we should get -5.0 0.0
        /// </summary>

           [Fact]
            public void SimpleSequence2DTest()
            {
                LearningApi api = new LearningApi();
                api.UseActionModule<object, double[][]>((notUsed, ctx) =>
                {
                    const int maxSamples = 10000;
                    ctx.DataDescriptor = get2DDescriptor();
                    double[][] data = new double[maxSamples][];

                    for (int i = 0; i < maxSamples / 2; i++)
                    {
                        data[2 * i] = new double[3];
                        data[2 * i][0] = i;
                        data[2 * i][1] = 5.0;
                        data[2 * i][2] = 1.0;

                        data[2 * i + 1] = new double[3];
                        data[2 * i + 1][0] = i;
                        data[2 * i + 1][1] = -5.0;
                        data[2 * i + 1][2] = 0.0;
                    }
                    return data;
                });

                api.UseDeltaLearning(0.2, 1000);

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

        /// <summary>
        /// OR gate implementation
        /// </summary>

        [Fact]
        public void OR_Test()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                ctx.DataDescriptor = get2DDescriptor();
                double[][] data = new double[4][];


                data[0] = new double[] { 0, 0, 0, 0.0 };
                data[1] = new double[] { 0, 1, 1, 0.0 };
                data[2] = new double[] { 1, 0, 1, 0.0 };
                data[3] = new double[] { 1, 1, 1, 0.0 };
               
                return data;
            });

            api.UseDeltaLearning(0.2, 1000);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[3][];
            testData[0] = new double[] { 0, 0, 0.0 };
            testData[1] = new double[] { 1, 1, 0.0 };
            testData[2] = new double[] { 0, 1, 0.0 };
            
            var result = api.Algorithm.Predict(testData, api.Context);
            
            Assert.True(result[0] == 0);
            Assert.True(result[1] == 1);
            Assert.True(result[2] == 1);

        }
    }
    }

