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
using System.Globalization;

namespace MLPerceptron
{
    /// <summary>
    /// Class MLPerceptronAlgorithm implements the interface "NeuralNetCore" methods, and is the main class which implements the neural network feed forward step and the predict result method
    /// </summary>
    public class MLPerceptronAlgorithm : NeuralNetCore
    {
        #region Private Fields

        public double m_LearningRate = 0.1;

        public int[] m_HiddenLayerNeurons = { 4, 3, 5 };

        public int m_OutputLayerNeurons;

        public int m_Iterations = 10000;

        public int m_batchSize = 1;

        private Func<double, double> m_ActivationFunction = MLPerceptron.NeuralNetworkCore.ActivationFunctions.HyperbolicTan;//TODO Patrick

        public int m_InpDims;

        public double[][,] m_Weights;

        public double[][] m_Biases;

        public Boolean m_SoftMax = true;

        public int TestCaseNumber = 0;

        #endregion

        #region MLPerceptronAlgorithm constructor
        /// <summary>
        /// MLPerceptronAlgorithm constructor
        /// </summary>
        /// <param name="learningRate">learning rate of the network</param>
        /// <param name="iterations">number of epochs</param>
        /// <param name="hiddenLayerNeurons">number of hidden layer neurons</param>
        /// <param name="activationfunction">activation function to be used by the netwokr</param>
        public MLPerceptronAlgorithm(double learningRate, int iterations, int batchSize, int testCaseNumber, int[] hiddenLayerNeurons, Func<double, double> activationfunction = null, bool SoftMax = true)
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

            if (SoftMax != true)
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

            int numOfInputVectors = trainingData.Length;

            m_InpDims = ctx.DataDescriptor.Features.Count();

            m_OutputLayerNeurons = data[0].Length - m_InpDims;

            m_Weights = new double[numberOfHiddenAndOutputLayers][,];

            m_Biases = new double[numberOfHiddenAndOutputLayers][];

            // Initialize the weights and biases at every layer of the neural network
            InitializeWeightsandBiasesinputlayer(m_InpDims);

            InitializeWeightsandBiaseshiddenlayers(m_HiddenLayerNeurons);

            InitializeWeightsandBiasesoutputlayer(m_HiddenLayerNeurons);

            var score = new MLPerceptronAlgorithmScore();

            double lastLoss = 0;

            double lastValidationLoss = 0;

