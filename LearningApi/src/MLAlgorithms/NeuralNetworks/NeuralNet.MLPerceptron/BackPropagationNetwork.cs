using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using MLPerceptron.NeuralNetworkCore;

namespace MLPerceptron.BackPropagation
{
    /// <summary>
    /// Class BackPropagationNetwork contains methods which implement the back propagation part of the neural network
    /// </summary>
    public class BackPropagationNetwork
    {

        #region VariableDeclaration

        private double[][,] m_CostChangeDueToWeights;

        private double[][] m_CostChangeDueToBiases;

        private Func<double, double> m_DerivativeActivationFunction = ActivationFunctions.DerivativeHyperbolicTan;
        #endregion

        #region Properties
        /// <summary>
        /// Get the property Errors
        /// </summary>
        public double[][,] Errors { get; }

        public int TrainingSetAccuracy { get; set; }

        /// <summary>
        /// Get the property CostChangeDueToBiases
        /// </summary>
        public double[][] CostChangeDueToBiases
        {
            get
            {
                return this.m_CostChangeDueToBiases;
            }
        }

        /// <summary>
        /// Get the property CostChangeDueWeights
        /// </summary>
        public double[][,] CostChangeDueWeights
        {
            get
            {
                return this.m_CostChangeDueToWeights;
            }
        }
        #endregion

        #region BackPropagationNetwork Constructor
        /// <summary>
        /// BackPropagationNetwork Constructor
        /// </summary>
        /// <param name="numberofhiddenlayers">number of hidden layer neurons</param>
        public BackPropagationNetwork(double[][] biases, int[] hiddenlayerneurons, int outputlayerneurons, int numOfInputNeurons)
        {
            Errors = new double[hiddenlayerneurons.Length + 1][,];

            int hidLayerIndx = hiddenlayerneurons.Length;

            m_CostChangeDueToBiases = new double[hiddenlayerneurons.Length + 1][];

            for (int i = 0; i <= hiddenlayerneurons.Length; i++)
            {
                m_CostChangeDueToBiases[i] = new double[biases[i].Length];
            }

            m_CostChangeDueToWeights = new double[hiddenlayerneurons.Length + 1][,];

            hidLayerIndx = 0;

            while (hidLayerIndx <= hiddenlayerneurons.Length)
            {
                if (hidLayerIndx == 0)
                {
                    m_CostChangeDueToWeights[hidLayerIndx] = new double[hiddenlayerneurons[hidLayerIndx], numOfInputNeurons];
                }
                else if (hidLayerIndx == hiddenlayerneurons.Length)
                {
                    m_CostChangeDueToWeights[hidLayerIndx] = new double[outputlayerneurons, hiddenlayerneurons[hidLayerIndx - 1]];
                }
                else
                {
                    m_CostChangeDueToWeights[hidLayerIndx] = new double[hiddenlayerneurons[hidLayerIndx], hiddenlayerneurons[hidLayerIndx - 1]];
                }

                hidLayerIndx++;
            }
        }

        #endregion

        #region OutputLayerErrorCalculation
        /// <summary>
        /// This method calculates the error at the output layer
        /// </summary>
        /// <param name="output">output layer result</param>
        /// <param name="hiddenlayerneurons">number of hidden layer neurons</param>
        /// <param name="outputSum">array of weighted inputs</param>
        /// <param name="inputVector">training data containing features (input) and labels (output).</param>
        public void CalcOutputError(double[,] output, int[] hiddenlayerneurons, double[,] outputSum, double[][] inputVector, int batchIndex, int batchSize, IContext ctx)
        {
            Errors[hiddenlayerneurons.Length] = new double[batchSize, output.Length];

            double[,] grad = new double[batchSize, output.Length];

            //int lastIndexOfActualOp = ctx.DataDescriptor.Features.Length;// featureValues.Length - 1;
            int lastIndexOfActualOp = inputVector.Length - 1;

            for (int i = batchIndex; i < batchIndex + batchSize; i++)
            {
                // Calculating error as difference between input and calculated output.
                for (int j = (output.Length - 1); j >= 0; j--)
                {
                    grad[i - batchIndex, j] = output[i - batchIndex, j] - inputVector[i][lastIndexOfActualOp--];// TODO: Support for multiple output neurons required.
                                                                                                                //grad[i] = calculatedop[i] - featureValues[lastIndexOfActualOp--];
                }
            }

            for (int i = 0; i < batchSize; i++)
            {
                for (int j = 0; j < output.Length; j++)
                {
                    Errors[hiddenlayerneurons.Length][i, j] = grad[i, j] * ActivationFunctions.DerivativeSigmoid(outputSum[i, j]);

                    m_CostChangeDueToBiases[hiddenlayerneurons.Length][j] += Math.Abs(Errors[hiddenlayerneurons.Length][i, j]);
                }
            }

            for (int i = 0; i < output.Length; i++)
            {
                m_CostChangeDueToBiases[hiddenlayerneurons.Length][i] = m_CostChangeDueToBiases[hiddenlayerneurons.Length][i] / batchSize;
            }
        }

