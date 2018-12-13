using Akka.Actor;
using Akka.Configuration;
using LearningFoundation;
using NeuralNetworks.Core;
using System;
using System.Linq;


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
        public AkaMLPerceptronAlgorithm(string akkaSystemName, int nnumOfNode)
        {
            // akka.tcp://DeployTarget@localhost:8090"
            string configString = @"
                akka {  
                    actor{
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        deployment {
                            /remoteecho {
                                remote = ""@TARGET""
                            }
                        }
                    }
                    remote {
                        helios.tcp {
		                    port = 0
		                    hostname = localhost
                        }
                    }
                }";

            configString = configString.Replace("@TARGET", akkaSystemName);

            this.actorSystem = ActorSystem.Create("Deployer", ConfigurationFactory.ParseString(configString));
        }


        public override IScore Run(double[][] data, IContext ctx)
        {
            int miniBatchStartIndex = 0;
            var trainingData = data.Take((int)(data.Length * 0.8)).ToArray();
            var numOfInputVectors = 5;

            while (miniBatchStartIndex < numOfInputVectors)
            {
                string targetUri = "akka.tcp://@TARGET@localhost:8090";
                var remoteAddress = Address.Parse(targetUri.Replace("@TARGET", this.akkaSystemName));
                var remoteBackPropagationActor =
                     this.actorSystem.ActorOf(
                         Props.Create(() => new BackPropagationActor(m_Biases, m_HiddenLayerNeurons,
                         m_OutputLayerNeurons, m_InpDims, numOfInputVectors / this.numOfNodes))
                             .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), "remotebackpropagation");

                this.actorSystem.ActorSelection("/user/remoteecho").Tell(new BackPropActorIn() { });
            }

            return null;
        }
    }
}
