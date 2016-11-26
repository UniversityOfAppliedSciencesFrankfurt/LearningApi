using NeuralNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;

namespace NeuralNet.BackPropagation
{
    public class BackPropagationNetwork : NeuralNetCore
    {
        private IActivationFunction activationFnc;
        private int hiddenLayerCount;
        private double learningRate;
        private double momentum;

        public BackPropagationNetwork()
        {
        }

        public BackPropagationNetwork(int hiddenLayerCount, double momentum, double learningRate, IActivationFunction activationFnc)
        {
            this.hiddenLayerCount = hiddenLayerCount;
            this.momentum = momentum;
            this.learningRate = learningRate;
            this.activationFnc = activationFnc;
        }

        public override Task<double> Train(double[] featureValues, double label)
        {
           
            return Task.FromResult<double>(0.7);
        }
    }
}
