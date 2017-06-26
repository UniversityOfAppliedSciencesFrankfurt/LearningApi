using LearningFoundation;
using NeuralNet.RestrictedBoltzmannMachine;
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

        [Test]
        //[Theory]
        public void NetworkTestRBM()
        {
            // Create some sample inputs and outputs. Note that the
            // first four vectors belong to one class, and the other
            // four belong to another (you should see that the 1s
            // accumulate on the beginning for the first four vectors
            // and on the end for the second four).
            #region Accord test
            //double[][] inputs =
            //{
            //    new double[] { 1,1,1,0,0,0 }, // class a
            //    new double[] { 1,0,1,0,0,0 }, // class a
            //    new double[] { 1,1,1,0,0,0 }, // class a
            //    new double[] { 0,0,1,1,1,0 }, // class b
            //    new double[] { 0,0,1,1,0,0 }, // class b
            //    new double[] { 0,0,1,1,1,0 }, // class b

            //};

            //// Create a Bernoulli activation function
            //var function = new BernoulliFunction(alpha: 0.5);

            //// Create a Restricted Boltzmann Machine for 6 inputs and with 1 hidden neuron
            //var rbm = new RestrictedBoltzmannMachine(function, inputsCount: 6, hiddenNeurons: 2);

            //// Create the learning algorithm for RBMs
            //var teacher = new ContrastiveDivergenceLearning(rbm, 5000, inputs, 0.1)
            //{
            //    Momentum = 0,
            //    Decay = 0
            //};
            //double[] a = rbm.Compute(new double[] { 1, 1, 1, 0, 0, 0 }); // { 0.99, 0.00 }
            //double[] b = rbm.Compute(new double[] { 0, 0, 0, 1, 1, 1 }); // { 0.00, 0.99 }
            //double[] c = rbm.Compute(new double[] { 1, 0, 1, 0, 0, 0 });
            //double[] d = rbm.Compute(new double[] { 1, 0, 1, 1, 0, 0 });
            //double[] e = rbm.Compute(new double[] { 1, 0, 1, 0, 1, 0 });
            //double[] f = rbm.Compute(new double[] { 1, 0, 1, 0, 0, 1 });
            //double[] g = rbm.Compute(new double[] { 0, 1, 0, 1, 0, 1 });
            //// As we can see, the first neuron responds to vectors belonging
            //// to the first class, firing 0.99 when we feed vectors which 
            //// have 1s at the beginning. Likewise, the second neuron fires 
            //// when the vector belongs to the second class.

            //// We can also generate input vectors given the classes:

            //double[] xa = rbm.GenerateInput(new double[] { 1, 0 }); // { 1, 1, 1, 0, 0, 0 }
            //double[] xb = rbm.GenerateInput(new double[] { 0, 1 }); // { 0, 0, 1, 1, 1, 0 }

            //// As we can see, if we feed an output pattern where the first neuron
            //// is firing and the second isn't, the network generates an example of
            //// a vector belonging to the first class. The same goes for the second
            ////// neuron and the second class.
            //Assert.IsTrue(((a[0] > a[1]) && (b[0] < b[1]))
            //      ^ ((a[0] < a[1]) && (b[0] > b[1])));


            #endregion

            ///////////////////////////////////////////////////////////////////////////////////
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 10000;
                ctx.DataDescriptor = getDescriptor();
                double[][] data = new double[maxSamples][];
                //need to check the data type
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
                    data[i][0] = i; ///exp: data[1][0] = 1
                    data[i][1] = (i > (maxSamples / 2)) ? 1 : 0; //exp: data[1][1] = 0, data[5001][1]=1,0;
                    //0000011111
                }

                return data;
            });
            ///////////////////////////////
            double[][] sample =
          {
                new double[] { 1,1,1,0,0,0 }, // class a
                new double[] { 1,0,1,0,0,0 }, // class a
                new double[] { 1,1,1,0,0,0 }, // class a
                new double[] { 0,0,0,1,1,1 }, // class b
                new double[] { 0,0,0,1,0,1 }, // class b
                new double[] { 0,0,0,1,1,1 },// class b
            };
            int inputscount = 6;
            int hiddenneurons = 2;
            int iteration = 15000;
            double learningrates = 0.5;
            double momentums = 0.5;
            double decays = 0.5;
            var m_rbm = new RestrictedBoltzmannMachine(inputscount, hiddenneurons);
            var m_teaching = new ContrastiveDivergenceLearning(m_rbm, iteration, sample)
            //var m_teaching = new ContrastiveDivergenceLearning(m_rbm,iteration,sample)

            {
                LearningRate = learningrates,
                Momentum = momentums,
                Decay = decays
            };

            // var abc = api.UseRestrictedBoltzmannMachine(inputscount, hiddenneurons,iteration, sample, learningrates, momentums,  decays);
            //IScore score = api.Run() as IScore;
            
            
            //// We can also generate input vectors given the classes:
            double[] xa1 = m_rbm.GenerateInput(new double[] { 1, 0 }); //000101
            double[] xb1 = m_rbm.GenerateInput(new double[] { 0, 1 }); //111000
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
            //// Create the learning algorithm for RBMs
            double[][] m_testresult = new double[m_testcount][];
            for (int i = 0; i < m_testcount; i++)
            {
                m_testresult[i] = m_rbm.Compute(m_testdata[i]);
                // m_testresult[i] = m_rbm.Predict(m_testdata[i]);
               
            };

            //same as // var result = api.Algorithm.Predict(testData, api.Context);


            //// As we can see, if we feed an output pattern where the first neuron
            //// is firing and the second isn't, the network generates an example of
            //// a vector belonging to the first class. The same goes for the second
            ////// neuron and the second class.
            for (int i = 0; i < m_testcount - 1; i++)
            {
                Assert.IsTrue(((m_testresult[i][0] > m_testresult[i][1]) && (m_testresult[i + 1][0] < m_testresult[i + 1][1]))
             ^ ((m_testresult[i][0] < m_testresult[i][1]) && (m_testresult[i + 1][0] > m_testresult[i + 1][1])));
            };

            //var result = api.Algorithm.Predict(m_testdata, api.Context);

            //Assert.True(result[0] == 0);
            //Assert.True(result[1] == 1);
            //Assert.True(result[2] == 1);
            //Assert.True(result[3] == 1);  

            //LearningApi api = new LearningApi();
            //api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            //{
            //    const int maxSamples = 10000;
            //    ctx.DataDescriptor = getDescriptor();
            //    double[][] data = new double[maxSamples][];

            //    for (int i = 0; i < maxSamples; i++)
            //    {
            //        data[i] = new double[2];
            //        data[i][0] = i;
            //        data[i][1] = (i > (maxSamples / 2)) ? 1 : 0;
            //    }

            //    return data;
            //});     


        }

    }
}