            string path = Directory.GetCurrentDirectory() + "\\mnist_performance_params_" + this.TestCaseNumber.ToString() + ".csv";

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            using (var performanceData = new StreamWriter(path))
            {
                Stopwatch watch = new Stopwatch();

                double timeElapsed = 0;

                performanceData.WriteLine("{0},{1},{2},{3},{4},{5}", "Epoch", "Epoch Loss", "Epoch Accuracy", "Validation Loss", "Validation Accuracy", "Time Elapsed");

                for (int i = 0; i < m_Iterations; i++)
                {
                    watch.Restart();

                    score.Loss = 0;

                    double batchAccuracy = 0;

                    int miniBatchStartIndex = 0;

                    for (int inputVectIndx = miniBatchStartIndex; inputVectIndx < numOfInputVectors; inputVectIndx = inputVectIndx + m_batchSize)
                    {

                        BackPropagationNetwork backPropagation = new BackPropagationNetwork(m_Biases, m_HiddenLayerNeurons, m_OutputLayerNeurons, m_InpDims);

                        // Z2 = actFnc(X * W1)
                        CalcFirstHiddenLayer(trainingData, inputVectIndx, m_InpDims, out hidLyrOut[0], out hidLyrNeuronSum[0]);

                        // We use output of first layer as input of second layer.
                        CalcRemainingHiddenLayers(hidLyrOut[0], hidLyrNeuronSum[0], m_InpDims, out hidLyrOut, out hidLyrNeuronSum);

                        // Zk = ak-1 * Wk-1
                        CalculateResultatOutputlayer(hidLyrOut[m_HiddenLayerNeurons.Length - 1], m_InpDims, m_SoftMax, out hidLyrOut[m_HiddenLayerNeurons.Length], out hidLyrNeuronSum[m_HiddenLayerNeurons.Length]);

                        if (m_SoftMax == true)
                        {
                            backPropagation.CalcOutputErrorSoftMax(hidLyrOut, m_HiddenLayerNeurons, trainingData, inputVectIndx, m_batchSize, ctx);

                        }
                        else
                        {
                            //  BackPropagationNetwork backPropagation = new BackPropagationNetwork(m_HiddenLayerNeurons.Length);
                            backPropagation.CalcOutputError(hidLyrOut[m_HiddenLayerNeurons.Length], m_HiddenLayerNeurons, hidLyrNeuronSum[m_HiddenLayerNeurons.Length], trainingData, inputVectIndx, m_batchSize, ctx);
                        }
                        backPropagation.CalcHiddenLayersError(hidLyrOut, m_Weights, m_HiddenLayerNeurons, hidLyrNeuronSum, trainingData, inputVectIndx, m_batchSize);

                        //backPropagation.CostFunctionChangeWithBiases(m_Biases, m_HiddenLayerNeurons, m_LearningRate);

                        //backPropagation.CostFunctionChangeWithWeights(m_Weights, hidLyrOut, m_HiddenLayerNeurons, m_LearningRate, trainingData[inputVectIndx]);

                        backPropagation.UpdateBiases(m_Biases, m_HiddenLayerNeurons, m_LearningRate, out m_Biases);

                        backPropagation.UpdateWeights(m_Weights, hidLyrOut, m_HiddenLayerNeurons, m_LearningRate, m_InpDims, out m_Weights);

                        score.Errors = backPropagation.CostChangeDueToBiases[m_HiddenLayerNeurons.Length];

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

                        //miniBatchStartIndex = miniBatchStartIndex + m_batchSize;
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

                    //  Debug.WriteLine($"Loss: {score.Loss}, Last loss: {lastLoss}, Delta: {deltaLoss}, Accuracy: {accuracy}, ValidationLoss: {validationSetLoss}, Last Validationloss: {lastValidationLoss}, Delta: {deltaValidationLoss}, ValidationAccuracy: {validationAccuracy}, TimeElapsed: {timeElapsed}");

                    performanceData.WriteLine("{0},{1},{2},{3},{4},{5}", i.ToString(), score.Loss.ToString("F3", CultureInfo.InvariantCulture), accuracy.ToString("F3", CultureInfo.InvariantCulture), validationSetLoss.ToString("F3", CultureInfo.InvariantCulture), validationAccuracy.ToString("F3", CultureInfo.InvariantCulture), timeElapsed.ToString("F3", CultureInfo.InvariantCulture));

                    lastLoss = score.Loss;

                    lastValidationLoss = validationSetLoss;
                }

                ctx.Score = score;
                return ctx.Score;
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

                PredictResultatOutputlayer(calcuLatedOutput[m_HiddenLayerNeurons.Length - 1], m_InpDims, m_SoftMax, out calcuLatedOutput[m_HiddenLayerNeurons.Length], out weightedInputs[m_HiddenLayerNeurons.Length]);

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
        private void CalcFirstHiddenLayer(double[][] input, int batchIndex, int numOfFeatures, out double[,] layerOutput, out double[,] layerNeuronSum)
        {
            layerOutput = new double[m_batchSize, m_HiddenLayerNeurons[0]];

            layerNeuronSum = new double[m_batchSize, m_HiddenLayerNeurons[0]];

            for (int i = batchIndex; i < batchIndex + m_batchSize && i < input.Length; i++)
            {
                for (int j = 0; j < m_HiddenLayerNeurons[0]; j++)
                {
                    layerNeuronSum[i - batchIndex, j] = 0.0;

                    for (int k = 0; k < m_InpDims; k++)
                    {
                        layerNeuronSum[i - batchIndex, j] += m_Weights[0][j, k] * input[i][k];
                    }

                    layerNeuronSum[i - batchIndex, j] += m_Biases[0][j];

                    layerOutput[i - batchIndex, j] = m_ActivationFunction(layerNeuronSum[i - batchIndex, j]);
                }
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
        private void CalcRemainingHiddenLayers(double[,] input, double[,] firstlayerweightedip, int numOfFeatures, out double[][,] layerOutput, out double[][,] layerNeuronSum)
        {
            int numberOfHiddenAndOutputLayers = m_HiddenLayerNeurons.Length + 1;

            layerOutput = new double[numberOfHiddenAndOutputLayers][,];

            layerNeuronSum = new double[numberOfHiddenAndOutputLayers][,];

            double[,] currentInput = input;

            int index = 0;

            layerOutput[0] = new double[m_batchSize, m_HiddenLayerNeurons[index]];

            layerOutput[0] = input;

            layerNeuronSum[0] = new double[m_batchSize, m_HiddenLayerNeurons[index]];

            layerNeuronSum[0] = firstlayerweightedip;

            while (index < (m_HiddenLayerNeurons.Length - 1))
            {
                int numofhiddenneuronsincurrentlayer = m_HiddenLayerNeurons[index];

                int numofhiddenneuronsinfollayer = m_HiddenLayerNeurons[index + 1];

                layerOutput[index + 1] = new double[m_batchSize, numofhiddenneuronsinfollayer];

                layerNeuronSum[index + 1] = new double[m_batchSize, numofhiddenneuronsinfollayer];

                for (int i = 0; i < m_batchSize; i++)
                {
                    for (int k = 0; k < numofhiddenneuronsinfollayer; k++)
                    {
                        layerNeuronSum[index + 1][i, k] = 0.0;

                        for (int j = 0; j < numofhiddenneuronsincurrentlayer; j++)
                        {
                            layerNeuronSum[index + 1][i, k] += m_Weights[index + 1][k, j] * currentInput[i, j];
                        }

                        layerNeuronSum[index + 1][i, k] += m_Biases[index + 1][k];

                        layerOutput[index + 1][i, k] = m_ActivationFunction(layerNeuronSum[index + 1][i, k]);
                    }
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
        private void CalculateResultatOutputlayer(double[,] input, int numOfFeatures, bool softmax, out double[,] output, out double[,] outputSum)
        {
            output = new double[m_batchSize, m_OutputLayerNeurons];

            outputSum = new double[m_batchSize, m_OutputLayerNeurons];

            int numOfHiddenNeuronsInLastHiddenLayer = m_HiddenLayerNeurons[m_HiddenLayerNeurons.Length - 1];

            for (int i = 0; i < m_batchSize; i++)
            {
                for (int j = 0; j < m_OutputLayerNeurons; j++)
                {
                    outputSum[i, j] = 0.0;

                    for (int k = 0; k < numOfHiddenNeuronsInLastHiddenLayer; k++)
                    {
                        outputSum[i, j] += m_Weights[m_HiddenLayerNeurons.Length][j, k] * input[i, k];
                    }

                    outputSum[i, j] += m_Biases[m_HiddenLayerNeurons.Length][j];

                    if (softmax == false)
                    {
                        output[i, j] = MLPerceptron.NeuralNetworkCore.ActivationFunctions.Sigmoid(outputSum[i, j]);
                    }
                    else
                    {
                        // Do nothing
                    }
                }
            }

            if (softmax == true)
            {
                output = MLPerceptron.NeuralNetworkCore.ActivationFunctions.SoftMaxClassifierTrain(outputSum, m_batchSize);
            }
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
    }
}