        public void CalcOutputErrorSoftMax(double[][,] hidLayersOutputs, int[] hiddenlayerneurons, double[][] inputVector, int batchIndex, int batchSize, IContext ctx)
        {
            Errors[hiddenlayerneurons.Length] = new double[batchSize, hidLayersOutputs[hiddenlayerneurons.Length].GetLength(1)];

            int lastIndexOfActualOp = 0;

            bool result = true;

            for (int i = batchIndex; i < batchIndex + batchSize; i++)
            {
                result = true;

                lastIndexOfActualOp = inputVector[i].Length - 1;

                for (int j = (hidLayersOutputs[hiddenlayerneurons.Length].GetLength(1) - 1); j >= 0; j--)
                {
                    int inputVectorIndex = lastIndexOfActualOp--;

                    Errors[hiddenlayerneurons.Length][i - batchIndex, j] = hidLayersOutputs[hiddenlayerneurons.Length][i - batchIndex, j] - inputVector[i][inputVectorIndex];

                    for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < hidLayersOutputs[hiddenlayerneurons.Length - 1].GetLength(1); prevHidLayerOutputIndx++)
                    {
                        m_CostChangeDueToWeights[hiddenlayerneurons.Length][j, prevHidLayerOutputIndx] += hidLayersOutputs[hiddenlayerneurons.Length - 1][i - batchIndex, prevHidLayerOutputIndx] * Errors[hiddenlayerneurons.Length][i - batchIndex, j];
                    }

                    m_CostChangeDueToBiases[hiddenlayerneurons.Length][j] += Math.Abs(Errors[hiddenlayerneurons.Length][i - batchIndex, j]);

                    if (inputVector[i][inputVectorIndex] != (hidLayersOutputs[hiddenlayerneurons.Length][i - batchIndex, j] >= 0.5 ? 1 : 0))
                    {
                        result = false;
                    }
                }

                if (result == true)
                {
                    TrainingSetAccuracy++;
                }
            }

            /*
            for (int i = 0; i < hidLayersOutputs[hiddenlayerneurons.Length].GetLength(1); i++)
            {
                m_CostChangeDueToBiases[hiddenlayerneurons.Length][i] = m_CostChangeDueToBiases[hiddenlayerneurons.Length][i] / batchSize;
            }

            for (int i = 0; i < hidLayersOutputs[hiddenlayerneurons.Length].GetLength(1); i++)
            {
                for (int j = 0; j < hidLayersOutputs[hiddenlayerneurons.Length - 1].GetLength(1); j++)
                {
                    m_CostChangeDueToWeights[hiddenlayerneurons.Length][i, j] = m_CostChangeDueToWeights[hiddenlayerneurons.Length][i, j] / batchSize;
                }
            }
            */
        }
        #endregion

