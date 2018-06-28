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
        public double[][] Errors { get; }

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
        public BackPropagationNetwork(int numberofhiddenlayers)
        {
            Errors = new double[numberofhiddenlayers + 1][];

            m_CostChangeDueToBiases = new double[numberofhiddenlayers + 1][];

            m_CostChangeDueToWeights = new double[numberofhiddenlayers + 1][,];

            // to do: activation function check
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
        public void CalcOutputError(double[] output, int[] hiddenlayerneurons, double[] outputSum, double[] inputVector, IContext ctx)
        {
            Errors[hiddenlayerneurons.Length] = new double[output.Length];

            double[] grad = new double[output.Length];

            //int lastIndexOfActualOp = ctx.DataDescriptor.Features.Length;// featureValues.Length - 1;
            int lastIndexOfActualOp = inputVector.Length - 1;

            // 
            // Calculating error as difference between input and calculated output.
            for (int i = (output.Length - 1); i >= 0; i--)
            {
                grad[i] = output[i] - inputVector[lastIndexOfActualOp--];// TODO: Support for multiple output neurons required.
              //grad[i] = calculatedop[i] - featureValues[lastIndexOfActualOp--];

            }

            for (int i = 0; i < output.Length; i++)
            {
                Errors[hiddenlayerneurons.Length][i] = grad[i] * ActivationFunctions.DerivativeSigmoid(outputSum[i]);
            }
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
        /// <param name="inputVector">training data feature inputs/outputs</param>
        public void CalcHiddenLayersError(double[][] hidLayersOutputs, double[][,] weights, int[] hiddenlayerneurons, double[][] hidLayersSums, double[] inputVector)
        {
            int hidLayerIndx = hiddenlayerneurons.Length;

            while(hidLayerIndx != 0)
            {
                Errors[hidLayerIndx - 1] = new double[hiddenlayerneurons[hidLayerIndx - 1]];

                for (int hidLayerNeuronIndx = 0; hidLayerNeuronIndx < hiddenlayerneurons[hidLayerIndx - 1]; hidLayerNeuronIndx++)
                {
                    double layerError = 0.0;

                    for (int j = 0; j < hidLayersOutputs[hidLayerIndx].Length; j++)
                    {
                        layerError += weights[hidLayerIndx][j, hidLayerNeuronIndx] * Errors[hidLayerIndx][j];
                    }

                    Errors[hidLayerIndx - 1][hidLayerNeuronIndx] = layerError * m_DerivativeActivationFunction(hidLayersSums[hidLayerIndx - 1][hidLayerNeuronIndx]);
                }

                hidLayerIndx--;
            }

        }
        #endregion

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
        public void CostFunctionChangeWithWeights(double[][,] currWeights, double[][] hidLayersOutputs, int[] hiddenlayerneurons, double learningrate, double[] inputVector,
            out double[][,] newWeights)
        {
            newWeights = new double[hiddenlayerneurons.Length + 1][,];

            int hidLayerIndx = 0;
       
            int numOfInputNeurons = inputVector.Length - hidLayersOutputs[hiddenlayerneurons.Length].Length;

            while (hidLayerIndx <= hiddenlayerneurons.Length)
            {
                if (hidLayerIndx == 0)
                {
                    m_CostChangeDueToWeights[hidLayerIndx] = new double[hidLayersOutputs[hidLayerIndx].Length, numOfInputNeurons];

                    newWeights[hidLayerIndx] = new double[hidLayersOutputs[hidLayerIndx].Length, numOfInputNeurons];

                    for (int hidLayerNeuronIndx = 0; hidLayerNeuronIndx < hidLayersOutputs[hidLayerIndx].Length; hidLayerNeuronIndx++)
                    {
                        for (int inputNeuronIndx = 0; inputNeuronIndx < numOfInputNeurons; inputNeuronIndx++)
                        {
                            m_CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx] = inputVector[inputNeuronIndx] * Errors[hidLayerIndx][hidLayerNeuronIndx];

                            newWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx] = currWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx] - learningrate * m_CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, inputNeuronIndx];
                        }
                    }
                }
                else
                {
                    m_CostChangeDueToWeights[hidLayerIndx] = new double[hidLayersOutputs[hidLayerIndx].Length, hidLayersOutputs[hidLayerIndx - 1].Length];

                    newWeights[hidLayerIndx] = new double[hidLayersOutputs[hidLayerIndx].Length, hidLayersOutputs[hidLayerIndx - 1].Length];

                    for (int hidLayerNeuronIndx = 0; hidLayerNeuronIndx < hidLayersOutputs[hidLayerIndx].Length; hidLayerNeuronIndx++)
                    {
                        for (int prevHidLayerOutputIndx = 0; prevHidLayerOutputIndx < hidLayersOutputs[hidLayerIndx - 1].Length; prevHidLayerOutputIndx++)
                        {
                            m_CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx] = hidLayersOutputs[hidLayerIndx - 1][prevHidLayerOutputIndx] * Errors[hidLayerIndx][hidLayerNeuronIndx];

                            newWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx] = currWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx] - learningrate * m_CostChangeDueToWeights[hidLayerIndx][hidLayerNeuronIndx, prevHidLayerOutputIndx];
                        }
                    }
                }

                hidLayerIndx++;
            }

        }
        #endregion

        #region CostFunctionChangeWithBiases
        /// <summary>
        /// This method calculates the rate of change of cost function with respect to all the biases that are a part of the network 
        /// </summary>
        /// <param name="currentbiases">current value of biases</param>
        /// <param name="hiddenlayerneurons">number of hidden layer neurons</param>
        /// <param name="learningrate">learning rate of the network</param>
        /// <param name="newbiases">output parameter to store the updated biases</param>
        public void CostFunctionChangeWithBiases(double[][] currentbiases, int[] hiddenlayerneurons, double learningrate, out double[][] newbiases)
        {
            newbiases = new double[hiddenlayerneurons.Length + 1][];

            for (int i = 0; i <= hiddenlayerneurons.Length; i++)
            {
                m_CostChangeDueToBiases[i] = new double[currentbiases[i].Length];

                newbiases[i] = new double[currentbiases[i].Length];

                for (int j = 0; j < currentbiases[i].Length; j++)
                {
                    m_CostChangeDueToBiases[i][j] = Errors[i][j];

                    newbiases[i][j] = currentbiases[i][j] - learningrate * m_CostChangeDueToBiases[i][j];
                }
            }
        }
        #endregion

    }
}
