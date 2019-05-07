using Akka.Actor;
using Akka.Configuration;
using LearningFoundation;
using NeuralNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using NeuralNet.MLPerceptron;
using MLPerceptron;

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

        public int[] m_HiddenLayerNeurons = { 16, 20, 24 };

        public int m_OutputLayerNeurons;

        public int m_Iterations = 10000;

        public int m_batchSize = 1;

        private Func<double, double> m_ActivationFunction = MLPerceptron.NeuralNetworkCore.ActivationFunctions.HyperbolicTan;

        public int m_InpDims;

        public double[][,] m_Weights;

        public double[][] m_Biases;

        #endregion
        public AkaMLPerceptronAlgorithm(string akkaSystemName, string[] akkaNodes, double learningRate, int iterations, int batchSize, int[] hiddenLayerNeurons)
        {
            this.m_LearningRate = learningRate;

            this.m_Iterations = iterations;

            this.m_batchSize = batchSize;

            if (hiddenLayerNeurons != null)
            {
                this.m_HiddenLayerNeurons = hiddenLayerNeurons;
            }

            if (akkaNodes == null || akkaNodes.Length == 0)
                throw new ArgumentException("Cluster nodes must be specified.");


            // akka.tcp://DeployTarget@localhost:8090"
            string configString = @"
                akka {  
                    actor{
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""                        
                    }
                    remote {
                        maximum-payload-bytes = 30000000 bytes
                        helios.tcp {
		                    port = 0
		                    hostname = localhost
                            message-frame-size =  30000000b
                            send-buffer-size =  30000000b
                            receive-buffer-size =  30000000b
                            maximum-frame-size =   30000000b
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
            int numberOfHiddenAndOutputLayers = m_HiddenLayerNeurons.Length + 1;

            // Sum for every layer. hidLyrNeuronSum1 = x11*w11+x12*w21+..+x1N*wN1
            double[][,] hidLyrNeuronSum = new double[numberOfHiddenAndOutputLayers][,];

            // outputs = ActFnx(hidLyrNeuronSum+Bias)
            double[][,] hidLyrOut = new double[numberOfHiddenAndOutputLayers][,];

            // Utilize 80% of training data for training and 20% for validation for every epoch
            double[][] trainingData = new double[(int)(data.Length * 0.8)][];

            double[][] validationData = new double[(int)(data.Length * 0.2)][];

            trainingData = data.Take((int)(data.Length * 0.8)).ToArray();

            validationData = data.Skip((int)(data.Length * 0.8)).ToArray();

            //double[][] trainingData = new double[data.Length][];

            //trainingData = data;

            int numOfInputVectors = trainingData.Length;

            m_InpDims = ctx.DataDescriptor.Features.Count();

            m_OutputLayerNeurons = data[0].Length - m_InpDims;

            double[][,] errors = new double[numberOfHiddenAndOutputLayers][,];

            double[][,] costChangeWeights = new double[numberOfHiddenAndOutputLayers][,];

            double[][] costChangeBiases = new double[numberOfHiddenAndOutputLayers][];

            m_Weights = new double[numberOfHiddenAndOutputLayers][,];

            m_Biases = new double[numberOfHiddenAndOutputLayers][];

            for (int layer = 0; layer < numberOfHiddenAndOutputLayers; layer++)
            {
                if (layer == 0)
                {
                    hidLyrOut[layer] = new double[m_batchSize, m_HiddenLayerNeurons[layer]];
                    hidLyrNeuronSum[layer] = new double[m_batchSize, m_HiddenLayerNeurons[layer]];
                    costChangeBiases[layer] = new double[m_HiddenLayerNeurons[layer]];
                    costChangeWeights[layer] = new double[m_HiddenLayerNeurons[layer], m_InpDims];
                    errors[layer] = new double[m_batchSize, m_HiddenLayerNeurons[layer]];
                }
                else if (layer < numberOfHiddenAndOutputLayers - 1)
                {
                    hidLyrOut[layer] = new double[m_batchSize, m_HiddenLayerNeurons[layer]];
                    hidLyrNeuronSum[layer] = new double[m_batchSize, m_HiddenLayerNeurons[layer]];
                    costChangeBiases[layer] = new double[m_HiddenLayerNeurons[layer]];
                    costChangeWeights[layer] = new double[m_HiddenLayerNeurons[layer], m_HiddenLayerNeurons[layer - 1]];
                    errors[layer] = new double[m_batchSize, m_HiddenLayerNeurons[layer]];
                }
                else
                {
                    hidLyrOut[layer] = new double[m_batchSize, m_OutputLayerNeurons];
                    hidLyrNeuronSum[layer] = new double[m_batchSize, m_OutputLayerNeurons];
                    costChangeBiases[layer] = new double[m_OutputLayerNeurons];
                    costChangeWeights[layer] = new double[m_OutputLayerNeurons, m_HiddenLayerNeurons[layer - 1]];
                    errors[layer] = new double[m_batchSize, m_OutputLayerNeurons];
                }
            }

            // Initialize the weights and biases at every layer of the neural network
            InitializeWeightsandBiasesinputlayer(m_InpDims);

            InitializeWeightsandBiaseshiddenlayers(m_HiddenLayerNeurons);

            InitializeWeightsandBiasesoutputlayer(m_HiddenLayerNeurons);

            var score = new MLPerceptronAlgorithmScore();

            double lastLoss = 0;

            double lastValidationLoss = 0;

            string path = Directory.GetCurrentDirectory() + "\\wmnist_performance_params.csv";

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }


            IActorRef remoteBackPropagationActor;
            // Number of slots inside of weight matrix. It specifies in how many
            // slots (parts) the matrix will be split to execute calculation.
            int numOfSlots = 2;

            List<Task> tasks = new List<Task>();

            for (int slot = 0; slot < numOfSlots; slot++)
            {
                // Here we make sure that all actor are shared accross all specified node.
                string targetUri = this.akkaNodes[slot % this.numOfNodes];
                var remoteAddress = Address.Parse(targetUri);

                remoteBackPropagationActor =
                this.actorSystem.ActorOf(Props.Create(() => new BackPropagationActor(this.m_HiddenLayerNeurons, this.m_OutputLayerNeurons, this.m_InpDims, this.m_LearningRate))
                .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), $"bp{slot}");
            }

            int[][] NumOfNeuronsForCurrentActor = new int[numOfSlots][];
            int[][] StartIndex = new int[numOfSlots][];

            for (int i = 0; i < numOfSlots; i++)
            {
                NumOfNeuronsForCurrentActor[i] = new int[numberOfHiddenAndOutputLayers];
                StartIndex[i] = new int[numberOfHiddenAndOutputLayers];
            }


            for (int layerNum = 0; layerNum < numberOfHiddenAndOutputLayers; layerNum++)
            {
                if (layerNum == numberOfHiddenAndOutputLayers - 1)
                {
                    for (int i = 0; i < numOfSlots - 1; i++)
                    {
                        if (i == 0)
                        {
                            NumOfNeuronsForCurrentActor[i][layerNum] = m_OutputLayerNeurons / numOfSlots;
                            StartIndex[i][layerNum] = i;
                        }
                        else
                        {
                            NumOfNeuronsForCurrentActor[i][layerNum] = m_OutputLayerNeurons / numOfSlots;
                            StartIndex[i][layerNum] = i * NumOfNeuronsForCurrentActor[i - 1][layerNum];
                        }
                    }

                    if (numOfSlots == 1)
                    {
                        NumOfNeuronsForCurrentActor[numOfSlots - 1][layerNum] = (m_OutputLayerNeurons / numOfSlots) + (m_OutputLayerNeurons % numOfSlots);
                        StartIndex[numOfSlots - 1][layerNum] = (numOfSlots - 1);
                    }
                    else
                    {
                        NumOfNeuronsForCurrentActor[numOfSlots - 1][layerNum] = (m_OutputLayerNeurons / numOfSlots) + (m_OutputLayerNeurons % numOfSlots);
                        StartIndex[numOfSlots - 1][layerNum] = (numOfSlots - 1) * NumOfNeuronsForCurrentActor[numOfSlots - 2][layerNum];
                    }
                }
                else
                {

                    for (int i = 0; i < numOfSlots - 1; i++)
                    {
                        if (i == 0)
                        {
                            NumOfNeuronsForCurrentActor[i][layerNum] = m_HiddenLayerNeurons[layerNum] / numOfSlots;
                            StartIndex[i][layerNum] = i;
                        }
                        else
                        {
                            NumOfNeuronsForCurrentActor[i][layerNum] = m_HiddenLayerNeurons[layerNum] / numOfSlots;
                            StartIndex[i][layerNum] = i * NumOfNeuronsForCurrentActor[i - 1][layerNum];
                        }

                    }

                    if (numOfSlots == 1)
                    {
                        NumOfNeuronsForCurrentActor[numOfSlots - 1][layerNum] = (m_HiddenLayerNeurons[layerNum] / numOfSlots) + (m_HiddenLayerNeurons[layerNum] % numOfSlots);
                        StartIndex[numOfSlots - 1][layerNum] = (numOfSlots - 1);
                    }
                    else
                    {
                        NumOfNeuronsForCurrentActor[numOfSlots - 1][layerNum] = (m_HiddenLayerNeurons[layerNum] / numOfSlots) + (m_HiddenLayerNeurons[layerNum] % numOfSlots);
                        StartIndex[numOfSlots - 1][layerNum] = (numOfSlots - 1) * NumOfNeuronsForCurrentActor[numOfSlots - 2][layerNum];
                    }
                }
            }

            using (var performanceData = new StreamWriter(path))
            {
                Stopwatch watch = new Stopwatch();

                double timeElapsed = 0;

                performanceData.WriteLine("{0},{1},{2},{3},{4},{5}", "Epoch", "Epoch Loss", "Epoch Accuracy", "Validation Loss", "Validation Accuracy", "Time Elapsed");

                for (int iterNum = 0; iterNum < m_Iterations; iterNum++)
                {
                    watch.Restart();

                    score.Loss = 0;

                    double batchAccuracy = 0;

                    int miniBatchStartIndex = 0;

                    double[][] batchInputData = new double[this.m_batchSize][];

                    for (int i = 0; i < this.m_batchSize; i++)
                    {
                        batchInputData[i] = new double[this.m_InpDims];
                    }

                    for (int inputVectIndx = miniBatchStartIndex; inputVectIndx < numOfInputVectors; inputVectIndx = inputVectIndx + m_batchSize)
                    {
                        batchInputData = trainingData.Skip((int)inputVectIndx).Take((int)this.m_batchSize).ToArray();

                        tasks.Clear();

                        for (int slot = 0; slot < numOfSlots; slot++)
                        {
                            // In this loop, we have to setup BackPropActorIn with all required parameters.
                            // This is the place to provide all input vector with all weights.
                            tasks.Add(this.actorSystem.ActorSelection($"/user/bp{slot}").Ask<ForwardPropActorOut>(new FirstHiddenLayerForwardPropActorIn() { actorNum = slot, batchSize = m_batchSize, batchIndex = inputVectIndx, StartIndex = StartIndex[slot][0], NeuronsCurrentActor = NumOfNeuronsForCurrentActor[slot][0], Input = batchInputData, PreviousLayerBiases = this.m_Biases[0], PrevLayerWeights = this.m_Weights[0] }));
                        }

                        Task.WaitAll(tasks.ToArray());

                        foreach (var task in tasks)
                        {
                            var actorResult = ((Task<ForwardPropActorOut>)task).Result;

                            for (int batchIndex = 0; batchIndex < m_batchSize; batchIndex++)
                            {
                                for (int index = 0; index < actorResult.LayerOutput.GetLength(1); index++)
                                {
                                    hidLyrOut[0][batchIndex, index + StartIndex[actorResult.actorNum][0]] = actorResult.LayerOutput[batchIndex, index];
                                    hidLyrNeuronSum[0][batchIndex, index + StartIndex[actorResult.actorNum][0]] = actorResult.LayerNeuronSum[batchIndex, index];
                                }
                            }
                        }

                        for (int layer = 1; layer < numberOfHiddenAndOutputLayers; layer++)
                        {
                            tasks.Clear();

                            for (int slot = 0; slot < numOfSlots; slot++)
                            {
                                // In this loop, we have to setup BackPropActorIn with all required parameters.
                                // This is the place to provide all input vector with all weights.
                                tasks.Add(this.actorSystem.ActorSelection($"/user/bp{slot}").Ask<ForwardPropActorOut>(new RemainingLayersForwardPropActorIn() { batchIndex = inputVectIndx, batchSize = m_batchSize, PreviousLayerBiases = this.m_Biases[layer], PrevLayerWeights = this.m_Weights[layer], actorNum = slot, isOutputLayer = (layer == m_HiddenLayerNeurons.Length ? true : false), NeuronsCurrentActor = NumOfNeuronsForCurrentActor[slot][layer], StartIndex = StartIndex[slot][layer], Input = hidLyrOut[layer - 1] }));
                            }

                            Task.WaitAll(tasks.ToArray());

                            foreach (var task in tasks)
                            {
                                var actorResult = ((Task<ForwardPropActorOut>)task).Result;

                                for (int batchIndex = 0; batchIndex < m_batchSize; batchIndex++)
                                {
                                    if (layer != m_HiddenLayerNeurons.Length)
                                    {
                                        for (int index = 0; index < actorResult.LayerNeuronSum.GetLength(1); index++)
                                        {
                                            hidLyrOut[layer][batchIndex, index + StartIndex[actorResult.actorNum][layer]] = actorResult.LayerOutput[batchIndex, index];
                                            hidLyrNeuronSum[layer][batchIndex, index + StartIndex[actorResult.actorNum][layer]] = actorResult.LayerNeuronSum[batchIndex, index];
                                        }
                                    }
                                    else
                                    {
                                        for (int index = 0; index < actorResult.LayerNeuronSum.GetLength(1); index++)
                                        {
                                            hidLyrNeuronSum[layer][batchIndex, index + StartIndex[actorResult.actorNum][layer]] = actorResult.LayerNeuronSum[batchIndex, index];
                                        }

                                    }
                                }
                            }

                            if (layer == m_HiddenLayerNeurons.Length)
                            {
                                hidLyrOut[layer] = MLPerceptron.NeuralNetworkCore.ActivationFunctions.SoftMaxClassifierTrain(hidLyrNeuronSum[layer], this.m_batchSize);
                            }
                        }

                        tasks.Clear();

                        //
                        // Error calculation starting at last layer.
                        //

                        for (int slot = 0; slot < numOfSlots; slot++)
                        {
                            // In this loop, we have to setup BackPropActorIn with all required parameters.
                            // This is the place to provide all input vector with all weights.
                            tasks.Add(this.actorSystem.ActorSelection($"/user/bp{slot}").Ask<BackPropActorOut>(new OutputLayerBackPropActorIn() { batchIndex = inputVectIndx, batchSize = m_batchSize, Input = batchInputData, actorNum = slot, LayerOutput = hidLyrOut[m_HiddenLayerNeurons.Length], PrevLayerOutput = hidLyrOut[m_HiddenLayerNeurons.Length - 1], NeuronsCurrentActor = NumOfNeuronsForCurrentActor[slot][m_HiddenLayerNeurons.Length], StartIndex = StartIndex[slot][m_HiddenLayerNeurons.Length] }));
                        }

                        bool[] accuracyArr = new bool[this.m_batchSize];

                        for (int i = 0; i < this.m_batchSize; i++)
                        {
                            accuracyArr[i] = true;
                        }

                        Task.WaitAll(tasks.ToArray());

                        foreach (var task in tasks)
                        {
                            var actorResult = ((Task<BackPropActorOut>)task).Result;

                            for (int batchIndex = 0; batchIndex < m_batchSize; batchIndex++)
                            {
                                accuracyArr[batchIndex] = accuracyArr[batchIndex] && actorResult.result[batchIndex];

                                for (int index = 0; index < actorResult.Errors.GetLength(1); index++)
                                {
                                    errors[numberOfHiddenAndOutputLayers - 1][batchIndex, index + StartIndex[actorResult.actorNum][numberOfHiddenAndOutputLayers - 1]] = actorResult.Errors[batchIndex, index];
                                    costChangeBiases[numberOfHiddenAndOutputLayers - 1][index + StartIndex[actorResult.actorNum][numberOfHiddenAndOutputLayers - 1]] = actorResult.CostChangeDueToBiases[index];
                                    for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < hidLyrOut[m_HiddenLayerNeurons.Length - 1].GetLength(1); prevHidLayerOutputIndx++)
                                        costChangeWeights[numberOfHiddenAndOutputLayers - 1][index + StartIndex[actorResult.actorNum][numberOfHiddenAndOutputLayers - 1], prevHidLayerOutputIndx] = actorResult.CostChangeDueToWeights[index, prevHidLayerOutputIndx];
                                }
                            }
                        }

                        batchAccuracy += ((double)accuracyArr.Count(c => c) / m_batchSize);

                        Debug.WriteLine($"Batch Accuracy = {batchAccuracy}");

                        //
                        // Propagating error from last hidden to first layer
                        //

                        for (int layer = m_HiddenLayerNeurons.Length - 1; layer >= 0; layer--)
                        {
                            if (layer == 0)
                            {
                                tasks.Clear();

                                for (int slot = 0; slot < numOfSlots; slot++)
                                {
                                    // In this loop, we have to setup BackPropActorIn with all required parameters.
                                    // This is the place to provide all input vector with all weights.
                                    tasks.Add(this.actorSystem.ActorSelection($"/user/bp{slot}").Ask<BackPropActorOut>(new FirstHiddenLayerBackPropActorIn() { batchIndex = inputVectIndx, batchSize = m_batchSize, actorNum = slot, FollLayerWeights = m_Weights[layer + 1], LayerSum = hidLyrNeuronSum[layer], PrevLayerOutput = batchInputData, NeuronsCurrentActor = NumOfNeuronsForCurrentActor[slot][layer], StartIndex = StartIndex[slot][layer], FollLayerErrors = errors[layer + 1] }));
                                }

                                Task.WaitAll(tasks.ToArray());

                                foreach (var task in tasks)
                                {
                                    var actorResult = ((Task<BackPropActorOut>)task).Result;

                                    for (int batchIndex = 0; batchIndex < m_batchSize; batchIndex++)
                                    {
                                        for (int index = 0; index < actorResult.Errors.GetLength(1); index++)
                                        {
                                            errors[layer][batchIndex, index + StartIndex[actorResult.actorNum][layer]] = actorResult.Errors[batchIndex, index];
                                            costChangeBiases[layer][index + StartIndex[actorResult.actorNum][layer]] = actorResult.CostChangeDueToBiases[index];
                                            for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < m_InpDims; prevHidLayerOutputIndx++)
                                                costChangeWeights[layer][index + StartIndex[actorResult.actorNum][layer], prevHidLayerOutputIndx] = actorResult.CostChangeDueToWeights[index, prevHidLayerOutputIndx];
                                        }

                                    }
                                }
                            }
                            else
                            {
                                tasks.Clear();

                                for (int slot = 0; slot < numOfSlots; slot++)
                                {
                                    // In this loop, we have to setup BackPropActorIn with all required parameters.
                                    // This is the place to provide all input vector with all weights.
                                    tasks.Add(this.actorSystem.ActorSelection($"/user/bp{slot}").Ask<BackPropActorOut>(new HiddenLayersBackPropActorIn() { batchIndex = inputVectIndx, batchSize = m_batchSize, actorNum = slot, FollLayerWeights = m_Weights[layer + 1], LayerSum = hidLyrNeuronSum[layer], PrevLayerOutput = hidLyrOut[layer - 1], NeuronsCurrentActor = NumOfNeuronsForCurrentActor[slot][layer], StartIndex = StartIndex[slot][layer], FollLayerErrors = errors[layer + 1] }));
                                }

                                Task.WaitAll(tasks.ToArray());

                                foreach (var task in tasks)
                                {
                                    var actorResult = ((Task<BackPropActorOut>)task).Result;

                                    for (int batchIndex = 0; batchIndex < m_batchSize; batchIndex++)
                                    {
                                        for (int index = 0; index < actorResult.Errors.GetLength(1); index++)
                                        {
                                            errors[layer][batchIndex, index + StartIndex[actorResult.actorNum][layer]] = actorResult.Errors[batchIndex, index];
                                            costChangeBiases[layer][index + StartIndex[actorResult.actorNum][layer]] = actorResult.CostChangeDueToBiases[index];
                                            for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < hidLyrOut[layer - 1].GetLength(1); prevHidLayerOutputIndx++)
                                                costChangeWeights[layer][index + StartIndex[actorResult.actorNum][layer], prevHidLayerOutputIndx] = actorResult.CostChangeDueToWeights[index, prevHidLayerOutputIndx];
                                        }

                                    }
                                }
                            }
                        }

                        tasks.Clear();

                        //
                        // Updating weights
                        // 
                        for (int slot = 0; slot < numOfSlots; slot++)
                        {
                            // In this loop, we have to setup BackPropActorIn with all required parameters.
                            // This is the place to provide all input vector with all weights.
                            tasks.Add(this.actorSystem.ActorSelection($"/user/bp{slot}").Ask<WeightUpdateBackPropActorOut>(new WeightUpdateBackPropActorIn() { batchIndex = inputVectIndx, batchSize = m_batchSize, actorNum = slot, CostChangeDueToWeights = costChangeWeights, currentweights = m_Weights, NeuronsCurrentActor = NumOfNeuronsForCurrentActor[slot], StartIndex = StartIndex[slot], hidLayersOutputs = hidLyrOut }));
                        }

                        Task.WaitAll(tasks.ToArray());

                        foreach (var task in tasks)
                        {
                            var actorResult = ((Task<WeightUpdateBackPropActorOut>)task).Result;

                            for (int layer = 0; layer <= m_HiddenLayerNeurons.Length; layer++)
                            {
                                for (int neuron = 0; neuron < actorResult.newweights[layer].GetLength(0); neuron++)
                                {
                                    for (int prevLayerNeuron = 0; prevLayerNeuron < actorResult.newweights[layer].GetLength(1); prevLayerNeuron++)
                                    {
                                        this.m_Weights[layer][neuron + StartIndex[actorResult.actorNum][layer], prevLayerNeuron] = actorResult.newweights[layer][neuron, prevLayerNeuron];
                                    }
                                }


                            }

                        }

                        tasks.Clear();

                        for (int slot = 0; slot < numOfSlots; slot++)
                        {
                            // while(batch)
                            //        tasks.Add(slotActor.Tell(uploadBatch))
                            //        tasks.Add(slotActor.Ask<BiasUpdateBackPropActorOut>(new BiasUpdateBackPropActorIn() { batchIndex = inputVectIndx, batchSize = m_batchSize, actorNum = slot, CostChangeDueToBiases = costChangeBiases, currentbiases = m_Biases, NeuronsCurrentActor = NumOfNeuronsForCurrentActor[slot], StartIndex = StartIndex[slot] }));



                            // In this loop, we have to setup BackPropActorIn with all required parameters.
                            // This is the place to provide all input vector with all weights.
                            tasks.Add(this.actorSystem.ActorSelection($"/user/bp{slot}").Ask<BiasUpdateBackPropActorOut>(new BiasUpdateBackPropActorIn() { batchIndex = inputVectIndx, batchSize = m_batchSize, actorNum = slot, CostChangeDueToBiases = costChangeBiases, currentbiases = m_Biases, NeuronsCurrentActor = NumOfNeuronsForCurrentActor[slot], StartIndex = StartIndex[slot] }));
                        }

                        Task.WaitAll(tasks.ToArray());

                        foreach (var task in tasks)
                        {
                            var actorResult = ((Task<BiasUpdateBackPropActorOut>)task).Result;

                            for (int layer = 0; layer <= m_HiddenLayerNeurons.Length; layer++)
                            {
                                for (int neuron = 0; neuron < actorResult.newbiases[layer].Length; neuron++)
                                {
                                    this.m_Biases[layer][neuron + StartIndex[actorResult.actorNum][layer]] = actorResult.newbiases[layer][neuron];
                                }
                            }
                        }

                        score.Errors = costChangeBiases[m_HiddenLayerNeurons.Length];

                        double sum = 0;

                        foreach (var outLyrErr in score.Errors)
                        {
                            sum += outLyrErr;
                        }

                        /*
                        1 - mean of errors
                        score.Loss = 1 - (Math.Abs(sum) / score.Errors.Length);
                        */

                        score.Loss += Math.Abs(sum);
                    }

                    double deltaLoss = lastLoss - score.Loss;

                    double accuracy = ((double)batchAccuracy * m_batchSize) / numOfInputVectors;

                    var result = ((MLPerceptronResult)Predict(validationData, ctx)).results;

                    int accurateResults = 0;

                    double validationSetLoss = 0.0;

                    // Check if the test data has been correctly classified by the neural network
                    for (int j = 0; j < validationData.Length; j++)
                    {
                        accurateResults++;

                        for (int k = 0; k < m_OutputLayerNeurons; k++)
                        {
                            validationSetLoss += Math.Abs(validationData[j][(validationData[j].Length - m_OutputLayerNeurons) + k] - result[j * m_OutputLayerNeurons + k]);

                            //Assert.True(testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0));
                            if (validationData[j][(validationData[j].Length - m_OutputLayerNeurons) + k] != (result[j * m_OutputLayerNeurons + k] >= 0.5 ? 1 : 0))
                            {
                                accurateResults--;
                                break;
                            }
                        }
                    }

                    double deltaValidationLoss = lastValidationLoss - validationSetLoss;

                    double validationAccuracy = (double)accurateResults / validationData.Length;

                    watch.Stop();

                    timeElapsed += ((double)watch.ElapsedMilliseconds / 1000);

                    Debug.WriteLine($"Loss: {score.Loss}, Last loss: {lastLoss}, Delta: {deltaLoss}, Accuracy: {accuracy}, ValidationLoss: {validationSetLoss}, Last Validationloss: {lastValidationLoss}, Delta: {deltaValidationLoss}, ValidationAccuracy: {validationAccuracy}, TimeElapsed: {timeElapsed}");

                    performanceData.WriteLine("{0},{1},{2},{3},{4},{5}", iterNum.ToString(), score.Loss.ToString("F3", CultureInfo.InvariantCulture), accuracy.ToString("F3", CultureInfo.InvariantCulture), validationSetLoss.ToString("F3", CultureInfo.InvariantCulture), validationAccuracy.ToString("F3", CultureInfo.InvariantCulture), timeElapsed.ToString("F3", CultureInfo.InvariantCulture));

                    lastLoss = score.Loss;

                    lastValidationLoss = validationSetLoss;
                }
            }

            ctx.Score = score;
            return ctx.Score;
        }

        #region InitializeWeightsandBiasesinputlayer
        /// <summary>
        /// This method intializes the weights and biases at the input layer
        /// </summary>
        private void InitializeWeightsandBiasesinputlayer(int inpDims)
        {
            Random rnd = new Random();

            m_Weights[0] = new double[m_HiddenLayerNeurons[0], m_InpDims];

            m_Biases[0] = new double[m_HiddenLayerNeurons[0]];

            for (int i = 0; i < m_InpDims; i++)
            {
                for (int j = 0; j < m_HiddenLayerNeurons[0]; j++)
                {
                    double randVal = rnd.NextDouble();

                    while (randVal == 0.0)
                    {
                        randVal = rnd.NextDouble();
                    }

                    m_Weights[0][j, i] = randVal * Math.Sqrt((double)1 / (double)inpDims);
                }
            }

            for (int j = 0; j < m_HiddenLayerNeurons[0]; j++)
            {
                double randVal = rnd.NextDouble();

                while (randVal == 0.0)
                {
                    randVal = rnd.NextDouble();
                }

                m_Biases[0][j] = randVal * Math.Sqrt((double)1 / (double)inpDims);
            }
        }
        #endregion

        #region InitializeWeightsandBiaseshiddenlayers
        /// <summary>
        /// This method initializes the weights and biases at the hidden layers
        /// </summary>
        private void InitializeWeightsandBiaseshiddenlayers(int[] hiddenLayerNeurons)
        {
            Random rnd = new Random();

            int index = 0;

            while (index < (m_HiddenLayerNeurons.Length - 1))
            {
                int numOfHiddenNeuronsInCurrentLayer = m_HiddenLayerNeurons[index];

                int numOfHiddenNeuronsInFolLayer = m_HiddenLayerNeurons[index + 1];

                m_Weights[index + 1] = new double[numOfHiddenNeuronsInFolLayer, numOfHiddenNeuronsInCurrentLayer];

                m_Biases[index + 1] = new double[numOfHiddenNeuronsInFolLayer];

                for (int j = 0; j < numOfHiddenNeuronsInCurrentLayer; j++)
                {
                    for (int k = 0; k < numOfHiddenNeuronsInFolLayer; k++)
                    {
                        double randVal = rnd.NextDouble();

                        while (randVal == 0.0)
                        {
                            randVal = rnd.NextDouble();
                        }

                        m_Weights[index + 1][k, j] = randVal * Math.Sqrt((double)1 / (double)hiddenLayerNeurons[index]);
                    }
                }

                for (int k = 0; k < numOfHiddenNeuronsInFolLayer; k++)
                {
                    double randVal = rnd.NextDouble();

                    while (randVal == 0.0)
                    {
                        randVal = rnd.NextDouble();
                    }

                    m_Biases[index + 1][k] = randVal * Math.Sqrt((double)1 / (double)hiddenLayerNeurons[index]);
                }

                index++;
            }
        }
        #endregion

        #region InitializeWeightsandBiasesoutputlayer
        /// <summary>
        /// This method initializes the weights and biases at the output layer
        /// </summary>
        private void InitializeWeightsandBiasesoutputlayer(int[] hiddenLayerNeurons)
        {
            Random rnd = new Random();

            int numOfHiddenNeuronsInLastHiddenLayer = m_HiddenLayerNeurons[m_HiddenLayerNeurons.Length - 1];

            m_Weights[m_HiddenLayerNeurons.Length] = new double[m_OutputLayerNeurons, numOfHiddenNeuronsInLastHiddenLayer];

            m_Biases[m_HiddenLayerNeurons.Length] = new double[m_OutputLayerNeurons];

            for (int j = 0; j < numOfHiddenNeuronsInLastHiddenLayer; j++)
            {
                for (int k = 0; k < m_OutputLayerNeurons; k++)
                {
                    double randVal = rnd.NextDouble();

                    while (randVal == 0.0)
                    {
                        randVal = rnd.NextDouble();
                    }

                    m_Weights[m_HiddenLayerNeurons.Length][k, j] = randVal * Math.Sqrt((double)1 / (double)hiddenLayerNeurons[m_HiddenLayerNeurons.Length - 1]);
                }
            }

            for (int k = 0; k < m_OutputLayerNeurons; k++)
            {
                double randVal = rnd.NextDouble();

                while (randVal == 0.0)
                {
                    randVal = rnd.NextDouble();
                }

                m_Biases[m_HiddenLayerNeurons.Length][k] = randVal * Math.Sqrt((double)1 / (double)hiddenLayerNeurons[m_HiddenLayerNeurons.Length - 1]);
            }
        }
        #endregion

        #region Prediction
        /// <summary>
        /// This method accepts the test data as input and determines the output for each sample of test data
        /// </summary>
        /// <param name="data">test data inputs</param>
        /// <param name="ctx">test data descriptions</param>
        /// <returns>double[]</returns>
        public override IResult Predict(double[][] data, IContext ctx)
        {
            int numberOfHiddenAndOutputLayers = m_HiddenLayerNeurons.Length + 1;

            double[][] calcuLatedOutput = new double[numberOfHiddenAndOutputLayers][];

            double[][] weightedInputs = new double[numberOfHiddenAndOutputLayers][];

            MLPerceptronResult result = new MLPerceptronResult()
            {
                results = new double[data.Length * m_OutputLayerNeurons],
            };

            for (int i = 0; i < data.Length; i++)
            {
                PredictFirstHiddenLayer(data[i], m_InpDims, out calcuLatedOutput[0], out weightedInputs[0]);

                PredictRemainingHiddenLayers(calcuLatedOutput[0], weightedInputs[0], m_InpDims, out calcuLatedOutput, out weightedInputs);

                PredictResultatOutputlayer(calcuLatedOutput[m_HiddenLayerNeurons.Length - 1], m_InpDims, true, out calcuLatedOutput[m_HiddenLayerNeurons.Length], out weightedInputs[m_HiddenLayerNeurons.Length]);

                for (int j = 0; j < m_OutputLayerNeurons; j++)
                {
                    result.results[m_OutputLayerNeurons * i + j] = calcuLatedOutput[m_HiddenLayerNeurons.Length][j];
                }
            }

            return result;
        }

        #endregion

        #region ResultPredictionFirstHiddenLayer
        /// <summary>
        /// This method calculates the results of the network at the first hidden layer
        /// </summary>
        /// <param name="input">input layer data</param>
        /// <param name="numOfFeatures">number of input neurons</param>
        /// <param name="layerOutput">None linear output of the 1st hidden layer outputs</param>
        /// <param name="layerNeuronSum">Output sum of the 1st hidden layer for each layer neuron.</param>
        private void PredictFirstHiddenLayer(double[] input, int numOfFeatures, out double[] layerOutput, out double[] layerNeuronSum)
        {
            layerOutput = new double[m_HiddenLayerNeurons[0]];

            layerNeuronSum = new double[m_HiddenLayerNeurons[0]];

            for (int j = 0; j < m_HiddenLayerNeurons[0]; j++)
            {
                layerNeuronSum[j] = 0.0;

                for (int i = 0; i < m_InpDims; i++)
                {
                    layerNeuronSum[j] += m_Weights[0][j, i] * input[i];
                }

                layerNeuronSum[j] += m_Biases[0][j];

                layerOutput[j] = m_ActivationFunction(layerNeuronSum[j]);
            }
        }
        #endregion

        #region ResultPredictionRemainingHiddenLayers
        /// <summary>
        /// This method calculates the results of the network at the hidden layers that follow the first hidden layer
        /// </summary>
        /// <param name="input">input layer data</param>
        /// <param name="firstlayerweightedip">weightged outputs at the first hidden layer</param>
        /// <param name="numOfFeatures">number of input neurons</param>
        /// <param name="layerOutput">output parameter to store outputs at the hidden layers that follow the first hidden layer</param>
        /// <param name="layerNeuronSum">output parameter to store weighted inputs at the hidden layers that follow the first hidden layer</param>
        private void PredictRemainingHiddenLayers(double[] input, double[] firstlayerweightedip, int numOfFeatures, out double[][] layerOutput, out double[][] layerNeuronSum)
        {
            layerOutput = new double[m_HiddenLayerNeurons.Length + 1][];

            layerNeuronSum = new double[m_HiddenLayerNeurons.Length + 1][];

            double[] currentInput = input;

            int index = 0;

            layerOutput[0] = new double[m_HiddenLayerNeurons[index]];

            layerOutput[0] = input;

            layerNeuronSum[0] = new double[m_HiddenLayerNeurons[index]];

            layerNeuronSum[0] = firstlayerweightedip;

            while (index < (m_HiddenLayerNeurons.Length - 1))
            {
                int numofhiddenneuronsincurrentlayer = m_HiddenLayerNeurons[index];

                int numofhiddenneuronsinfollayer = m_HiddenLayerNeurons[index + 1];

                layerOutput[index + 1] = new double[numofhiddenneuronsinfollayer];

                layerNeuronSum[index + 1] = new double[numofhiddenneuronsinfollayer];

                for (int k = 0; k < numofhiddenneuronsinfollayer; k++)
                {
                    layerNeuronSum[index + 1][k] = 0.0;

                    for (int j = 0; j < numofhiddenneuronsincurrentlayer; j++)
                    {
                        layerNeuronSum[index + 1][k] += m_Weights[index + 1][k, j] * currentInput[j];
                    }

                    layerNeuronSum[index + 1][k] += m_Biases[index + 1][k];

                    layerOutput[index + 1][k] = m_ActivationFunction(layerNeuronSum[index + 1][k]);
                }

                currentInput = layerOutput[index + 1];

                index++;
            }

        }
        #endregion

        #region ResultPredictionOutputLayer
        /// <summary>
        /// This method calculates the results of the network at the output layer
        /// </summary>
        /// <param name="input">input layer data</param>
        /// <param name="numOfFeatures">number of input neurons</param>
        /// <param name="output">output parameter to store outputs at the output layer</param>
        /// <param name="outputSum">output sum of the last layer.</param>
        private void PredictResultatOutputlayer(double[] input, int numOfFeatures, bool softmax, out double[] output, out double[] outputSum)
        {
            output = new double[m_OutputLayerNeurons];

            outputSum = new double[m_OutputLayerNeurons];

            int numOfHiddenNeuronsInLastHiddenLayer = m_HiddenLayerNeurons[m_HiddenLayerNeurons.Length - 1];

            for (int j = 0; j < m_OutputLayerNeurons; j++)
            {
                outputSum[j] = 0.0;

                for (int i = 0; i < numOfHiddenNeuronsInLastHiddenLayer; i++)
                {
                    outputSum[j] += m_Weights[m_HiddenLayerNeurons.Length][j, i] * input[i];
                }

                outputSum[j] += m_Biases[m_HiddenLayerNeurons.Length][j];

                if (softmax == false)
                {
                    output[j] = MLPerceptron.NeuralNetworkCore.ActivationFunctions.Sigmoid(outputSum[j]);
                }
                else
                {
                    // Do nothing
                }
            }

            if (softmax == true)
            {
                output = MLPerceptron.NeuralNetworkCore.ActivationFunctions.SoftMaxClassifier(outputSum);
            }
        }
        #endregion
    }
}