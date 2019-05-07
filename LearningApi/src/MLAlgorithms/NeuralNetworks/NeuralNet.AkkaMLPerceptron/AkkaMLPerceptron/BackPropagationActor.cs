using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using NeuralNet.MLPerceptron;

namespace AkkaMLPerceptron
{
    /// <summary>
    /// Implement partial backpropagation algorithm, which is executed on a single node in cluster.
    /// </summary>
    public class BackPropagationActor : ReceiveActor
    {
        private readonly int[] hiddenlayerneurons;
        private readonly int outputlayerneurons;
        private readonly int numOfInputNeurons;
        //private readonly int numberOfPartitions;
        private readonly double learningRate;
        private readonly Func<double, double> activationFunction = MLPerceptron.NeuralNetworkCore.ActivationFunctions.HyperbolicTan;
        private readonly Func<double, double> derivativeActivationFunction = MLPerceptron.NeuralNetworkCore.ActivationFunctions.DerivativeHyperbolicTan;
        /// <summary>
        /// Performs BackPropagation calculation of a single layer for specified calculationWidth.
        /// </summary>
        /// <param name="biases"></param>
        /// <param name="hiddenlayerneurons"></param>
        /// <param name="outputlayerneurons"></param>
        /// <param name="numOfInputNeurons"></param>
        /// <param name="calculationWidth">Number of features to be calculated by this instance. Calculation
        /// of back-propagation is shared across multiple nodes. This value specifies how many will be calculated by thi sinstance.</param>
        public BackPropagationActor(int[] hiddenlayerneurons, int outputlayerneurons, int numOfInputNeurons, double learningRate)
        {
            this.hiddenlayerneurons = hiddenlayerneurons;
            this.outputlayerneurons = outputlayerneurons;
            this.numOfInputNeurons = numOfInputNeurons;
            this.learningRate = learningRate;
            //this.numberOfPartitions = numberOfPartitions;

            //Receive<BackPropActorIn>(new Action<BackPropActorIn>(CalculateLayer));
            Receive<FirstHiddenLayerForwardPropActorIn>(new Action<FirstHiddenLayerForwardPropActorIn>(CalculateFirstHiddenLayer));
            Receive<RemainingLayersForwardPropActorIn>(new Action<RemainingLayersForwardPropActorIn>(CalculateRemainingLayers));
            Receive<OutputLayerBackPropActorIn>(new Action<OutputLayerBackPropActorIn>(CalculateErrorsOutputLayer));
            Receive<HiddenLayersBackPropActorIn>(new Action<HiddenLayersBackPropActorIn>(CalculateErrorsHiddenLayers));
            Receive<FirstHiddenLayerBackPropActorIn>(new Action<FirstHiddenLayerBackPropActorIn>(CalculateErrorsFirstHiddenLayers));
            Receive<WeightUpdateBackPropActorIn>(new Action<WeightUpdateBackPropActorIn>(UpdateWeights));
            Receive<BiasUpdateBackPropActorIn>(new Action<BiasUpdateBackPropActorIn>(UpdateBiases));

        }

        protected override void PreStart()
        {
            Console.WriteLine($"Started Actor: {Context.Self.Path}");

            base.PreStart();
        }

        private void CalculateLayer(BackPropActorIn msg)
        {
            Console.WriteLine($"Entered calculation: {Context.Self.Path}");

            BackPropActorOut obj = new BackPropActorOut();

            //Thread.Sleep(15000);
            obj.CostChangeDueToBiases = new double[4];

            obj.CostChangeDueToBiases[0] = 1;
            obj.CostChangeDueToBiases[1] = 2;
            obj.CostChangeDueToBiases[2] = 3;
            obj.CostChangeDueToBiases[3] = 4;

            //Sender.Tell(new BackPropActorOut() { actorNum = 1, CostChangeDueToBiases = new double[2] { 3,4} });
            Sender.Tell(obj);
        }

