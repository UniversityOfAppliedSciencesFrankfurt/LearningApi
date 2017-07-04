using LearningFoundation;
using NeuralNet.RestrictedBoltzmannMachine;
using NeuralNetworks.Core.ActivationFunctions;
using NUnit.Framework;

//using Xunit;

namespace test.NeuronNetwork
{
    [TestFixture]
    public class RestrictedBoltzmannTestACC
    {
        static RestrictedBoltzmannTestACC()
        {

        }
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
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 1,
                Name = "B",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 1,
                Name = "C",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 1,
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
                Id = 1,
                Name = "Z",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,

            };


            desc.LabelIndex = 6;

            return desc;
        }
        [Test]
        public void RBMAccord()
        {
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

            // Create a Restricted Boltzmann Machine for 6 inputs and with 1 hidden neuron
            var rbm = new RestrictedBoltzmannMachine(function, inputsCount: 6, hiddenNeurons: 2);

            // Create the learning algorithm for RBMs
            var teacher = new ContrastiveDivergenceLearning(rbm, 5000, sample)
            {
                LearningRate = 0.1,
                Momentum = 0,
                Decay = 0
            };
            double[] a = rbm.Compute(new double[] { 1, 1, 1, 0, 0, 0 }); // { 0.99, 0.00 }
            double[] b = rbm.Compute(new double[] { 0, 0, 0, 1, 1, 1 }); // { 0.00, 0.99 }
            double[] c = rbm.Compute(new double[] { 1, 0, 1, 0, 0, 0 });
            double[] d = rbm.Compute(new double[] { 1, 0, 1, 1, 0, 0 });
            double[] e = rbm.Compute(new double[] { 1, 0, 1, 0, 1, 0 });
            double[] f = rbm.Compute(new double[] { 1, 0, 1, 0, 0, 1 });
            double[] g = rbm.Compute(new double[] { 0, 1, 0, 1, 0, 1 });
            // As we can see, the first neuron responds to vectors belonging
            // to the first class, firing 0.99 when we feed vectors which 
            // have 1s at the beginning. Likewise, the second neuron fires 
            // when the vector belongs to the second class.

            // We can also generate input vectors given the classes:

            double[] xa = rbm.GenerateInput(new double[] { 1, 0 }); // { 1, 1, 1, 0, 0, 0 }
            double[] xb = rbm.GenerateInput(new double[] { 0, 1 }); // { 0, 0, 1, 1, 1, 0 }

            // As we can see, if we feed an output pattern where the first neuron
            // is firing and the second isn't, the network generates an example of
            // a vector belonging to the first class. The same goes for the second
            //// neuron and the second class.
            Assert.IsTrue(((a[0] > a[1]) && (b[0] < b[1]))
                  ^ ((a[0] < a[1]) && (b[0] > b[1])));
        }

        [Test]
        //[Theory]
        public void NetworkTestRBM()
        {
            // Create some sample inputs and outputs. Note that the
            // first four vectors belong to one class, and the other
            // four belong to another (you should see that the 1s
            // accumulate on the beginning for the first four vectors
            // and on the end for the second four).

            ///////////////////////////////////////////////////////////////////////////////////
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 6;
                ctx.DataDescriptor = getDescriptor();
                double[][] sample = new double[maxSamples][];
                ////need to check the data type

                //double[][] samples =
                //{
                //    new double[] { 1,1,1,0,0,0 }, // class a
                //    new double[] { 1,0,1,0,0,0 }, // class a
                //    new double[] { 1,1,1,0,0,0 }, // class a
                //    new double[] { 0,0,1,1,1,0 }, // class b
                //    new double[] { 0,0,1,1,0,0 }, // class b
                //    new double[] { 0,0,1,1,1,0 }, // class b
                //};

                sample[0] = new double[] { 1, 1, 1, 0, 0, 0 }; // class a
                sample[1] = new double[] { 1, 0, 1, 0, 0, 0 }; // class a
                sample[2] = new double[] { 1, 1, 1, 0, 0, 0 }; // class a
                sample[3] = new double[] { 0, 0, 0, 1, 1, 1 }; // class b
                sample[4] = new double[] { 0, 0, 0, 1, 0, 1 }; // class b
                sample[5] = new double[] { 0, 0, 0, 1, 1, 1 }; // class b
                return sample;
            });
            ///////////////////////////////

            int inputscount = 6;
            int hiddenneurons = 2;
            int iteration = 15000;
            double learningrates = 0.5;
            double momentums = 0.5;
            double decays = 0.5;

            /// Call the algorithm
            api.UseRestrictedBoltzmannMachine(inputscount, hiddenneurons, iteration, learningrates, momentums, decays);
            ///Run the algorithm
            IScore score = api.Run() as IScore;
            ///Create test data
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

            /// Calculate the network output
            // m_testresult[i] = m_rbm.Compute(m_testdata[i]); 
            // m_testresultp[0][0]=99.9;  
            // m_testresult[0][1]=0.1;
            // m_testresult[0]=[99.9,0.1]
            /// 
            var m_testresult = api.Algorithm.Predict(m_testdata, api.Context);

            //// As we can see, if we feed an output pattern where the first neuron
            //// is firing and the second isn't, the network generates an example of
            //// a vector belonging to the first class. The same goes for the second
            ////// neuron and the second class.
            for (int i = 0; i < m_testcount - 1; i++)
            {
               // Assert.True(((m_testresult[i] == 0) && (m_testresult[i + 1] == 1)) ^ ((m_testresult[i] == 1) && (m_testresult[i + 1] == 0)));
                Assert.IsTrue(((m_testresult[i] > m_testresult[i+1]) && (m_testresult[i+1] < m_testresult[i+2]))
                 ^ ((m_testresult[i] < m_testresult[i+1]) && (m_testresult[i+1] > m_testresult[i+2])));
            }
        }

    }
}
