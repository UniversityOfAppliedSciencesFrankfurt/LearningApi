using Akka.Actor;
using Akka.Configuration;
using LearningFoundation;
using NeuralNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaMLPerceptron
{
    public class AkaMLPerceptronAlgorithm : NeuralNetCore
    {

        #region Private Fields

        #region ActorModel related fields
        /// <summary>
        /// Name of the actor cluster system.
        /// </summary>
        private string akkaSystemName;

        /// <summary>
        /// URIs of akka nodes in cluster.
        /// </summary>
        private string[] akkaNodes;

        /// <summary>
        /// Instance of the actor cluster system.
        /// </summary>
        private ActorSystem actorSystem;

        /// <summary>
        /// Number of nodes in cluster.
        /// </summary>
        private int numOfNodes;

        #endregion


        public double m_LearningRate = 0.1;

        public int[] m_HiddenLayerNeurons = { 4, 3, 5 };

        public int m_OutputLayerNeurons;

        public int m_Iterations = 10000;

        public int m_batchSize = 1;

        private Func<double, double> m_ActivationFunction = ActivationFunctions.HyperbolicTan;//TODO Patrick

        public int m_InpDims;

        public double[][,] m_Weights;

        public double[][] m_Biases;

        public Boolean m_SoftMax = true;

        public int TestCaseNumber = 0;

        #endregion
        public AkaMLPerceptronAlgorithm(string akkaSystemName, string[] akkaNodes)
        {
            if (akkaNodes == null || akkaNodes.Length == 0)
                throw new ArgumentException("Cluster nodes must be specified.");


            // akka.tcp://DeployTarget@localhost:8090"
            string configString = @"
                akka {  
                    actor{
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""                        
                    }
                    remote {
                        helios.tcp {
		                    port = 0
		                    hostname = localhost
                        }
                    }
                }";

            //configString = configString.Replace("@TARGET", $"akka.tcp://{akkaSystemName}@localhost:8090");
            this.akkaSystemName = akkaSystemName;
            this.akkaNodes = akkaNodes;
            this.numOfNodes = akkaNodes.Length;

            this.actorSystem = ActorSystem.Create(this.akkaSystemName, ConfigurationFactory.ParseString(configString));
        }


        public override IScore Run(double[][] data, IContext ctx)
        {
            // Number of slots inside of weight matrix. It specifies in how many
            // slots (parts) the matrix will be split to execute calculation.
            int numOfSlots = 5;

            var trainingData = data.Take((int)(data.Length * 0.8)).ToArray();
            var numOfInputVectors = data.Length;
            int i = 0;

            while (i < numOfInputVectors)
            {
                List<Task> tasks = new List<Task>();

                for (int slot = 0; slot < numOfSlots; slot++)
                {
                    // Here we make sure that all actor are shared accross all specified node.
                    string targetUri = this.akkaNodes[i % this.numOfNodes];
                    var remoteAddress = Address.Parse(targetUri);

                    var remoteBackPropagationActor =
                    this.actorSystem.ActorOf(Props.Create(() => new BackPropagationActor(m_Biases, m_HiddenLayerNeurons, m_OutputLayerNeurons, m_InpDims, numOfInputVectors / this.numOfNodes))
                    .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), $"bp{++i}");
                }

                for (int slot = 0; slot < numOfSlots; slot++)
                {
                    // In this loop, we have to setup BackPropActorIn with all required parameters.
                    // This is the place to provide all input vector with all weights.
                    tasks.Add(this.actorSystem.ActorSelection($"/user/bp{slot}").Ask<BackPropActorOut>(new BackPropActorIn() { }));
                }
                while (true)
                {
                    Thread.Sleep(500);
                }
                Task.WaitAll(tasks.ToArray());

                //
                // Here is the place to collect results from all actors,
                // which have partially calculated results.
                foreach (var task in tasks)
                {

                }

            }

            //List<Task> tasks = new List<Task>();
            //for (int k = 0; k < numOfInputVectors; k++)
            //{
            //    // In this loop, we have to setup BackPropActorIn with all required parameters.
            //    // This is the place to provide all input vector with all weights.
            //    tasks.Add(this.actorSystem.ActorSelection($"/user/bp{k}").Ask<BackPropActorOut>(new BackPropActorIn() { }));
            //}

            //Task.WaitAll(tasks.ToArray());



            return null;
        }
    }
}
