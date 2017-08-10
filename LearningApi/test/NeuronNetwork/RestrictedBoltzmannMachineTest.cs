using LearningFoundation;
using LearningFoundation.DataMappers;
using LearningFoundation.DataProviders;
using NeuralNet.RestrictedBoltzmannMachine;
using NeuralNetworks.Core.ActivationFunctions;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace test.NeuronNetwork
{

    public class RestrictedBoltzmannTest
    {
        //Define the data location
        string m_binary_data_path = @"SampleData\binary\binary.csv";
        int FeatureAmount = 10;

        static RestrictedBoltzmannTest()
        {

        }

        /// <summary>
        /// Describe the sample data for training,
        /// including the ID numbers and labels of features 
        /// </summary>
        /// <returns> Sample data description </returns>
        private DataDescriptor getDescriptor()
        {
            DataDescriptor desc = new DataDescriptor();
            desc.Features = new LearningFoundation.DataMappers.Column[6];

            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "A",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.Features[1] = new LearningFoundation.DataMappers.Column()
            {
                Id = 1,
                Name = "B",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.Features[2] = new LearningFoundation.DataMappers.Column()
            {
                Id = 2,
                Name = "C",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.Features[3] = new LearningFoundation.DataMappers.Column()
            {
                Id = 3,
                Name = "X",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.Features[4] = new LearningFoundation.DataMappers.Column()
            {
                Id = 4,
                Name = "Y",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.Features[5] = new LearningFoundation.DataMappers.Column()
            {
                Id = 5,
                Name = "Z",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 6;
            return desc;
        }

        /// <summary>
        /// Describe the auto generate training sample using the capitalize characters as the feature name
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptor_Auto()
        {
            DataDescriptor desc = new DataDescriptor();
            int NumOfFeature = FeatureAmount;
            desc.Features = new LearningFoundation.DataMappers.Column[NumOfFeature];
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (int i = 0; i < NumOfFeature; i++)
            {
                desc.Features[i] = new LearningFoundation.DataMappers.Column()
                {
                    Id = i,
                    Name = alpha[i].ToString(),
                    Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                    Index = i,
                };
            }
            desc.LabelIndex = NumOfFeature;
            return desc;
        }
        

        /// <summary>
        /// Accord .Net standard test for Restricted Boltzmann Machine
        /// </summary>
        [Fact]
        public void RBM_AccordTest()
        {
            //Create some sample data for the 
            double[][] sample =
            {
            new double[] { 1,1,1,0,0,0 }, // class a
            new double[] { 1,0,1,0,0,0 }, // class a
            new double[] { 1,1,1,0,0,0 }, // class a
            new double[] { 0,0,1,1,1,0 }, // class b
            new double[] { 0,0,1,1,0,0 }, // class b
            new double[] { 0,0,1,1,1,0 }, // class b           
        };

            // Create a Bernoulli activation function
            var function = new BernoulliFunction(alpha: 0.5);

            // Create a Restricted Boltzmann Machine algorithm with 6 inputs and 2 hidden neuron
            var rbm = new RestrictedBoltzmannMachine(function, inputsCount: 6, hiddenNeurons: 2);

            // Create the training algorithm for RBM
            var teacher = new ContrastiveDivergenceLearning(rbm)
            {
                LearningRate = 0.1,
                Momentum = 0.9,
                Decay = 0.01
            };

            // Run the learning algorithm
            for (int i = 0; i < 5000; i++)
                teacher.RunEpoch(sample);

            // Compute the machine answers for the given test data inputs:
            double[] a = rbm.Compute(new double[] { 1, 1, 1, 0, 0, 0 });
            double[] b = rbm.Compute(new double[] { 0, 0, 0, 1, 1, 1 });
            double[] c = rbm.Compute(new double[] { 1, 1, 0, 0, 0, 0 });
            double[] d = rbm.Compute(new double[] { 0, 0, 0, 1, 1, 0 });
            double[] e = rbm.Compute(new double[] { 1, 0, 1, 0, 0, 0 });
            double[] f = rbm.Compute(new double[] { 0, 0, 0, 1, 0, 1 });
            double[] g = rbm.Compute(new double[] { 0, 1, 1, 0, 0, 0 });
            double[] h = rbm.Compute(new double[] { 0, 0, 0, 0, 1, 1 });
            

            // Generate input vectors given the classes:
            double[] xa = rbm.GenerateInput(new double[] { 1, 0 });/// 1.1.1.0.0.0
            double[] xb = rbm.GenerateInput(new double[] { 0, 1 });

            //Testing the test data
            Assert.True(((a[0] > a[1]) && (b[0] < b[1]))
                  ^ ((a[0] < a[1]) && (b[0] > b[1])));
        }


        /// <summary>
        /// Test using pre-define data
        /// </summary>
        [Fact]
        public void RBM_SimpleTest()
        {
            // Create some sample inputs. Note that the
            // first three vectors belong to one class, and the other
            // three belong to another           
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 6;
                ctx.DataDescriptor = getDescriptor();

                double[][] sample = new double[maxSamples][];
                sample[0] = new double[] { 1, 1, 1, 0, 0, 0 };
                sample[1] = new double[] { 1, 0, 1, 0, 0, 0 };
                sample[2] = new double[] { 1, 1, 1, 0, 0, 0 };
                sample[3] = new double[] { 0, 0, 1, 1, 1, 0 };
                sample[4] = new double[] { 0, 0, 1, 1, 0, 0 };
                sample[5] = new double[] { 0, 0, 1, 1, 1, 0 };

                return sample;
            });

            //Define the value of Restricted Boltzmann Machine training variables
            int InputsCount = 6;
            int HiddenNeurons = 2;
            int Iteration = 5000;
            double LearningRates = 0.1;
            double Momentums = 0.9;
            double Decays = 0.01;
            Random rand = new Random();

            // Call the algorithm
            api.UseRestrictedBoltzmannMachine(InputsCount, HiddenNeurons, Iteration, LearningRates, Momentums, Decays);

            //Run the algorithm
            IScore score = api.Run() as IScore;

            //Create some test data manually
            int m_testcount = 8;
            double[][] m_testdata = new double[m_testcount][];
            m_testdata[0] = new double[] { 1, 1, 1, 0, 0, 0 };
            m_testdata[1] = new double[] { 0, 0, 0, 1, 1, 1 };
            m_testdata[2] = new double[] { 1, 1, 0, 0, 0, 0 };
            m_testdata[3] = new double[] { 0, 0, 0, 1, 1, 0 };
            m_testdata[4] = new double[] { 1, 0, 1, 0, 0, 0 };
            m_testdata[5] = new double[] { 0, 0, 0, 1, 0, 1 };
            m_testdata[6] = new double[] { 0, 1, 1, 0, 0, 0 };
            m_testdata[7] = new double[] { 0, 0, 0, 0, 1, 1 };

            //
            // Calculate the network output based on the test data
            var m_testresult = api.Algorithm.Predict(m_testdata, api.Context);

            //
            // Testing the test data in the specific order
            for (   int i = 0; i < m_testcount - 2; i++)
            {
                Assert.True(((m_testresult[i] > m_testresult[i + 1])
                    && (m_testresult[i + 1] < m_testresult[i + 2]))
   ^ ((m_testresult[i] < m_testresult[i + 1])
   && (m_testresult[i + 1] > m_testresult[i + 2])));
            }
        }

        /// <summary>
        /// Test using auto generate randomly data
        /// </summary>
        [Fact]
        public void RBM_SimpleTest2()
        {
            // Create some randomly generated sample inputs. 
            LearningApi api = new LearningApi();

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                ctx.DataDescriptor = getDescriptor_Auto();
                int NumOfFeature = api.Context.DataDescriptor.Features.Length;
                var max = Math.Pow(2, NumOfFeature);
                int maxSamples = Convert.ToInt32(max);

                Random random = new Random();

                double[][] sample = new double[maxSamples][];

                for (int i = 0; i < maxSamples; i++)
                {
                    sample[i] = Enumerable.Repeat(0.0, api.Context.DataDescriptor.Features.Length).ToArray();
                }

                for (int i = 0; i < maxSamples; i++)
                {
                    for (int j = 0; j < NumOfFeature; j++)
                    {
                        sample[i][j] = random.Next(0, 2);
                    }
                }

                return sample;
            });

            //
            //Define the value of Restricted Boltzmann Machine training variables
            int InputsCount = FeatureAmount;
            int HiddenNeurons = 5;
            int Iteration = 15000;
            double LearningRates = 0.07;
            double Momentums = 0.9;
            double Decays = 0.01;
            
            //
            // Call the algorithm
            api.UseRestrictedBoltzmannMachine(InputsCount, HiddenNeurons, Iteration, LearningRates, Momentums, Decays);

            //
            //Run the algorithm
            IScore score = api.Run() as IScore;

            //
            //Auto Generate some test data
            int m_testcountAuto = 10000;
            Random rand = new Random();
            double[][] m_testdataAuto = new double[m_testcountAuto][];

            for (int i = 0; i < m_testcountAuto; i++)
            {
                m_testdataAuto[i] = Enumerable.Repeat(0.0, api.Context.DataDescriptor.Features.Length).ToArray();
            }

            for (int i = 0; i < m_testcountAuto; i++)
            {
                for (int j = 0; j < api.Context.DataDescriptor.Features.Length; j++)
                {
                    m_testdataAuto[i][j] = rand.Next(0, 2);
                }
            }

            // Calculate the network output based on the auto generated test data
            var m_testresultAuto = api.Algorithm.Predict(m_testdataAuto, api.Context);
            for (int i = 0; i < m_testcountAuto; i++)
            {
                Assert.True(m_testresultAuto[i] > 0);
            }
        }

        [Fact]
        /// <summary>
        ///Test using pre-created data file
        /// </summary>
        public void RBM_GetDataTest()
        {
            var binary_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_binary_data_path);

            //
            LearningApi api = new LearningApi(TestHelpers.GetDescriptorBinary());

            //
            api.UseCsvDataProvider(binary_path, ',', 1);

            //
            api.UseDefaultDataMapper();
            
            //
            int NumOfFeature = api.Context.DataDescriptor.Features.Length;


            //
            //Define the value of Restricted Boltzmann Machine training variables
            int InputsCount = NumOfFeature;
            int HiddenNeurons = 4;
            int Iteration = 15000;
            double LearningRates = 0.07;
            double Momentums = 0.9;
            double Decays = 0.01;

            // Call the algorithm
            api.UseRestrictedBoltzmannMachine(InputsCount, HiddenNeurons, Iteration, LearningRates, Momentums, Decays);

            //Run the algorithm
            IScore score = api.Run() as IScore;


            //
            //Auto Generate some test data
            int m_testcountAuto = 1000;
            Random rand = new Random();
            double[][] m_testdataAuto = new double[m_testcountAuto][];

            for (int i = 0; i < m_testcountAuto; i++)
            {
                m_testdataAuto[i] = Enumerable.Repeat(0.0, api.Context.DataDescriptor.Features.Length).ToArray();
            }

            for (int i = 0; i < m_testcountAuto; i++)
            {
                for (int j = 0; j < api.Context.DataDescriptor.Features.Length; j++)
                {
                    m_testdataAuto[i][j] = rand.Next(0, 2);
                }
            }

            // Calculate the network output based on the auto generated test data
            var m_testresultAuto = api.Algorithm.Predict(m_testdataAuto, api.Context);
            for (int i = 0; i < m_testcountAuto; i++)
            {
                Assert.True(m_testresultAuto[i] > 0);
            }
        }
    }
}
