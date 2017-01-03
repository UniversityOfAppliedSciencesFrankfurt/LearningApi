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

    
        public override IScore Run(double[][] data, IContext ctx)
        {
            //
            return ctx.Score;
        }

    }
}
