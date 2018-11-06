using MLPerceptron.NeuralNetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using System.Diagnostics;
using MLPerceptron.BackPropagation;
using NeuralNet.MLPerceptron;
using NeuralNetworks.Core;
using System.IO;

namespace MLPerceptron
{
    /// <summary>
    /// Class MLPerceptronAlgorithm implements the interface "NeuralNetCore" methods, and is the main class which implements the neural network feed forward step and the predict result method
    /// </summary>
    public class MLPerceptronAlgorithm : NeuralNetCore
    {
        #region Private Fields

        private double m_LearningRate = 0.1;

        private int[] m_HiddenLayerNeurons = {4,3,5};

        private int m_OutputLayerNeurons;

        private int m_Iterations = 10000;

        private int m_batchSize = 1;

        private Func<double, double> m_ActivationFunction = MLPerceptron.NeuralNetworkCore.ActivationFunctions.HyperbolicTan;//TODO Patrick

        private int m_InpDims;

        private double[][,] m_Weights;

        private double[][] m_Biases;

        private Boolean m_SoftMax = true;

        private int TestCaseNumber = 0;

        #endregion

        #region MLPerceptronAlgorithm constructor
        /// <summary>
        /// MLPerceptronAlgorithm constructor
        /// </summary>
        /// <param name="learningRate">learning rate of the network</param>
        /// <param name="iterations">number of epochs</param>
        /// <param name="hiddenLayerNeurons">number of hidden layer neurons</param>
        /// <param name="activationfunction">activation function to be used by the netwokr</param>
        public MLPerceptronAlgorithm(double learningRate, int iterations, int batchSize, int testCaseNumber, int[] hiddenLayerNeurons, Func<double,double> activationfunction = null, bool SoftMax = false)
        {
            this.m_LearningRate = learningRate;

            this.m_Iterations = iterations;

            this.m_batchSize = batchSize;

            this.TestCaseNumber = testCaseNumber;

            if (hiddenLayerNeurons != null)
            {
                this.m_HiddenLayerNeurons = hiddenLayerNeurons;
            }

            if (activationfunction != null)
            {
                this.m_ActivationFunction = activationfunction;
            }

            if (SoftMax != false)
            {
                this.m_SoftMax = SoftMax;
            }
        }

        /// <summary>
        /// MLPerceptronAlgorithm default constructor
        /// </summary>
        public MLPerceptronAlgorithm()
        {
            // do nothing
        }

        #endregion