        private void CalculateFirstHiddenLayer(FirstHiddenLayerForwardPropActorIn msg)
        {
            Console.WriteLine("\n");

            Console.WriteLine($"Entered First Hidden Layer calculation: {Context.Self.Path}");

            ForwardPropActorOut backPropActorOut = new ForwardPropActorOut();

            backPropActorOut.LayerOutput = new double[msg.batchSize, msg.NeuronsCurrentActor];

            backPropActorOut.LayerNeuronSum = new double[msg.batchSize, msg.NeuronsCurrentActor];

            for (int i = 0; i < msg.batchSize; i++)
            {
                for (int j = msg.StartIndex; j < msg.StartIndex + msg.NeuronsCurrentActor; j++)
                {
                    backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] = 0.0;

                    for (int k = 0; k < this.numOfInputNeurons; k++)
                    {
                        backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] += msg.PrevLayerWeights[j, k] * msg.Input[i][k];
                    }

                    backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] += msg.PreviousLayerBiases[j];

                    backPropActorOut.LayerOutput[i, j - msg.StartIndex] = this.activationFunction(backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex]);
                }
            }

            backPropActorOut.actorNum = msg.actorNum;

            Sender.Tell(backPropActorOut);
        }

        private void CalculateRemainingLayers(RemainingLayersForwardPropActorIn msg)
        {
            ForwardPropActorOut backPropActorOut = new ForwardPropActorOut();

            if (msg.isOutputLayer)
            {
                Console.WriteLine($"Entered Output Layer calculation: {Context.Self.Path}");

                backPropActorOut.LayerNeuronSum = new double[msg.batchSize, msg.NeuronsCurrentActor];

                for (int i = 0; i < msg.batchSize; i++)
                {
                    for (int j = msg.StartIndex; j < msg.StartIndex + msg.NeuronsCurrentActor; j++)
                    {
                        backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] = 0.0;

                        for (int k = 0; k < msg.Input.GetLength(1); k++)
                        {
                            backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] += msg.PrevLayerWeights[j, k] * msg.Input[i, k];
                        }

                        backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] += msg.PreviousLayerBiases[j];
                    }
                }
            }
            else
            {
                Console.WriteLine($"Entered Next Hidden Layer calculation: {Context.Self.Path}");

                backPropActorOut.LayerOutput = new double[msg.batchSize, msg.NeuronsCurrentActor];

                backPropActorOut.LayerNeuronSum = new double[msg.batchSize, msg.NeuronsCurrentActor];

                for (int i = 0; i < msg.batchSize; i++)
                {
                    for (int j = msg.StartIndex; j < msg.StartIndex + msg.NeuronsCurrentActor; j++)
                    {
                        backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] = 0.0;

                        for (int k = 0; k < msg.Input.GetLength(1); k++)
                        {
                            backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] += msg.PrevLayerWeights[j, k] * msg.Input[i, k];
                        }

                        backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex] += msg.PreviousLayerBiases[j];

                        backPropActorOut.LayerOutput[i, j - msg.StartIndex] = this.activationFunction(backPropActorOut.LayerNeuronSum[i, j - msg.StartIndex]);
                    }
                }
            }

            backPropActorOut.actorNum = msg.actorNum;

            Sender.Tell(backPropActorOut);
        }

        private void CalculateErrorsOutputLayer(OutputLayerBackPropActorIn msg)
        {
            Console.WriteLine($"Entered Output Layer Error calculation: {Context.Self.Path}");

            BackPropActorOut backPropActorOut = new BackPropActorOut();

            backPropActorOut.Errors = new double[msg.batchSize, msg.NeuronsCurrentActor];

            backPropActorOut.CostChangeDueToBiases = new double[msg.NeuronsCurrentActor];

            backPropActorOut.CostChangeDueToWeights = new double[msg.NeuronsCurrentActor, msg.PrevLayerOutput.GetLength(1)];

            int firstIndexOfActualOp = 0;

            backPropActorOut.result = new bool[msg.batchSize];

            for (int i = 0; i < msg.batchSize; i++)
            {
                backPropActorOut.result[i] = true;

                firstIndexOfActualOp = msg.Input[i].Length - 10;

                for (int j = msg.StartIndex; j < msg.StartIndex + msg.NeuronsCurrentActor; j++)
                {
                    int inputVectorIndex = firstIndexOfActualOp++;

                    inputVectorIndex = inputVectorIndex + msg.StartIndex;

                    backPropActorOut.Errors[i, j - msg.StartIndex] = msg.LayerOutput[i, j] - msg.Input[i][inputVectorIndex];

                    for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < msg.PrevLayerOutput.GetLength(1); prevHidLayerOutputIndx++)
                    {
                        backPropActorOut.CostChangeDueToWeights[j - msg.StartIndex, prevHidLayerOutputIndx] += msg.PrevLayerOutput[i, prevHidLayerOutputIndx] * backPropActorOut.Errors[i, j - msg.StartIndex];
                    }

                    backPropActorOut.CostChangeDueToBiases[j - msg.StartIndex] += Math.Abs(backPropActorOut.Errors[i, j - msg.StartIndex]);

                    if (msg.Input[i][inputVectorIndex] != (msg.LayerOutput[i, j] >= 0.5 ? 1 : 0))
                    {
                        backPropActorOut.result[i] = false;
                    }
                }
            }

            backPropActorOut.actorNum = msg.actorNum;

            Sender.Tell(backPropActorOut);
        }

        private void CalculateErrorsHiddenLayers(HiddenLayersBackPropActorIn msg)
        {
            Console.WriteLine($"Entered Previous Hidden Layer Error calculation: {Context.Self.Path}");

            BackPropActorOut backPropActorOut = new BackPropActorOut();

            backPropActorOut.Errors = new double[msg.batchSize, msg.NeuronsCurrentActor];

            backPropActorOut.CostChangeDueToBiases = new double[msg.NeuronsCurrentActor];

            backPropActorOut.CostChangeDueToWeights = new double[msg.NeuronsCurrentActor, msg.PrevLayerOutput.GetLength(1)];

            for (int i = msg.batchIndex; i < msg.batchIndex + msg.batchSize; i++)
            {
                for (int hidLayerNeuronIndx = msg.StartIndex; hidLayerNeuronIndx < msg.StartIndex + msg.NeuronsCurrentActor; hidLayerNeuronIndx++)
                {
                    double layerError = 0.0;

                    for (int j = 0; j < msg.FollLayerErrors.GetLength(1); j++)
                    {
                        layerError += msg.FollLayerWeights[j, hidLayerNeuronIndx] * msg.FollLayerErrors[i - msg.batchIndex, j];
                    }

                    backPropActorOut.Errors[i - msg.batchIndex, hidLayerNeuronIndx - msg.StartIndex] = layerError * this.derivativeActivationFunction(msg.LayerSum[i - msg.batchIndex, hidLayerNeuronIndx]);

                    for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < msg.PrevLayerOutput.GetLength(1); prevHidLayerOutputIndx++)
                    {
                        backPropActorOut.CostChangeDueToWeights[hidLayerNeuronIndx - msg.StartIndex, prevHidLayerOutputIndx] += msg.PrevLayerOutput[i - msg.batchIndex, prevHidLayerOutputIndx] * backPropActorOut.Errors[i - msg.batchIndex, hidLayerNeuronIndx - msg.StartIndex];
                    }

                    backPropActorOut.CostChangeDueToBiases[hidLayerNeuronIndx - msg.StartIndex] += Math.Abs(backPropActorOut.Errors[i - msg.batchIndex, hidLayerNeuronIndx - msg.StartIndex]);
                }
            }

            backPropActorOut.actorNum = msg.actorNum;

            Sender.Tell(backPropActorOut);
        }

        private void CalculateErrorsFirstHiddenLayers(FirstHiddenLayerBackPropActorIn msg)
        {
            Console.WriteLine($"Entered First Hidden Layer Error calculation: {Context.Self.Path}");

            BackPropActorOut backPropActorOut = new BackPropActorOut();

            backPropActorOut.Errors = new double[msg.batchSize, msg.NeuronsCurrentActor];

            backPropActorOut.CostChangeDueToBiases = new double[msg.NeuronsCurrentActor];

            backPropActorOut.CostChangeDueToWeights = new double[msg.NeuronsCurrentActor, msg.PrevLayerOutput[0].Length];

            for (int i = 0; i < msg.batchSize; i++)
            {
                for (int hidLayerNeuronIndx = msg.StartIndex; hidLayerNeuronIndx < msg.StartIndex + msg.NeuronsCurrentActor; hidLayerNeuronIndx++)
                {
                    double layerError = 0.0;

                    for (int j = 0; j < msg.FollLayerErrors.GetLength(1); j++)
                    {
                        layerError += msg.FollLayerWeights[j, hidLayerNeuronIndx] * msg.FollLayerErrors[i, j];
                    }

                    backPropActorOut.Errors[i, hidLayerNeuronIndx - msg.StartIndex] = layerError * this.derivativeActivationFunction(msg.LayerSum[i, hidLayerNeuronIndx]);

                    for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < msg.PrevLayerOutput[i].Length; prevHidLayerOutputIndx++)
                    {
                        backPropActorOut.CostChangeDueToWeights[hidLayerNeuronIndx - msg.StartIndex, prevHidLayerOutputIndx] += msg.PrevLayerOutput[i][prevHidLayerOutputIndx] * backPropActorOut.Errors[i, hidLayerNeuronIndx - msg.StartIndex];
                    }

                    backPropActorOut.CostChangeDueToBiases[hidLayerNeuronIndx - msg.StartIndex] += Math.Abs(backPropActorOut.Errors[i, hidLayerNeuronIndx - msg.StartIndex]);
                }
            }

            backPropActorOut.actorNum = msg.actorNum;

            Sender.Tell(backPropActorOut);
        }

        private void UpdateWeights(WeightUpdateBackPropActorIn msg)
        {
            Console.WriteLine($"Entered Weight Update calculation: {Context.Self.Path}");

            WeightUpdateBackPropActorOut backPropActorOut = new WeightUpdateBackPropActorOut();

            backPropActorOut.newweights = new double[this.hiddenlayerneurons.Length + 1][,];

            int hidLayerIndx = 0;

            while (hidLayerIndx <= this.hiddenlayerneurons.Length)
            {
                if (hidLayerIndx == 0)
                {
                    backPropActorOut.newweights[hidLayerIndx] = new double[msg.NeuronsCurrentActor[hidLayerIndx], this.numOfInputNeurons];

                    for (int hidLayerNeuronIndx = msg.StartIndex[hidLayerIndx]; hidLayerNeuronIndx < msg.StartIndex[hidLayerIndx] + msg.NeuronsCurrentActor[hidLayerIndx]; hidLayerNeuronIndx++)
                    {
                        for (int inputNeuronIndx = 0; inputNeuronIndx < this.numOfInputNeurons; inputNeuronIndx++)
                        {
                            backPropActorOut.newweights[hidLayerIndx][hidLayerNeuronIndx - msg.StartIndex[hidLayerIndx], inputNeuronIndx] = msg.currentweights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx] - this.learningRate * msg.CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx];
                        }
                    }
                }
                else
                {
                    backPropActorOut.newweights[hidLayerIndx] = new double[msg.NeuronsCurrentActor[hidLayerIndx], msg.hidLayersOutputs[hidLayerIndx - 1].GetLength(1)];

                    for (int hidLayerNeuronIndx = msg.StartIndex[hidLayerIndx]; hidLayerNeuronIndx < msg.StartIndex[hidLayerIndx] + msg.NeuronsCurrentActor[hidLayerIndx]; hidLayerNeuronIndx++)
                    {
                        for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < msg.hidLayersOutputs[hidLayerIndx - 1].GetLength(1); prevHidLayerOutputIndx++)
                        {
                            backPropActorOut.newweights[hidLayerIndx][hidLayerNeuronIndx - msg.StartIndex[hidLayerIndx], prevHidLayerOutputIndx] = msg.currentweights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx] - this.learningRate * msg.CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx];
                        }
                    }
                }

                hidLayerIndx++;
            }

            backPropActorOut.actorNum = msg.actorNum;

            Sender.Tell(backPropActorOut);

        }

        private void UpdateBiases(BiasUpdateBackPropActorIn msg)
        {
            Console.WriteLine($"Entered Bias Update calculation: {Context.Self.Path}");

            BiasUpdateBackPropActorOut backPropActorOut = new BiasUpdateBackPropActorOut();

            backPropActorOut.newbiases = new double[this.hiddenlayerneurons.Length + 1][];

            for (int i = 0; i <= this.hiddenlayerneurons.Length; i++)
            {
                backPropActorOut.newbiases[i] = new double[msg.NeuronsCurrentActor[i]];

                for (int j = msg.StartIndex[i]; j < msg.StartIndex[i] + msg.NeuronsCurrentActor[i]; j++)
                {
                    backPropActorOut.newbiases[i][j - msg.StartIndex[i]] = msg.currentbiases[i][j] - this.learningRate * msg.CostChangeDueToBiases[i][j];
                }
            }

            backPropActorOut.actorNum = msg.actorNum;

            Sender.Tell(backPropActorOut);

        }
    }
}
