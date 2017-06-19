﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using NeuralNet.RestrictedBoltmannMachine;

namespace test.NeuronNetwork
{
    [TestFixture]
    public class DeepBeliefNetworkTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        //[Test]
        //public void PushPopTestDBN()
        //{
        //    DeepBeliefNetwork network = new DeepBeliefNetwork( 6, 2, 9 );

        //    Assert.AreEqual( 2, network.Machines.Count );
        //    Assert.AreEqual( 6, network.InputsCount );
        //    Assert.AreEqual( 9, network.OutputCount );
        //    Assert.AreEqual( 2, network.Machines[0].Hidden.Neurons.Length );
        //    Assert.AreEqual( 9, network.Machines[1].Hidden.Neurons.Length );
        //    Assert.AreEqual( 6, network.Machines[0].Visible.Neurons.Length );
        //    Assert.AreEqual( 2, network.Machines[1].Visible.Neurons.Length );

        //    network.Pop( );

        //    Assert.AreEqual( 1, network.Machines.Count );
        //    Assert.AreEqual( 6, network.InputsCount );
        //    Assert.AreEqual( 2, network.OutputCount );
        //    Assert.AreEqual( 2, network.Machines[0].Hidden.Neurons.Length );
        //    Assert.AreEqual( 6, network.Machines[0].Visible.Neurons.Length );

        //    network.Push( 4 );
        //    network.Push( 10 );

        //    Assert.AreEqual( 3, network.Machines.Count );
        //    Assert.AreEqual( 6, network.InputsCount );
        //    Assert.AreEqual( 10, network.OutputCount );
        //    Assert.AreEqual( 2, network.Machines[0].Hidden.Neurons.Length );
        //    Assert.AreEqual( 4, network.Machines[1].Hidden.Neurons.Length );
        //    Assert.AreEqual( 10, network.Machines[2].Hidden.Neurons.Length );

        //    Assert.AreEqual( 6, network.Machines[0].Visible.Neurons.Length );
        //    Assert.AreEqual( 2, network.Machines[1].Visible.Neurons.Length );
        //    Assert.AreEqual( 4, network.Machines[2].Visible.Neurons.Length );

        //    network.Pop( );

        //    Assert.AreEqual( 2, network.Machines.Count );
        //    Assert.AreEqual( 6, network.InputsCount );
        //    Assert.AreEqual( 4, network.OutputCount );
        //    Assert.AreEqual( 2, network.Machines[0].Hidden.Neurons.Length );
        //    Assert.AreEqual( 4, network.Machines[1].Hidden.Neurons.Length );

        //    Assert.AreEqual( 6, network.Machines[0].Visible.Neurons.Length );
        //    Assert.AreEqual( 2, network.Machines[1].Visible.Neurons.Length );
        //}

        [Test]
        public void ConstructorTestDBN()
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork( 6, 2, 1 );