        #region Run
        /// <summary>
        /// This method accepts the training examples as input and performs the training of the MLP neural network
        /// </summary>
        /// <param name="data">training data pairs</param>
        /// <param name="ctx">training data descriptions</param>
        /// <returns>IScore</returns>
        public override IScore Run(double[][] data, IContext ctx)
        {

            // Sum for every layer. hidLyrNeuronSum1 = x11*w11+x12*w21+..+x1N*wN1
            double[][] hidLyrNeuronSum = new double[m_HiddenLayerNeurons.Length + 1][];

            // outputs = ActFnx(hidLyrNeuronSum+Bias)
            double[][] hidLyrOut = new double[m_HiddenLayerNeurons.Length + 1][];

            double[][] trainingData = new double[(int)(data.Length * 0.8)][];

            double[][] validationData = new double[(int)(data.Length * 0.2)][];

            trainingData = data.Take((int)(data.Length * 0.8)).ToArray();

            validationData = data.Skip((int)(data.Length * 0.8)).ToArray();

            int numOfInputVectors = trainingData.Length;

            m_InpDims = ctx.DataDescriptor.Features.Count(); 

            m_OutputLayerNeurons = data[0].Length - m_InpDims;

            m_Weights = new double[m_HiddenLayerNeurons.Length + 1][,];

            m_Biases = new double[m_HiddenLayerNeurons.Length + 1][];

            InitializeWeightsandBiasesinputlayer(m_InpDims);

            InitializeWeightsandBiaseshiddenlayers(m_HiddenLayerNeurons);

            InitializeWeightsandBiasesoutputlayer(m_HiddenLayerNeurons);

            var score = new MLPerceptronAlgorithmScore();

            double lastLoss = 0;

            double lastValidationLoss = 0;

            Stopwatch watch = new Stopwatch();

            double timeElapsed = 0;
			
            for (int i = 0; i < m_Iterations; i++)
            {
                watch.Restart();
                score.Loss = 0;

                double batchAccuracy = 0;

                int miniBatchStartIndex = 0;

                while (miniBatchStartIndex < numOfInputVectors)
                {
                    BackPropagationNetwork backPropagation = new BackPropagationNetwork(m_Biases, m_HiddenLayerNeurons, m_OutputLayerNeurons, m_InpDims);

                    for (int inputVectIndx = miniBatchStartIndex; inputVectIndx < m_batchSize + miniBatchStartIndex; inputVectIndx++)
                    {
                        // Z2 = actFnc(X * W1)
                        CalcFirstHiddenLayer(trainingData[inputVectIndx], m_InpDims, out hidLyrOut[0], out hidLyrNeuronSum[0]);

                        // We use output of first layer as input of second layer.
                        CalcRemainingHiddenLayers(hidLyrOut[0], hidLyrNeuronSum[0], m_InpDims, out hidLyrOut, out hidLyrNeuronSum);

                        // Zk = ak-1 * Wk-1
                        CalculateResultatOutputlayer(hidLyrOut[m_HiddenLayerNeurons.Length - 1], m_InpDims, m_SoftMax, out hidLyrOut[m_HiddenLayerNeurons.Length], out hidLyrNeuronSum[m_HiddenLayerNeurons.Length]);

                        if (m_SoftMax == true)
                        {
                            backPropagation.CalcOutputErrorSoftMax(hidLyrOut[m_HiddenLayerNeurons.Length], m_HiddenLayerNeurons, trainingData[inputVectIndx], ctx);

                        }
                        else
                        {
                            //  BackPropagationNetwork backPropagation = new BackPropagationNetwork(m_HiddenLayerNeurons.Length);
                            backPropagation.CalcOutputError(hidLyrOut[m_HiddenLayerNeurons.Length], m_HiddenLayerNeurons, hidLyrNeuronSum[m_HiddenLayerNeurons.Length], trainingData[inputVectIndx], ctx);
                        }
                        backPropagation.CalcHiddenLayersError(hidLyrOut, m_Weights, m_HiddenLayerNeurons, hidLyrNeuronSum, trainingData[inputVectIndx]);

                        backPropagation.CostFunctionChangeWithBiases(m_Biases, m_HiddenLayerNeurons, m_LearningRate);

                        backPropagation.CostFunctionChangeWithWeights(m_Weights, hidLyrOut, m_HiddenLayerNeurons, m_LearningRate, trainingData[inputVectIndx]);

                    }

                    backPropagation.UpdateBiases(m_Biases, m_HiddenLayerNeurons, m_LearningRate, out m_Biases);

                    backPropagation.UpdateWeights(m_Weights, hidLyrOut, m_HiddenLayerNeurons, m_LearningRate, m_InpDims, out m_Weights);

                    score.Errors = backPropagation.MiniBatchError[m_HiddenLayerNeurons.Length];

                    batchAccuracy += ((double)backPropagation.TrainingSetAccuracy / m_batchSize);

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

                    miniBatchStartIndex = miniBatchStartIndex + m_batchSize;
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

                lastLoss = score.Loss;

                lastValidationLoss = validationSetLoss;
            }

            ctx.Score = score;
            return ctx.Score;

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
            double[][] calcuLatedOutput = new double[m_HiddenLayerNeurons.Length + 1][];

            double[][] weightedInputs = new double[m_HiddenLayerNeurons.Length + 1][];

            MLPerceptronResult result = new MLPerceptronResult()
            {
                results = new double[data.Length * m_OutputLayerNeurons],
            };

            for (int i = 0; i < data.Length; i++)
            {
                CalcFirstHiddenLayer(data[i], m_InpDims, out calcuLatedOutput[0], out weightedInputs[0]);

                CalcRemainingHiddenLayers(calcuLatedOutput[0], weightedInputs[0], m_InpDims, out calcuLatedOutput, out weightedInputs);

                CalculateResultatOutputlayer(calcuLatedOutput[m_HiddenLayerNeurons.Length - 1], m_InpDims, m_SoftMax, out calcuLatedOutput[m_HiddenLayerNeurons.Length], out weightedInputs[m_HiddenLayerNeurons.Length]);

                for (int j = 0; j < m_OutputLayerNeurons; j++)
                {
                    result.results[m_OutputLayerNeurons * i + j] = calcuLatedOutput[m_HiddenLayerNeurons.Length][j];
                }
            }

            return result;
        }

        #endregion

        #region ResultCalculationFirstHiddenLayer
        /// <summary>
        /// This method calculates the results of the network at the first hidden layer
        /// </summary>
        /// <param name="input">input layer data</param>
        /// <param name="numOfFeatures">number of input neurons</param>
        /// <param name="layerOutput">None linear output of the 1st hidden layer outputs</param>
        /// <param name="layerNeuronSum">Output sum of the 1st hidden layer for each layer neuron.</param>
        private void CalcFirstHiddenLayer(double[] input, int numOfFeatures, out double[] layerOutput, out double[] layerNeuronSum)
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

        #region ResultCalculationRemainingHiddenLayers
        /// <summary>
        /// This method calculates the results of the network at the hidden layers that follow the first hidden layer
        /// </summary>
        /// <param name="input">input layer data</param>
        /// <param name="firstlayerweightedip">weightged outputs at the first hidden layer</param>
        /// <param name="numOfFeatures">number of input neurons</param>
        /// <param name="layerOutput">output parameter to store outputs at the hidden layers that follow the first hidden layer</param>
        /// <param name="layerNeuronSum">output parameter to store weighted inputs at the hidden layers that follow the first hidden layer</param>
        private void CalcRemainingHiddenLayers(double[] input, double[] firstlayerweightedip, int numOfFeatures, out double[][] layerOutput, out double[][] layerNeuronSum)
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

        #region ResultCalculationOutputLayer
        /// <summary>
        /// This method calculates the results of the network at the output layer
        /// </summary>
        /// <param name="input">input layer data</param>
        /// <param name="numOfFeatures">number of input neurons</param>
        /// <param name="output">output parameter to store outputs at the output layer</param>
        /// <param name="outputSum">output sum of the last layer.</param>
        private void CalculateResultatOutputlayer(double[] input, int numOfFeatures, bool softmax, out double[] output, out double[] outputSum)
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

                    m_Weights[0][j, i] = randVal * Math.Sqrt((double)1/(double)inpDims);
                }
            }

