using LearningFoundation;
using MLPerceptron;
using MLPerceptron.NeuralNetworkCore;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xunit;
using System.Diagnostics;
using NeuralNet.MLPerceptron;
using ImageBinarizer;
using System.Globalization;
using AkkaMLPerceptron;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;

namespace test.MLPerceptron
{
    /// <summary>
    /// 
    /// </summary>
    public class AkkaMLPUnitTests
    {
        private static string clusterSystemName = "ClusterSystem";

        private static string[] akkaNodes;

        /// <summary>
        /// 
        /// </summary>
        static AkkaMLPUnitTests()
        {
            //akkaNodes = new string[] { $"akka.tcp://{clusterSystemName}@dado-sr1:8081", $"akka.tcp://{clusterSystemName}@DADO-SR1:8082" };

            //akkaNodes = new string[] { $"akka.tcp://{clusterSystemName}@localhost:8081", $"akka.tcp://{clusterSystemName}@localhost:8082" };

            akkaNodes = new string[] { $"akka.tcp://{clusterSystemName}@akkahost1.westeurope.azurecontainer.io:8081", $"akka.tcp://{clusterSystemName}@akkahost2.westeurope.azurecontainer.io:8081" };

        }

        private static ActorSystem runAkkaSystem()
        {

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


            return ActorSystem.Create(clusterSystemName, ConfigurationFactory.ParseString(configString));
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void UnitTestZero()
        {

            AkaMLPerceptronAlgorithm alg = new AkaMLPerceptronAlgorithm(clusterSystemName, akkaNodes);

            List<double[]> data = new List<double[]>();

            for (int i = 0; i < 10; i++)
            {
                int numFeatures = 1024;
                double[] features = new double[numFeatures];

                for (int j = 0; i < numFeatures; i++)
                {
                    features[j] = i;
                }

                data.Add(features);
            }

            alg.Run(data.ToArray(), null);


        }


        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void DeployManyActorsTest()
        {
            List<double[]> data = new List<double[]>();
            List<Task> tasks = new List<Task>();
            int numActors = 100;

            var sys = runAkkaSystem();

            for (int n = 0; n < 100; n++)
            {
                for (int actorId = 0; actorId < numActors; actorId++)
                {
                    string targetUri = akkaNodes[actorId % akkaNodes.Length];
                    //string targetUri = akkaNodes[0];
                    var remoteAddress = Address.Parse(targetUri);

                    var remoteBackPropagationActor = sys.ActorOf(Props.Create(() => new UnitTestActor())
                    .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), $"ut{actorId}-{n}");
                }

                for (int actorId = 0; actorId < numActors; actorId++)
                {
                    tasks.Add(sys.ActorSelection($"/user/ut{actorId}-{n}").Ask<string>($"[{actorId}-{n}", TimeSpan.FromSeconds(60)));
                }

                Task.WaitAll(tasks.ToArray());

                Debug.WriteLine(n);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void PingAciTest()
        {
            List<double[]> data = new List<double[]>();
            List<Task> tasks = new List<Task>();

            var sys = runAkkaSystem();
            
            for (int actorId = 0; actorId < 5; actorId++)
            {
                string targetUri = akkaNodes[actorId % akkaNodes.Length];
                //string targetUri = akkaNodes[0];

                var remoteAddress = Address.Parse(targetUri);

                var remoteBackPropagationActor = sys.ActorOf(Props.Create(() => new UnitTestActor())
                .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), $"ut{actorId}");
            }

            for (int actorId = 0; actorId < 5; actorId++)
            {
                tasks.Add(sys.ActorSelection($"/user/ut{actorId}").Ask<string>($"[{actorId}", TimeSpan.FromSeconds(30)));
            }

            Task.WaitAll(tasks.ToArray());

        }
    }


    public class UnitTestActor : ReceiveActor
    {
        public UnitTestActor()
        {
            Receive<string>(new Action<string>(Compute));
        }

        protected override void PreStart()
        {
            Console.WriteLine($"Started Actor: {Context.Self.Path}");

            base.PreStart();
        }

        protected void Compute(string msg)
        {
            Console.WriteLine($"Entered calculation: {Context.Self.Path}");

            Thread.Sleep(2500);

            Sender.Tell($"{msg} ]");
        }
    }
}






