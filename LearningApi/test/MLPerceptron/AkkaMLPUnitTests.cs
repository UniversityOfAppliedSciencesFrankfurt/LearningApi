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

            akkaNodes = new string[] { $"akka.tcp://{clusterSystemName}@localhost:8081", $"akka.tcp://{clusterSystemName}@localhost:8082" };

            //akkaNodes = new string[] { $"akka.tcp://{clusterSystemName}@akkahost1.westeurope.azurecontainer.io:8081", $"akka.tcp://{clusterSystemName}@akkahost2.westeurope.azurecontainer.io:8081" };

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
            int[] hidLay = new int[] { 12, 16, 19 };

            AkaMLPerceptronAlgorithm alg = new AkaMLPerceptronAlgorithm(clusterSystemName, akkaNodes, 0.1, 50, 5, hidLay);

            List<double[]> data = new List<double[]>();

            for (int i = 1; i <= 10; i++)
            {
                int numFeatures = 16;
                double[] features = new double[numFeatures];

                for (int j = 0; j < numFeatures; j++)
                {
                    features[j] = j + 1;
                }

                data.Add(features);
            }

            alg.Run(data.ToArray(), null);


        }

        [Theory]
        //[InlineData(new int[] { 5, 2 })]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32, 32, 32, 32, 32 }, 10)]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32, 32, 32, 32 }, 9)]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32, 32, 32 }, 8)]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32, 32 }, 7)]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32, 32 }, 6)]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32, 32 }, 5)]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32, 32, 32 }, 4)]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32, 32 }, 3)]
        //[InlineData(25, 0.01, 128, new int[] { 32, 32 }, 2)]
        [InlineData(5, 0.01, 100, new int[] { 64, 64, 64 })]

        public void UnitTestMNIST(int iterations, double learningrate, int batchSize, int[] hiddenLayerNeurons)
        {
            Thread.Sleep(15000);

            MNISTFileRead mnistObj = new MNISTFileRead();

            MNISTFileRead.ReadMNISTTrainingData();

            MNISTFileRead.ReadMNISTTestData();

            // TODO
            // test by using same test cases in dependence on number of iterations.
            // test by dependence on number of layers
            // test by dependence on number of neurons in layers.
            // [2 layers]: [x,x] [1,2] [1,3] [1,4]

            // Read the csv file which contains the training data

            int numberOfOutputs = 10;

            LearningApi api = new LearningApi();

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                ctx.DataDescriptor = new DataDescriptor();

                ctx.DataDescriptor.Features = new LearningFoundation.DataMappers.Column[MNISTFileRead.inputFeaturesAndLabelValues.Length - 1];

                for (int i = 0; i < (MNISTFileRead.inputFeaturesAndLabelValues.Length - 1); i++)
                {
                    ctx.DataDescriptor.Features[i] = new LearningFoundation.DataMappers.Column
                    {
                        Id = i,
                        Name = MNISTFileRead.inputFeaturesAndLabelValues[i],
                        Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                        Index = i,
                    };
                }

                ctx.DataDescriptor.LabelIndex = MNISTFileRead.inputFeaturesAndLabelValues.Length - 1;

                return MNISTFileRead.trainingData;
            });

            //int[] hiddenLayerNeurons = { 6 };
            // Invoke the MLPerecptronAlgorithm with a specific learning rate, number of iterations
            api.UseAkkaMLPerceptron(clusterSystemName, akkaNodes, learningrate, iterations, batchSize, hiddenLayerNeurons);

            IScore score = api.Run() as IScore;

            // Invoke the Predict method to predict the results on the test data
            var result = ((MLPerceptronResult)api.Algorithm.Predict(MNISTFileRead.testData, api.Context)).results;

            //Create file to store the test data results
            StreamWriter resultFile = new StreamWriter($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\mnist_result.csv");

            double[] tempResultArray = new double[10];

            int index = 0;

            // Update the predictedResult file with the predicted results on the test dataset
            while (index < result.Length)
            {
                for (int i = index; i < index + numberOfOutputs; i++)
                {
                    tempResultArray[i - index] = result[i];
                }

                double max = tempResultArray.Max();

                resultFile.WriteLine(Array.IndexOf(tempResultArray, max));

                index = index + numberOfOutputs;
            }

            int numberOfCorrectClassifications = 0;

            // Calculate the number of test data elements that have been correctly classified
            for (int i = 0; i < MNISTFileRead.testData.Length; i++)
            {
                numberOfCorrectClassifications++;

                for (int j = 0; j < numberOfOutputs; j++)
                {
                    if (MNISTFileRead.testData[i][(MNISTFileRead.testData[i].Length - numberOfOutputs) + j] != (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0))
                    {
                        numberOfCorrectClassifications--;
                        break;
                    }
                }
            }

            Debug.WriteLine($"Number of correct classifications: {numberOfCorrectClassifications}");

            //Calculate accuracy by using the following formula:
            // accuracy = numberOfCorrectClassifications/numberOfTestDataElements
            double accuracy = ((double)numberOfCorrectClassifications * numberOfOutputs) / result.Length;
            resultFile.WriteLine("Accuracy = {0}", accuracy.ToString());

            resultFile.Close();

        }

        /*
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
        */
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