            for(int j = 0; j < m_HiddenLayerNeurons[0]; j++)
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

                        m_Weights[index + 1][k,j] = randVal * Math.Sqrt((double)1/(double)hiddenLayerNeurons[index]);
                    }
                }

                for (int k = 0; k < numOfHiddenNeuronsInFolLayer; k++)
                {
                    double randVal = rnd.NextDouble();

                    while (randVal == 0.0)
                    {
                        randVal = rnd.NextDouble();
                    }

                    m_Biases[index + 1][k] = randVal * Math.Sqrt((double)1/(double)hiddenLayerNeurons[index]);
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

                    m_Weights[m_HiddenLayerNeurons.Length][k, j] = randVal * Math.Sqrt((double)1/(double)hiddenLayerNeurons[m_HiddenLayerNeurons.Length - 1]);
                }
            }

            for (int k = 0; k < m_OutputLayerNeurons; k++)
            {
                double randVal = rnd.NextDouble();

                while (randVal == 0.0)
                {
                    randVal = rnd.NextDouble();
                }

                m_Biases[m_HiddenLayerNeurons.Length][k] = randVal * Math.Sqrt((double)1/(double)hiddenLayerNeurons[m_HiddenLayerNeurons.Length - 1]);
            }
        }         
        #endregion
    }
}