            Assert.AreEqual( 2, network.Machines.Count );
            Assert.AreEqual( 6, network.InputsCount );
            Assert.AreEqual( 1, network.OutputCount );
            Assert.AreEqual( 2, network.Machines[0].Hidden.Neurons.Length );
            Assert.AreEqual( 1, network.Machines[1].Hidden.Neurons.Length );
            Assert.AreEqual( 6, network.Machines[0].Visible.Neurons.Length );
            Assert.AreEqual( 2, network.Machines[1].Visible.Neurons.Length );
        }

        private static DeepBeliefNetwork createNetwork( double[][] inputs )
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork( 6, 2, 1 );

            network.Machines[0].Hidden.Neurons[0].Weights[0] = 0.00461421;
            network.Machines[0].Hidden.Neurons[0].Weights[1] = 0.04337112;
            network.Machines[0].Hidden.Neurons[0].Weights[2] = -0.10839599;
            network.Machines[0].Hidden.Neurons[0].Weights[3] = -0.06234004;
            network.Machines[0].Hidden.Neurons[0].Weights[4] = -0.03017057;
            network.Machines[0].Hidden.Neurons[0].Weights[5] = 0.09520391;
            network.Machines[0].Hidden.Neurons[0].Threshold = 0;

            network.Machines[0].Hidden.Neurons[1].Weights[0] = 0.08263872;
            network.Machines[0].Hidden.Neurons[1].Weights[1] = -0.118437;
            network.Machines[0].Hidden.Neurons[1].Weights[2] = -0.21710971;
            network.Machines[0].Hidden.Neurons[1].Weights[3] = 0.02332903;
            network.Machines[0].Hidden.Neurons[1].Weights[4] = 0.00953116;
            network.Machines[0].Hidden.Neurons[1].Weights[5] = 0.09870652;
            network.Machines[0].Hidden.Neurons[1].Threshold = 0;

            network.Machines[0].Visible.Neurons[0].Threshold = 0;
            network.Machines[0].Visible.Neurons[1].Threshold = 0;
            network.Machines[0].Visible.Neurons[2].Threshold = 0;
            network.Machines[0].Visible.Neurons[3].Threshold = 0;
            network.Machines[0].Visible.Neurons[4].Threshold = 0;
            network.Machines[0].Visible.Neurons[5].Threshold = 0;

            network.UpdateVisibleWeights( );


            DeepBeliefNetworkLearning target = new DeepBeliefNetworkLearning( network )
            {
                Algorithm = ( h, v, i ) => new ContrastiveDivergenceLearning( h, v )
            };

            for (int layer = 0; layer < 2; layer++)
            {

                target.LayerIndex = layer;

                double[][] layerInputs = target.GetLayerInput( inputs );

                int iterations = 5000;
                double[] errors = new double[iterations];
                for (int i = 0; i < iterations; i++)
                    errors[i] = target.RunEpoch( layerInputs );
            }

            return network;
        }


        //    public void ExampleTest1()
        //    {
        //        Accord.Math.Tools.SetupGenerator(0);

        //        // We'll use a simple XOR function as input. 

        //        double[][] inputs =
        //        {
        //            new double[] { 0, 0 }, // 0 xor 0
        //            new double[] { 0, 1 }, // 0 xor 1
        //            new double[] { 1, 0 }, // 1 xor 0
        //            new double[] { 1, 1 }, // 1 xor 1
        //        };

        //        // XOR output, corresponding to the input.
        //        double[][] outputs =
        //        {
        //            new double[] { 0 }, // 0 xor 0 = 0
        //            new double[] { 1 }, // 0 xor 1 = 1
        //            new double[] { 1 }, // 1 xor 0 = 1
        //            new double[] { 0 }, // 1 xor 1 = 0
        //        };

        //        // Setup the deep belief network (2 inputs, 3 hidden, 1 output)
        //        DeepBeliefNetwork network = new DeepBeliefNetwork(2, 3, 1);

        //        // Initialize the network with Gaussian weights
        //        new GaussianWeights(network, 0.1).Randomize();

        //        // Update the visible layer with the new weights
        //        network.UpdateVisibleWeights();


        //        // Setup the learning algorithm.
        //        DeepBeliefNetworkLearning teacher = new DeepBeliefNetworkLearning(network)
        //        {
        //            Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
        //            {
        //                LearningRate = 0.1,
        //                Momentum = 0.5,
        //                Decay = 0.001,
        //            }
        //        };



        //        // Unsupervised learning on each hidden layer, except for the output.
        //        for (int i = 0; i < network.Layers.Length - 1; i++)
        //        {
        //            teacher.LayerIndex = i;

        //            // Compute the learning data with should be used
        //            var layerInput = teacher.GetLayerInput(inputs);

        //            // Train the layer iteratively
        //            for (int j = 0; j < 5000; j++)
        //                teacher.RunEpoch(layerInput);
        //        }




    }
}