        #region HiddenLayersErrorCalculation
        /// <summary>
        /// This method calculates the error that is back propagated at each neuron present in the hidden layers 
        /// </summary>
        /// <param name="hidLayersOutputs">hidden layers output result</param>
        /// <param name="weights">weight array</param>
        /// <param name="hiddenlayerneurons">number of hidden layer neurons</param>
        /// <param name="hidLayersSums">hidden layers weighted inputs</param>
        public void CalcHiddenLayersError(double[][,] hidLayersOutputs, double[][,] weights, int[] hiddenlayerneurons, double[][,] hidLayersSums, double[][] inputVector, int batchIndex, int batchSize)
        {
            int hidLayerIndx = hiddenlayerneurons.Length;

            int numOfInputNeurons = inputVector[0].Length - hidLayersOutputs[hiddenlayerneurons.Length].GetLength(1);

            while (hidLayerIndx != 0)
            {
                Errors[hidLayerIndx - 1] = new double[batchSize, hiddenlayerneurons[hidLayerIndx - 1]];

                for (int i = batchIndex; i < batchSize + batchIndex; i++)
                {
                    for (int hidLayerNeuronIndx = 0; hidLayerNeuronIndx < hiddenlayerneurons[hidLayerIndx - 1]; hidLayerNeuronIndx++)
                    {
                        double layerError = 0.0;

                        for (int j = 0; j < hidLayersOutputs[hidLayerIndx].GetLength(1); j++)
                        {
                            layerError += weights[hidLayerIndx][j, hidLayerNeuronIndx] * Errors[hidLayerIndx][i - batchIndex, j];
                        }

                        Errors[hidLayerIndx - 1][i - batchIndex, hidLayerNeuronIndx] = layerError * m_DerivativeActivationFunction(hidLayersSums[hidLayerIndx - 1][i - batchIndex, hidLayerNeuronIndx]);

                        if (hidLayerIndx != 1)
                        {
                            for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < hidLayersOutputs[hidLayerIndx - 2].GetLength(1); prevHidLayerOutputIndx++)
                            {
                                m_CostChangeDueToWeights[hidLayerIndx - 1][hidLayerNeuronIndx, prevHidLayerOutputIndx] += hidLayersOutputs[hidLayerIndx - 2][i - batchIndex, prevHidLayerOutputIndx] * Errors[hidLayerIndx - 1][i - batchIndex, hidLayerNeuronIndx];
                            }
                        }
                        else
                        {
                            for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < numOfInputNeurons; prevHidLayerOutputIndx++)
                            {
                                m_CostChangeDueToWeights[hidLayerIndx - 1][hidLayerNeuronIndx, prevHidLayerOutputIndx] += inputVector[i][prevHidLayerOutputIndx] * Errors[hidLayerIndx - 1][i - batchIndex, hidLayerNeuronIndx];
                            }
                        }

                        m_CostChangeDueToBiases[hidLayerIndx - 1][hidLayerNeuronIndx] += Math.Abs(Errors[hidLayerIndx - 1][i - batchIndex, hidLayerNeuronIndx]);
                    }
                }
                /*

                for (int i = 0; i < hidLayersOutputs[hidLayerIndx - 1].GetLength(1); i++)
                {
                    m_CostChangeDueToBiases[hidLayerIndx - 1][i] = m_CostChangeDueToBiases[hidLayerIndx - 1][i] / batchSize;
                }

                if (hidLayerIndx != 1)
                {
                    for (int i = 0; i < hidLayersOutputs[hidLayerIndx - 1].GetLength(1); i++)
                    {
                        for (int j = 0; j < hidLayersOutputs[hidLayerIndx - 2].GetLength(1); j++)
                        {
                            m_CostChangeDueToWeights[hidLayerIndx - 1][i, j] = m_CostChangeDueToWeights[hidLayerIndx - 1][i, j] / batchSize;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < hidLayersOutputs[hidLayerIndx - 1].GetLength(1); i++)
                    {
                        for (int j = 0; j < numOfInputNeurons; j++)
                        {
                            m_CostChangeDueToWeights[hidLayerIndx - 1][i, j] = m_CostChangeDueToWeights[hidLayerIndx - 1][i, j] / batchSize;
                        }
                    }
                }
                */
                hidLayerIndx--;
            }

        }
        #endregion
        /*
        #region CostFunctionChangeWithWeights
        /// <summary>
        /// This method calculates the rate of change of cost function with respect to all the weights that are a part of the network 
        /// </summary>
        /// <param name="currWeights">current values of weights</param>
        /// <param name="hidLayersOutputs">hidden layers and output layers output result</param>
        /// <param name="hiddenlayerneurons">number of hidden layer neurons</param>
        /// <param name="learningrate">learning rate of the network</param>
        /// <param name="inputVector">training data feature inputs/outputs</param>
        /// <param name="newWeights">output parameter to store new weights</param>
        public void CostFunctionChangeWithWeights(double[][,] currWeights, double[][] hidLayersOutputs, int[] hiddenlayerneurons, double learningrate, double[] inputVector)
        {

            int hidLayerIndx = 0;

            int numOfInputNeurons = inputVector.Length - hidLayersOutputs[hiddenlayerneurons.Length].Length;

            while (hidLayerIndx <= hiddenlayerneurons.Length)
            {
                if (hidLayerIndx == 0)
                {
                    for (int hidLayerNeuronIndx = 0; hidLayerNeuronIndx < hidLayersOutputs[hidLayerIndx].Length; hidLayerNeuronIndx++)
                    {
                        for (int inputNeuronIndx = 0; inputNeuronIndx < numOfInputNeurons; inputNeuronIndx++)
                        {
                            m_CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx] = inputVector[inputNeuronIndx] * MiniBatchError[hidLayerIndx][hidLayerNeuronIndx];
                        }
                    }
                }
                else
                {
                    for (int hidLayerNeuronIndx = 0; hidLayerNeuronIndx < hidLayersOutputs[hidLayerIndx].Length; hidLayerNeuronIndx++)
                    {
                        for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < hidLayersOutputs[hidLayerIndx - 1].Length; prevHidLayerOutputIndx++)
                        {
                            m_CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx] = hidLayersOutputs[hidLayerIndx - 1][prevHidLayerOutputIndx] * MiniBatchError[hidLayerIndx][hidLayerNeuronIndx];
                        }
                    }
                }

                hidLayerIndx++;
            }

        }
        */

        public void UpdateWeights(double[][,] currWeights, double[][,] hidLayersOutputs, int[] hiddenlayerneurons, double learningrate, int numOfInputNeurons, out double[][,] newWeights)
        {
            newWeights = new double[hiddenlayerneurons.Length + 1][,];

            int hidLayerIndx = 0;

            while (hidLayerIndx <= hiddenlayerneurons.Length)
            {
                if (hidLayerIndx == 0)
                {
                    newWeights[hidLayerIndx] = new double[hidLayersOutputs[hidLayerIndx].GetLength(1), numOfInputNeurons];

                    for (int hidLayerNeuronIndx = 0; hidLayerNeuronIndx < hidLayersOutputs[hidLayerIndx].GetLength(1); hidLayerNeuronIndx++)
                    {
                        for (int inputNeuronIndx = 0; inputNeuronIndx < numOfInputNeurons; inputNeuronIndx++)
                        {
                            newWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx] = currWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx] - learningrate * m_CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx];
                        }
                    }
                }
                else
                {
                    newWeights[hidLayerIndx] = new double[hidLayersOutputs[hidLayerIndx].GetLength(1), hidLayersOutputs[hidLayerIndx - 1].GetLength(1)];

                    for (int hidLayerNeuronIndx = 0; hidLayerNeuronIndx < hidLayersOutputs[hidLayerIndx].GetLength(1); hidLayerNeuronIndx++)
                    {
                        for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < hidLayersOutputs[hidLayerIndx - 1].GetLength(1); prevHidLayerOutputIndx++)
                        {
                            newWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx] = currWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx] - learningrate * m_CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx];
                        }
                    }
                }

                hidLayerIndx++;
            }

        }

        #region CostFunctionChangeWithBiases
        /// <summary>
        /// This method calculates the rate of change of cost function with respect to all the biases that are a part of the network 
        /// </summary>
        /// <param name="currentbiases">current value of biases</param>
        /// <param name="hiddenlayerneurons">number of hidden layer neurons</param>
        /// <param name="learningrate">learning rate of the network</param>
        /// <param name="newbiases">output parameter to store the updated biases</param>
        /*
        public void CostFunctionChangeWithBiases(double[][] currentbiases, int[] hiddenlayerneurons, double learningrate)
        {
            for (int i = 0; i <= hiddenlayerneurons.Length; i++)
            {
                for (int j = 0; j < currentbiases[i].Length; j++)
                {
                    m_CostChangeDueToBiases[i][j] = MiniBatchErrorBiases[i][j];
                }
            }
        }
        */

        public void UpdateBiases(double[][] currentbiases, int[] hiddenlayerneurons, double learningrate, out double[][] newbiases)
        {
            newbiases = new double[hiddenlayerneurons.Length + 1][];

            for (int i = 0; i <= hiddenlayerneurons.Length; i++)
            {
                newbiases[i] = new double[currentbiases[i].Length];

                for (int j = 0; j < currentbiases[i].Length; j++)
                {
                    newbiases[i][j] = currentbiases[i][j] - learningrate * m_CostChangeDueToBiases[i][j];
                }
            }
        }
        #endregion

    }
}
