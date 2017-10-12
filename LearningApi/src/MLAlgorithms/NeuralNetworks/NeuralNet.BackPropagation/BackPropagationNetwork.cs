using NeuralNetworks.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using NeuralNetworks.Core.Layers;
using NeuralNetworks.Core.Neurons;
using NeuralNetworks.Core.Networks;
using System;
using NeuralNetworks.Core.ActivationFunctions;

namespace NeuralNet.BackPropagation
{
    public class BackPropagationNetwork : NeuralNetCore
    {
        private IActivationFunction activationFnc;
        private int m_HiddenLayerCount;
        private double m_Momentum = 0.9;
        private double m_LearningRate = 0.1;
        private int m_Iterations = 5000;

        // Activation function
        private IStochasticFunction m_StochasticFunction = new BernoulliFunction(alpha: 0.5);
        private Func<double, double> m_ActivationFunction = new BernoulliFunction(alpha: 0.5).Function;


        // network to teach
        private ActivationNetwork network;
        // neuron's errors
        private double[][] neuronErrors = null;
        // weight's updates
        private double[][][] weightsUpdates = null;
        // threshold's updates
        private double[][] thresholdsUpdates = null;

        #region Public Function
        public BackPropagationNetwork()
        {
        }


        /// <summary>
        ///   Gets or sets the momentum term of the
        ///   learning algorithm. Default is 0.9.
        /// </summary>
        /// 
        public double Momentum
        {
            get { return m_Momentum; }
            set { m_Momentum = value; }
        }


        /// <summary>
        ///   Gets or sets the learning rate of the
        ///   learning algorithm.Default is 0.1.
        /// </summary>
        /// 
        public double LearningRate
        {
            get { return m_LearningRate; }
            set { m_LearningRate = value; }
        }

        /// <summary>
        ///   Gets or sets the Iterations of the
        ///   learning algorithm.Default is 5000.
        /// </summary>
        /// 
        public int Iteration
        {
            get { return m_Iterations; }
            set { m_Iterations = value; }
        }


        public BackPropagationNetwork(int m_hiddenLayerCount, Func<double, double> activationfunction = null)
        {


            if (activationfunction != null)
                this.m_ActivationFunction = activationfunction;

            //Create network
            networkInitialize();
        }



        public override IScore Run(double[][] featureValues, IContext ctx)
        {
            int numOfInput = featureValues.Length;



            //network.Compute(input);

            //
            // calculate weights updates
            for (int i = 0; i < m_Iterations; i++)
            {

                for (int inputVectIndx = 0; inputVectIndx < numOfInput; inputVectIndx++)
                {
                    calculateUpdates(featureValues[inputVectIndx]);
                    updateNetwork();
                }

            }
            
            return ctx.Score;
        }
        #endregion

        #region Private function

        /// <summary>
        /// Create network for the algorithm
        /// </summary>
        private void networkInitialize()
        {
            //TODO Create back propagartion network

            neuronErrors = new double[network.Layers.Length][];
            weightsUpdates = new double[network.Layers.Length][][];
            thresholdsUpdates = new double[network.Layers.Length][];

            // initialize errors and deltas arrays for each layer
            for (int i = 0; i < network.Layers.Length; i++)
            {
                Layer layer = network.Layers[i];

                neuronErrors[i] = new double[layer.Neurons.Length];
                weightsUpdates[i] = new double[layer.Neurons.Length][];
                thresholdsUpdates[i] = new double[layer.Neurons.Length];

                // for each neuron
                for (int j = 0; j < weightsUpdates[i].Length; j++)
                {
                    weightsUpdates[i][j] = new double[layer.InputsCount];
                }
            }

        }


        /// <summary>
        /// Calculate weights updates.
        /// </summary>
        /// 
        /// <param name="input">Network's input vector.</param>
        /// 
        private void calculateUpdates(double[] input)
        {
            // 1 - calculate updates for the first layer
            Layer layer = network.Layers[0];
            double[] errors = neuronErrors[0];
            double[][] layerWeightsUpdates = weightsUpdates[0];
            double[] layerThresholdUpdates = thresholdsUpdates[0];

            // cache for frequently used values
            double cachedMomentum = m_LearningRate * m_Momentum;
            double cached1mMomentum = m_LearningRate * (1 - m_Momentum);
            double cachedError;

            // for each neuron of the layer
            for (int i = 0; i < layer.Neurons.Length; i++)
            {
                cachedError = errors[i] * cached1mMomentum;
                double[] neuronWeightUpdates = layerWeightsUpdates[i];

                // for each weight of the neuron
                for (int j = 0; j < neuronWeightUpdates.Length; j++)
                {
                    // calculate weight update
                    neuronWeightUpdates[j] = cachedMomentum * neuronWeightUpdates[j] + cachedError * input[j];
                }

                // calculate threshold update
                layerThresholdUpdates[i] = cachedMomentum * layerThresholdUpdates[i] + cachedError;
            }

            // 2 - for all other layers
            for (int k = 1; k < network.Layers.Length; k++)
            {
                Layer layerPrev = network.Layers[k - 1];
                layer = network.Layers[k];
                errors = neuronErrors[k];
                layerWeightsUpdates = weightsUpdates[k];
                layerThresholdUpdates = thresholdsUpdates[k];

                // for each neuron of the layer
                for (int i = 0; i < layer.Neurons.Length; i++)
                {
                    cachedError = errors[i] * cached1mMomentum;
                    double[] neuronWeightUpdates = layerWeightsUpdates[i];

                    // for each synapse of the neuron
                    for (int j = 0; j < neuronWeightUpdates.Length; j++)
                    {
                        // calculate weight update
                        neuronWeightUpdates[j] = cachedMomentum * neuronWeightUpdates[j] + cachedError * layerPrev.Neurons[j].Output;
                    }

                    // calculate threshold update
                    layerThresholdUpdates[i] = cachedMomentum * layerThresholdUpdates[i] + cachedError;
                }
            }
        }

        /// <summary>
        /// Update network's weights.
        /// </summary>
        /// 
        private void updateNetwork()
        {
            // current neuron
            ActivationNeuron neuron;
            // current layer
            Layer layer;
            // layer's weights updates
            double[][] layerWeightsUpdates;
            // layer's thresholds updates
            double[] layerThresholdUpdates;
            // neuron's weights updates
            double[] neuronWeightUpdates;

            // for each layer of the network
            for (int i = 0; i < network.Layers.Length; i++)
            {
                layer = network.Layers[i];
                layerWeightsUpdates = weightsUpdates[i];
                layerThresholdUpdates = thresholdsUpdates[i];

                // for each neuron of the layer
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    neuron = layer.Neurons[j] as ActivationNeuron;
                    neuronWeightUpdates = layerWeightsUpdates[j];

                    // for each weight of the neuron
                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
                        // update weight
                        neuron.Weights[k] += neuronWeightUpdates[k];
                    }
                    // update treshold
                    neuron.Threshold += layerThresholdUpdates[j];
                }
            }
        }

        #endregion

    }
}
